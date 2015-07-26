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
    /// Unit tests for <see cref="SA1009ClosingParenthesisMustBeSpacedCorrectly"/>
    /// </summary>
    public class SA1009UnitTests : DiagnosticVerifier
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

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments(" not", "preceded").WithLocation(5, 25);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
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
            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments(" not", "preceded").WithLocation(5, 47);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
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
            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments(" not", "followed").WithLocation(4, 61);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
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
            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments(string.Empty, "followed").WithLocation(5, 21);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLambdaExpressionWithSpaceAfterClosingParenthesisAsync()
        {
            var validStatement = @"System.EventHandler handler = (s, e) => { };";

            await this.TestWhitespaceInStatementOrDeclAsync(validStatement, EmptyDiagnosticResults).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLambdaExpressionWithNoSpaceAfterClosingParenthesisAsync()
        {
            var invalidStatement = @"System.EventHandler handler = (s, e)=> { };";

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments(string.Empty, "followed").WithLocation(7, 48);

            await this.TestWhitespaceInStatementOrDeclAsync(invalidStatement, expected).ConfigureAwait(false);
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

            await this.TestWhitespaceInStatementOrDeclAsync(validStatement, EmptyDiagnosticResults).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestNoSpaceAfterParenthisisInAddOperationAsync()
        {
            // Note - this looks wrong but according to comments in the implementation "this will be reported as SA1022"
            var invalidStatement = @"var i = (1 + 1)+ 2;";

            await this.TestWhitespaceInStatementOrDeclAsync(invalidStatement, EmptyDiagnosticResults).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestNoSpaceAfterParenthisisInSubtractOperationAsync()
        {
            // Note - this looks wrong but according to comments in the implementation "this will be reported as SA1021"
            var invalidStatement = @"var i = (1 + 1)- 2;";

            await this.TestWhitespaceInStatementOrDeclAsync(invalidStatement, EmptyDiagnosticResults).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("*")]
        [InlineData("/")]
        public async Task TestNoSpaceAfterParenthisisInArithmeticOperationAsync(string operatorValue)
        {
            // e.g. var i = (1 + 1)* 2;
            var invalidStatement = string.Format(@"var i = (1 + 1){0} 2;", operatorValue);

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments(string.Empty, "followed").WithLocation(7, 27);

            await this.TestWhitespaceInStatementOrDeclAsync(invalidStatement, expected).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestNoSpaceAfterParenthisInIncrementingForLoopAsync()
        {
            var validStatement = @"for (int i = 0; i < 10; i++)
            {
            }";

            await this.TestWhitespaceInStatementOrDeclAsync(validStatement, EmptyDiagnosticResults).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestSpaceBeforeParenthisInIncrementingForLoopAsync()
        {
            var invalidStatement = @"for (int i = 0; i < 10; i++ )
            {
            }";

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments(" not", "preceded").WithLocation(7, 41);

            await this.TestWhitespaceInStatementOrDeclAsync(invalidStatement, expected).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestNoSpaceBeforeParenthisInDecrementingForLoopAsync()
        {
            var validStatement = @"for (int i = 0; i < 10; i--)
            {
            }";

            await this.TestWhitespaceInStatementOrDeclAsync(validStatement, EmptyDiagnosticResults).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestSpaceBeforeParenthisInDecrementingForLoopAsync()
        {
            var invalidStatement = @"for (int i = 0; i < 10; i-- )
            {
            }";

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments(" not", "preceded").WithLocation(7, 41);

            await this.TestWhitespaceInStatementOrDeclAsync(invalidStatement, expected).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestNoSpaceInCastAsync()
        {
            var validStatement = @"var i = (int)1;";

            await this.TestWhitespaceInStatementOrDeclAsync(validStatement, EmptyDiagnosticResults).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestSpaceInCastAsync()
        {
            var invalidStatement = @"var i = (int) 1;";

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments(" not", "followed").WithLocation(7, 25);

            await this.TestWhitespaceInStatementOrDeclAsync(invalidStatement, expected).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestNoSpaceInConstructorCallAsync()
        {
            var validStatement = @"var o = new object();";

            await this.TestWhitespaceInStatementOrDeclAsync(validStatement, EmptyDiagnosticResults).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestSpaceInConstructorCallAsync()
        {
            var invalidStatement = @"var o = new object() ;";

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments(" not", "followed").WithLocation(7, 32);

            await this.TestWhitespaceInStatementOrDeclAsync(invalidStatement, expected).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestNoSpaceMethodCallFollowedByPropertyGetAsync()
        {
            var validStatement = @"var o = new Baz().Test;";

            await this.TestWhitespaceInStatementOrDeclAsync(validStatement, EmptyDiagnosticResults).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestSpaceMethodCallFollowedByPropertyGetAsync()
        {
            var validStatement = @"var o = new Baz() .Test;";

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments(" not", "followed").WithLocation(7, 29);

            await this.TestWhitespaceInStatementOrDeclAsync(validStatement, expected).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestNoSpaceMethodCallFollowedByConditionalAccessPropertyGetAsync()
        {
            var validStatement = @"var o = new Baz()?.Test;";

            await this.TestWhitespaceInStatementOrDeclAsync(validStatement, EmptyDiagnosticResults).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestSpaceMethodCallFollowedByConditionalAccessPropertyGetAsync()
        {
            var validStatement = @"var o = new Baz() ?.Test;";

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments(" not", "followed").WithLocation(7, 29);

            await this.TestWhitespaceInStatementOrDeclAsync(validStatement, expected).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestNoSpaceOperationInDoubleSetOfParenthesisAsync()
        {
            var validStatement = @"var o = ((1 + 1));";

            await this.TestWhitespaceInStatementOrDeclAsync(validStatement, EmptyDiagnosticResults).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestSpaceOperationInDoubleSetOfParenthesisAsync()
        {
            var invalidStatement = @"var o = ((1 + 1) );";

            DiagnosticResult expected1 = this.CSharpDiagnostic().WithArguments(" not", "followed").WithLocation(7, 28);
            DiagnosticResult expected2 = this.CSharpDiagnostic().WithArguments(" not", "preceded").WithLocation(7, 30);

            await this.TestWhitespaceInStatementOrDeclAsync(invalidStatement, expected1, expected2).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("++")]
        [InlineData("--")]
        public async Task TestNoSpaceIncrementOrDecrementOperatorFollowingParenthesisAsync(string operatorValue)
        {
            var validStatement = string.Format(@"int i = 0;
            (i){0};",
            operatorValue);

            await this.TestWhitespaceInStatementOrDeclAsync(validStatement, EmptyDiagnosticResults).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("++")]
        [InlineData("--")]
        public async Task TestSpaceIncrementOrDecrementOperatorFollowingParenthesisAsync(string operatorValue)
        {
            var invalidStatement = string.Format(@"int i = 0;
            (i) {0};",
            operatorValue);

            DiagnosticResult expected1 = this.CSharpDiagnostic().WithArguments(" not", "followed").WithLocation(8, 15);

            await this.TestWhitespaceInStatementOrDeclAsync(invalidStatement, expected1).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestNoSpaceBetweenClosingBraceAndParenthesisAsync()
        {
            var validStatement = @"var x = new System.Action(() => { });";

            await this.TestWhitespaceInStatementOrDeclAsync(validStatement, EmptyDiagnosticResults).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestSpaceBetweenClosingBraceAndParenthesisAsync()
        {
            var invalidStatement = @"var x = new System.Action(() => { } );";

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments(" not", "preceded").WithLocation(7, 49);

            await this.TestWhitespaceInStatementOrDeclAsync(invalidStatement, expected).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestNoSpaceClosingParenthesisFollowedByParenthesisPairAsync()
        {
            var validStatement = @"new System.Action(() => { })();";

            await this.TestWhitespaceInStatementOrDeclAsync(validStatement, EmptyDiagnosticResults).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestSpaceClosingParenthesisFollowedByParenthesisPairAsync()
        {
            var invalidStatement = @"new System.Action(() => { }) ();";

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments(" not", "followed").WithLocation(7, 40);

            await this.TestWhitespaceInStatementOrDeclAsync(invalidStatement, expected).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestNoSpaceParenthesisFollowedByBracketAsync()
        {
            var validStatement = @"var a = GetA()[0];";

            await this.TestWhitespaceInStatementOrDeclAsync(validStatement, EmptyDiagnosticResults).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestSpaceParenthesisFollowedByBracketAsync()
        {
            var invalidStatement = @"var a = GetA() [0];";

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments(" not", "followed").WithLocation(7, 26);

            await this.TestWhitespaceInStatementOrDeclAsync(invalidStatement, expected).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestSpaceParenthesisFollowedByColonAsync()
        {
            var validStatement = @"var x = true ? GetA() : GetB();";

            await this.TestWhitespaceInStatementOrDeclAsync(validStatement, EmptyDiagnosticResults).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestNoSpaceParenthesisFollowedByColonAsync()
        {
            var invalidStatement = @"var x = true ? GetA(): GetB();";

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments(string.Empty, "followed").WithLocation(7, 33);

            await this.TestWhitespaceInStatementOrDeclAsync(invalidStatement, expected).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestSpaceParenthesisFollowedByQuestionAsync()
        {
            var validStatement = @"var x = (true == true) ? GetA() : GetB();";

            await this.TestWhitespaceInStatementOrDeclAsync(validStatement, EmptyDiagnosticResults).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestNoSpaceParenthesisFollowedByQuestionAsync()
        {
            var invalidStatement = @"var x = (true == true)? GetA() : GetB();";

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments(string.Empty, "followed").WithLocation(7, 34);

            await this.TestWhitespaceInStatementOrDeclAsync(invalidStatement, expected).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestNoSpaceFollowingInInterpolatedStringAsync()
        {
            var validStatement = @"var x = $""{typeof(string).ToString()}"";";

            await this.TestWhitespaceInStatementOrDeclAsync(validStatement, EmptyDiagnosticResults).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestWithSpaceFollowingInInterpolatedStringAsync()
        {
            var validStatement = @"var x = $""{typeof(string).ToString() }"";";

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments(" not", "followed").WithLocation(7, 48);

            await this.TestWhitespaceInStatementOrDeclAsync(validStatement, expected).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1009ClosingParenthesisMustBeSpacedCorrectly();
        }

        private async Task TestWhitespaceInStatementOrDeclAsync(string originalStatement, params DiagnosticResult[] expected)
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

            await this.VerifyCSharpDiagnosticAsync(originalCode, expected, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
