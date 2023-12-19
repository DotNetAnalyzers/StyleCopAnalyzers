// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.SpacingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.SpacingRules;
    using Xunit;
    using static StyleCop.Analyzers.SpacingRules.SA1009ClosingParenthesisMustBeSpacedCorrectly;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.SpacingRules.SA1009ClosingParenthesisMustBeSpacedCorrectly,
        StyleCop.Analyzers.SpacingRules.TokenSpacingCodeFixProvider>;

    /// <summary>
    /// Unit tests for <see cref="SA1009ClosingParenthesisMustBeSpacedCorrectly"/>.
    /// </summary>
    public class SA1009UnitTests
    {
        [Fact]
        public async Task TestMethodWithNoParametersAsync()
        {
            const string testCode = @"using System;

public class Foo
{
    public void Method()
    {
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMethodWithWhitespaceBeforeClosingParenthesisAsync()
        {
            const string testCode = @"using System;

public class Foo
{
    public void Method( )
    {
    }
}";
            const string fixedCode = @"using System;

public class Foo
{
    public void Method()
    {
    }
}";

            DiagnosticResult expected = Diagnostic(DescriptorNotPreceded).WithLocation(5, 25);

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestDocumentationMethodReferenceWithWhitespaceBeforeClosingParenthesisAsync()
        {
            const string testCode = @"
public class Foo
{
    /// <see cref=""Method( )""/>
    public void Method()
    {
    }
}";
            const string fixedCode = @"
public class Foo
{
    /// <see cref=""Method()""/>
    public void Method()
    {
    }
}";

            DiagnosticResult expected = Diagnostic(DescriptorNotPreceded).WithLocation(4, 28);

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(2985, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2985")]
        public async Task TestDocumentationMethodReferenceInSingleQuotesWithWhitespaceAfterClosingParenthesisAsync()
        {
            const string testCode = @"
public class Foo
{
    /// <see cref='Method({|#0:)|} '/>
    public void Method()
    {
    }
}";

            const string fixedCode = @"
public class Foo
{
    /// <see cref='Method()'/>
    public void Method()
    {
    }
}";

            DiagnosticResult expected = Diagnostic(DescriptorNotFollowed).WithLocation(0);

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMethodWith2CorrectlySpacedParametersAsync()
        {
            const string testCode = @"using System;

public class Foo
{
    public void Method(int param1, int param2)
    {
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMethodWith2ParametersAndSpaceBeforeClosingParenthesisAsync()
        {
            const string testCode = @"using System;

public class Foo
{
    public void Method(int param1, int param2 )
    {
    }
}";

            const string fixedCode = @"using System;

public class Foo
{
    public void Method(int param1, int param2)
    {
    }
}";

            DiagnosticResult expected = Diagnostic(DescriptorNotPreceded).WithLocation(5, 47);

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestAttributeWithParametersAndSpaceAfterClosingParenthesisAsync()
        {
            const string testCode = @"using System;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Auto, CharSet = CharSet.Auto{|#0:)|} ]
public class Foo
{
    public void Method(int param1, int param2)
    {
    }
}";

            const string fixedCode = @"using System;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Auto, CharSet = CharSet.Auto)]
public class Foo
{
    public void Method(int param1, int param2)
    {
    }
}";

            DiagnosticResult expected = Diagnostic(DescriptorNotFollowed).WithLocation(0);

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConstructorInheritenceWithSpaceAfterClosingParenthesisAsync()
        {
            const string testCode = @"using System;

public class Foo
{
    public Foo(int i) : base()
    {
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConstructorInheritenceWithNoSpaceAfterClosingParenthesisAsync()
        {
            const string testCode = @"using System;

public class Foo
{
    public Foo(int i): base()
    {
    }
}";

            const string fixedCode = @"using System;

public class Foo
{
    public Foo(int i) : base()
    {
    }
}";

            DiagnosticResult expected = Diagnostic(DescriptorFollowed).WithLocation(5, 21);

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLambdaExpressionWithNoSpaceAfterClosingParenthesisAsync()
        {
            var invalidStatement = @"System.EventHandler handler = (s, e)=> { };";
            var validStatement = @"System.EventHandler handler = (s, e) => { };";

            DiagnosticResult expected = Diagnostic(DescriptorFollowed).WithLocation(7, 48);

            await this.TestWhitespaceInStatementOrDeclAsync(invalidStatement, validStatement, expected).ConfigureAwait(false);
        }

        /// <summary>
        /// This is a regression test for DotNetAnalyzers/StyleCopAnalyzers#255 and
        /// DotNetAnalyzers/StyleCopAnalyzers#256.
        /// </summary>
        /// <param name="operatorToken">The operator to test.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [Theory]
        [InlineData("+")]
        [InlineData("-")]
        public async Task TestNotReportedWhenFollowedByUnaryPlusOrMinusAsync(string operatorToken)
        {
            // This will be reported as SA1021 or SA1022
            var ignoredStatement = $"var i = (int) {operatorToken}2;";
            var correctStatement = $"var i = (int){operatorToken}2;";

            await this.TestWhitespaceInStatementOrDeclAsync(ignoredStatement, null, DiagnosticResult.EmptyDiagnosticResults).ConfigureAwait(false);
            await this.TestWhitespaceInStatementOrDeclAsync(correctStatement, null, DiagnosticResult.EmptyDiagnosticResults).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestSpaceBeforeParenthisInIncrementingForLoopAsync()
        {
            var invalidStatement = @"for (int i = 0; i < 10; i++ )
            {
            }";
            var validStatement = @"for (int i = 0; i < 10; i++)
            {
            }";

            DiagnosticResult expected = Diagnostic(DescriptorNotPreceded).WithLocation(7, 41);

            await this.TestWhitespaceInStatementOrDeclAsync(invalidStatement, validStatement, expected).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestSpaceBeforeParenthisInDecrementingForLoopAsync()
        {
            var invalidStatement = @"for (int i = 0; i < 10; i-- )
            {
            }";
            var validStatement = @"for (int i = 0; i < 10; i--)
            {
            }";

            DiagnosticResult expected = Diagnostic(DescriptorNotPreceded).WithLocation(7, 41);

            await this.TestWhitespaceInStatementOrDeclAsync(invalidStatement, validStatement, expected).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestSpaceInCastAsync()
        {
            var invalidStatement = @"var i = (int) 1;";
            var validStatement = @"var i = (int)1;";

            DiagnosticResult expected = Diagnostic(DescriptorNotFollowed).WithLocation(7, 25);

            await this.TestWhitespaceInStatementOrDeclAsync(invalidStatement, validStatement, expected).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestSpaceInConstructorCallAsync()
        {
            var invalidStatement = @"var o = new object() ;";
            var validStatement = @"var o = new object();";

            DiagnosticResult expected = Diagnostic(DescriptorNotFollowed).WithLocation(7, 32);

            await this.TestWhitespaceInStatementOrDeclAsync(invalidStatement, validStatement, expected).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestSpaceMethodCallFollowedByPropertyGetAsync()
        {
            var invalidStatement = @"var o = new Baz() .Test;";
            var validStatement = @"var o = new Baz().Test;";

            DiagnosticResult expected = Diagnostic(DescriptorNotFollowed).WithLocation(7, 29);

            await this.TestWhitespaceInStatementOrDeclAsync(invalidStatement, validStatement, expected).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestSpaceMethodCallFollowedByConditionalAccessPropertyGetAsync()
        {
            var invalidStatement = @"var o = new Baz() ?.Test;";
            var validStatement = @"var o = new Baz()?.Test;";

            DiagnosticResult expected = Diagnostic(DescriptorNotFollowed).WithLocation(7, 29);

            await this.TestWhitespaceInStatementOrDeclAsync(invalidStatement, validStatement, expected).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestSpaceMethodCallFollowedByPointerDereferenceAsync()
        {
            var invalidStatement = @"var o = GetPointer() ->ToString();";
            var validStatement = @"var o = GetPointer()->ToString();";

            DiagnosticResult expected = Diagnostic(DescriptorNotFollowed).WithLocation(7, 32);

            await this.TestWhitespaceInStatementOrDeclAsync(invalidStatement, validStatement, expected).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestSpaceOperationInDoubleSetOfParenthesisAsync()
        {
            var invalidStatement = @"var o = ((1 + 1) );";
            var validStatement = @"var o = ((1 + 1));";

            DiagnosticResult[] expected =
            {
                Diagnostic(DescriptorNotFollowed).WithLocation(7, 28),
                Diagnostic(DescriptorNotPreceded).WithLocation(7, 30),
            };

            await this.TestWhitespaceInStatementOrDeclAsync(invalidStatement, validStatement, expected).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("++")]
        [InlineData("--")]
        public async Task TestSpaceIncrementOrDecrementOperatorFollowingParenthesisAsync(string operatorValue)
        {
            var invalidStatement = string.Format(
                @"int i = 0;
            (i) {0};",
                operatorValue);
            var validStatement = string.Format(
                @"int i = 0;
            (i){0};",
                operatorValue);

            DiagnosticResult expected = Diagnostic(DescriptorNotFollowed).WithLocation(8, 15);

            await this.TestWhitespaceInStatementOrDeclAsync(invalidStatement, validStatement, expected).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestSpaceBetweenClosingBraceAndParenthesisAsync()
        {
            var invalidStatement = @"var x = new System.Action(() => { } );";
            var validStatement = @"var x = new System.Action(() => { });";

            DiagnosticResult expected = Diagnostic(DescriptorNotPreceded).WithLocation(7, 49);

            await this.TestWhitespaceInStatementOrDeclAsync(invalidStatement, validStatement, expected).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestSpaceClosingParenthesisFollowedByParenthesisPairAsync()
        {
            var invalidStatement = @"new System.Action(() => { }) ();";
            var validStatement = @"new System.Action(() => { })();";

            DiagnosticResult expected = Diagnostic(DescriptorNotFollowed).WithLocation(7, 40);

            await this.TestWhitespaceInStatementOrDeclAsync(invalidStatement, validStatement, expected).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestSpaceParenthesisFollowedByBracketAsync()
        {
            var invalidStatement = @"var a = GetA() [0];";
            var validStatement = @"var a = GetA()[0];";

            DiagnosticResult expected = Diagnostic(DescriptorNotFollowed).WithLocation(7, 26);

            await this.TestWhitespaceInStatementOrDeclAsync(invalidStatement, validStatement, expected).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestNoSpaceParenthesisFollowedByColonAsync()
        {
            var invalidStatement = @"var x = true ? GetA(): GetB();";
            var validStatement = @"var x = true ? GetA() : GetB();";

            DiagnosticResult expected = Diagnostic(DescriptorFollowed).WithLocation(7, 33);

            await this.TestWhitespaceInStatementOrDeclAsync(invalidStatement, validStatement, expected).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestNoSpaceParenthesisFollowedByQuestionAsync()
        {
            var invalidStatement = @"var x = (true == true)? GetA() : GetB();";
            var validStatement = @"var x = (true == true) ? GetA() : GetB();";

            DiagnosticResult expected = Diagnostic(DescriptorFollowed).WithLocation(7, 34);

            await this.TestWhitespaceInStatementOrDeclAsync(invalidStatement, validStatement, expected).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestWithSpaceFollowingInInterpolatedStringAsync()
        {
            var invalidStatement = @"var x = $""{typeof(string).ToString() }"";";
            var validStatement = @"var x = $""{typeof(string).ToString()}"";";

            DiagnosticResult expected = Diagnostic(DescriptorNotFollowed).WithLocation(7, 48);

            await this.TestWhitespaceInStatementOrDeclAsync(invalidStatement, validStatement, expected).ConfigureAwait(false);
        }

        /// <summary>
        /// This is a regression test for "SA1009: mis-fires for verbatim strings".
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(1064, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1064")]
        public async Task TestVerbatimStringArgumentAsync()
        {
            string testCode = @"using System;
public class Foo
{
    public void Method()
    {
        Console.WriteLine(@""
    Written content...
"");
    }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies the analyzer will properly handle trailing single line comments.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestTrailingSingleLineCommentAsync()
        {
            var testCode = @"
public class TestClass
{
    public bool TestMethod1() { return true; }

    public void TestMethod2()
    {
        TestMethod1()// some comment
;
        TestMethod3(
            TestMethod1()// some comment
            , TestMethod1());

        // This is a regression test for https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1206
        TestMethod3(
            true,
            (false || true) // comment
        );

        // This is a regression test for https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1206
        if (
            true || // Comment 1
            false // Comment 2
           )
        {
        }

        // This is a regression test for https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1206
        if (
            true || // Comment 1
            false
    // Comment 2
           )
        {
        }

        // This is a regression test for https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1206
        if (
            true || // Comment 1
            false
    // Comment 2
           ) // Comment 3
        {
        }
    }

    public void TestMethod3(bool a, bool b) { }
}
";

            var fixedCode = @"
public class TestClass
{
    public bool TestMethod1() { return true; }

    public void TestMethod2()
    {
        TestMethod1() // some comment
;
        TestMethod3(
            TestMethod1() // some comment
            , TestMethod1());

        // This is a regression test for https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1206
        TestMethod3(
            true,
            (false || true)); // comment

        // This is a regression test for https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1206
        if (
            true || // Comment 1
            false) // Comment 2
        {
        }

        // This is a regression test for https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1206
        if (
            true || // Comment 1
            false)
    // Comment 2
        {
        }

        // This is a regression test for https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1206
        if (
            true || // Comment 1
            false)
    // Comment 2
            // Comment 3
        {
        }
    }

    public void TestMethod3(bool a, bool b) { }
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic(DescriptorFollowed).WithLocation(8, 21),
                Diagnostic(DescriptorFollowed).WithLocation(11, 25),
                Diagnostic(DescriptorNotPreceded).WithLocation(18, 9),
                Diagnostic(DescriptorNotPreceded).WithLocation(24, 12),
                Diagnostic(DescriptorNotPreceded).WithLocation(33, 12),
                Diagnostic(DescriptorNotPreceded).WithLocation(42, 12),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies the analyzer will properly handle trailing multi-line comments.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestTrailingMultiLineCommentAsync()
        {
            var testCode = @"
public class TestClass
{
    public bool TestMethod1() { return true; }

    public void TestMethod2()
    {
        TestMethod1()/* some comment */
;
        TestMethod3(
            TestMethod1()/* some comment */
            , TestMethod1());
    }

    public void TestMethod3(bool a, bool b) { }
}
";

            var fixedCode = @"
public class TestClass
{
    public bool TestMethod1() { return true; }

    public void TestMethod2()
    {
        TestMethod1() /* some comment */
;
        TestMethod3(
            TestMethod1() /* some comment */
            , TestMethod1());
    }

    public void TestMethod3(bool a, bool b) { }
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic(DescriptorFollowed).WithLocation(8, 21),
                Diagnostic(DescriptorFollowed).WithLocation(11, 25),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies preprocessor directives will be properly handled.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestDirectiveTriviaAsync()
        {
            var testCode = @"
public class TestClass
{
    public void TestMethod1(int a,
#if false
int b
#else
int c
#endif
        )
    {
    }

    public void TestMethod2(
#if true
int a )
#endif
    {
        TestMethod3(TestMethod3(
#if true
                0 )
#else
                1 )
#endif
                );
    }

    public int TestMethod3(int a) { return a; }

    public int TestMethod4(string[] args)
    {
#if !(X || NOT )
        return 1;
#endif
    }
}
";

            var fixedCode = @"
public class TestClass
{
    public void TestMethod1(int a,
#if false
int b
#else
int c
#endif
        )
    {
    }

    public void TestMethod2(
#if true
int a)
#endif
    {
        TestMethod3(TestMethod3(
#if true
                0)
#else
                1 )
#endif
                );
    }

    public int TestMethod3(int a) { return a; }

    public int TestMethod4(string[] args)
    {
#if !(X || NOT)
        return 1;
#endif
    }
}
";

            await new CSharpTest
            {
                TestCode = testCode,
                ExpectedDiagnostics =
                {
                    Diagnostic(DescriptorNotPreceded).WithLocation(10, 9),
                    Diagnostic(DescriptorNotPreceded).WithLocation(16, 7),
                    Diagnostic(DescriptorNotPreceded).WithLocation(21, 19),
                    Diagnostic(DescriptorNotPreceded).WithLocation(25, 17),
                    Diagnostic(DescriptorNotPreceded).WithLocation(32, 16),
                },
                FixedCode = fixedCode,
                RemainingDiagnostics =
                {
                    Diagnostic(DescriptorNotPreceded).WithLocation(10, 9),
                    Diagnostic(DescriptorNotPreceded).WithLocation(25, 17),
                },
                NumberOfFixAllIterations = 2,
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(684, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/684")]
        public async Task TestEmbeddedCommentAsync()
        {
            var testCode = @"
public class TestClass
{
    public void TestMethod()
    {
        System.Console.WriteLine(""{0}"", 1 /*text*/ );
    }
}
";
            var fixedCode = @"
public class TestClass
{
    public void TestMethod()
    {
        System.Console.WriteLine(""{0}"", 1 /*text*/);
    }
}
";

            DiagnosticResult expected = Diagnostic(DescriptorNotPreceded).WithLocation(6, 52);

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestFollowedByStickyColonAsync()
        {
            string testCode = @"
class ClassName
{
    void Method()
    {
        switch (0)
        {
        case(1) :
        default:
            break;
        }
    }
}
";
            string fixedCode = @"
class ClassName
{
    void Method()
    {
        switch (0)
        {
        case(1):
        default:
            break;
        }
    }
}
";

            DiagnosticResult expected = Diagnostic(DescriptorNotFollowed).WithLocation(8, 15);

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// This is a regression test for DotNetAnalyzers/StyleCopAnalyzers#2409.
        /// </summary>
        /// <param name="operatorToken">The operator to test.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [Theory]
        [InlineData("*")]
        [InlineData("/")]
        [InlineData("%")]
        [InlineData("+")]
        [InlineData("-")]
        [InlineData("<<")]
        [InlineData(">>")]
        [InlineData("<")]
        [InlineData(">")]
        [InlineData("<=")]
        [InlineData(">=")]
        [InlineData("==")]
        [InlineData("!=")]
        [InlineData("&")]
        [InlineData("^")]
        [InlineData("|")]
        public async Task TestFollowedByBinaryOperatorAsync(string operatorToken)
        {
            string testCode = $"var x = (3 + 2){operatorToken} 4;";
            string fixedCode = $"var x = (3 + 2) {operatorToken} 4;";

            DiagnosticResult expected = Diagnostic(DescriptorFollowed).WithLocation(7, 27);

            await this.TestWhitespaceInStatementOrDeclAsync(testCode, fixedCode, expected).ConfigureAwait(false);
        }

        /// <summary>
        /// This is a regression test for DotNetAnalyzers/StyleCopAnalyzers#2409.
        /// </summary>
        /// <param name="operatorToken">The operator to test.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [Theory]
        [InlineData("&&")]
        [InlineData("||")]
        public async Task TestFollowedByConditionalOperatorAsync(string operatorToken)
        {
            string testCode = $"var x = (true){operatorToken} false;";
            string fixedCode = $"var x = (true) {operatorToken} false;";

            DiagnosticResult expected = Diagnostic(DescriptorFollowed).WithLocation(7, 26);

            await this.TestWhitespaceInStatementOrDeclAsync(testCode, fixedCode, expected).ConfigureAwait(false);
        }

        /// <summary>
        /// This is a regression test for DotNetAnalyzers/StyleCopAnalyzers#2409.
        /// </summary>
        /// <param name="operatorToken">The operator to test.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [Theory]
        [InlineData("=")]
        [InlineData("+=")]
        [InlineData("-=")]
        [InlineData("*=")]
        [InlineData("/=")]
        [InlineData("%=")]
        [InlineData("&=")]
        [InlineData("|=")]
        [InlineData("^=")]
        [InlineData("<<=")]
        [InlineData(">>=")]
        public async Task TestFollowedByAssignmentOperatorAsync(string operatorToken)
        {
            string testCode = $"var x = 0; (x){operatorToken} 4;";
            string fixedCode = $"var x = 0; (x) {operatorToken} 4;";

            DiagnosticResult expected = Diagnostic(DescriptorFollowed).WithLocation(7, 26);

            await this.TestWhitespaceInStatementOrDeclAsync(testCode, fixedCode, expected).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMissingTokenAsync()
        {
            string testCode = @"
class ClassName
{
    ClassName()
        : base(
    {
    }
}
";

            DiagnosticResult[] expected =
            {
                DiagnosticResult.CompilerError("CS1026").WithMessage(") expected").WithLocation(5, 16),
            };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(2475, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2475")]
        public async Task TestSingleLineIfStatementAsync()
        {
            var testCode = @"public class TestClass
{
    public void TestMethod()
    {
        if (true) (true ? 1 : 0).ToString();
    }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(2473, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2473")]
        public async Task TestCodefixBehaviorWithCommentAndSemiColonAsync()
        {
            var testCode = @"using System.Threading.Tasks;

public class TestClass
{
    public async void TestMethod()
    {
        await Task.Delay(1000 // Comment
            );
    }
}
";

            var fixedCode = @"using System.Threading.Tasks;

public class TestClass
{
    public async void TestMethod()
    {
        await Task.Delay(1000); // Comment
    }
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic(DescriptorNotPreceded).WithLocation(8, 13),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(2474, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2474")]
        public async Task TestCodefixBehaviorWithMemberAccessAsync()
        {
            var testCode = @"using System.Threading.Tasks;

public class TestClass
{
    public async void TestMethod()
    {
        await Task.Delay(1000
            ).ConfigureAwait(false);
    }
}
";

            var fixedCode = @"using System.Threading.Tasks;

public class TestClass
{
    public async void TestMethod()
    {
        await Task.Delay(1000)
            .ConfigureAwait(false);
    }
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic(DescriptorNotPreceded).WithLocation(8, 13),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("if (true)")]
        [InlineData("while (true)")]
        [InlineData("for (var i = 0; i < 10; i++)")]
        [InlineData("foreach (var i in a)")]
        [WorkItem(3731, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3731")]
        public async Task TestControlStatementWithBodyOnSameLineAsync(string stmt)
        {
            var testCode = $@"
public class TestClass
{{
    public async void TestMethod(int x, int[] a)
    {{
        {stmt}++x;
        {stmt}--x;
        {stmt}x++;
        {stmt}{{ x++; }}
    }}
}}";

            var fixedCode = $@"
public class TestClass
{{
    public async void TestMethod(int x, int[] a)
    {{
        {stmt} ++x;
        {stmt} --x;
        {stmt} x++;
        {stmt} {{ x++; }}
    }}
}}";

            var expected = new[]
            {
                Diagnostic(DescriptorFollowed).WithLocation(6, 8 + stmt.Length),
                Diagnostic(DescriptorFollowed).WithLocation(7, 8 + stmt.Length),
                Diagnostic(DescriptorFollowed).WithLocation(8, 8 + stmt.Length),
                Diagnostic(DescriptorFollowed).WithLocation(9, 8 + stmt.Length),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        private async Task TestWhitespaceInStatementOrDeclAsync(string originalStatement, string fixedStatement, params DiagnosticResult[] expected)
        {
            string template = @"namespace Foo
{{
    class Bar
    {{
        unsafe void DoIt()
        {{
            {0}
        }}

        Baz GetA()
        {{
            return null;
        }}

        Baz GetB()
        {{
            return null;
        }}

        unsafe int* GetPointer()
        {{
            return null;
        }}

        class Baz
        {{
            public object this[int i]
            {{
                get
                {{
                    return null;
                }}
            }}

            public object Test
            {{
                get
                {{
                    return null;
                }}
            }}
        }}
    }}
}}
";
            string originalCode = string.Format(template, originalStatement);
            string fixedCode = string.Format(template, fixedStatement ?? originalStatement);

            var test = new CSharpTest
            {
                TestCode = originalCode,
                FixedCode = fixedCode,
            };

            test.ExpectedDiagnostics.AddRange(expected);
            await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }
    }
}
