// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.ReadabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<StyleCop.Analyzers.ReadabilityRules.SA1114ParameterListMustFollowDeclaration, StyleCop.Analyzers.ReadabilityRules.SA1114CodeFixProvider>;

    public class SA1114UnitTests
    {
        [Fact]
        [WorkItem(505, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/505")]
        public async Task TestCodeFixRemovesBlankLinesAfterOpeningParenthesisAsync()
        {
            var testCode = @"
class Foo
{
    public void Bar(

        int value)
    {
    }
}";
            var fixedCode = @"
class Foo
{
    public void Bar(
        int value)
    {
    }
}";

            await VerifyCSharpFixAsync(testCode, Diagnostic().WithLocation(6, 9), fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(505, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/505")]
        public async Task TestCodeFixPreservesCommentsAsync()
        {
            var testCode = @"
class Foo
{
    public void Bar(int value) { }
    public void Call()
    {
        Bar(

            // Keep this comment.
            1);
    }
}";
            var fixedCode = @"
class Foo
{
    public void Bar(int value) { }
    public void Call()
    {
        Bar(
            // Keep this comment.
            1);
    }
}";

            await VerifyCSharpFixAsync(testCode, Diagnostic().WithLocation(10, 13), fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(505, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/505")]
        public async Task TestCodeFixHandlesOtherListKindsAsync()
        {
            var testCode = @"
[System.Obsolete(

    ""message"")]
class Foo
{
    public void Bar(

        int value) { }
    public void Call()
    {
        Bar(

            1);
        var values = new int[

            1];
    }
}";
            var fixedCode = @"
[System.Obsolete(
    ""message"")]
class Foo
{
    public void Bar(
        int value) { }
    public void Call()
    {
        Bar(
            1);
        var values = new int[
            1];
    }
}";
            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(4, 5),
                Diagnostic().WithLocation(9, 9),
                Diagnostic().WithLocation(14, 13),
                Diagnostic().WithLocation(17, 13),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMethodDeclarationParametersList2LinesAfterOpeningParenthesisAsync()
        {
            var testCode = @"
class Foo
{
    public void Bar(

string s)
    {

    }
}";

            DiagnosticResult expected = Diagnostic().WithLocation(6, 1);

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMethodDeclarationParametersListOnNextLineAsOpeningParenthesisAsync()
        {
            var testCode = @"
class Foo
{
    public void Bar(
string s)
    {

    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMethodDeclarationParametersListOnSameLineAsOpeningParenthesisAsync()
        {
            var testCode = @"
class Foo
{
    public void Bar(string s)
    {

    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMethodDeclarationNoParametersAsync()
        {
            var testCode = @"
class Foo
{
    public void Bar(

)
    {

    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMethodCallParametersList2LinesAfterOpeningParenthesisAsync()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        var e = 1.Equals(

1);
    }
}";

            DiagnosticResult expected = Diagnostic().WithLocation(8, 1);

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMethodCallParametersListOnNextLineAsOpeningParenthesisAsync()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        var e = 1.Equals(
1);
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMethodCallParametersListOnSameLineAsOpeningParenthesisAsync()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        var e = 1.Equals(1);
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that comments grouping arguments do not count as blank lines.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(2917, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2917")]
        public async Task TestMethodCallWithGroupedArgumentsAsync()
        {
            var testCode = @"
class Foo
{
    void Func(string a, string b, string c, int d, int e, int f) { }
    void M()
    {
        Func(
// Chars
""A"", ""B"", ""C"",
// Numbers
1, 2, 3);
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that blank lines before or after an argument comment still result in diagnostics.
        /// </summary>
        /// <param name="beforeComment">The whitespace before the comment.</param>
        /// <param name="afterComment">The whitespace after the comment.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData("\r\n\r\n", "\r\n")]
        [InlineData("\r\n", "\r\n\r\n")]
        [WorkItem(2917, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2917")]
        public async Task TestMethodCallWithCommentAndBlankLineAsync(string beforeComment, string afterComment)
        {
            var testCode = @"
class Foo
{
    void Func(string a) { }
    void M()
    {
        Func(" + beforeComment + "// Chars" + afterComment + @"{|#0:""A""|});
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, Diagnostic().WithLocation(0), CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that comments are allowed in each kind of list, but surrounding blank lines are not.
        /// </summary>
        /// <param name="code">The code containing a list.</param>
        /// <param name="comment">The comment before the first item.</param>
        /// <param name="beforeComment">The whitespace before the comment.</param>
        /// <param name="blankLinePosition">The position of an optional blank line.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [CombinatorialData]
        [WorkItem(2917, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2917")]
        public async Task TestListsWithCommentsAsync(
            [CombinatorialValues(
                "class Foo { void M($$int a) { } }",
                "class Foo { public int this[$$int a] { get { return a; } } }",
                "class Foo { void M() { M($$); } void M(int a) { } }",
                "class Foo { void M(int[] a) { var b = a[$$]; } }",
                "[System.Obsolete($$)] class Foo { }",
                "[$$] class Foo { }",
                "class Foo { void M() { var a = new int[$$]; } }")] string code,
            [CombinatorialValues("// Comment", "// First\r\n// Second", "/* Comment */", "/* First\r\nSecond */", "/* First\r\n\r\nSecond */")] string comment,
            [CombinatorialValues("\r\n", " ")] string beforeComment,
            [CombinatorialValues(0, 1, 2)] int blankLinePosition)
        {
            string item;
            if (code.Contains("$$int a"))
            {
                item = "int a";
                code = code.Replace("$$int a", "$$");
            }
            else if (code.StartsWith("[System.Obsolete"))
            {
                item = "\"message\"";
            }
            else if (code.StartsWith("[$$"))
            {
                item = "System.Obsolete";
            }
            else
            {
                item = "1";
            }

            var trivia = (blankLinePosition == 1 ? "\r\n\r\n" : beforeComment)
                + comment
                + (blankLinePosition == 2 ? "\r\n\r\n" : "\r\n")
                + "    ";
            var testCode = code.Replace("$$", trivia + (blankLinePosition == 0 ? item : "{|#0:" + item + "|}"));
            var expected = blankLinePosition == 0 ? DiagnosticResult.EmptyDiagnosticResults : new[] { Diagnostic().WithLocation(0) };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the comment exemption does not change the handling of conditional directives in arguments.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestMethodCallWithConditionalDirectiveAsync()
        {
            var testCode = @"
class Foo
{
    void M(int a) { }
    void M()
    {
        M(
#if true
            {|#0:1|}
#endif
        );
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, Diagnostic().WithLocation(0), CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMethodCallNoParametersAsync()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        var i = 1.ToString(
                
            );
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConstructorDeclarationParametersList2LinesAfterOpeningParenthesisAsync()
        {
            var testCode = @"
class Foo
{
    public Foo(

string s)
    {

    }
}";

            DiagnosticResult expected = Diagnostic().WithLocation(6, 1);

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConstructorDeclarationParametersListOnNextLineAsOpeningParenthesisAsync()
        {
            var testCode = @"
class Foo
{
    public Foo(
string s)
    {

    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConstructorDeclarationParametersListOnSameLineAsOpeningParenthesisAsync()
        {
            var testCode = @"
class Foo
{
    public Foo(string s)
    {

    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConstructorDeclarationNoParametersAsync()
        {
            var testCode = @"
class Foo
{
    public Foo () 
    {
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConstructorCallParametersList2LinesAfterOpeningParenthesisAsync()
        {
            var testCode = @"
public class Foo
{
    public Foo(int i, int j)
    {
    }

    public void Bar()
    {
        var f = new Foo(

1,2);
    }
}";

            DiagnosticResult expected = Diagnostic().WithLocation(12, 1);

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConstructorallParametersListOnNextLineAsOpeningParenthesisAsync()
        {
            var testCode = @"
public class Foo
{
    public Foo(int i, int j)
    {
    }

    public void Bar()
    {
        var f = new Foo(
1,2);
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConstructorCallParametersListOnSameLineAsOpeningParenthesisAsync()
        {
            var testCode = @"
public class Foo
{
    public Foo(int i, int j)
    {
    }

    public void Bar()
    {
        var f = new Foo(1,2);
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConstructorCallNoParametersAsync()
        {
            var testCode = @"
public class Foo
{
    public void Bar()
    {
       var f = new Foo(

);
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestIndexerDeclarationParametersList2LinesAfterOpeningParenthesisAsync()
        {
            var testCode = @"
class Foo
{
    int this[

int i]
    {
        get
        {
            return 1;
        }
    }
}";

            DiagnosticResult expected = Diagnostic().WithLocation(6, 1);

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestIndexerDeclarationParametersListOnNextLineAsOpeningParenthesisAsync()
        {
            var testCode = @"
class Foo
{
    int this[
int i]
    {
        get
        {
            return 1;
        }
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestIndexerDeclarationParametersListOnSameLineAsOpeningParenthesisAsync()
        {
            var testCode = @"
class Foo
{
    int this[int i]
    {
        get
        {
            return 1;
        }
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestArrayDeclarationSizes2LinesAfterOpeningParenthesisAsync()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        int[,] array2Da = new int[

4, 2] { { 1, 2 }, { 3, 4 }, { 5, 6 }, { 7, 8 } };
    }
}";

            DiagnosticResult expected = Diagnostic().WithLocation(8, 1);

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMultidimensionalArrayDeclarationSizes2LinesAfterOpeningParenthesisAsync()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
var a = new int[

1][

,]
            {
                new int[

1, 1]
                {
                    {1}
                }
            };
    }
}";

            DiagnosticResult[] expected =
                {
                    Diagnostic().WithLocation(8, 1),
                    Diagnostic().WithLocation(14, 1),
                };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestArrayDeclarationSizesOnNextLineAsOpeningParenthesisAsync()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        int[,] array2Da = new int[
4, 2] { { 1, 2 }, { 3, 4 }, { 5, 6 }, { 7, 8 } };
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestArrayDeclarationSizesOnSameLineAsOpeningParenthesisAsync()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        int[,] array2Da = new int[4, 2] { { 1, 2 }, { 3, 4 }, { 5, 6 }, { 7, 8 } };
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestIndexerCallParameters2LinesAfterOpeningBracketAsync()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        System.Collections.Generic.List<int> list = new System.Collections.Generic.List<int>();
        var i = list[

1];
    }
}";

            DiagnosticResult expected = Diagnostic().WithLocation(9, 1);

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestIndexerCallParametersOnNextLineAsOpeningBracketAsync()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        System.Collections.Generic.List<int> list = new System.Collections.Generic.List<int>();
        var i = list[
1];
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestIndexerCallParametersOnSameLineAsOpeningBracketAsync()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        System.Collections.Generic.List<int> list = new System.Collections.Generic.List<int>();
        var i = list[1];
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestArrayCallParameters2LinesAfterOpeningBracketAsync()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        int[][,] jaggedArray4 = new int[3][,] 
        {
            new int[,] { {1,3}, {5,7} },
            new int[,] { {0,2}, {4,6}, {8,10} },
            new int[,] { {11,22}, {99,88}, {0,9} } 
        };
        var i = jaggedArray4[

0][

1, 0];
    }
}";

            DiagnosticResult[] expected =
                {
                    Diagnostic().WithLocation(14, 1),
                    Diagnostic().WithLocation(16, 1),
                };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestArrayCallParametersOnNextLineAsOpeningBracketAsync()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        int[][,] jaggedArray4 = new int[3][,] 
        {
            new int[,] { {1,3}, {5,7} },
            new int[,] { {0,2}, {4,6}, {8,10} },
            new int[,] { {11,22}, {99,88}, {0,9} } 
        };
        var i = jaggedArray4[
0][
1, 0];
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestArrayCallParametersOnSameLineAsOpeningBracketAsync()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        int[][,] jaggedArray4 = new int[3][,] 
        {
            new int[,] { {1,3}, {5,7} },
            new int[,] { {0,2}, {4,6}, {8,10} },
            new int[,] { {11,22}, {99,88}, {0,9} } 
        };
        var i = jaggedArray4[0][1, 0];
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestAttributeParametersList2LinesAfterOpeningParenthesisAsync()
        {
            var testCode = @"
using System.Diagnostics;
class Foo
{
    [Conditional(

""DEBUG"")]
    public void Bar()
    {
    }
}";

            DiagnosticResult expected = Diagnostic().WithLocation(7, 1);

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestAttributeParametersListOnNextLineAsOpeningParenthesisAsync()
        {
            var testCode = @"
using System.Diagnostics;
class Foo
{
    [Conditional(
""DEBUG"")]
    public void Bar()
    {
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestAtributeParametersListOnSameLineAsOpeningParenthesisAsync()
        {
            var testCode = @"
using System.Diagnostics;
class Foo
{
    [Conditional(""DEBUG"")]
    public void Bar()
    {
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestAttributeNoParametersAsync()
        {
            var testCode = @"
[System.Serializable]
class Foo
{

}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestAttributesListParametersList2LinesAfterOpeningParenthesisAsync()
        {
            var testCode = @"
using System.Diagnostics;
class Foo
{
    [

Conditional(""DEBUG""),Conditional(""DEBUG2"")]
    public void Bar()
    {
    }
}";

            DiagnosticResult expected = Diagnostic().WithLocation(7, 1);

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestAttributesListParametersListOnNextLineAsOpeningParenthesisAsync()
        {
            var testCode = @"
using System.Diagnostics;
class Foo
{
    [
Conditional(""DEBUG""),Conditional(""DEBUG2"")]
    public void Bar()
    {
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestAtributesListParametersListOnSameLineAsOpeningParenthesisAsync()
        {
            var testCode = @"
using System.Diagnostics;
class Foo
{
    [Conditional(""DEBUG""),Conditional(""DEBUG2"")]
    public void Bar()
    {
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestDelegateDeclarationParametersList2LinesAfterOpeningParenthesisAsync()
        {
            var testCode = @"
class Foo
{
    public delegate void Bar(

string s);
}";

            DiagnosticResult expected = Diagnostic().WithLocation(6, 1);

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestDelegateDeclarationParametersListOnNextLineAsOpeningParenthesisAsync()
        {
            var testCode = @"
class Foo
{
    public delegate void Bar(
string s);
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestDelegateDeclarationParametersListOnSameLineAsOpeningParenthesisAsync()
        {
            var testCode = @"
class Foo
{
    public delegate void Bar(string s);
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestDelegateDeclarationNoParametersAsync()
        {
            var testCode = @"
class Foo
{
    public delegate void Bar();
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestAnonymousMethodDeclarationParametersList2LinesAfterOpeningParenthesisAsync()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        System.Action<int,int> c = delegate(

int z, int j)
        {

        };
    }
}";

            DiagnosticResult expected = Diagnostic().WithLocation(8, 1);

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestAnonymousMethodDeclarationParametersListOnNextLineAsOpeningParenthesisAsync()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        System.Action<int,int> c = delegate(
int z, int j)
        {

        };
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestAnonymousMethodDeclarationParametersListOnSameLineAsOpeningParenthesisAsync()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        System.Action<int,int> c = delegate(int z, int j)
        {

        };
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestAnonymousMethodDeclarationNoParametersAsync()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        System.Action c = delegate()
        {

        };
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestAnonymousMethodDeclarationNoOpeningParenthesisAsync()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        System.Action c = delegate
        {

        };
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLambdaExpressionDeclarationParametersList2LinesAfterOpeningParenthesisAsync()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        System.Action<int,int> c = (

z,j) =>
        {

        };
    }
}";

            DiagnosticResult expected = Diagnostic().WithLocation(8, 1);

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLambdaExpressionDeclarationParametersListOnNextLineAsOpeningParenthesisAsync()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        System.Action<int,int> c = (
z,j) =>
        {

        };
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLambdaExpressionDeclarationParametersListOnSameLineAsOpeningParenthesisAsync()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        System.Action<int,int> c = (z,j) =>
        {

        };
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLambdaExpressionDeclarationNoParametersAsync()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        System.Action c = () => 
        {

        };
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestCastOperatorDeclarationParametersList2LinesAfterOpeningParenthesisAsync()
        {
            var testCode = @"
public class Foo
{
    public static explicit operator Foo(

int i)
    {
        return new Foo();
    }
}";

            DiagnosticResult expected = Diagnostic().WithLocation(6, 1);

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestCastOperatorDeclarationDeclarationParametersListOnNextLineAsOpeningParenthesisAsync()
        {
            var testCode = @"
public class Foo
{
    public static explicit operator Foo(
int i)
    {
        return new Foo();
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestCastOperatorDeclarationParametersListOnSameLineAsOpeningParenthesisAsync()
        {
            var testCode = @"
public class Foo
{
    public static explicit operator Foo(int i)
    {
        return new Foo();
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestOperatorOverloadDeclarationParametersList2LinesAfterOpeningParenthesisAsync()
        {
            var testCode = @"
public class Foo
{
    public static Foo operator +(

Foo a, Foo b)
    {
        return new Foo();
    }
}";

            DiagnosticResult expected = Diagnostic().WithLocation(6, 1);

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestUnaryOperatorOverloadDeclarationParametersList2LinesAfterOpeningParenthesisAsync()
        {
            var testCode = @"
public class Foo
{
    public static Foo operator +(

Foo a)
    {
        return new Foo();
    }
}";

            DiagnosticResult expected = Diagnostic().WithLocation(6, 1);

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestOperatorOverloadDeclarationParametersListOnNextLineAsOpeningParenthesisAsync()
        {
            var testCode = @"
public class Foo
{
    public static Foo operator +(
Foo a, Foo b)
    {
        return new Foo();
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestOperatorOverloadDeclarationParametersListOnSameLineAsOpeningParenthesisAsync()
        {
            var testCode = @"
public class Foo
{
    public static Foo operator +(Foo a, Foo b)
    {
        return new Foo();
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestObjectCreationNoArgumentListAsync()
        {
            var testCode = @"
public class Foo
{
    public static void Bar()
    {
        var list = new System.Collections.Generic.List<int> { 42 };
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestPragmaDirectivesAsync()
        {
            var testCode = @"using System;

public class SomeOtherClass
{
    private void SomeMethod()
    {
        this.SomeOtherMethod(
#pragma warning disable 618
                this.SomeObsoleteMethod());
#pragma warning restore 618
    }

    [Obsolete(""Don't use me!"")]
    private int SomeObsoleteMethod()
    {
        return 0;
    }

    private void SomeOtherMethod(int someParameter)
    {
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that directive trivia will not result in diagnostics.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(1623, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1623")]
        public async Task TestWithDirectiveTriviaAsync()
        {
            var testCode = @"
public interface ITestInterface1 { }

public interface ITestInterface2 { }

public class TestClass
{
    public void TestMethod(
#if TESTSYMBOL
        ITestInterface1 instance)
#else
        ITestInterface2 instance)
#endif
    {
    }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
