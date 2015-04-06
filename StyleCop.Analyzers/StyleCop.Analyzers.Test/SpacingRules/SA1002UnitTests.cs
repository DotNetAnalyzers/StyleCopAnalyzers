namespace StyleCop.Analyzers.Test.SpacingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.SpacingRules;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// This class contains unit tests for <see cref="SA1002SemicolonsMustBeSpacedCorrectly"/> and
    /// <see cref="SA1002CodeFixProvider"/>.
    /// </summary>
    public class SA1002UnitTests : CodeFixVerifier
    {
        [Fact]
        public async Task TestForLoop()
        {
            string testCode = @"
class ClassName
{
    void MethodName()
    {
        for (int x =0;x<10;x++)
        {
        }
    }
}
";
            string fixedCode = @"
class ClassName
{
    void MethodName()
    {
        for (int x =0; x<10; x++)
        {
        }
    }
}
";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithArguments(string.Empty, "followed").WithLocation(6, 22),
                this.CSharpDiagnostic().WithArguments(string.Empty, "followed").WithLocation(6, 27),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None);
        }

        [Fact]
        public async Task TestInfiniteForLoop()
        {
            string testCode = @"
class ClassName
{
    void MethodName()
    {
        for (;;)
        {
        }
    }
}
";
            string fixedCode = @"
class ClassName
{
    void MethodName()
    {
        for (; ;)
        {
        }
    }
}
";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithArguments(string.Empty, "followed").WithLocation(6, 14)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None);
        }

        [Fact]
        public async Task TestReturnValueStatement()
        {
            string testCode = @"
class ClassName
{
    int MethodName()
    {
        return 0 ;
    }
}
";
            string fixedCode = @"
class ClassName
{
    int MethodName()
    {
        return 0;
    }
}
";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithArguments(" not", "preceded").WithLocation(6, 18)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None);
        }

        [Fact]
        public async Task TestEmptyReturnStatement()
        {
            string testCode = @"
class ClassName
{
    void MethodName()
    {
        return ;
    }
}
";
            string fixedCode = @"
class ClassName
{
    void MethodName()
    {
        return;
    }
}
";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithArguments(" not", "preceded").WithLocation(6, 16)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None);
        }

        [Fact]
        public async Task TestThrowStatements()
        {
            string testCode = @"
using System;
class ClassName
{
    int MethodName()
    {
        try
        {
            throw new InvalidOperationException() ;
        }
        catch
        {
            throw ;
        }
    }
}
";
            string fixedCode = @"
using System;
class ClassName
{
    int MethodName()
    {
        try
        {
            throw new InvalidOperationException();
        }
        catch
        {
            throw;
        }
    }
}
";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithArguments(" not", "preceded").WithLocation(9, 51),
                this.CSharpDiagnostic().WithArguments(" not", "preceded").WithLocation(13, 19),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None);
        }

        [Fact]
        public async Task TestEmptyStatement()
        {
            string testCode = @"
class ClassName
{
    void MethodName()
    {
        ;
    }
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestSingleLineAccessors()
        {
            string testCode = @"
class ClassName
{
    int property;
    int PropertyName
    {
        get{return this.property;}
        set{this.property=value;}
    }
}
";
            string fixedCode = @"
class ClassName
{
    int property;
    int PropertyName
    {
        get{return this.property; }
        set{this.property=value; }
    }
}
";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithArguments(string.Empty, "followed").WithLocation(7, 33),
                this.CSharpDiagnostic().WithArguments(string.Empty, "followed").WithLocation(8, 32),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None);
        }

        [Fact]
        public async Task TestFollowedByLineComment()
        {
            string testCode = @"
class ClassName
{
    int property;// Comment
}
";
            string fixedCode = @"
class ClassName
{
    int property; // Comment
}
";

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments(string.Empty, "followed").WithLocation(4, 17);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None);
        }

        [Fact]
        public async Task TestFollowedByBlockComment()
        {
            string testCode = @"
class ClassName
{
    int property;/* Comment
        Comment Line 2 */
}
";
            string fixedCode = @"
class ClassName
{
    int property; /* Comment
        Comment Line 2 */
}
";

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments(string.Empty, "followed").WithLocation(4, 17);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new SA1002SemicolonsMustBeSpacedCorrectly();
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new SA1002CodeFixProvider();
        }
    }
}
