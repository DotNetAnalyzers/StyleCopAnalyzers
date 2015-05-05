namespace StyleCop.Analyzers.Test.SpacingRules
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.SpacingRules;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// This class contains unit tests for <see cref="SA1028NoTrailingWhitespace"/> and
    /// <see cref="SA1028CodeFixProvider"/>.
    /// </summary>
    public class SA1028UnitTests : CodeFixVerifier
    {
        [Fact]
        public async Task TrailingWhitespaceAfterStatement()
        {
            string testCode = @"
class ClassName
{
    void MethodName()
    {
        System.Console.WriteLine(); 
    }
}
";

            string fixedCode = @"
class ClassName
{
    void MethodName()
    {
        System.Console.WriteLine();
    }
}
";
            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(6, 36),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None);
        }

        [Fact]
        public async Task TrailingWhitespaceAfterDeclaration()
        {
            string testCode = @"
class ClassName 
{
}
";

            string fixedCode = @"
class ClassName
{
}
";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(2, 16),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None);
        }

        [Fact]
        public async Task TrailingWhitespaceAfterSingleLineComment()
        {
            string testCode = @"
// hi there    
";

            string fixedCode = @"
// hi there
";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(2, 12),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None);
        }

        [Fact]
        public async Task TrailingWhitespaceInsideMultiLineComment()
        {
            string testCode = @"/* 
 foo   
  bar   
*/  
";

            string fixedCode = @"/*
 foo
  bar
*/
";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(1, 3),
                this.CSharpDiagnostic().WithLocation(2, 5),
                this.CSharpDiagnostic().WithLocation(3, 6),
                this.CSharpDiagnostic().WithLocation(4, 3),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None);
        }

        [Fact]
        public async Task TrailingWhitespaceInsideXmlDocComment()
        {
            string testCode = @"
/// <summary>  
/// Some description    
/// </summary>  
class Foo { }
";

            string fixedCode = @"
/// <summary>
/// Some description
/// </summary>
class Foo { }
";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(2, 14),
                this.CSharpDiagnostic().WithLocation(3, 21),
                this.CSharpDiagnostic().WithLocation(4, 15),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None);

            // Enable this code fix when we have it working.
            ////await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None);
        }

        [Fact]
        public async Task TrailingWhitespaceWithinAndAfterHereString()
        {
            string testCode = @"
class ClassName
{
    string foo = @""      
more text    
"";  
}
";

            string fixedCode = @"
class ClassName
{
    string foo = @""      
more text    
"";
}
";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(6, 3),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None);
        }

        [Fact]
        public async Task TrailingWhitespaceAfterDirectives()
        {
            string testCode = @"
#define Zoot  
#undef Zoot2  
using System;  
#if Foo  
#elif Bar  
#else  
#endif 
#warning Some warning  
#region Some region  
#endregion  
";

            string fixedCode = @"
#define Zoot
#undef Zoot2
using System;
#if Foo
#elif Bar
#else
#endif
#warning Some warning
#region Some region
#endregion
";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(2, 13),
                this.CSharpDiagnostic().WithLocation(3, 13),
                this.CSharpDiagnostic().WithLocation(4, 14),
                this.CSharpDiagnostic().WithLocation(5, 8),
                this.CSharpDiagnostic().WithLocation(6, 10),
                this.CSharpDiagnostic().WithLocation(7, 6),
                this.CSharpDiagnostic().WithLocation(8, 7),
                this.CSharpDiagnostic().WithLocation(9, 22),
                this.CSharpDiagnostic().WithLocation(10, 20),
                this.CSharpDiagnostic().WithLocation(11, 11),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None);

            // We don't have code fixes available for directives yet.
            ////await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None);
        }

        [Fact]
        public async Task NoTrailingWhitespaceAfterBlockComment()
        {
            string testCode = @"
class Program    /* some block comment that follows several spaces */
{
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new SA1028NoTrailingWhitespace();
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new SA1028CodeFixProvider();
        }
    }
}
