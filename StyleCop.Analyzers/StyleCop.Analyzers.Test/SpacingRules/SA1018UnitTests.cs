// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.SpacingRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.SpacingRules;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// This class contains unit tests for the <see cref="SA1018NullableTypeSymbolsMustNotBePrecededBySpace"/> class.
    /// </summary>
    public class SA1018UnitTests : CodeFixVerifier
    {
        /// <summary>
        /// Verifies that nullable types with different kinds of spacing will report
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

            DiagnosticResult[] expectedResults =
            {
                // v1
                this.CSharpDiagnostic().WithLocation(7, 17),

                // v2
                this.CSharpDiagnostic().WithLocation(8, 28),

                // v3
                this.CSharpDiagnostic().WithLocation(10, 1),

                // v4
                this.CSharpDiagnostic().WithLocation(13, 1),

                // v5
                this.CSharpDiagnostic().WithLocation(16, 1),

                // v6
                this.CSharpDiagnostic().WithLocation(22, 1)
            };

            // The fixed test code will have diagnostics, because not all cases can be code fixed automatically.
            DiagnosticResult[] fixedExpectedResults =
            {
                this.CSharpDiagnostic().WithLocation(13, 1),
                this.CSharpDiagnostic().WithLocation(19, 1)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expectedResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, fixedExpectedResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// This is a regression test for DotNetAnalyzers/StyleCopAnalyzers#1256.
        /// https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1256
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
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
                new DiagnosticResult
                {
                    Id = "CS1031",
                    Message = "Type expected",
                    Severity = DiagnosticSeverity.Error,
                    Locations = new[] { new DiagnosticResultLocation("Test0.cs", 10, 2) }
                }
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1018NullableTypeSymbolsMustNotBePrecededBySpace();
        }

        /// <inheritdoc/>
        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new SA1018CodeFixProvider();
        }
    }
}
