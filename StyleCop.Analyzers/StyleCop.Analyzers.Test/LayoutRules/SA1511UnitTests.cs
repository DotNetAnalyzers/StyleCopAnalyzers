// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.LayoutRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.LayoutRules;
    using TestHelper;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.LayoutRules.SA1511WhileDoFooterMustNotBePrecededByBlankLine,
        StyleCop.Analyzers.LayoutRules.SA1511CodeFixProvider>;

    /// <summary>
    /// Unit tests for <see cref="SA1511WhileDoFooterMustNotBePrecededByBlankLine"/>.
    /// </summary>
    public class SA1511UnitTests
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

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
                Diagnostic().WithLocation(14, 13),
                Diagnostic().WithLocation(29, 13),
                Diagnostic().WithLocation(44, 13),
                Diagnostic().WithLocation(61, 13),
            };

            await VerifyCSharpFixAsync(testCode, expectedDiagnostic, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
