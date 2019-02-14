// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.ReadabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.ReadabilityRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        Analyzers.ReadabilityRules.SA110xQueryClauses,
        Analyzers.ReadabilityRules.SA1103CodeFixProvider>;

    /// <summary>
    /// Unit tests for the SA1103 analyzer.
    /// </summary>
    public class SA1103UnitTests
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
                Diagnostic(SA110xQueryClauses.SA1103Descriptor).WithLocation(11, 21),
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
                Diagnostic(SA110xQueryClauses.SA1103Descriptor).WithLocation(11, 21),
            };

            var test = new CSharpTest
            {
                TestCode = testCode,
                CodeActionIndex = 1,
                FixedCode = fixedTestCode,
            };

            test.ExpectedDiagnostics.AddRange(expectedDiagnostics);
            await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
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
                Diagnostic(SA110xQueryClauses.SA1103Descriptor).WithLocation(12, 17),
            };

            var test = new CSharpTest
            {
                TestCode = testCode,
                CodeActionIndex = 1,
                FixedCode = fixedTestCode,
            };

            test.ExpectedDiagnostics.AddRange(expectedDiagnostics);
            await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
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
                Diagnostic(SA110xQueryClauses.SA1103Descriptor).WithLocation(11, 21),
            };

            await VerifyCSharpFixAsync(testCode, expectedDiagnostics, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
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
                Diagnostic(SA110xQueryClauses.SA1103Descriptor).WithLocation(11, 21),
            };

            var test = new CSharpTest
            {
                TestCode = testCode,
                CodeActionIndex = 1,
                FixedCode = fixedTestCode,
            };

            test.ExpectedDiagnostics.AddRange(expectedDiagnostics);
            await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
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
                Diagnostic(SA110xQueryClauses.SA1103Descriptor).WithLocation(11, 21),
            };

            await VerifyCSharpFixAsync(testCode, expectedDiagnostics, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(2888, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2888")]
        public async Task TestQueryExpressionWithMissingSelectAsync()
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
                    where element > 1;
        }
    }
}
";

            var expectedDiagnostics = new DiagnosticResult("CS0742", DiagnosticSeverity.Error).WithLocation(12, 38).WithMessage("A query body must end with a select clause or a group clause");
            await VerifyCSharpFixAsync(testCode, expectedDiagnostics, testCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(2888, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2888")]
        public async Task TestQueryExpressionWithMissingSelect2Async()
        {
            var testCode = @"namespace TestNamespace
{
    using System.Linq;

    public class TestClass
    {
        private int[] testArray = { 1, 2, 3, 4, 5 };

        public void TestMethod()
        {
            var x = from element in testArray where element < 3
                    where element > 1;
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
                where element < 3
                    where element > 1;
        }
    }
}
";

            await new CSharpTest
            {
                TestState =
                {
                    Sources = { testCode },
                    ExpectedDiagnostics =
                    {
                        Diagnostic(SA110xQueryClauses.SA1103Descriptor).WithLocation(11, 21),
                        new DiagnosticResult("CS0742", DiagnosticSeverity.Error).WithLocation(12, 38).WithMessage("A query body must end with a select clause or a group clause"),
                    },
                },
                FixedState =
                {
                    Sources = { fixedTestCode },
                    ExpectedDiagnostics =
                    {
                        new DiagnosticResult("CS0742", DiagnosticSeverity.Error).WithLocation(13, 38).WithMessage("A query body must end with a select clause or a group clause"),
                    },
                },
                CodeActionIndex = 1,
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }
    }
}
