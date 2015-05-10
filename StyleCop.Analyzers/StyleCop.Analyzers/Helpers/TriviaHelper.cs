namespace StyleCop.Analyzers.Helpers
{
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;

    /// <summary>
    /// Provides helper methods to work with trivia (lists).
    /// </summary>
    internal static class TriviaHelper
    {
        /// <summary>
        /// Returns the index of the first non-whitespace trivia in the given trivia list.
        /// </summary>
        /// <param name="triviaList">The trivia list to process.</param>
        /// <returns>The index where the non-whitespace starts, or -1 if there is no non-whitespace trivia.</returns>
        internal static int IndexOfFirstNonWhitespaceTrivia(SyntaxTriviaList triviaList)
        {
            for (var index = 0; index < triviaList.Count; index++)
            {
                var currentTrivia = triviaList[index];
                switch (currentTrivia.Kind())
                {
                case SyntaxKind.EndOfLineTrivia:
                case SyntaxKind.WhitespaceTrivia:
                    break;

                default:
                    // encountered non-whitespace trivia -> the search is done.
                    return index;
                }
            }

            return -1;
        }

        /// <summary>
        /// Returns the index of the first trivia that is not part of a blank line.
        /// </summary>
        /// <param name="triviaList">The trivia list to process.</param>
        /// <returns>The index of the first trivia that is not part of a blank line, or -1 if there is no such trivia.</returns>
        internal static int IndexOfFirstNonBlankLineTrivia(SyntaxTriviaList triviaList)
        {
            var firstNonWhitespaceTriviaIndex = IndexOfFirstNonWhitespaceTrivia(triviaList);
            var startIndex = (firstNonWhitespaceTriviaIndex == -1) ? triviaList.Count : firstNonWhitespaceTriviaIndex;

            for (var index = startIndex - 1; index >= 0; index--)
            {
                // Find an end-of-line trivia, to indicate that there actually are blank lines and not just excess whitespace.
                if (triviaList[index].IsKind(SyntaxKind.EndOfLineTrivia))
                {
                    return index == (triviaList.Count - 1) ? -1 : index + 1;
                }
            }

            return 0;
        }

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

        /// <summary>
        /// Strips all trailing whitespace trivia from the trivia list until a non-whitespace trivia is encountered.
        /// </summary>
        /// <param name="triviaList">The trivia list to strip of its trailing whitespace.</param>
        /// <returns>The modified triviaList.</returns>
        internal static SyntaxTriviaList WithoutTrailingWhitespace(this SyntaxTriviaList triviaList)
        {
            var trailingWhitespaceIndex = IndexOfTrailingWhitespace(triviaList);
            return (trailingWhitespaceIndex >= 0) ? SyntaxFactory.TriviaList(triviaList.Take(trailingWhitespaceIndex)) : triviaList;
        }
    }
}
