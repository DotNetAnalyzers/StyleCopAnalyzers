// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp11.Lightup
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Text;
    using StyleCop.Analyzers.Lightup;
    using StyleCop.Analyzers.Test.CSharp10.Lightup;
    using StyleCop.Analyzers.Test.Verifiers;
    using Xunit;

    public partial class IImportScopeWrapperCSharp11UnitTests : IImportScopeWrapperCSharp10UnitTests
    {
        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public async Task TestCompatibleInstanceAsync(int numberOfAliasSymbols)
        {
            var obj = await CreateImportScopeAsync(numberOfAliasSymbols, CancellationToken.None).ConfigureAwait(false);
            Assert.True(IImportScopeWrapper.IsInstance(obj));
            var wrapper = IImportScopeWrapper.FromObject(obj);
            Assert.Equal(obj.Aliases, wrapper.Aliases);
        }

        private static async Task<IImportScope> CreateImportScopeAsync(int numberOfAliasSymbols, CancellationToken cancellationToken)
        {
            var aliasDirectives = new List<string>(numberOfAliasSymbols);
            for (var i = 0; i < numberOfAliasSymbols; i++)
            {
                aliasDirectives.Add($"global using Alias{i} = System.String;");
            }

            var source = string.Join(Environment.NewLine, aliasDirectives);
            if (source.Length > 0)
            {
                source += Environment.NewLine;
            }

            source += @"
namespace TestNamespace
{
    public class TestClass
    {
    }
}
";

            var workspace = await GenericAnalyzerTest.CreateWorkspaceAsync().ConfigureAwait(false);
            var projectId = ProjectId.CreateNewId();

            var references = await GenericAnalyzerTest.ReferenceAssemblies
                .ResolveAsync(LanguageNames.CSharp, cancellationToken).ConfigureAwait(false);

            var solution = workspace.CurrentSolution
                .AddProject(projectId, "TestProject", "TestProject", LanguageNames.CSharp)
                .WithProjectCompilationOptions(projectId, new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
                .WithProjectParseOptions(projectId, new CSharpParseOptions(LanguageVersionEx.CSharp11))
                .AddMetadataReferences(projectId, references);

            var documentId = DocumentId.CreateNewId(projectId);
            solution = solution.AddDocument(documentId, "Test.cs", SourceText.From(source));

            var project = solution.GetProject(projectId)!;
            var compilation = (await project.GetCompilationAsync(cancellationToken).ConfigureAwait(false))!;
            var syntaxTree = compilation.SyntaxTrees.Single();
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var importScopes = semanticModel.GetImportScopes(syntaxTree.Length, cancellationToken);

            var result = importScopes.FirstOrDefault(scope => scope.Aliases.Length == numberOfAliasSymbols);
            if (result == null)
            {
                throw new InvalidOperationException("Could not create an import scope with the expected number of alias symbols.");
            }

            return result;
        }
    }
}
