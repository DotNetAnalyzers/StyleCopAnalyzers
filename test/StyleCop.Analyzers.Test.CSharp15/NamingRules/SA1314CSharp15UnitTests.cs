// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp15.NamingRules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Testing;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.NamingRules.SA1314TypeParameterNamesMustBeginWithT,
        StyleCop.Analyzers.NamingRules.SA1314CodeFixProvider>;

    public partial class SA1314CSharp15UnitTests
    {
        [Fact]
        [WorkItem(2966, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2966")]
        public async Task TestTypeParameterDoesNotStartWithTAndGeneratedPartialContainsCrefAsync()
        {
            var test = new CSharpTestWithGeneratedPartial
            {
                TestCode = @"public partial class Outer<{|#0:IDENT|}> { }",
                FixedCode = @"public partial class Outer<TIDENT> { }",
                TestBehaviors = TestBehaviors.SkipGeneratedSourcesCheck,
            };
            test.ExpectedDiagnostics.Add(Diagnostic().WithLocation(0));

            await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

#pragma warning disable RS1041, RS1042 // This test generator must be compiled with the test assembly.
        [Generator]
        public sealed class GeneratedPartialGenerator : ISourceGenerator
        {
            public void Initialize(GeneratorInitializationContext context)
            {
            }

            public void Execute(GeneratorExecutionContext context)
            {
                var typeDeclaration = context.Compilation.SyntaxTrees
                    .SelectMany(tree => tree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>())
                    .FirstOrDefault(declaration => declaration.Identifier.ValueText == "Outer");
                if (typeDeclaration?.TypeParameterList?.Parameters.FirstOrDefault() is not TypeParameterSyntax typeParameter)
                {
                    return;
                }

                string name = typeParameter.Identifier.ValueText;
                context.AddSource(
                    "GeneratedPartial.g.cs",
                    "/// <summary><see cref=\"Outer{" + name + "}\"/></summary>\npublic partial class Outer<" + name + "> { }");
            }
        }
#pragma warning restore RS1041, RS1042

        private sealed class CSharpTestWithGeneratedPartial : CSharpTest
        {
            protected override IEnumerable<Type> GetSourceGenerators()
            {
                return new[] { typeof(GeneratedPartialGenerator) };
            }
        }
    }
}
