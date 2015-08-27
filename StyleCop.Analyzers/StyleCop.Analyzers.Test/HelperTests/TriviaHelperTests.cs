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
    private int i = 0;

    public int Prop
    {
        /// <summary>
        /// The setter documentation
        /// </summary>
        set
        {
            i = value;
        }

        /// <summary>
        /// The getter documentation
        /// </summary>
        get
        {
            return i;
        }
    }
}";
            var syntaxTree = CSharpSyntaxTree.ParseText(testCode);

            foreach (var token in syntaxTree.GetRoot().DescendantTokens(descendIntoTrivia: true))
            {
                token.WithoutLeadingBlankLines();
            }
        }
    }
}
