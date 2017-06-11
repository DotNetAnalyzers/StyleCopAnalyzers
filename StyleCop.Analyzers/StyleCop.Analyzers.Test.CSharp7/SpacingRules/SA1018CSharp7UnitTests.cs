// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp7.SpacingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using StyleCop.Analyzers.Test.SpacingRules;
    using TestHelper;
    using Xunit;

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

            DiagnosticResult[] expectedResults =
            {
                // v1
                this.CSharpDiagnostic().WithLocation(7, 24),

                // v2
                this.CSharpDiagnostic().WithLocation(8, 35),

                // v3
                this.CSharpDiagnostic().WithLocation(10, 1),

                // v4
                this.CSharpDiagnostic().WithLocation(13, 1),

                // v5
                this.CSharpDiagnostic().WithLocation(16, 1),

                // v6
                this.CSharpDiagnostic().WithLocation(22, 1),
            };

            // The fixed test code will have diagnostics, because not all cases can be code fixed automatically.
            DiagnosticResult[] fixedExpectedResults =
            {
                this.CSharpDiagnostic().WithLocation(13, 1),
                this.CSharpDiagnostic().WithLocation(19, 1),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expectedResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, fixedExpectedResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode, numberOfFixAllIterations: 2, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }
    }
}
