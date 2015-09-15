// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

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
    /// Unit tests for the SA1103 analyzer.
    /// </summary>
    public class SA1103UnitTests : CodeFixVerifier
    {
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
            var x = from element in testArray where (element > 1)
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
            var x = from element in testArray where (element > 1) select element;
        }
    }
}
";

            DiagnosticResult[] expectedDiagnostics =
            {
                this.CSharpDiagnostic(SA110xQueryClauses.SA1103Descriptor).WithLocation(11, 21)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostics, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode, codeFixIndex: 0).ConfigureAwait(false);
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
            var x = from element in testArray where (element[0] > 1)
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
            var x = from element in testArray
                where (element[0] > 1)
                group element by element.Length;
        }
    }
}
";

            DiagnosticResult[] expectedDiagnostics =
            {
                this.CSharpDiagnostic(SA110xQueryClauses.SA1103Descriptor).WithLocation(11, 21)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostics, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode, codeFixIndex: 1).ConfigureAwait(false);
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
                this.CSharpDiagnostic(SA110xQueryClauses.SA1103Descriptor).WithLocation(12, 17)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostics, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode, codeFixIndex: 1).ConfigureAwait(false);
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
            var x = from element in testArray where (element > 1) /* test */
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
            var x = from element in testArray where (element > 1) /* test */ select element;
        }
    }
}
";

            DiagnosticResult[] expectedDiagnostics =
            {
                this.CSharpDiagnostic(SA110xQueryClauses.SA1103Descriptor).WithLocation(11, 21)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostics, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode, codeFixIndex: 0).ConfigureAwait(false);
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
            var x = from element in testArray where (element > 1) /* test */
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
            var x = from element in testArray
                where (element > 1) /* test */
                select element;
        }
    }
}
";

            DiagnosticResult[] expectedDiagnostics =
            {
                this.CSharpDiagnostic(SA110xQueryClauses.SA1103Descriptor).WithLocation(11, 21)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostics, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode, codeFixIndex: 1).ConfigureAwait(false);
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
            var x = from element in testArray where (element > 1)
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
            var x = from element in testArray
                where (element > 1)
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
                this.CSharpDiagnostic(SA110xQueryClauses.SA1103Descriptor).WithLocation(11, 21)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostics, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode, codeFixIndex: 0).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA110xQueryClauses();
        }

        /// <inheritdoc/>
        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new SA1103CodeFixProvider();
        }
    }
}
