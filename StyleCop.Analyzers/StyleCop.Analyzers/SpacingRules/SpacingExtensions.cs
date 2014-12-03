namespace StyleCop.Analyzers.SpacingRules
{
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;

    internal static class SpacingExtensions
    {
        public static SyntaxToken WithoutLeadingWhitespace(this SyntaxToken token, bool removeEndOfLineTrivia = false)
        {
            if (!token.HasLeadingTrivia)
                return token;

            return token.WithLeadingTrivia(token.LeadingTrivia.WithoutWhitespace(removeEndOfLineTrivia));
        }

        public static SyntaxToken WithoutTrailingWhitespace(this SyntaxToken token, bool removeEndOfLineTrivia = false)
        {
            if (!token.HasTrailingTrivia)
                return token;

            return token.WithTrailingTrivia(token.TrailingTrivia.WithoutWhitespace(removeEndOfLineTrivia));
        }

        public static SyntaxToken WithoutWhitespace(this SyntaxToken token, bool removeEndOfLineTrivia = false)
        {
            return token.WithoutLeadingWhitespace(removeEndOfLineTrivia).WithoutTrailingWhitespace(removeEndOfLineTrivia);
        }

        public static SyntaxTriviaList WithoutWhitespace(this SyntaxTriviaList syntaxTriviaList, bool removeEndOfLineTrivia = false)
        {
            if (syntaxTriviaList.Count == 0)
                return syntaxTriviaList;

            var trivia = syntaxTriviaList.Where(i => !i.IsKind(SyntaxKind.WhitespaceTrivia));
            if (removeEndOfLineTrivia)
                trivia = trivia.Where(i => !i.IsKind(SyntaxKind.EndOfLineTrivia));

            return SyntaxFactory.TriviaList(trivia);
        }
    }
}
