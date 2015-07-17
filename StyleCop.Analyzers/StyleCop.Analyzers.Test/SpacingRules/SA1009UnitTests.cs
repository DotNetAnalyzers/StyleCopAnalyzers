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
    public class SA1009UnitTests : CodeFixVerifier
    {
        private const string SimpleMethodOutline = @"using System;

public class Foo
{
    public void Method()
    {
    }
}";

        [Fact]
        public async Task TestEmptySourceAsync()
        {
            var testCode = string.Empty;
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

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
        public async Task TestMethodWith2CorrectlySpaceParametersAsync()
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
        public async Task TestMethodWith2ParametersndSpaceBeforeClosingParenthesisAsync()
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

        [Fact]
        public async Task TestSpaceAfterParenthisisInArithmeticAsync()
        {
            var validStatement = @"var i = (1 + 1) * 2;";

            await this.TestWhitespaceInStatementOrDeclAsync(validStatement, EmptyDiagnosticResults).ConfigureAwait(false);
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
            var validStatement = @"for (int i = 0; i < 10; i++ )
            {
            }";

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments(" not", "preceded").WithLocation(7, 41);

            await this.TestWhitespaceInStatementOrDeclAsync(validStatement, expected).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestNoSpaceAfterParenthisInDecrementingForLoopAsync()
        {
            var validStatement = @"for (int i = 0; i < 10; i--)
            {
            }";

            await this.TestWhitespaceInStatementOrDeclAsync(validStatement, EmptyDiagnosticResults).ConfigureAwait(false);
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
        void Baz()
        {{
            {0}
        }}
    }}
}}
";
            string originalCode = string.Format(template, originalStatement);

            await this.VerifyCSharpDiagnosticAsync(originalCode, expected, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
