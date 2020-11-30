// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.LayoutRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.LayoutRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.LayoutRules.SA1510ChainedStatementBlocksMustNotBePrecededByBlankLine,
        StyleCop.Analyzers.LayoutRules.SA1510CodeFixProvider>;

    /// <summary>
    /// Unit tests for <see cref="SA1510ChainedStatementBlocksMustNotBePrecededByBlankLine"/>.
    /// </summary>
    public class SA1510UnitTests
    {
        /// <summary>
        /// Verifies that valid chained statements will not produce any diagnostics.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestValidChainedStatementsAsync()
        {
            var testCode = @"using System;

namespace Foo
{
    public class Bar
    {
        public int TestElseStatement(int x)
        {
            if (x > 0)
            {
                return -x;
            }
            else
            {
                return x * x;
            }
        }

        public void TestCatchFinallyStatements()
        {
            var x = 0;

            try
            {
                x = x + 1;
            }
            catch (Exception)
            {
                x = 2;
            }
            finally
            {
                x = 3;
            }
        }
    }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that an else statement preceded by a blank line will report the expected diagnostic.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestInvalidElseStatementAsync()
        {
            var testCode = @"using System;

namespace Foo
{
    public class Bar
    {
        public int TestElseStatement(int x)
        {
            if (x > 0)
            {
                return -x;
            }

            else
            {
                return x * x;
            }
        }
    }
}
";

            var fixedTestCode = @"using System;

namespace Foo
{
    public class Bar
    {
        public int TestElseStatement(int x)
        {
            if (x > 0)
            {
                return -x;
            }
            else
            {
                return x * x;
            }
        }
    }
}
";

            DiagnosticResult[] expectedDiagnostics =
            {
                Diagnostic().WithLocation(14, 13).WithArguments("else"),
            };

            await VerifyCSharpFixAsync(testCode, expectedDiagnostics, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that an else statement preceded by a blank line will report the expected diagnostic.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestInvalidCatchFinallyStatementsAsync()
        {
            var testCode = @"using System;

namespace Foo
{
    public class Bar
    {
        public void TestCatchFinallyStatements()
        {
            var x = 0;

            try
            {
                x = x + 1;
            }

            catch (Exception)
            {
                x = 2;
            }

            finally
            {
                x = 3;
            }
        }
    }
}
";

            var fixedTestCode = @"using System;

namespace Foo
{
    public class Bar
    {
        public void TestCatchFinallyStatements()
        {
            var x = 0;

            try
            {
                x = x + 1;
            }
            catch (Exception)
            {
                x = 2;
            }
            finally
            {
                x = 3;
            }
        }
    }
}
";

            DiagnosticResult[] expectedDiagnostics =
            {
                Diagnostic().WithLocation(16, 13).WithArguments("catch"),
                Diagnostic().WithLocation(21, 13).WithArguments("finally"),
            };

            await VerifyCSharpFixAsync(testCode, expectedDiagnostics, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
