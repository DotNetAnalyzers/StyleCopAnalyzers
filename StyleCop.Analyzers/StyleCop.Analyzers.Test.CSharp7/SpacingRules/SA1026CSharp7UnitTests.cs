// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp7.SpacingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.SpacingRules;
    using StyleCop.Analyzers.Test.SpacingRules;
    using StyleCop.Analyzers.Test.Verifiers;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.CustomDiagnosticVerifier<StyleCop.Analyzers.SpacingRules.SA1026CodeMustNotContainSpaceAfterNewKeywordInImplicitlyTypedArrayAllocation>;

    public class SA1026CSharp7UnitTests : SA1026UnitTests
    {
        [Theory]
        [InlineData(" ")]
        [InlineData("  ")]
        [InlineData("\t")]
        [InlineData("\r")]
        [InlineData("\n")]
        [InlineData("\r\n")]
        [InlineData(" \t \r\n")]
        [InlineData(" \t \r\n\t ")]
        public async Task TestImplicitStackAllocArrayCreationExpressionAsync(string space)
        {
            string testCode = $"public class Foo {{ public unsafe Foo() {{ int* ints = stackalloc{space}[] {{ 1, 2, 3 }}; }} }}";
            const string expectedCode = "public class Foo { public unsafe Foo() { int* ints = stackalloc[] { 1, 2, 3 }; } }";
            DiagnosticResult[] expected = { Diagnostic().WithArguments("stackalloc").WithLocation(1, 54) };

            await VerifyCSharpFixAsync(LanguageVersion.CSharp7_3, testCode, expected, expectedCode, CancellationToken.None).ConfigureAwait(false);
        }

        private static Task VerifyCSharpFixAsync(LanguageVersion languageVersion, string source, DiagnosticResult[] expected, string fixedSource, CancellationToken cancellationToken)
        {
            var test = new CSharpTest(languageVersion)
            {
                TestCode = source,
                FixedCode = fixedSource,
            };

            if (source == fixedSource)
            {
                test.FixedState.InheritanceMode = StateInheritanceMode.AutoInheritAll;
                test.FixedState.MarkupHandling = MarkupMode.Allow;
                test.BatchFixedState.InheritanceMode = StateInheritanceMode.AutoInheritAll;
                test.BatchFixedState.MarkupHandling = MarkupMode.Allow;
            }

            test.ExpectedDiagnostics.AddRange(expected);
            return test.RunAsync(cancellationToken);
        }

        private class CSharpTest : StyleCopCodeFixVerifier<SA1026CodeMustNotContainSpaceAfterNewKeywordInImplicitlyTypedArrayAllocation, TokenSpacingCodeFixProvider>.CSharpTest
        {
            public CSharpTest(LanguageVersion languageVersion)
            {
                this.SolutionTransforms.Add((solution, projectId) =>
                {
                    var parseOptions = (CSharpParseOptions)solution.GetProject(projectId).ParseOptions;
                    return solution.WithProjectParseOptions(projectId, parseOptions.WithLanguageVersion(languageVersion));
                });
            }
        }
    }
}
