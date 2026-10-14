// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp8.Lightup;

using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Testing;
using StyleCop.Analyzers.Lightup;
using Xunit;

public partial class NullableLightupCSharp8UnitTests
{
    [Theory]
    [InlineData("wrapper.Body.ToString();", "CS8602")]
    [InlineData("wrapper.ExpressionBody.ToString();", "CS8602")]
    [InlineData("wrapper.ReturnType.ToString();")]
    [InlineData("wrapper.WithBody(null);")]
    [InlineData("wrapper.WithReturnType(null);", "CS8625")]
    [InlineData("wrapper.SyntaxNode.ToString();", "CS8602")]
    [InlineData("StatementSyntax syntax = wrapper;", "CS8600")]
    [InlineData("_ = (PatternSyntaxWrapper)node;")]
    [InlineData("if (PatternSyntaxWrapper.IsInstance(node)) { node.ToString(); }")]
    [InlineData("SyntaxWrapperHelper.GetWrappedType(typeof(PatternSyntaxWrapper)).ToString();", "CS8602")]
    [InlineData("SyntaxWrapperHelper.GetWrappedType(null);", "CS8625")]
    public Task TestSyntaxContractsAsync(string statement, params string[] expectedDiagnostics)
    {
        return VerifyDiagnosticsAsync(statement, expectedDiagnostics);
    }

    [Theory]
    [InlineData("IInvocationOperationWrapper.FromOperation(operation).Instance.ToString();", "CS8602")]
    [InlineData("IInvocationOperationWrapper.FromOperation(operation).Type.ToString();", "CS8602")]
    [InlineData("IInvocationOperationWrapper.FromOperation(operation).WrappedOperation.ToString();", "CS8602")]
    [InlineData("IReturnOperationWrapper.FromOperation(operation).ReturnedValue.ToString();", "CS8602")]
    [InlineData("IObjectCreationOperationWrapper.FromOperation(operation).Constructor.ToString();", "CS8602")]
    [InlineData("IObjectCreationOperationWrapper.FromOperation(operation).Initializer.WrappedOperation.ToString();", "CS8602")]
    [InlineData("if (IInvocationOperationWrapper.IsInstance(operation)) { operation.ToString(); }")]
    [InlineData("OperationWrapperHelper.GetWrappedType(typeof(IInvocationOperationWrapper)).ToString();", "CS8602")]
    [InlineData("IOperationWrapper.FromOperation(operation).ConstantValue.Value.ToString();", "CS8602")]
    [InlineData("IOperationWrapper.FromOperation(operation).Language.ToString();", "CS8602")]
    [InlineData("IOperationWrapper.FromOperation(operation).Children.GetEnumerator();", "CS8602")]
    [InlineData("OperationWrapperHelper.GetWrappedType(null);", "CS8625")]
    public Task TestOperationContractsAsync(string statement, params string[] expectedDiagnostics)
    {
        return VerifyDiagnosticsAsync(statement, expectedDiagnostics);
    }

    [Theory]
    [InlineData("BaseNamespaceDeclarationSyntaxWrapper ns = (NamespaceDeclarationSyntax?)null;")]
    [InlineData("BaseObjectCreationExpressionSyntaxWrapper creation = (ObjectCreationExpressionSyntax?)null;")]
    [InlineData("CommonForEachStatementSyntaxWrapper loop = (ForEachStatementSyntax?)null;")]
    [InlineData("SyntaxWrapper<StatementSyntax>.Default.Wrap(null);")]
    [InlineData("SyntaxWrapper<StatementSyntax>.Default.Wrap(node).ToString();", "CS8602")]
    [InlineData("SyntaxWrapper<StatementSyntax>.Default.Unwrap(null).ToString();", "CS8602")]
    [InlineData("SyntaxWrapper<PatternSyntaxWrapper>.Default.Unwrap(default).ToString();", "CS8602")]
    [InlineData("AnalyzerConfigOptionsWrapper.FromObject(null);")]
    [InlineData("AnalyzerConfigOptionsProviderWrapper.FromObject(null);")]
    [InlineData("if (AnalyzerConfigOptionsWrapper.IsInstance(node)) { node.ToString(); }")]
    [InlineData("if (AnalyzerConfigOptionsProviderWrapper.IsInstance(node)) { node.ToString(); }")]
    [InlineData("var options = AnalyzerConfigOptionsWrapper.FromObject(null); if (options.TryGetValue(\"key\", out var value)) { value.ToString(); }")]
    [InlineData("var options = AnalyzerConfigOptionsWrapper.FromObject(null); options.TryGetValue(\"key\", out var value); value.ToString();", "CS8602")]
    [InlineData("TryGetValueAccessor<object, string, string> accessor = null!; if (accessor(new object(), \"key\", out var value)) { value.ToString(); }")]
    [InlineData("TryGetValueAccessor<object, string, string> accessor = null!; accessor(new object(), \"key\", out var value); value.ToString();", "CS8602")]
    [InlineData("WrapperHelper.GetWrappedType(typeof(AnalyzerConfigOptionsWrapper)).ToString();", "CS8602")]
    [InlineData("WrapperHelper.GetWrappedType(null);", "CS8625")]
    public Task TestHandwrittenContractsAsync(string statement, params string[] expectedDiagnostics)
    {
        return VerifyDiagnosticsAsync(statement, expectedDiagnostics);
    }

    [Fact]
    public async Task TestAnnotationsMatchAvailableRoslynPropertiesAsync()
    {
        var compilation = await CreateCompilationAsync(string.Empty).ConfigureAwait(false);
        var lightupNamespace = compilation.GetTypeByMetadataName(typeof(PatternSyntaxWrapper).FullName!)!.ContainingNamespace;
        var verifiedProperties = 0;
        foreach (var wrapper in lightupNamespace.GetTypeMembers().Where(type => type.TypeKind == TypeKind.Struct))
        {
            // Handwritten wrappers are migrated separately.
            if (wrapper.GetMembers("SyntaxNode").Concat(wrapper.GetMembers("WrappedOperation"))
                .OfType<IPropertySymbol>().FirstOrDefault()?.Type.NullableAnnotation != NullableAnnotation.Annotated)
            {
                continue;
            }

            var wrappedTypeName = wrapper.GetMembers("WrappedTypeName").OfType<IFieldSymbol>().Single().ConstantValue as string;
            var wrappedType = compilation.GetTypeByMetadataName(wrappedTypeName!);
            if (wrappedType is null)
            {
                continue;
            }

            foreach (var property in wrapper.GetMembers().OfType<IPropertySymbol>())
            {
                if (wrapper.Name == nameof(IOperationWrapper) && property.Name is "Children" or "Language")
                {
                    // These required modern properties return null on the oldest supported hosts.
                    continue;
                }

                var wrappedProperty = GetProperty(wrappedType, property.Name);
                if (wrappedProperty is null || !property.Type.IsReferenceType || !wrappedProperty.Type.IsReferenceType
                    || wrappedProperty.Type.NullableAnnotation == NullableAnnotation.None)
                {
                    continue;
                }

                Assert.True(
                    wrappedProperty.Type.NullableAnnotation == property.Type.NullableAnnotation,
                    $"{wrapper.Name}.{property.Name}: expected {wrappedProperty.Type.NullableAnnotation}, actual {property.Type.NullableAnnotation}.");
                verifiedProperties++;

                var withMethod = wrapper.GetMembers("With" + property.Name).OfType<IMethodSymbol>().SingleOrDefault();
                var wrappedWithMethod = wrappedType.GetMembers("With" + property.Name).OfType<IMethodSymbol>().SingleOrDefault();
                if (withMethod is not null && wrappedWithMethod is not null)
                {
                    Assert.Equal(wrappedWithMethod.Parameters[0].Type.NullableAnnotation, withMethod.Parameters[0].Type.NullableAnnotation);
                }
            }
        }

        Assert.True(verifiedProperties > 0, "No annotated Roslyn properties were available for comparison.");
    }

    private static async Task VerifyDiagnosticsAsync(string statement, string[] expectedDiagnostics)
    {
        var compilation = await CreateCompilationAsync(statement).ConfigureAwait(false);
        var diagnostics = compilation.GetDiagnostics();
        Assert.True(
            !diagnostics.Any(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error),
            string.Join("\n", diagnostics));
        Assert.Equal(expectedDiagnostics, diagnostics
            .Where(diagnostic => diagnostic.Severity >= DiagnosticSeverity.Warning)
            .Select(diagnostic => diagnostic.Id));
    }

    private static IPropertySymbol? GetProperty(INamedTypeSymbol type, string name)
    {
        return type.GetMembers(name).OfType<IPropertySymbol>().SingleOrDefault()
            ?? (type.BaseType is { } baseType ? GetProperty(baseType, name) : null)
            ?? type.AllInterfaces.SelectMany(@interface => @interface.GetMembers(name)).OfType<IPropertySymbol>().FirstOrDefault();
    }

    private static async Task<CSharpCompilation> CreateCompilationAsync(string statement)
    {
        var references = await ReferenceAssemblies.Net.Net100
            .ResolveAsync(LanguageNames.CSharp, CancellationToken.None).ConfigureAwait(false);
        references = references.AddRange(new[]
        {
            MetadataReference.CreateFromFile(typeof(SyntaxNode).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(CSharpSyntaxNode).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(PatternSyntaxWrapper).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(ImmutableArray<>).Assembly.Location),
        });

        var source = /* lang=c#-test */ @"
#nullable enable
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using StyleCop.Analyzers.Lightup;
class C
{
    void M(LocalFunctionStatementSyntaxWrapper wrapper, SyntaxNode? node, IOperation? operation)
    {
        " + statement + @"
    }
}";
        var assemblyName = typeof(NullableLightupCSharp8UnitTests).Assembly.GetName();

        // The shipped assembly references Roslyn 1.2.1; these probes intentionally use the current host.
        return CSharpCompilation.Create(
            assemblyName.Name!,
            new[] { CSharpSyntaxTree.ParseText(source, new CSharpParseOptions(LanguageVersion.CSharp8)) },
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
                .WithCryptoPublicKey(ImmutableArray.Create(assemblyName.GetPublicKey()!))
                .WithSpecificDiagnosticOptions(ImmutableDictionary<string, ReportDiagnostic>.Empty.Add("CS1701", ReportDiagnostic.Suppress)));
    }
}
