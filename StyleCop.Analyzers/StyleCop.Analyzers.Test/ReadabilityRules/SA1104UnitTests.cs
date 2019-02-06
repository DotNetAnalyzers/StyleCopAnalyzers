// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.ReadabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.ReadabilityRules;
    using TestHelper;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.ReadabilityRules.SA110xQueryClauses,
        StyleCop.Analyzers.ReadabilityRules.SA1104SA1105CodeFixProvider>;

    /// <summary>
    /// Unit tests for the SA1104 analyzer.
    /// </summary>
    public class SA1104UnitTests
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
        public int[] TestMethod1(int x)
        {
            return new[] { x };
        }

        public void TestMethod2()
        {
            var x =
                from element in TestMethod1
                (
                    12
                ) select element;
        }
    }
}
";

            var fixedTestCode = @"namespace TestNamespace
{
    using System.Linq;

    public class TestClass
    {
        public int[] TestMethod1(int x)
        {
            return new[] { x };
        }

        public void TestMethod2()
        {
            var x =
                from element in TestMethod1
                (
                    12
                )
                select element;
        }
    }
}
";

            DiagnosticResult[] expectedDiagnostics =
            {
                Diagnostic(SA110xQueryClauses.SA1104Descriptor).WithLocation(18, 19),
            };

            await VerifyCSharpFixAsync(testCode, expectedDiagnostics, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
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
        public int[][] TestMethod1(int x)
        {
            return new[] { new[] { x } };
        }

        public void TestMethod2()
        {
            var x = from element in TestMethod1
                (
                    12
                ) group element by element.Length;
        }
    }
}
";

            var fixedTestCode = @"namespace TestNamespace
{
    using System.Linq;

    public class TestClass
    {
        public int[][] TestMethod1(int x)
        {
            return new[] { new[] { x } };
        }

        public void TestMethod2()
        {
            var x = from element in TestMethod1
                (
                    12
                )
                group element by element.Length;
        }
    }
}
";

            DiagnosticResult[] expectedDiagnostics =
            {
                Diagnostic(SA110xQueryClauses.SA1104Descriptor).WithLocation(17, 19),
            };

            await VerifyCSharpFixAsync(testCode, expectedDiagnostics, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
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
                where g.Any(
                    t => true) orderby g.Key
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
                where g.Any(
                    t => true)
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
                Diagnostic(SA110xQueryClauses.SA1104Descriptor).WithLocation(16, 32),
            };

            await VerifyCSharpFixAsync(testCode, expectedDiagnostics, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
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
        public int[] TestMethod1(int x)
        {
            return new[] { x };
        }

        public void TestMethod2()
        {
            var x =
                from element in TestMethod1
                (
                    12
                ) /* test */ select element;
        }
    }
}
";

            var fixedTestCode = @"namespace TestNamespace
{
    using System.Linq;

    public class TestClass
    {
        public int[] TestMethod1(int x)
        {
            return new[] { x };
        }

        public void TestMethod2()
        {
            var x =
                from element in TestMethod1
                (
                    12
                ) /* test */
                select element;
        }
    }
}
";

            DiagnosticResult[] expectedDiagnostics =
            {
                Diagnostic(SA110xQueryClauses.SA1104Descriptor).WithLocation(18, 30),
            };

            await VerifyCSharpFixAsync(testCode, expectedDiagnostics, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
