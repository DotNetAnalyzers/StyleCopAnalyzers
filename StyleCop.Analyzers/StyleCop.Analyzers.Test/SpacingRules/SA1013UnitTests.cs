namespace StyleCop.Analyzers.Test.SpacingRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.SpacingRules;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// Unit tests for <see cref="SA1013ClosingCurlyBracketsMustBeSpacedCorrectly"/>
    /// </summary>
    public class SA1013UnitTests : DiagnosticVerifier
    {
        /// <summary>
        /// Verifies that the analyzer will properly handle valid closing curly brackets.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestValidCurlyBracketSpacingAsync()
        {
            var testCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public void TestMethod()
        {
            var x = new { };
        }

        public int TestProperty { get; set; } 
    }
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will properly handle closing curly brackets in string interpolation.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestStringInterpolationAsync()
        {
            var testCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public void TestMethod()
        {
            var test = 2;
            var x = $""{test}"";
            x = $""{test}"";
            x = $""({test})"";
            x = $""({test} )"";
            x = $""{test }"";
            x = $""{test } "";
        }
    }
}
";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(12, 25).WithArguments(" not", "preceded"),
                this.CSharpDiagnostic().WithLocation(13, 25).WithArguments(" not", "preceded")
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will properly handle closing curly brackets in property declaration.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestPropertyDeclarationAsync()
        {
            var testCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public int TestProperty1 { get; set; }
        public int TestProperty2 { get; set;}
    }
}
";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(6, 45).WithArguments(string.Empty, "preceded")
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will properly handle closing curly brackets in nested curly brackets.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestNestedCurlyBracketsAsync()
        {
            var testCode = @"namespace TestNamespace
{
    using System.Collections.Generic;

    public class TestClass
    {
        public void TestMethod()
        {
            new Dictionary<int, int> { { 1, 1 } };
            new Dictionary<int, int> { { 1, 1} };
            new Dictionary<int, int> { { 1, 1 }};
            new Dictionary<int, int> { { 1, 1}};
        }
    }
}
";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(10, 46).WithArguments(string.Empty, "preceded"),
                this.CSharpDiagnostic().WithLocation(11, 47).WithArguments(string.Empty, "followed"),
                this.CSharpDiagnostic().WithLocation(11, 48).WithArguments(string.Empty, "preceded"),
                this.CSharpDiagnostic().WithLocation(12, 46).WithArguments(string.Empty, "preceded"),
                this.CSharpDiagnostic().WithLocation(12, 46).WithArguments(string.Empty, "followed"),
                this.CSharpDiagnostic().WithLocation(12, 47).WithArguments(string.Empty, "preceded")
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will properly handle closing curly brackets with a trailing comma.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestTrailingCommaAsync()
        {
            var testCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public void TestMethod()
        {
            object[] x = { new { }, new { } , new { } };
        }
    }
}
";

            // space between closing curly bracket and closing parenthesis should be reported by SA1001
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will properly handle closing curly brackets with a trailing semicolon.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestTrailingSemicolonAsync()
        {
            var testCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public void TestMethod()
        {
            new object { };
            new object { } ;
        }
    }
}
";

            // space between closing curly bracket and semicolon should be reported by SA1002
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will properly handle closing curly brackets in parentheses.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestParenthesesAsync()
        {
            var testCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public void TestMethod1(object[] a)
        {
        }        

        public void TestMethod2()
        {
            TestMethod1(new object[] { });
            TestMethod1(new object[] {});
            TestMethod1(new object[] { } );
            TestMethod1(new object[] {} );
        }
    }
}
";

            // space between closing curly bracket and closing parenthesis should be reported by SA1009
            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(12, 39).WithArguments(string.Empty, "preceded"),
                this.CSharpDiagnostic().WithLocation(14, 39).WithArguments(string.Empty, "preceded"),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1013ClosingCurlyBracketsMustBeSpacedCorrectly();
        }
    }
}
