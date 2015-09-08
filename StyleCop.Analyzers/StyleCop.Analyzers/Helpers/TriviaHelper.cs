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
        /// <param name="endOfLineIsWhitespace"><see langword="true"/> to treat <see cref="SyntaxKind.EndOfLineTrivia"/>
        /// as whitespace; otherwise, <see langword="false"/>.</param>
        /// <returns>The index where the non-whitespace starts, or -1 if there is no non-whitespace trivia.</returns>
        internal static int IndexOfFirstNonWhitespaceTrivia(IReadOnlyList<SyntaxTrivia> triviaList, bool endOfLineIsWhitespace = true)
        {
            for (var index = 0; index < triviaList.Count; index++)
            {
                var currentTrivia = triviaList[index];
                switch (currentTrivia.Kind())
                {
                case SyntaxKind.EndOfLineTrivia:
                    if (!endOfLineIsWhitespace)
                    {
                        return index;
                    }

                    break;

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
        /// Returns the index of the last trivia of a specified kind in the trivia list.
        /// </summary>
        /// <param name="list">The trivia list.</param>
        /// <param name="kind">The syntax kind to find.</param>
        /// <returns>
        /// <para>The non-negative index of the last trivia which matches <paramref name="kind"/>.</para>
        /// <para>-or-</para>
        /// <para>-1, if the list did not contain any matching trivia.</para>
        /// </returns>
        internal static int LastIndexOf(this SyntaxTriviaList list, SyntaxKind kind)
        {
            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (list[i].IsKind(kind))
                {
                    return i;
                }
            }

            return -1;
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
        /// <param name="endOfLineIsWhitespace"><see langword="true"/> to treat <see cref="SyntaxKind.EndOfLineTrivia"/>
        /// as whitespace; otherwise, <see langword="false"/>.</param>
        /// <returns>The modified triviaList.</returns>
        internal static SyntaxTriviaList WithoutLeadingWhitespace(this SyntaxTriviaList triviaList, bool endOfLineIsWhitespace = true)
        {
            var nonWhitespaceIndex = IndexOfFirstNonWhitespaceTrivia(triviaList, endOfLineIsWhitespace);
            return (nonWhitespaceIndex >= 0) ? SyntaxFactory.TriviaList(triviaList.Skip(nonWhitespaceIndex)) : SyntaxFactory.TriviaList();
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

            triviaIndex = BinarySearch(token.TrailingTrivia, trivia);
            if (triviaIndex != -1)
            {
                var nextToken = token.GetNextToken(includeZeroWidth: true);

                part1 = token.TrailingTrivia;
                part2 = nextToken.LeadingTrivia;
            }
            else
            {
                var prevToken = token.GetPreviousToken();
                triviaIndex = prevToken.TrailingTrivia.Count + BinarySearch(token.LeadingTrivia, trivia);

                part1 = prevToken.TrailingTrivia;
                part2 = token.LeadingTrivia;
            }

            return new DualTriviaListHelper(part1, part2);
        }

        /// <summary>
        /// Merges the given trivia lists into a new single trivia list.
        /// </summary>
        /// <param name="list1">The first part of the new list.</param>
        /// <param name="list2">The second part of the new list.</param>
        /// <returns>The merged trivia list.</returns>
        internal static IReadOnlyList<SyntaxTrivia> MergeTriviaLists(IReadOnlyList<SyntaxTrivia> list1, IReadOnlyList<SyntaxTrivia> list2)
        {
            return new DualTriviaListHelper(list1, list2);
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

            if ((index < 0) || !triviaList[index].HasBuiltinEndLine())
            {
                return false;
            }

            var blankLineCount = -1;
            while (index >= 0)
            {
                if (triviaList[index].HasBuiltinEndLine() && !triviaList[index].IsKind(SyntaxKind.EndOfLineTrivia))
                {
                    blankLineCount++;
                    return blankLineCount > 0;
                }

                switch (triviaList[index].Kind())
                {
                case SyntaxKind.WhitespaceTrivia:
                    // ignore;
                    break;

                case SyntaxKind.EndOfLineTrivia:
                    blankLineCount++;
                    break;

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
        internal static SyntaxToken WithoutBlankLines(this SyntaxToken token)
        {
            var leadingTrivia = token.LeadingTrivia;

            var list = new List<SyntaxTrivia>();
            for (var i = 0; i < leadingTrivia.Count; i++)
            {
                var currentTrivia = leadingTrivia[i];
                if (!currentTrivia.IsKind(SyntaxKind.WhitespaceTrivia) && !currentTrivia.IsKind(SyntaxKind.EndOfLineTrivia))
                {
                    var skipIndex = i > 0 && leadingTrivia[i - 1].IsKind(SyntaxKind.WhitespaceTrivia) ? i - 1 : i;
                    var takeCount = 1;
                    if (skipIndex != i)
                    {
                        takeCount++;
                    }

                    while (leadingTrivia.Count >= i)
                    {
                        takeCount++;
                        i++;
                        if (leadingTrivia[i + 1].IsKind(SyntaxKind.EndOfLineTrivia))
                        {
                            break;
                        }
                    }

                    list.AddRange(leadingTrivia.Skip(skipIndex).Take(takeCount));
                }
            }

            if (leadingTrivia.Count > 1)
            {
                var lastTrivia = leadingTrivia[leadingTrivia.Count - 1];
                if (lastTrivia.IsKind(SyntaxKind.WhitespaceTrivia))
                {
                    list.Add(lastTrivia);
                }
            }

            return token.WithLeadingTrivia(list);
        }

        /// <summary>
        /// Strips all leading blank lines from the given token.
        /// </summary>
        /// <param name="token">The token to strip.</param>
        /// <returns>A new token without leading blank lines.</returns>
        internal static SyntaxToken WithoutLeadingBlankLines(this SyntaxToken token)
        {
            var leadingTrivia = token.LeadingTrivia;

            var skipIndex = 0;
            for (var i = 0; i < leadingTrivia.Count; i++)
            {
                var currentTrivia = leadingTrivia[i];
                if (currentTrivia.IsKind(SyntaxKind.EndOfLineTrivia))
                {
                    skipIndex = i + 1;
                }
                else if (!currentTrivia.IsKind(SyntaxKind.WhitespaceTrivia))
                {
                    // Preceded by whitespace
                    skipIndex = i > 0 && leadingTrivia[i - 1].IsKind(SyntaxKind.WhitespaceTrivia) ? i - 1 : i;
                    break;
                }
            }

            return token.WithLeadingTrivia(leadingTrivia.Skip(skipIndex));
        }

        internal static bool HasBuiltinEndLine(this SyntaxTrivia trivia)
        {
            return trivia.IsDirective
                || trivia.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia)
                || trivia.IsKind(SyntaxKind.EndOfLineTrivia);
        }

        private static int BinarySearch(SyntaxTriviaList leadingTrivia, SyntaxTrivia trivia)
        {
            int low = 0;
            int high = leadingTrivia.Count - 1;
            while (low <= high)
            {
                int index = low + ((high - low) >> 1);
                int order = leadingTrivia[index].Span.CompareTo(trivia.Span);

                if (order == 0)
                {
                    return index;
                }

                if (order < 0)
                {
                    low = index + 1;
                }
                else
                {
                    high = index - 1;
                }
            }

            // Entry was not found
            return -1;
        }

        /// <summary>
        /// Helper class that merges two SyntaxTriviaLists with (hopefully) the lowest possible performance penalty.
        /// </summary>
        private class DualTriviaListHelper : IReadOnlyList<SyntaxTrivia>
        {
            private readonly IReadOnlyList<SyntaxTrivia> part1;
            private readonly int part1Count;
            private readonly IReadOnlyList<SyntaxTrivia> part2;

            public DualTriviaListHelper(IReadOnlyList<SyntaxTrivia> part1, IReadOnlyList<SyntaxTrivia> part2)
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
