// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.LayoutRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.LayoutRules;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// Unit tests for <see cref="SA1511WhileDoFooterMustNotBePrecededByBlankLine"/>
    /// </summary>
    public class SA1511UnitTests : CodeFixVerifier
    {
        /// <summary>
        /// Verifies that a valid do ... while statement will not produce a diagnostic.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestValidDoWhileStatementAsync()
        {
            var testCode = @"namespace Foo
{
    public class Bar
    {
        public int Baz()
        {
            var sum = 0;

            do
            {
                sum += sum + 1;
            }
            while (sum < 1000);

            return sum;
        }

        public int Qux()
        {
            var sum = 0;

            do
            {
                sum += sum + 1;
            }
            // This should not trigger SA1511
            while (sum < 1000);

            return sum;
        }

        public int Quux()
        {
            var sum = 0;

            do
            {
                sum += sum + 1;
            }
#if (SHORT_TEST)
            while (sum < 100);
#else
            while (sum < 1000);
#endif

            return sum;
        }
    }
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that do ... while statements with (a) blank line(s) produce the expected diagnostic.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestInvalidDoWhileStatementAsync()
        {
            var testCode = @"namespace Foo
{
    public class Bar
    {
        public int Baz()
        {
            var sum = 0;

            do
            {
                sum += sum + 1;
            }

            while (sum < 1000);

            return sum;
        }

        public int Qux()
        {
            var sum = 0;

            do
            {
                sum += sum + 1;
            }


            while (sum < 1000);

            return sum;
        }

        public int Quux()
        {
            var sum = 0;

            do
            {
                sum += sum + 1;
            }
            /* This should trigger SA1511 */

            while (sum < 1000);

            return sum;
        }

        public int Corge()
        {
            var sum = 0;

            do
            {
                sum += sum + 1;
            }
#if (SHORT_TEST)
            while (sum < 100);
#else

            while (sum < 1000);
#endif

            return sum;
        }
    }
}
";

            var fixedTestCode = @"namespace Foo
{
    public class Bar
    {
        public int Baz()
        {
            var sum = 0;

            do
            {
                sum += sum + 1;
            }
            while (sum < 1000);

            return sum;
        }

        public int Qux()
        {
            var sum = 0;

            do
            {
                sum += sum + 1;
            }
            while (sum < 1000);

            return sum;
        }

        public int Quux()
        {
            var sum = 0;

            do
            {
                sum += sum + 1;
            }
            /* This should trigger SA1511 */
            while (sum < 1000);

            return sum;
        }

        public int Corge()
        {
            var sum = 0;

            do
            {
                sum += sum + 1;
            }
#if (SHORT_TEST)
            while (sum < 100);
#else
            while (sum < 1000);
#endif

            return sum;
        }
    }
}
";

            DiagnosticResult[] expectedDiagnostic =
            {
                this.CSharpDiagnostic().WithLocation(14, 13),
                this.CSharpDiagnostic().WithLocation(29, 13),
                this.CSharpDiagnostic().WithLocation(44, 13),
                this.CSharpDiagnostic().WithLocation(61, 13)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostic, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1511WhileDoFooterMustNotBePrecededByBlankLine();
        }

        /// <inheritdoc/>
        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new SA1511CodeFixProvider();
        }
    }
}
