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
    /// Unit tests for <see cref="SA1009ClosingParenthesisMustBeSpacedCorrectly"/>
    /// </summary>
    public class SA1009UnitTests : CodeFixVerifier
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments(" not", "preceded").WithLocation(5, 25);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments(" not", "preceded").WithLocation(5, 47);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestAttributeWithParametersAndNoSpaceAfterClosingParenthesisAsync()
        {
            const string testCode = @"using System;
using System.Security.Permissions;

[PermissionSet(SecurityAction.LinkDemand, Name = ""FullTrust"")]
public class Foo
{
    public void Method(int param1, int param2)
    {
    }
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestAttributeWithParametersAndSpaceAfterClosingParenthesisAsync()
        {
            const string testCode = @"using System;
using System.Security.Permissions;

[PermissionSet(SecurityAction.LinkDemand, Name = ""FullTrust"") ]
public class Foo
{
    public void Method(int param1, int param2)
    {
    }
}";

            const string fixedCode = @"using System;
using System.Security.Permissions;

[PermissionSet(SecurityAction.LinkDemand, Name = ""FullTrust"")]
public class Foo
{
    public void Method(int param1, int param2)
    {
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments(" not", "followed").WithLocation(4, 61);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments(string.Empty, "followed").WithLocation(5, 21);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLambdaExpressionWithNoSpaceAfterClosingParenthesisAsync()
        {
            var invalidStatement = @"System.EventHandler handler = (s, e)=> { };";
            var validStatement = @"System.EventHandler handler = (s, e) => { };";

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments(string.Empty, "followed").WithLocation(7, 48);

            await this.TestWhitespaceInStatementOrDeclAsync(invalidStatement, validStatement, expected).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("+")]
        [InlineData("-")]
        [InlineData("*")]
        [InlineData("/")]
        public async Task TestSpaceAfterParenthisisInArithmeticOperationAsync(string operatorValue)
        {
            // e.g. var i = (1 + 1) + 2
            var validStatement = string.Format(@"var i = (1 + 1) {0} 2;", operatorValue);

            await this.TestWhitespaceInStatementOrDeclAsync(validStatement, string.Empty, EmptyDiagnosticResults).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestNoSpaceAfterParenthisisInAddOperationAsync()
        {
            // Note - this looks wrong but according to comments in the implementation "this will be reported as SA1022"
            var invalidStatement = @"var i = (1 + 1)+ 2;";

            await this.TestWhitespaceInStatementOrDeclAsync(invalidStatement, string.Empty, EmptyDiagnosticResults).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestNoSpaceAfterParenthisisInSubtractOperationAsync()
        {
            // Note - this looks wrong but according to comments in the implementation "this will be reported as SA1021"
            var invalidStatement = @"var i = (1 + 1)- 2;";

            await this.TestWhitespaceInStatementOrDeclAsync(invalidStatement, string.Empty, EmptyDiagnosticResults).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("*")]
        [InlineData("/")]
        public async Task TestNoSpaceAfterParenthisisInArithmeticOperationAsync(string operatorValue)
        {
            // e.g. var i = (1 + 1)* 2;
            var invalidStatement = string.Format(@"var i = (1 + 1){0} 2;", operatorValue);
            var validStatement = string.Format(@"var i = (1 + 1) {0} 2;", operatorValue);

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments(string.Empty, "followed").WithLocation(7, 27);

            await this.TestWhitespaceInStatementOrDeclAsync(invalidStatement, validStatement, expected).ConfigureAwait(false);
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

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments(" not", "preceded").WithLocation(7, 41);

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

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments(" not", "preceded").WithLocation(7, 41);

            await this.TestWhitespaceInStatementOrDeclAsync(invalidStatement, validStatement, expected).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestSpaceInCastAsync()
        {
            var invalidStatement = @"var i = (int) 1;";
            var validStatement = @"var i = (int)1;";

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments(" not", "followed").WithLocation(7, 25);

            await this.TestWhitespaceInStatementOrDeclAsync(invalidStatement, validStatement, expected).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestSpaceInConstructorCallAsync()
        {
            var invalidStatement = @"var o = new object() ;";
            var validStatement = @"var o = new object();";

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments(" not", "followed").WithLocation(7, 32);

            await this.TestWhitespaceInStatementOrDeclAsync(invalidStatement, validStatement, expected).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestSpaceMethodCallFollowedByPropertyGetAsync()
        {
            var invalidStatement = @"var o = new Baz() .Test;";
            var validStatement = @"var o = new Baz().Test;";

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments(" not", "followed").WithLocation(7, 29);

            await this.TestWhitespaceInStatementOrDeclAsync(invalidStatement, validStatement, expected).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestSpaceMethodCallFollowedByConditionalAccessPropertyGetAsync()
        {
            var invalidStatement = @"var o = new Baz() ?.Test;";
            var validStatement = @"var o = new Baz()?.Test;";

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments(" not", "followed").WithLocation(7, 29);

            await this.TestWhitespaceInStatementOrDeclAsync(invalidStatement, validStatement, expected).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestSpaceOperationInDoubleSetOfParenthesisAsync()
        {
            var invalidStatement = @"var o = ((1 + 1) );";
            var validStatement = @"var o = ((1 + 1));";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithArguments(" not", "followed").WithLocation(7, 28),
                this.CSharpDiagnostic().WithArguments(" not", "preceded").WithLocation(7, 30)
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

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments(" not", "followed").WithLocation(8, 15);

            await this.TestWhitespaceInStatementOrDeclAsync(invalidStatement, validStatement, expected).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestSpaceBetweenClosingBraceAndParenthesisAsync()
        {
            var invalidStatement = @"var x = new System.Action(() => { } );";
            var validStatement = @"var x = new System.Action(() => { });";

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments(" not", "preceded").WithLocation(7, 49);

            await this.TestWhitespaceInStatementOrDeclAsync(invalidStatement, validStatement, expected).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestSpaceClosingParenthesisFollowedByParenthesisPairAsync()
        {
            var invalidStatement = @"new System.Action(() => { }) ();";
            var validStatement = @"new System.Action(() => { })();";

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments(" not", "followed").WithLocation(7, 40);

            await this.TestWhitespaceInStatementOrDeclAsync(invalidStatement, validStatement, expected).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestSpaceParenthesisFollowedByBracketAsync()
        {
            var invalidStatement = @"var a = GetA() [0];";
            var validStatement = @"var a = GetA()[0];";

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments(" not", "followed").WithLocation(7, 26);

            await this.TestWhitespaceInStatementOrDeclAsync(invalidStatement, validStatement, expected).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestNoSpaceParenthesisFollowedByColonAsync()
        {
            var invalidStatement = @"var x = true ? GetA(): GetB();";
            var validStatement = @"var x = true ? GetA() : GetB();";

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments(string.Empty, "followed").WithLocation(7, 33);

            await this.TestWhitespaceInStatementOrDeclAsync(invalidStatement, validStatement, expected).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestNoSpaceParenthesisFollowedByQuestionAsync()
        {
            var invalidStatement = @"var x = (true == true)? GetA() : GetB();";
            var validStatement = @"var x = (true == true) ? GetA() : GetB();";

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments(string.Empty, "followed").WithLocation(7, 34);

            await this.TestWhitespaceInStatementOrDeclAsync(invalidStatement, validStatement, expected).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestWithSpaceFollowingInInterpolatedStringAsync()
        {
            var invalidStatement = @"var x = $""{typeof(string).ToString() }"";";
            var validStatement = @"var x = $""{typeof(string).ToString()}"";";

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments(" not", "followed").WithLocation(7, 48);

            await this.TestWhitespaceInStatementOrDeclAsync(invalidStatement, validStatement, expected).ConfigureAwait(false);
        }

        /// <summary>
        /// This is a regression test for DotNetAnalyzers/StyleCopAnalyzers#1064 "SA1009: mis-fires for verbatim
        /// strings"
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
            (false || true)) // comment
        ;

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
                this.CSharpDiagnostic().WithLocation(8, 21).WithArguments(string.Empty, "followed"),
                this.CSharpDiagnostic().WithLocation(11, 25).WithArguments(string.Empty, "followed"),
                this.CSharpDiagnostic().WithLocation(18, 9).WithArguments(" not", "preceded"),
                this.CSharpDiagnostic().WithLocation(24, 12).WithArguments(" not", "preceded"),
                this.CSharpDiagnostic().WithLocation(33, 12).WithArguments(" not", "preceded"),
                this.CSharpDiagnostic().WithLocation(42, 12).WithArguments(" not", "preceded"),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
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
                this.CSharpDiagnostic().WithLocation(8, 21).WithArguments(string.Empty, "followed"),
                this.CSharpDiagnostic().WithLocation(11, 25).WithArguments(string.Empty, "followed")
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
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
}
";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(10, 9).WithArguments(" not", "preceded"),
                this.CSharpDiagnostic().WithLocation(16, 7).WithArguments(" not", "preceded"),
                this.CSharpDiagnostic().WithLocation(21, 19).WithArguments(" not", "preceded"),
                this.CSharpDiagnostic().WithLocation(25, 17).WithArguments(" not", "preceded")
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        /// <summary>
        /// This is a regression test for DotNetAnalyzers/StyleCopAnalyzers#684:
        /// https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/684
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
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

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(6, 52).WithArguments(" not", "preceded");

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
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

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(8, 15).WithArguments(" not", "followed");

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
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
                new DiagnosticResult
                {
                    Id = "CS1026",
                    Severity = DiagnosticSeverity.Error,
                    Message = ") expected",
                    Locations = new[] { new DiagnosticResultLocation("Test0.cs", 5, 16) }
                }
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1009ClosingParenthesisMustBeSpacedCorrectly();
        }

        /// <inheritdoc/>
        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new TokenSpacingCodeFixProvider();
        }

        private async Task TestWhitespaceInStatementOrDeclAsync(string originalStatement, string fixedStatement, params DiagnosticResult[] expected)
        {
            string template = @"namespace Foo
{{
    class Bar
    {{
        void DoIt()
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
            string fixedCode = string.Format(template, fixedStatement);

            await this.VerifyCSharpDiagnosticAsync(originalCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            if (!string.IsNullOrEmpty(fixedStatement))
            {
                await this.VerifyCSharpFixAsync(originalCode, fixedCode).ConfigureAwait(false);
            }
        }
    }
}
