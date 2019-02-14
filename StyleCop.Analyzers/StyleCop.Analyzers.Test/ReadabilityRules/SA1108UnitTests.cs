// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.ReadabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopDiagnosticVerifier<StyleCop.Analyzers.ReadabilityRules.SA1108BlockStatementsMustNotContainEmbeddedComments>;

    public class SA1108UnitTests
    {
        [Fact]
        public async Task TestIfStatementCommentBetweenStatementDeclarationAndOpeningBraceAsync()
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

            var firstDiagnostic = Diagnostic().WithLocation(8, 9);
            var secondDiagnostic = Diagnostic().WithLocation(13, 9);

            await VerifyCSharpDiagnosticAsync(testCode, new[] { firstDiagnostic, secondDiagnostic }, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestIfStatementCommentBetweenStatementDeclarationAndOpeningBraceCommentedCodeAsync()
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

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestIfWithPreprocessorDirectiveCommentBetweenStatementDeclarationAndOpeningBraceCommentedCodeAsync()
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

            DiagnosticResult expected = Diagnostic().WithLocation(8, 9);

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestCodeBlockInsideMethodAsync()
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

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestWhileStatementCommentBetweenStatementDeclarationAndOpeningBraceAsync()
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
            DiagnosticResult expected = Diagnostic().WithLocation(8, 9);

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestWhileStatementCommentBetweenStatementDeclarationAndOpeningBraceOneLineAsync()
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

            DiagnosticResult expected = Diagnostic().WithLocation(7, 22);

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestDoWhileStatementCommentBetweenStatementDeclarationAndOpeningBraceAsync()
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

            DiagnosticResult expected = Diagnostic().WithLocation(8, 9);

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestCheckedUncheckedStatementCommentBetweenStatementDeclarationAndOpeningBraceAsync()
        {
            var testCode = @"
public class Foo
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

            DiagnosticResult first = Diagnostic().WithLocation(8, 9);
            DiagnosticResult second = Diagnostic().WithLocation(13, 9);

            await VerifyCSharpDiagnosticAsync(testCode, new[] { first, second }, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestFixedStatementCommentBetweenStatementDeclarationAndOpeningBraceAsync()
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

            DiagnosticResult expected = Diagnostic().WithLocation(13, 31);

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestTryCatchFinallyStatementCommentBetweenStatementDeclarationAndOpeningBraceAsync()
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
        catch (System.NullReferenceException e)
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

            DiagnosticResult first = Diagnostic().WithLocation(7, 9);
            DiagnosticResult second = Diagnostic().WithLocation(12, 9);
            DiagnosticResult third = Diagnostic().WithLocation(17, 9);
            DiagnosticResult fourth = Diagnostic().WithLocation(22, 9);

            await VerifyCSharpDiagnosticAsync(testCode, new[] { first, second, third, fourth }, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestSwitchStatementCommentBetweenStatementDeclarationAndOpeningBraceAsync()
        {
            var testCode = @"
public class Foo
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

            DiagnosticResult expected = Diagnostic().WithLocation(9, 9);

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestForStatementCommentBetweenStatementDeclarationAndOpeningBraceAsync()
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

            DiagnosticResult expected = Diagnostic().WithLocation(8, 9);

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestForeachStatementCommentBetweenStatementDeclarationAndOpeningBraceAsync()
        {
            var testCode = @"
public class Foo
{
    public void F()
    {
        //comment
        foreach (var e in System.Linq.Enumerable.Empty<int>())
        // Comment
        {
            //comment
        }
    }
}";

            DiagnosticResult expected = Diagnostic().WithLocation(8, 9);

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLockStatementCommentBetweenStatementDeclarationAndOpeningBraceAsync()
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

            DiagnosticResult expected = Diagnostic().WithLocation(8, 9);

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMethodDeclarationCommentBetweenDeclarationAndOpeningBraceAsync()
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

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestClassDeclarationCommentBetweenDeclarationAndOpeningBraceAsync()
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

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestPropertyDeclarationCommentBetweenDeclarationAndOpeningBraceAsync()
        {
            var testCode = @"
namespace Foo
{
    public class Bar
    {
        public int Prop
        {
            get
            //aa
            {
                return 0;
            }
            set
            //bb
            {
            }
        }
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestNamespaceDeclarationCommentBetweenDeclarationAndOpeningBraceAsync()
        {
            var testCode = @"
namespace Foo
//comment
{
    //inner
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
