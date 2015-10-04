// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.SpacingRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.SpacingRules;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// Unit tests for <see cref="SA1013ClosingCurlyBracketsMustBeSpacedCorrectly"/>
    /// </summary>
    public class SA1013UnitTests : CodeFixVerifier
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

            var fixedCode = @"namespace TestNamespace
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
            x = $""{test}"";
            x = $""{test} "";
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
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
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

            var fixedCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public int TestProperty1 { get; set; }
        public int TestProperty2 { get; set; }
    }
}
";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(6, 45).WithArguments(string.Empty, "preceded")
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
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

            var fixedCode = @"namespace TestNamespace
{
    using System.Collections.Generic;

    public class TestClass
    {
        public void TestMethod()
        {
            new Dictionary<int, int> { { 1, 1 } };
            new Dictionary<int, int> { { 1, 1 } };
            new Dictionary<int, int> { { 1, 1 } };
            new Dictionary<int, int> { { 1, 1 } };
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
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
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
        /// Verifies that the analyzer will properly handle closing curly brackets with a trailing dot.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestTrailingDotAsync()
        {
            var testCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public void TestMethod()
        {
            new object { }.ToString();
            new object[] { }.ToString();
        }
    }
}
";

            // space between closing curly bracket and dot should be reported by SA1019
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will properly handle closing curly brackets with a trailing question dot token.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestTrailingQuestionDotAsync()
        {
            var testCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public void TestMethod()
        {
            new object { }?.ToString();
            new object[] { }?.ToString();
        }
    }
}
";

            // space between closing curly bracket and question dot should be reported by SA1019
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

            var fixedCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public void TestMethod1(object[] a)
        {
        }        

        public void TestMethod2()
        {
            TestMethod1(new object[] { });
            TestMethod1(new object[] { });
            TestMethod1(new object[] { } );
            TestMethod1(new object[] { } );
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
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will properly handle anonymous classes in indexers.
        /// This is a regression test for <see href="https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1191">DotNetAnalyzers/StyleCopAnalyzers#1191</see>
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestIndexersAsync()
        {
            var testCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public void TestMethod()
        {
            var dictionary = new System.Collections.Generic.Dictionary<object, object>();
            dictionary[new { Foo = ""Foo"", Bar = 5 }] = 42;
        }
    }
}
";

            // no space between closing curly bracket and closing bracket should not be reported by SA1013
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will properly handle end of file without a new line.
        /// This is a regression test for <see href="https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/685">DotNetAnalyzers/StyleCopAnalyzers#685</see>
        /// </summary>
        /// <param name="declarationType">The declaration type.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData("namespace")]
        [InlineData("class")]
        [InlineData("interface")]
        [InlineData("struct")]
        [InlineData("enum")]
        public async Task TestEndOfFileWithoutNewLineAsync(string declarationType)
        {
            var testCode = $"{declarationType} TestItem {{ }}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMissingTokenAsync()
        {
            string testCode = @"
class ClassName
{
";

            DiagnosticResult[] expected =
            {
                new DiagnosticResult
                {
                    Id = "CS1513",
                    Severity = DiagnosticSeverity.Error,
                    Message = "} expected",
                    Locations = new[] { new DiagnosticResultLocation("Test0.cs", 3, 2) }
                }
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1013ClosingCurlyBracketsMustBeSpacedCorrectly();
        }

        /// <inheritdoc/>
        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new TokenSpacingCodeFixProvider();
        }
    }
}
