// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp7.LayoutRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.LayoutRules;
    using StyleCop.Analyzers.Test.LayoutRules;
    using StyleCop.Analyzers.Test.Verifiers;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.CustomDiagnosticVerifier<StyleCop.Analyzers.LayoutRules.SA1509OpeningBracesMustNotBePrecededByBlankLine>;

    public class SA1509CSharp7UnitTests : SA1509UnitTests
    {
        [Fact]
        public async Task TestLocalFunctionDeclarationOpeningBraceHasBlankLineAsync()
        {
            var testCode = @"
class Foo
{
    void Method()
    {
        void Bar()

        {
        }
    }
}";
            var fixedCode = @"
class Foo
{
    void Method()
    {
        void Bar()
        {
        }
    }
}";

            DiagnosticResult expected = Diagnostic().WithLocation(8, 9);
            await VerifyCSharpFixAsync(LanguageVersion.CSharp7_3, testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLocalFunctionDeclarationOpeningBraceHasTwoBlankLineAsync()
        {
            var testCode = @"
class Foo
{
    void Method()
    {
        void Bar()


        {
        }
    }
}";

            var fixedCode = @"
class Foo
{
    void Method()
    {
        void Bar()
        {
        }
    }
}";

            DiagnosticResult expected = Diagnostic().WithLocation(9, 9);
            await VerifyCSharpFixAsync(LanguageVersion.CSharp7_3, testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestStackAllocArrayCreationExpressionAsync()
        {
            var testCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public unsafe void TestMethod()
        {
            int* v1 = stackalloc int[]

            {
                1,
                2,
                3
            };
        }
    }
}
";

            var fixedTestCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public unsafe void TestMethod()
        {
            int* v1 = stackalloc int[]
            {
                1,
                2,
                3
            };
        }
    }
}
";

            var expectedDiagnostic = Diagnostic().WithLocation(9, 13);
            await VerifyCSharpFixAsync(LanguageVersion.CSharp7_3, testCode, expectedDiagnostic, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestImplicitStackAllocArrayCreationExpressionAsync()
        {
            var testCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public unsafe void TestMethod()
        {
            int* v1 = stackalloc[]

            {
                1,
                2,
                3
            };
        }
    }
}
";

            var fixedTestCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public unsafe void TestMethod()
        {
            int* v1 = stackalloc[]
            {
                1,
                2,
                3
            };
        }
    }
}
";

            var expectedDiagnostic = Diagnostic().WithLocation(9, 13);
            await VerifyCSharpFixAsync(LanguageVersion.CSharp7_3, testCode, expectedDiagnostic, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        private static Task VerifyCSharpFixAsync(LanguageVersion languageVersion, string source, DiagnosticResult expected, string fixedSource, CancellationToken cancellationToken)
        {
            return VerifyCSharpFixAsync(languageVersion, source, new[] { expected }, fixedSource, cancellationToken);
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

        private class CSharpTest : StyleCopCodeFixVerifier<SA1509OpeningBracesMustNotBePrecededByBlankLine, SA1509CodeFixProvider>.CSharpTest
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
