﻿// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.SpacingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.SpacingRules;
    using TestHelper;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.SpacingRules.SA1013ClosingBracesMustBeSpacedCorrectly,
        StyleCop.Analyzers.SpacingRules.TokenSpacingCodeFixProvider>;

    /// <summary>
    /// Unit tests for <see cref="SA1013ClosingBracesMustBeSpacedCorrectly"/>.
    /// </summary>
    public class SA1013UnitTests
    {
        /// <summary>
        /// Verifies that the analyzer will properly handle valid closing braces.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestValidBraceSpacingAsync()
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

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will properly handle closing braces in string interpolation.
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
                Diagnostic().WithLocation(12, 25).WithArguments(" not", "preceded"),
                Diagnostic().WithLocation(13, 25).WithArguments(" not", "preceded"),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will properly handle closing braces in property declaration.
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
                Diagnostic().WithLocation(6, 45).WithArguments(string.Empty, "preceded"),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will properly handle closing braces in nested braces.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestNestedBracesAsync()
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
                Diagnostic().WithLocation(10, 46).WithArguments(string.Empty, "preceded"),
                Diagnostic().WithLocation(11, 47).WithArguments(string.Empty, "followed"),
                Diagnostic().WithLocation(11, 48).WithArguments(string.Empty, "preceded"),
                Diagnostic().WithLocation(12, 46).WithArguments(string.Empty, "preceded"),
                Diagnostic().WithLocation(12, 46).WithArguments(string.Empty, "followed"),
                Diagnostic().WithLocation(12, 47).WithArguments(string.Empty, "preceded"),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will properly handle closing braces with a trailing comma.
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

            // space between closing brace and closing parenthesis should be reported by SA1001
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will properly handle closing braces with a trailing semicolon.
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

            // space between closing brace and semicolon should be reported by SA1002
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will properly handle closing braces with a trailing dot.
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

            // space between closing brace and dot should be reported by SA1019
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will properly handle closing braces with a trailing question dot token.
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

            // space between closing brace and question dot should be reported by SA1019
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will properly handle closing braces in parentheses.
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

            // space between closing brace and closing parenthesis should be reported by SA1009
            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(12, 39).WithArguments(string.Empty, "preceded"),
                Diagnostic().WithLocation(14, 39).WithArguments(string.Empty, "preceded"),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will properly handle anonymous classes in indexers.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(1191, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1191")]
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

            // no space between closing brace and closing bracket should not be reported by SA1013
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will properly handle end of file without a new line.
        /// </summary>
        /// <param name="declarationType">The declaration type.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData("namespace")]
        [InlineData("class")]
        [InlineData("interface")]
        [InlineData("struct")]
        [InlineData("enum")]
        [WorkItem(685, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/685")]
        public async Task TestEndOfFileWithoutNewLineAsync(string declarationType)
        {
            var testCode = $"{declarationType} TestItem {{ }}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults).ConfigureAwait(false);
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
                DiagnosticResult.CompilerError("CS1513").WithMessage("} expected").WithLocation(3, 2),
            };

            await VerifyCSharpDiagnosticAsync(testCode, expected).ConfigureAwait(false);
        }
    }
}
