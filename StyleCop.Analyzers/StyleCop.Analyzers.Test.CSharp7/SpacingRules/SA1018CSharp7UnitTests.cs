// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp7.SpacingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using StyleCop.Analyzers.Test.SpacingRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.SpacingRules.SA1018NullableTypeSymbolsMustNotBePrecededBySpace,
        StyleCop.Analyzers.SpacingRules.SA1018CodeFixProvider>;

    public class SA1018CSharp7UnitTests : SA1018UnitTests
    {
        /// <summary>
        /// Verifies that nullable tuple types with different kinds of spacing will report.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestNullableTupleTypeSpacingAsync()
        {
            var testCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public void TestMethod()
        {
            (int, int) ? v1;
            (int, int) /* test */ ? v2;
            (int, int)
? v3;
            (int, int)
/* test */
? v4;
            (int, int)
// test
? v5;

            (int, int)
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
            (int, int)? v1;
            (int, int)/* test */? v2;
            (int, int)? v3;
            (int, int)/* test */? v4;
            (int, int)
// test
? v5;

            (int, int)
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
                    Diagnostic().WithLocation(7, 24),

                    // v2
                    Diagnostic().WithLocation(8, 35),

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
                NumberOfFixAllIterations = 2,
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }
    }
}
