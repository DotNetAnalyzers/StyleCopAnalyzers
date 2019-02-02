// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp7.ReadabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.ReadabilityRules;
    using StyleCop.Analyzers.Test.ReadabilityRules;
    using StyleCop.Analyzers.Test.Verifiers;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.CustomDiagnosticVerifier<StyleCop.Analyzers.ReadabilityRules.SA1137ElementsShouldHaveTheSameIndentation>;

    public class SA1137CSharp7UnitTests : SA1137UnitTests
    {
        [Fact]
        public async Task TestTupleTypeAsync()
        {
            string testCode = @"
class Container
{
    (
        int x,
      int y,
int z) NonZeroAlignment;

    (
int x,
      int y,
        int z) ZeroAlignment;
}
";
            string fixedCode = @"
class Container
{
    (
        int x,
        int y,
        int z) NonZeroAlignment;

    (
int x,
int y,
int z) ZeroAlignment;
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(6, 1),
                Diagnostic().WithLocation(7, 1),
                Diagnostic().WithLocation(11, 1),
                Diagnostic().WithLocation(12, 1),
            };

            await VerifyCSharpFixAsync(LanguageVersion.CSharp7_3, testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestTupleExpressionAsync()
        {
            string testCode = @"
class Container
{
    (int x, int y, int z) NonZeroAlignment = (
        0,
      0,
0);

    (int x, int y, int z) ZeroAlignment = (
0,
      0,
        0);
}
";
            string fixedCode = @"
class Container
{
    (int x, int y, int z) NonZeroAlignment = (
        0,
        0,
        0);

    (int x, int y, int z) ZeroAlignment = (
0,
0,
0);
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(6, 1),
                Diagnostic().WithLocation(7, 1),
                Diagnostic().WithLocation(11, 1),
                Diagnostic().WithLocation(12, 1),
            };

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
            int* data1 = stackalloc int[]
            {
                0,
              0,
0,
            };
            int* data2 = stackalloc int[]
            {
0,
              0,
                0,
            };
        }
    }
}
";

            var fixedCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public unsafe void TestMethod()
        {
            int* data1 = stackalloc int[]
            {
                0,
                0,
                0,
            };
            int* data2 = stackalloc int[]
            {
0,
0,
0,
            };
        }
    }
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(10, 1),
                Diagnostic().WithLocation(11, 1),
                Diagnostic().WithLocation(16, 1),
                Diagnostic().WithLocation(17, 1),
            };

            await VerifyCSharpFixAsync(LanguageVersion.CSharp7_3, testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
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
            int* data1 = stackalloc[]
            {
                0,
              0,
0,
            };
            int* data2 = stackalloc[]
            {
0,
              0,
                0,
            };
        }
    }
}
";

            var fixedCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public unsafe void TestMethod()
        {
            int* data1 = stackalloc[]
            {
                0,
                0,
                0,
            };
            int* data2 = stackalloc[]
            {
0,
0,
0,
            };
        }
    }
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(10, 1),
                Diagnostic().WithLocation(11, 1),
                Diagnostic().WithLocation(16, 1),
                Diagnostic().WithLocation(17, 1),
            };

            await VerifyCSharpFixAsync(LanguageVersion.CSharp7_3, testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
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

        private class CSharpTest : StyleCopCodeFixVerifier<SA1137ElementsShouldHaveTheSameIndentation, IndentationCodeFixProvider>.CSharpTest
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
