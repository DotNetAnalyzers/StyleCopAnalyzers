namespace StyleCop.Analyzers.Test.HelperTests
{
    using System.Linq;
    using Analyzers.Helpers;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Xunit;

    public class TriviaHelperTests
    {
        [Fact]
        public void TestWithoutLeadingWhitespace()
        {
            // This is a regression test for https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1285
            string testCode = @"public class Foo
{
    /// Foo
    private int i = 0;
    #if true
    private int j = 0;
    #elif false
    private int k = 0;
    #else
    private int l = 0;
    #endif
    private int m = 0;
    #line 10
    private int n = 0;
    #region X
    private int o = 0;
    #endregion
    private int p = 0;
    #warning Foo
    private int q = 0;
    #error Bar
    private int r = 0;
}";
            var syntaxTree = CSharpSyntaxTree.ParseText(testCode);

            foreach (var token in syntaxTree.GetRoot().DescendantTokens(descendIntoTrivia: true))
            {
                token.WithoutLeadingBlankLines();
            }
        }
    }
}