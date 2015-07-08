namespace StyleCop.Analyzers.Test.ReadabilityRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.ReadabilityRules;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// Unit tests for the <see cref="SA1109BlockStatementsMustNotContainEmbeddedRegions"/> analyzer.
    /// </summary>
    public class SA1109UnitTests : CodeFixVerifier
    {
        /// <summary>
        /// Verifies that the analyzer will properly handle an empty source.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestEmptySourceAsync()
        {
            var testCode = string.Empty;
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will generate the correct diagnostics for the given statements.
        /// </summary>
        /// <param name="statement">The statement to test.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData("checked")]
        [InlineData("unchecked")]
        [InlineData("for (var i = 0; i < x; i++)")]
        [InlineData("foreach (var i in new[] { 1, 2, 3 })")]
        [InlineData("lock (this)")]
        [InlineData("unsafe")]
        [InlineData("using (var s = new System.IO.MemoryStream())")]
        [InlineData("while (x > y)")]
        public async Task TestStatementAsync(string statement)
        {
            var testCode = $@"namespace TestNamespace
{{
    public class TestClass
    {{
        public void TestMethod(int x, int y)
        {{
            {statement}
            #region
            {{
            }}
            #endregion
        }}
    }}
}}
";

            var fixedTestCode = $@"namespace TestNamespace
{{
    public class TestClass
    {{
        public void TestMethod(int x, int y)
        {{
            #region
            {statement}
            {{
            }}
            #endregion
        }}
    }}
}}
";

            DiagnosticResult[] expectedDiagnostics =
            {
                this.CSharpDiagnostic().WithLocation(8, 13)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostics, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will generate the correct diagnostics for the fixed statement.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestFixedStatementAsync()
        {
            var testCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public class HelperClass
        {
            public int A;
            public int B;
        }

        public unsafe void TestMethod(HelperClass x)
        {
            fixed (int* p = &x.A)
            #region
            {
                *p = 1;
            }
            #endregion
        }
    }
}
";

            var fixedTestCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public class HelperClass
        {
            public int A;
            public int B;
        }

        public unsafe void TestMethod(HelperClass x)
        {
            #region
            fixed (int* p = &x.A)
            {
                *p = 1;
            }
            #endregion
        }
    }
}
";

            DiagnosticResult[] expectedDiagnostics =
            {
                this.CSharpDiagnostic().WithLocation(14, 13)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostics, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will generate the correct diagnostics for the switch statement.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestSwitchStatementAsync()
        {
            var testCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public void TestMethod(int x, int y)
        {
            switch (x)
            #region
            {
                case 1:
                break;
            }
            #endregion
        }
    }
}
";

            var fixedTestCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public void TestMethod(int x, int y)
        {
        #region
        switch (x)
        {
            case 1:
            break;
        }
        #endregion
        }
    }
}
";

            DiagnosticResult[] expectedDiagnostics =
            {
                this.CSharpDiagnostic().WithLocation(8, 13)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostics, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will generate the correct diagnostics for the if ... else statement.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestIfElseStatementAsync()
        {
            var testCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public void TestMethod(int x, int y)
        {
            if (x > y)
            #region
            #endregion
            {
            }
            #region
            #endregion
            else if (x < y)
            #region
            #endregion
            {
            }
            #region
            #endregion
            else
            #region
            #endregion
            {
            }
        }
    }
}
";

            var fixedTestCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public void TestMethod(int x, int y)
        {
        }
    }
}
";

            DiagnosticResult[] expectedDiagnostics =
            {
                this.CSharpDiagnostic().WithLocation(8, 13),
                this.CSharpDiagnostic().WithLocation(12, 13),
                this.CSharpDiagnostic().WithLocation(15, 13),
                this.CSharpDiagnostic().WithLocation(19, 13),
                this.CSharpDiagnostic().WithLocation(22, 13)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostics, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will generate the correct diagnostics for the do ... while statement.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestDoWhileStatementAsync()
        {
            var testCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public void TestMethod(int x, int y)
        {
            do
            #region
            #endregion
            {
            }
            #region
            #endregion
            while (x < y);
        }
    }
}
";

            var fixedTestCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public void TestMethod(int x, int y)
        {
        }
    }
}
";

            DiagnosticResult[] expectedDiagnostics =
            {
                this.CSharpDiagnostic().WithLocation(8, 13),
                this.CSharpDiagnostic().WithLocation(12, 13)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostics, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will generate the correct diagnostics for the try ... catch ... finally statement.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestTryCatchStatementAsync()
        {
            var testCode = @"namespace TestNamespace
{
    using System;

    public class TestClass
    {
        public void TestMethod(int x, int y)
        {
            try
            #region
            #endregion
            {
            }
            #region
            #endregion
            catch (InvalidOperationException ex)
            #region
            #endregion
            {
            }
            #region
            #endregion
            catch
            #region
            #endregion
            {
            }
            #region
            #endregion
            finally
            #region
            #endregion
            {
            }
        }
    }
}
";

            var fixedTestCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public void TestMethod(int x, int y)
        {
        }
    }
}
";

            DiagnosticResult[] expectedDiagnostics =
            {
                this.CSharpDiagnostic().WithLocation(10, 13),
                this.CSharpDiagnostic().WithLocation(14, 13),
                this.CSharpDiagnostic().WithLocation(17, 13),
                this.CSharpDiagnostic().WithLocation(21, 13),
                this.CSharpDiagnostic().WithLocation(24, 13),
                this.CSharpDiagnostic().WithLocation(28, 13),
                this.CSharpDiagnostic().WithLocation(31, 13)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostics, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        //// TODO: Test exotic stuff like multiple regions
        //// TODO: Test SA1124 interaction

        /// <inheritdoc/>
        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1109BlockStatementsMustNotContainEmbeddedRegions();
        }

        /// <inheritdoc/>
        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new SA1109CodeFixProvider();
        }
    }
}
