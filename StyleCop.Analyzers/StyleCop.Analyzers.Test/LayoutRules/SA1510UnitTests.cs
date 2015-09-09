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
    /// Unit tests for <see cref="SA1510ChainedStatementBlocksMustNotBePrecededByBlankLine"/>
    /// </summary>
    public class SA1510UnitTests : CodeFixVerifier
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
                this.CSharpDiagnostic().WithLocation(14, 13).WithArguments("else")
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostics, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
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
                this.CSharpDiagnostic().WithLocation(16, 13).WithArguments("catch"),
                this.CSharpDiagnostic().WithLocation(21, 13).WithArguments("finally")
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostics, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1510ChainedStatementBlocksMustNotBePrecededByBlankLine();
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new SA1510CodeFixProvider();
        }
    }
}
