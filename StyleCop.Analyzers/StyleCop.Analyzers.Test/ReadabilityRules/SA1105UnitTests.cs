// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.ReadabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.ReadabilityRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.ReadabilityRules.SA110xQueryClauses,
        StyleCop.Analyzers.ReadabilityRules.SA1104SA1105CodeFixProvider>;

    /// <summary>
    /// Unit tests for the SA1105 analyzer.
    /// </summary>
    public class SA1105UnitTests
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
                from element in new[] { 1, 2, 3 } select TestMethod1
                (
                    element
                );
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
                from element in new[] { 1, 2, 3 }
                select TestMethod1
                (
                    element
                );
        }
    }
}
";

            DiagnosticResult[] expectedDiagnostics =
            {
                Diagnostic(SA110xQueryClauses.SA1105Descriptor).WithLocation(15, 51),
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
        public int TestMethod1(int x)
        {
            return x * x;
        }

        public void TestMethod2()
        {
            var x =
                from element in new[] { 1, 2, 3 } group element by TestMethod1
                (
                    element
                );
        }
    }
}
";

            var fixedTestCode = @"namespace TestNamespace
{
    using System.Linq;

    public class TestClass
    {
        public int TestMethod1(int x)
        {
            return x * x;
        }

        public void TestMethod2()
        {
            var x =
                from element in new[] { 1, 2, 3 }
                group element by TestMethod1
                (
                    element
                );
        }
    }
}
";

            DiagnosticResult[] expectedDiagnostics =
            {
                Diagnostic(SA110xQueryClauses.SA1105Descriptor).WithLocation(15, 51),
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
                into g where g.Any(
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
                Diagnostic(SA110xQueryClauses.SA1105Descriptor).WithLocation(14, 24),
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
                from element in new[] { 1, 2, 3 } /* test */ select TestMethod1
                (
                    element
                );
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
                from element in new[] { 1, 2, 3 } /* test */
                select TestMethod1
                (
                    element
                );
        }
    }
}
";

            DiagnosticResult[] expectedDiagnostics =
            {
                Diagnostic(SA110xQueryClauses.SA1105Descriptor).WithLocation(15, 62),
            };

            await VerifyCSharpFixAsync(testCode, expectedDiagnostics, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
