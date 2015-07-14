namespace StyleCop.Analyzers.Helpers
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
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
        internal static int IndexOfFirstNonWhitespaceTrivia(IReadOnlyList<SyntaxTrivia> triviaList)
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
        internal static int IndexOfFirstNonBlankLineTrivia(IReadOnlyList<SyntaxTrivia> triviaList)
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
        internal static int IndexOfTrailingWhitespace(IReadOnlyList<SyntaxTrivia> triviaList)
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

        /// <summary>
        /// Strips all leading whitespace trivia from the trivia list until a non-whitespace trivia is encountered.
        /// </summary>
        /// <param name="triviaList">The trivia list to strip of its leading whitespace.</param>
        /// <returns>The modified triviaList.</returns>
        internal static SyntaxTriviaList WithoutLeadingWhitespace(this SyntaxTriviaList triviaList)
        {
            var nonWhitespaceIndex = IndexOfFirstNonWhitespaceTrivia(triviaList);
            return (nonWhitespaceIndex >= 0) ? SyntaxFactory.TriviaList(triviaList.Take(nonWhitespaceIndex)) : SyntaxFactory.TriviaList();
        }

        /// <summary>
        /// <para>
        /// Builds a trivia list that contains the given trivia.
        /// </para>
        /// <para>
        /// This method combines the trailing and leading trivia of the tokens between which the given trivia is defined.
        /// </para>
        /// </summary>
        /// <param name="trivia">The trivia to create the list from.</param>
        /// <param name="triviaIndex">The index of the trivia in the created trivia list.</param>
        /// <returns>The created trivia list.</returns>
        internal static IReadOnlyList<SyntaxTrivia> GetContainingTriviaList(SyntaxTrivia trivia, out int triviaIndex)
        {
            var token = trivia.Token;
            SyntaxTriviaList part1;
            SyntaxTriviaList part2;

            triviaIndex = token.TrailingTrivia.IndexOf(trivia);
            if (triviaIndex != -1)
            {
                var nextToken = token.GetNextToken(includeZeroWidth: true);

                part1 = token.TrailingTrivia;
                part2 = nextToken.LeadingTrivia;
            }
            else
            {
                var prevToken = token.GetPreviousToken();
                triviaIndex = prevToken.TrailingTrivia.Count + token.LeadingTrivia.IndexOf(trivia);

                part1 = prevToken.TrailingTrivia;
                part2 = token.LeadingTrivia;
            }

            return new DualTriviaListHelper(part1, part2);
        }

        /// <summary>
        /// Determines if the given token has leading blank lines. Leading whitespace on the same line as the token is ignored.
        /// </summary>
        /// <param name="token">The token to check for leading blank lines.</param>
        /// <returns>True if the token has leading blank lines.</returns>
        internal static bool HasLeadingBlankLines(this SyntaxToken token)
        {
            if (!token.HasLeadingTrivia)
            {
                return false;
            }

            var triviaList = token.LeadingTrivia;

            // skip any leading whitespace
            var index = triviaList.Count - 1;
            while ((index >= 0) && triviaList[index].IsKind(SyntaxKind.WhitespaceTrivia))
            {
                index--;
            }

            if ((index < 0) || !triviaList[index].IsKind(SyntaxKind.EndOfLineTrivia))
            {
                return false;
            }

            var blankLineCount = -1;
            while (index >= 0)
            {
                switch (triviaList[index].Kind())
                {
                case SyntaxKind.WhitespaceTrivia:
                    // ignore;
                    break;
                case SyntaxKind.EndOfLineTrivia:
                    blankLineCount++;
                    break;
                case SyntaxKind.IfDirectiveTrivia:
                case SyntaxKind.ElifDirectiveTrivia:
                case SyntaxKind.ElseDirectiveTrivia:
                case SyntaxKind.EndIfDirectiveTrivia:
                    // directive trivia have an embedded end of line
                    blankLineCount++;
                    return blankLineCount > 0;
                default:
                    return blankLineCount > 0;
                }

                index--;
            }

            return true;
        }

        /// <summary>
        /// Strips all leading blank lines from the given token.
        /// </summary>
        /// <param name="token">The token to strip.</param>
        /// <returns>A new token without leading blank lines.</returns>
        internal static SyntaxToken WithoutLeadingBlankLines(this SyntaxToken token)
        {
            var triviaList = token.LeadingTrivia;
            var leadingWhitespaceStart = triviaList.Count - 1;

            // skip leading whitespace in front of the while keyword
            while ((leadingWhitespaceStart > 0) && triviaList[leadingWhitespaceStart - 1].IsKind(SyntaxKind.WhitespaceTrivia))
            {
                leadingWhitespaceStart--;
            }

            var blankLinesStart = leadingWhitespaceStart - 1;
            var done = false;
            while (!done && (blankLinesStart >= 0))
            {
                switch (triviaList[blankLinesStart].Kind())
                {
                case SyntaxKind.WhitespaceTrivia:
                case SyntaxKind.EndOfLineTrivia:
                    blankLinesStart--;
                    break;

                case SyntaxKind.IfDirectiveTrivia:
                case SyntaxKind.ElifDirectiveTrivia:
                case SyntaxKind.ElseDirectiveTrivia:
                case SyntaxKind.EndIfDirectiveTrivia:
                    // directives include an embedded end of line
                    blankLinesStart++;
                    done = true;
                    break;

                default:
                    // include the first end of line (as it is part of the non blank line trivia)
                    while (!triviaList[blankLinesStart].IsKind(SyntaxKind.EndOfLineTrivia))
                    {
                        blankLinesStart++;
                    }

                    blankLinesStart++;
                    done = true;
                    break;
                }
            }

            var newLeadingTrivia = SyntaxFactory.TriviaList(triviaList.Take(blankLinesStart).Concat(triviaList.Skip(leadingWhitespaceStart)));
            return token.WithLeadingTrivia(newLeadingTrivia);
        }

        /// <summary>
        /// Helper class that merges two SyntaxTriviaLists with (hopefully) the lowest possible performance penalty.
        /// </summary>
        private class DualTriviaListHelper : IReadOnlyList<SyntaxTrivia>
        {
            private SyntaxTriviaList part1;
            private int part1Count;
            private SyntaxTriviaList part2;

            public DualTriviaListHelper(SyntaxTriviaList part1, SyntaxTriviaList part2)
            {
                this.part1 = part1;
                this.part2 = part2;
                this.part1Count = part1.Count;
                this.Count = part1.Count + part2.Count;
            }

            public int Count { get; }

            public SyntaxTrivia this[int index]
            {
                get
                {
                    if (index < this.part1Count)
                    {
                        return this.part1[index];
                    }
                    else if (index < this.Count)
                    {
                        return this.part2[index - this.part1Count];
                    }
                    else
                    {
                        throw new IndexOutOfRangeException();
                    }
                }
            }

            public IEnumerator<SyntaxTrivia> GetEnumerator()
            {
                foreach (var item in this.part1)
                {
                    yield return item;
                }

                foreach (var item in this.part2)
                {
                    yield return item;
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }
        }
    }
}
