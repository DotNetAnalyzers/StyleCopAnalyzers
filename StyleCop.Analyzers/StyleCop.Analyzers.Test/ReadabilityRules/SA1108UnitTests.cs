namespace StyleCop.Analyzers.Test.ReadabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.ReadabilityRules;
    using TestHelper;
    using Xunit;

    public class SA1108UnitTests : CodeFixVerifier
    {
        [Fact]
        public async Task TestEmptySource()
        {
            var testCode = string.Empty;
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestIfStatementCommentBetweenStatementDeclarationAndOpeningCurlyBracket()
        {
            var testCode = @"
public class Foo
{
    public void F()
    {
        //comment
        if (true)
        // Comment
        {
            //comment
        }
        else
        //comment
        {
            //inner comment
        }
    }
}";

            var firstDiagnostic = this.CSharpDiagnostic().WithLocation(8, 9);
            var secondDiagnostic = this.CSharpDiagnostic().WithLocation(13, 9);

            await this.VerifyCSharpDiagnosticAsync(testCode, new [] {firstDiagnostic, secondDiagnostic}, CancellationToken.None);
        }

        [Fact]
        public async Task TestIfStatementCommentBetweenStatementDeclarationAndOpeningCurlyBracketCommentedCode()
        {
            var testCode = @"
public class Foo
{
    public void F()
    {
        if (true)
        //// if (true)
        {
        }
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestIfWithPreprocessorDirectiveCommentBetweenStatementDeclarationAndOpeningCurlyBracketCommentedCode()
        {
            var testCode = @"
public class Foo
{
    public void F()
    {
        if (true)
        #if true
        /* line 1
           line 2 */
        {
        }
        #endif
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(8, 9);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestCodeBlockInsideMethod()
        {
            var testCode = @"
public class Foo
{
    public void F()
    {
        //comment
        {

        }
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestWhileStatementCommentBetweenStatementDeclarationAndOpeningCurlyBracket()
        {
            var testCode = @"
public class Foo
{
    public void F()
    {
        //comment
        while (true)
        // Comment
        {
            //comment
        }
    }
}";
            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(8, 9);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestWhileStatementCommentBetweenStatementDeclarationAndOpeningCurlyBracketOneLine()
        {
            var testCode = @"
public class Foo
{
    public void F()
    {
        //comment
        while (true) /* Comment */ { /*comment */ }
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(7, 22);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestDoWhileStatementCommentBetweenStatementDeclarationAndOpeningCurlyBracket()
        {
            var testCode = @"
public class Foo
{
    public void F()
    {
        //comment
        do
        //aa
        {

        } while (true);
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(8, 9);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestCheckedUncheckedStatementCommentBetweenStatementDeclarationAndOpeningCurlyBracket()
        {
            var testCode = @"
public clas Foo
{
    public void F()
    {
        int a = 0;
        checked
        //aa
        {
            a++;
        }
        unchecked
        //bb
        {
            a++;
        }
    }
}";

            DiagnosticResult first = this.CSharpDiagnostic().WithLocation(8, 9);
            DiagnosticResult second = this.CSharpDiagnostic().WithLocation(13, 9);

            await this.VerifyCSharpDiagnosticAsync(testCode, new []{first, second}, CancellationToken.None);
        }

        [Fact]
        public async Task TestFixedStatementCommentBetweenStatementDeclarationAndOpeningCurlyBracket()
        {
            var testCode = @"
public class TestUnsafe
{
    class Point
    {
        public int x;
        public int y;
    } 

    unsafe static void TestMethod()
    {
        Point pt = new Point();
        fixed (int* p = &pt.x)//aa
        {
            *p = 1;
        }

    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(13, 31);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestTryCatchFinallyStatementCommentBetweenStatementDeclarationAndOpeningCurlyBracket()
        {
            var testCode = @"
public class Foo
{
    public void F()
    {
       try
        //aa
        {

        }
        catch (NullReferenceException e)
        //bb
        {
                
        }
        catch
        //cc
        {
                
        }
        finally
        /* line 1
           line 2 */
        {
                
        }
    }
}";

            DiagnosticResult first = this.CSharpDiagnostic().WithLocation(7, 9);
            DiagnosticResult second = this.CSharpDiagnostic().WithLocation(12, 9);
            DiagnosticResult third = this.CSharpDiagnostic().WithLocation(17, 9);
            DiagnosticResult fourth = this.CSharpDiagnostic().WithLocation(22, 9);

            await this.VerifyCSharpDiagnosticAsync(testCode, new []{first, second, third, fourth}, CancellationToken.None);
        }

        [Fact]
        public async Task TestSwitchStatementCommentBetweenStatementDeclarationAndOpeningCurlyBracket()
        {
            var testCode = @"
public clas Foo
{
    public void F()
    {
        var i = 0;
        //bb
        switch (i)
        //aa
        {
            // comment
            case 1:
                return;
            default:
                return;
        }
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(9, 9);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestForStatementCommentBetweenStatementDeclarationAndOpeningCurlyBracket()
        {
            var testCode = @"
public class Foo
{
    public void F()
    {
        //comment
        for (int i=0; i < 5; i++)
        // Comment
        {
            //comment
        }
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(8, 9);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestForeachStatementCommentBetweenStatementDeclarationAndOpeningCurlyBracket()
        {
            var testCode = @"
public class Foo
{
    public void F()
    {
        //comment
        foreach (var e in Enumerable.Empty<int>())
        // Comment
        {
            //comment
        }
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(8, 9);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestLockStatementCommentBetweenStatementDeclarationAndOpeningCurlyBracket()
        {
            var testCode = @"
public class Foo
{
    public void F()
    {
        //comment
        lock(this)
        // Comment
        {
            //comment
        }
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(8, 9);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestMethodDeclarationCommentBetweenDeclarationAndOpeningCurlyBracket()
        {
            var testCode = @"
public class Bar
{
    //comment
    public void F()
    //comment
    {
        //inner comment
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestClassDeclarationCommentBetweenDeclarationAndOpeningCurlyBracket()
        {
            var testCode = @"
namespace Foo
{
    //comment
    public class Bar
    //comment
    {
        //comment
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestPropertyDeclarationCommentBetweenDeclarationAndOpeningCurlyBracket()
        {
            var testCode = @"
namespace Foo
{
    public class Bar
    {
        public int Prop
        //aa
        {
            return 0;
        }
        set
        //bb
        {
        }
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestNamespaceDeclarationCommentBetweenDeclarationAndOpeningCurlyBracket()
        {
            var testCode = @"
namespace Foo
//comment
{
    //inner
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new SA1108BlockStatementsMustNotContainEmbeddedComments();
        }
    }
}