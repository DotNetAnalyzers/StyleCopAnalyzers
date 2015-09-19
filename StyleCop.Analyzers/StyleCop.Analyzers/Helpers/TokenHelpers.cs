// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

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
            var fullLineSpan = token.SyntaxTree.GetLineSpan(token.FullSpan);

            if (token.SyntaxTree == null || fullLineSpan.StartLinePosition.Character == 0)
            {
                return true;
            }

            var tokenLineSpan = token.SyntaxTree.GetLineSpan(token.Span);
            return tokenLineSpan.StartLinePosition.Line != fullLineSpan.StartLinePosition.Line;
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
    }
}
