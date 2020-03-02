// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.SpacingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.SpacingRules;
    using TestHelper;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.SpacingRules.SA1018NullableTypeSymbolsMustNotBePrecededBySpace,
        StyleCop.Analyzers.SpacingRules.SA1018CodeFixProvider>;

    /// <summary>
    /// This class contains unit tests for the <see cref="SA1018NullableTypeSymbolsMustNotBePrecededBySpace"/> class.
    /// </summary>
    public class SA1018UnitTests
    {
        /// <summary>
        /// Verifies that nullable types with different kinds of spacing will report.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestNullableTypeSpacingAsync()
        {
            var testCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public void TestMethod()
        {
            int ? v1;
            int /* test */ ? v2;
            int
? v3;
            int
/* test */
? v4;
            int
// test
? v5;

            int
#if TEST
? v6a;
#else
? v6b;
#endif
        }
    }
}
";

            var fixedTestCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public void TestMethod()
        {
            int? v1;
            int/* test */? v2;
            int? v3;
            int/* test */? v4;
            int
// test
? v5;

            int
#if TEST
? v6a;
#else
? v6b;
#endif
        }
    }
}
";

            await new CSharpTest
            {
                TestCode = testCode,
                ExpectedDiagnostics =
                {
                    // v1
                    Diagnostic().WithLocation(7, 17),

                    // v2
                    Diagnostic().WithLocation(8, 28),

                    // v3
                    Diagnostic().WithLocation(10, 1),

                    // v4
                    Diagnostic().WithLocation(13, 1),

                    // v5
                    Diagnostic().WithLocation(16, 1),

                    // v6
                    Diagnostic().WithLocation(22, 1),
                },
                FixedCode = fixedTestCode,
                RemainingDiagnostics =
                {
                    // The fixed test code will have diagnostics, because not all cases can be code fixed automatically.
                    Diagnostic().WithLocation(13, 1),
                    Diagnostic().WithLocation(19, 1),
                },
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(1256, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1256")]
        public async Task TestSyntaxErrorAtEndOfFileAsync()
        {
            string testCode = @"namespace StyleCopAnalyzers_Test
{
    public class Class1
    {
        private void Test()
        {
            var hello = ""world"";
        }
    }
}
?
";

            DiagnosticResult[] expected =
            {
                DiagnosticResult.CompilerError("CS1031").WithMessage("Type expected").WithLocation(10, 2),
            };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
