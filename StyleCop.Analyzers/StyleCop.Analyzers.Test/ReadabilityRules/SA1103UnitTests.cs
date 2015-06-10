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
    /// Unit tests for the <see cref="SA1103QueryClausesMustBeOnSeparateLinesOrAllOnOneLine"/> class.
    /// </summary>
    public class SA1103UnitTests : CodeFixVerifier
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
        /// Verifies that a select query expression produces the expected results.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestSelectQueryExpressionAsync()
        {
            var testCode = @"namespace TestNamespace
{
    using System.Linq;

    public class TestClass
    {
        private int[] testArray = { 1, 2, 3, 4, 5 };

        public void TestMethod()
        {
            var x = from element in testArray
                select element;
        }
    }
}
";

            var fixedTestCode = @"namespace TestNamespace
{
    using System.Linq;

    public class TestClass
    {
        private int[] testArray = { 1, 2, 3, 4, 5 };

        public void TestMethod()
        {
            var x = from element in testArray select element;
        }
    }
}
";

            DiagnosticResult[] expectedDiagnostics =
            {
                this.CSharpDiagnostic().WithLocation(11, 21)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostics, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode, 0).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a group query expression produces the expected results.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestGroupQueryExpressionAsync()
        {
            var testCode = @"namespace TestNamespace
{
    using System.Linq;

    public class TestClass
    {
        private int[][] testArray = { new[] { 1, 2 }, new[] { 3, 4, 5 }, new[] { 6, 7 }, new[] { 8, 9 } };

        public void TestMethod()
        {
            var x = from element in testArray
                group element by element.Length;
        }
    }
}
";

            var fixedTestCode = @"namespace TestNamespace
{
    using System.Linq;

    public class TestClass
    {
        private int[][] testArray = { new[] { 1, 2 }, new[] { 3, 4, 5 }, new[] { 6, 7 }, new[] { 8, 9 } };

        public void TestMethod()
        {
            var x =
                from element in testArray
                group element by element.Length;
        }
    }
}
";

            DiagnosticResult[] expectedDiagnostics =
            {
                this.CSharpDiagnostic().WithLocation(11, 21)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostics, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode, 1).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a continuation query expression produces the expected results.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestContinuationQueryExpressionAsync()
        {
            var testCode = @"namespace TestNamespace
{
    using System.Linq;

    public class TestClass
    {
        private int[][] testArray = { new[] { 1, 2 }, new[] { 3, 4, 5 }, new[] { 6, 7 }, new[] { 8, 9 } };

        public void TestMethod()
        {
            var x =
                from element in testArray
                group element by element.Length
                into g
                orderby g.Key select g;

            // verify that the placement of the into keyword is irrelevant (and preserved)
            var y =
                from element in testArray
                group element by element.Length into g
                orderby g.Key
                select g;
        }
    }
}
";

            var fixedTestCode = @"namespace TestNamespace
{
    using System.Linq;

    public class TestClass
    {
        private int[][] testArray = { new[] { 1, 2 }, new[] { 3, 4, 5 }, new[] { 6, 7 }, new[] { 8, 9 } };

        public void TestMethod()
        {
            var x =
                from element in testArray
                group element by element.Length
                into g
                orderby g.Key
                select g;

            // verify that the placement of the into keyword is irrelevant (and preserved)
            var y =
                from element in testArray
                group element by element.Length into g
                orderby g.Key
                select g;
        }
    }
}
";

            DiagnosticResult[] expectedDiagnostics =
            {
                this.CSharpDiagnostic().WithLocation(12, 17)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostics, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode, 1).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a query expression with inline comment trivia produces the expected results.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestQueryExpressionWithInlineCommentAsync()
        {
            var testCode = @"namespace TestNamespace
{
    using System.Linq;

    public class TestClass
    {
        private int[] testArray = { 1, 2, 3, 4, 5 };

        public void TestMethod()
        {
            var x = from element in testArray /* test */
                select element;
        }
    }
}
";

            var fixedTestCode = @"namespace TestNamespace
{
    using System.Linq;

    public class TestClass
    {
        private int[] testArray = { 1, 2, 3, 4, 5 };

        public void TestMethod()
        {
            var x = from element in testArray /* test */ select element;
        }
    }
}
";

            DiagnosticResult[] expectedDiagnostics =
            {
                this.CSharpDiagnostic().WithLocation(11, 21)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostics, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode, 0).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a query expression with inline comment trivia produces the expected results when using the multiple line code fix.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestQueryExpressionWithInlineCommentWithMultiLineFixAsync()
        {
            var testCode = @"namespace TestNamespace
{
    using System.Linq;

    public class TestClass
    {
        private int[] testArray = { 1, 2, 3, 4, 5 };

        public void TestMethod()
        {
            var x = from element in testArray /* test */
                select element;
        }
    }
}
";

            var fixedTestCode = @"namespace TestNamespace
{
    using System.Linq;

    public class TestClass
    {
        private int[] testArray = { 1, 2, 3, 4, 5 };

        public void TestMethod()
        {
            var x =
                from element in testArray /* test */
                select element;
        }
    }
}
";

            DiagnosticResult[] expectedDiagnostics =
            {
                this.CSharpDiagnostic().WithLocation(11, 21)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostics, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode, 1).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a query expression with single line comment trivia produces the expected results.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestQueryExpressionWithSingleLineCommentAsync()
        {
            var testCode = @"namespace TestNamespace
{
    using System.Linq;

    public class TestClass
    {
        private int[] testArray = { 1, 2, 3, 4, 5 };

        public void TestMethod()
        {
            var x = from element in testArray
                // test
                select element;
        }
    }
}
";

            var fixedTestCode = @"namespace TestNamespace
{
    using System.Linq;

    public class TestClass
    {
        private int[] testArray = { 1, 2, 3, 4, 5 };

        public void TestMethod()
        {
            var x =
                from element in testArray
                // test
                select element;
        }
    }
}
";

            DiagnosticResult[] expectedDiagnostics =
            {
                this.CSharpDiagnostic().WithLocation(11, 21)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostics, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode, 0).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a query expression with directive trivia produces the expected results.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestQueryExpressionWithDirectivesAsync()
        {
            var testCode = @"namespace TestNamespace
{
    using System.Linq;

    public class TestClass
    {
        private int[] testArray = { 1, 2, 3, 4, 5 };

        public void TestMethod()
        {
            var x = from element in testArray
#if TEST
                select element;
#else
                select -element;
#endif
        }
    }
}
";

            var fixedTestCode = @"namespace TestNamespace
{
    using System.Linq;

    public class TestClass
    {
        private int[] testArray = { 1, 2, 3, 4, 5 };

        public void TestMethod()
        {
            var x =
                from element in testArray
#if TEST
                select element;
#else
                select -element;
#endif
        }
    }
}
";

            DiagnosticResult[] expectedDiagnostics =
            {
                this.CSharpDiagnostic().WithLocation(11, 21)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostics, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode, 0).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1103QueryClausesMustBeOnSeparateLinesOrAllOnOneLine();
        }

        /// <inheritdoc/>
        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new SA1103CodeFixProvider();
        }
    }
}
