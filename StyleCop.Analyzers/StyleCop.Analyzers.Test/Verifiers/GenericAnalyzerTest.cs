// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.Verifiers
{
    using System;
    using System.Collections.Immutable;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Testing;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Lightup;

    internal static class GenericAnalyzerTest
    {
        private static readonly Lazy<ReferenceAssemblies> LazyReferenceAssemblies;

        private static readonly Lazy<MetadataReference> LazyCSharp15PreviewTypesReference;

        private static readonly AnalyzerTest<DefaultVerifier> WorkspaceHelper =
            new CSharpCodeFixTest<EmptyDiagnosticAnalyzer, EmptyCodeFixProvider, DefaultVerifier>();

        static GenericAnalyzerTest()
        {
            LazyReferenceAssemblies = new Lazy<ReferenceAssemblies>(CreateDefaultReferenceAssemblies);
            LazyCSharp15PreviewTypesReference = new Lazy<MetadataReference>(
                CreateCSharp15PreviewTypesReference,
                LazyThreadSafetyMode.PublicationOnly); // Used in case the nuget package download fails, so that won't automatically fail all tests
        }

        internal static ReferenceAssemblies ReferenceAssemblies
        {
            get
            {
                return LazyReferenceAssemblies.Value;
            }
        }

        // TODO: Remove when the reference assemblies include the compiler support types for C# 15
        internal static MetadataReference CSharp15PreviewTypesReference
        {
            get
            {
                return LazyCSharp15PreviewTypesReference.Value;
            }
        }

        internal static async Task<Workspace> CreateWorkspaceAsync()
        {
            return await WorkspaceHelper.CreateWorkspaceAsync().ConfigureAwait(false);
        }

        private static ReferenceAssemblies CreateDefaultReferenceAssemblies()
        {
            string codeAnalysisTestVersion =
                typeof(Compilation).Assembly.GetName().Version!.Major switch
                {
                    1 => "1.2.1",
                    2 => "2.8.2",
                    3 => "3.6.0",
                    4 => "4.0.1",
                    5 => "5.0.0",
                    _ => throw new InvalidOperationException("Unknown version."),
                };

            // Use appropriate default reference assemblies per the support matrix:
            // https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/configure-language-version
            // C# 13 ships with .NET 9 and C# 14 with .NET 10.
            ReferenceAssemblies defaultReferenceAssemblies;
            if (LightupHelpers.SupportsCSharp14)
            {
                defaultReferenceAssemblies = ReferenceAssemblies.Net.Net100;
            }
            else if (LightupHelpers.SupportsCSharp13)
            {
                defaultReferenceAssemblies = ReferenceAssemblies.Net.Net90;
            }
            else if (LightupHelpers.SupportsCSharp12)
            {
                defaultReferenceAssemblies = ReferenceAssemblies.Net.Net80;
            }
            else if (LightupHelpers.SupportsCSharp11)
            {
                defaultReferenceAssemblies = ReferenceAssemblies.Net.Net70;
            }
            else if (LightupHelpers.SupportsCSharp10)
            {
                defaultReferenceAssemblies = ReferenceAssemblies.Net.Net60;
            }
            else if (LightupHelpers.SupportsCSharp9)
            {
                defaultReferenceAssemblies = ReferenceAssemblies.Net.Net50;
            }
            else if (LightupHelpers.SupportsCSharp8)
            {
                defaultReferenceAssemblies = ReferenceAssemblies.NetCore.NetCoreApp30;
            }
            else if (LightupHelpers.SupportsCSharp7)
            {
                defaultReferenceAssemblies = ReferenceAssemblies.NetFramework.Net46.Default;
            }
            else
            {
                defaultReferenceAssemblies = ReferenceAssemblies.NetFramework.Net452.Default;
            }

            return defaultReferenceAssemblies.AddPackages(ImmutableArray.Create(
                new PackageIdentity("Microsoft.CodeAnalysis.CSharp", codeAnalysisTestVersion),
                new PackageIdentity("System.ValueTuple", "4.5.0")));
        }

        private static MetadataReference CreateCSharp15PreviewTypesReference()
        {
            var source = @"
#nullable enable

namespace System.Runtime.CompilerServices
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false)]
    public sealed class UnionAttribute : Attribute
    {
    }

    public interface IUnion
    {
        object? Value { get; }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class IsClosedTypeAttribute : Attribute
    {
    }
}
";

            // NOTE: Using .NET Standard here to work in all tests
            var netStandardReferences = ReferenceAssemblies.NetStandard.NetStandard20
                .ResolveAsync(LanguageNames.CSharp, CancellationToken.None)
                .GetAwaiter()
                .GetResult();

            var compilation = CSharpCompilation.Create(
                "StyleCop.Analyzers.Test.CSharp15PreviewTypes",
                [CSharpSyntaxTree.ParseText(source)],
                netStandardReferences,
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            using (var stream = new MemoryStream())
            {
                var emitResult = compilation.Emit(stream);
                if (!emitResult.Success)
                {
                    throw new InvalidOperationException(
                        "Failed to compile the synthetic C# 15 preview types assembly: "
                        + string.Join(Environment.NewLine, emitResult.Diagnostics));
                }

                return MetadataReference.CreateFromImage(stream.ToArray());
            }
        }
    }
}
