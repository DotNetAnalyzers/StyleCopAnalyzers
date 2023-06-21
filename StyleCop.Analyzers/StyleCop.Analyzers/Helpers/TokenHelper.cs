// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Helpers
{
    using System.Threading;
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
        /// <param name="allowNonWhitespaceTrivia"><see langword="true"/> to consider the token first-in-line even when
        /// non-whitespace trivia precedes the token; otherwise, <see langword="false"/> to only consider tokens first
        /// in line if they are preceded solely by whitespace.</param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="token"/> is first in line; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool IsFirstInLine(this SyntaxToken token, bool allowNonWhitespaceTrivia = true)
        {
            var fullLineSpan = token.SyntaxTree.GetLineSpan(token.FullSpan);

            bool firstInLine;
            if (token.SyntaxTree == null || fullLineSpan.StartLinePosition.Character == 0)
            {
                firstInLine = true;
            }
            else
            {
                var tokenLineSpan = token.SyntaxTree.GetLineSpan(token.Span);
                firstInLine = tokenLineSpan.StartLinePosition.Line != fullLineSpan.StartLinePosition.Line;
            }

            if (firstInLine && !allowNonWhitespaceTrivia)
            {
                foreach (var trivia in token.LeadingTrivia.Reverse())
                {
                    if (!trivia.IsKind(SyntaxKind.WhitespaceTrivia))
                    {
                        if (!trivia.HasBuiltinEndLine())
                        {
                            firstInLine = false;
                        }

                        break;
                    }
                }
            }

            return firstInLine;
        }

        /// <summary>
        /// Gets a value indicating whether the <paramref name="token"/> is an interpolated
        /// unary expression.
        /// </summary>
        /// <param name="token">The token to process.</param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="token"/> is an interpolated unary expression;
        /// otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool IsInterpolatedUnaryExpression(this SyntaxToken token)
        {
            SyntaxNode parentNode = (token.Parent.IsKind(SyntaxKind.UnaryMinusExpression)
                    || token.Parent.IsKind(SyntaxKind.UnaryPlusExpression))
                ? token.Parent.Parent
                : token.Parent;

            if (parentNode.IsKind(SyntaxKind.Interpolation))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets a value indicating whether the <paramref name="token"/> is last in line.
        /// </summary>
        /// <param name="token">The token to process.</param>
        /// <returns>true if token is last in line, otherwise false.</returns>
        internal static bool IsLastInLine(this SyntaxToken token)
        {
            var fullLineSpan = token.SyntaxTree.GetLineSpan(token.FullSpan);

            if (token.SyntaxTree == null || fullLineSpan.EndLinePosition.Character == 0)
            {
                return true;
            }

            var tokenLineSpan = token.SyntaxTree.GetLineSpan(token.Span);

            if (tokenLineSpan.EndLinePosition.Line != fullLineSpan.EndLinePosition.Line
                || token.SyntaxTree.Length == token.FullSpan.End)
            {
                return true;
            }

            // Use the slow path because we cant be sure if a multi line trivia is following this
            // token.
            var nextToken = token.GetNextToken();
            return nextToken.IsKind(SyntaxKind.None) || token.GetEndLine() < nextToken.GetLine();
        }

        /// <summary>
        /// Gets a value indicating whether the <paramref name="token"/> is preceded by a whitespace.
        /// </summary>
        /// <param name="token">The token to process.</param>
        /// <param name="cancellationToken">The cancellation token that the operation will observe.</param>
        /// <returns>true if token is preceded by a whitespace, otherwise false.</returns>
        internal static bool IsPrecededByWhitespace(this SyntaxToken token, CancellationToken cancellationToken)
        {
            // Perf: Directly access the text instead of the trivia.
            int pos = token.Span.Start - 1;
            if (pos < 0 || token.SyntaxTree == null)
            {
                return false;
            }

            return char.IsWhiteSpace(token.SyntaxTree.GetText(cancellationToken)[pos]);
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

            var precedingTriviaList = TriviaHelper.MergeTriviaLists(precedingToken.TrailingTrivia, token.LeadingTrivia);
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

        /// <summary>
        /// Returns the closest end of line trivia preceding the <paramref name="token"/>.
        /// This currently only looks immediately before the specified token.
        /// </summary>
        /// <param name="token">The token to process.</param>
        /// <returns>The closest preceding end of line trivia, or <see cref="SyntaxFactory.CarriageReturnLineFeed"/> if none is found.</returns>
        internal static SyntaxTrivia GetPrecedingEndOfLineTrivia(this SyntaxToken token)
        {
            var leadingTrivia = token.LeadingTrivia;
            for (var i = leadingTrivia.Count - 1; i >= 0; i--)
            {
                if (leadingTrivia[i].IsKind(SyntaxKind.EndOfLineTrivia))
                {
                    return leadingTrivia[i];
                }
            }

            var prevToken = token.GetPreviousToken();
            var prevTrailingTrivia = prevToken.TrailingTrivia;
            for (var i = prevTrailingTrivia.Count - 1; i >= 0; i--)
            {
                if (prevTrailingTrivia[i].IsKind(SyntaxKind.EndOfLineTrivia))
                {
                    return prevTrailingTrivia[i];
                }
            }

            return SyntaxFactory.CarriageReturnLineFeed;
        }
    }
}
