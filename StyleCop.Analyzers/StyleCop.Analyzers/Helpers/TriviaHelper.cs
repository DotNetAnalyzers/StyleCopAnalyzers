namespace StyleCop.Analyzers.Helpers
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;

    /// <summary>
    /// Provides helper methods to work with trivia (lists).
    /// </summary>
    internal static class TriviaHelper
    {
        /// <summary>
        /// Returns the index into the trivia list where the trailing whitespace starts.
        /// </summary>
        /// <param name="triviaList">The trivia list to process.</param>
        /// <returns>The index where the trailing whitespace starts, or -1 if there is no trailing whitespace.</returns>
        internal static int IndexOfTrailingWhitespace(SyntaxTriviaList triviaList)
        {
            var done = false;
            int whiteSpaceStartIndex = -1;
            var previousTriviaWasEndOfLine = false;

            for (var index = triviaList.Count - 1; !done && (index >= 0); index--)
            {
                var currentTrivia = triviaList[index];
                switch (currentTrivia.Kind())
                {
                case SyntaxKind.EndOfLineTrivia:
                    whiteSpaceStartIndex = index;
                    previousTriviaWasEndOfLine = true;
                    break;

                case SyntaxKind.WhitespaceTrivia:
                    whiteSpaceStartIndex = index;
                    previousTriviaWasEndOfLine = false;
                    break;

                default:
                    // encountered non-whitespace trivia -> the search is done.
                    if (previousTriviaWasEndOfLine)
                    {
                        whiteSpaceStartIndex++;
                    }

                    done = true;
                    break;
                }
            }

            return (whiteSpaceStartIndex < triviaList.Count) ? whiteSpaceStartIndex : -1;
        }
    }
}
