// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

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
        /// <typeparam name="T">The type of the trivia list.</typeparam>
        /// <returns>The index where the non-whitespace starts, or -1 if there is no non-whitespace trivia.</returns>
        internal static int IndexOfFirstNonWhitespaceTrivia<T>(T triviaList, bool endOfLineIsWhitespace = true)
            where T : IReadOnlyList<SyntaxTrivia>
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
        /// <typeparam name="T">The type of the trivia list.</typeparam>
        /// <returns>The index of the first trivia that is not part of a blank line, or -1 if there is no such trivia.</returns>
        internal static int IndexOfFirstNonBlankLineTrivia<T>(T triviaList)
            where T : IReadOnlyList<SyntaxTrivia>
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
        /// <typeparam name="T">The type of the trivia list.</typeparam>
        /// <returns>The index where the trailing whitespace starts, or -1 if there is no trailing whitespace.</returns>
        internal static int IndexOfTrailingWhitespace<T>(T triviaList)
            where T : IReadOnlyList<SyntaxTrivia>
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
        /// Removes a range of elements from the <see cref="SyntaxTriviaList"/>.
        /// </summary>
        /// <param name="list">The list to remove elements from.</param>
        /// <param name="index">The zero-based starting index of the range of elements to remove.</param>
        /// <param name="count">The number of elements to remove.</param>
        /// <returns>A copy of <paramref name="list"/> with the specified range of elements removed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <para>If <paramref name="index"/> is less than 0.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="count"/> is less than 0.</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <para>If <paramref name="index"/> and <paramref name="count"/> do not denote a valid range of elements in
        /// the <see cref="SyntaxTriviaList"/>.</para>
        /// </exception>
        internal static SyntaxTriviaList RemoveRange(this SyntaxTriviaList list, int index, int count)
        {
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }

            if (index > list.Count - count)
            {
                throw new ArgumentException("The specified range of elements does not exist in the list.");
            }

            SyntaxTrivia[] trivia = new SyntaxTrivia[list.Count - count];
            for (int i = 0; i < index; i++)
            {
                trivia[i] = list[i];
            }

            for (int i = index; i + count < list.Count; i++)
            {
                trivia[i] = list[i + count];
            }

            return SyntaxFactory.TriviaList(trivia);
        }

        internal static SyntaxTriviaList WithoutDirectiveTrivia(this SyntaxTriviaList triviaList)
        {
            var resultTriviaList = new List<SyntaxTrivia>(triviaList.Count);
            foreach (var trivia in triviaList)
            {
                if (!trivia.IsDirective)
                {
                    resultTriviaList.Add(trivia);
                }
            }

            return SyntaxFactory.TriviaList(resultTriviaList);
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
        internal static DualTriviaListHelper GetContainingTriviaList(SyntaxTrivia trivia, out int triviaIndex)
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
        internal static DualTriviaListHelper MergeTriviaLists(SyntaxTriviaList list1, SyntaxTriviaList list2)
        {
            return new DualTriviaListHelper(list1, list2);
        }

        /// <summary>
        /// Determines if the given token is immediately preceded by blank lines. Leading whitespace on the same line as
        /// the token is ignored.
        /// </summary>
        /// <param name="token">The token to check for immediately preceding blank lines.</param>
        /// <returns>
        /// <see langword="true"/> if the token is immediately preceded by blank lines; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool IsPrecededByBlankLines(this SyntaxToken token)
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
                    while (!triviaList[blankLinesStart].HasBuiltinEndLine())
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
        internal struct DualTriviaListHelper : IReadOnlyList<SyntaxTrivia>
        {
            private readonly SyntaxTriviaList part1;
            private readonly int part1Count;
            private readonly SyntaxTriviaList part2;

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

            public SyntaxTrivia First()
            {
                return this[0];
            }

            public SyntaxTrivia Last()
            {
                return this[this.Count - 1];
            }
        }
    }
}
