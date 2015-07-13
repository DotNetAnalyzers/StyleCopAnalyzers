namespace StyleCop.Analyzers.Helpers
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;

    /// <summary>
    /// Provides helper methods to work with token.
    /// </summary>
    internal static class TokenHelper
    {
        /// <summary>
        /// Gets a value indicating whether the <paramref name="token"/> is first in line.
        /// </summary>
        /// <param name="token">The token to process.</param>
        /// <returns>true if token is first in line, otherwise false.</returns>
        internal static bool IsFirstInLine(this SyntaxToken token)
        {
            SyntaxToken previousToken = token.GetPreviousToken();
            return previousToken.IsKind(SyntaxKind.None) || previousToken.GetLine() < token.GetLine();
        }

        /// <summary>
        /// Gets a value indicating whether the <paramref name="token"/> is last in line.
        /// </summary>
        /// <param name="token">The token to process.</param>
        /// <returns>true if token is last in line, otherwise false.</returns>
        internal static bool IsLastInLine(this SyntaxToken token)
        {
            SyntaxToken nextToken = token.GetNextToken();
            return nextToken.IsKind(SyntaxKind.None) || token.GetLine() < nextToken.GetLine();
        }

        /// <summary>
        /// Gets a value indicating whether the <paramref name="token"/> is preceded by a whitespace.
        /// </summary>
        /// <param name="token">The token to process.</param>
        /// <returns>true if token is preceded by a whitespace, otherwise false.</returns>
        internal static bool IsPrecededByWhitespace(this SyntaxToken token)
        {
            SyntaxTriviaList triviaList = token.LeadingTrivia;
            if (triviaList.Count > 0)
            {
                return triviaList.Last().IsKind(SyntaxKind.WhitespaceTrivia);
            }

            triviaList = token.GetPreviousToken().TrailingTrivia;
            return triviaList.Count > 0 && triviaList.Last().IsKind(SyntaxKind.WhitespaceTrivia);
        }

        /// <summary>
        /// Gets a value indicating whether the <paramref name="token"/> is the first token in a line and it is only preceded by whitespace.
        /// </summary>
        /// <param name="token">The token to process.</param>
        /// <returns>true if the token is the first token in a line and it is only preceded by whitespace.</returns>
        internal static bool IsOnlyPrecededByWhitespaceInLine(this SyntaxToken token)
        {
            SyntaxToken precedingToken = token.GetPreviousToken();

            if (!precedingToken.IsKind(SyntaxKind.None) && (precedingToken.GetLine() == token.GetLine()))
            {
                return false;
            }

            var precedingTriviaList = precedingToken.TrailingTrivia.AddRange(token.LeadingTrivia);
            for (var i = precedingTriviaList.Count - 1; i >= 0; i--)
            {
                switch (precedingTriviaList[i].Kind())
                {
                    case SyntaxKind.WhitespaceTrivia:
                        break;
                    case SyntaxKind.EndOfLineTrivia:
                        return true;
                    default:
                        return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Gets a value indicating whether the <paramref name="token"/> is followed by a whitespace.
        /// </summary>
        /// <param name="token">The token to process.</param>
        /// <returns>true if token is followed by a whitespace, otherwise false.</returns>
        internal static bool IsFollowedByWhitespace(this SyntaxToken token)
        {
            SyntaxTriviaList triviaList = token.TrailingTrivia;
            if (triviaList.Count > 0)
            {
                return triviaList.First().IsKind(SyntaxKind.WhitespaceTrivia);
            }

            triviaList = token.GetNextToken().LeadingTrivia;
            return triviaList.Count > 0 && triviaList.First().IsKind(SyntaxKind.WhitespaceTrivia);
        }
    }
}
