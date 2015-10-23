// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Helpers
{
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;

    internal static class SpacingExtensions
    {
        public static bool IsMissingOrDefault(this SyntaxToken token)
        {
            return token.IsKind(SyntaxKind.None)
                || token.IsMissing;
        }

        public static SyntaxToken WithoutLeadingWhitespace(this SyntaxToken token, bool removeEndOfLineTrivia = false)
        {
            if (!token.HasLeadingTrivia)
            {
                return token;
            }

            return token.WithLeadingTrivia(token.LeadingTrivia.WithoutWhitespace(removeEndOfLineTrivia));
        }

        public static SyntaxTriviaList WithoutWhitespace(this SyntaxTriviaList syntaxTriviaList, bool removeEndOfLineTrivia = false)
        {
            if (syntaxTriviaList.Count == 0)
            {
                return syntaxTriviaList;
            }

            var trivia = syntaxTriviaList.Where(i => !i.IsKind(SyntaxKind.WhitespaceTrivia));
            if (removeEndOfLineTrivia)
            {
                trivia = trivia.Where(i => !i.IsKind(SyntaxKind.EndOfLineTrivia));
            }

            return SyntaxFactory.TriviaList(trivia);
        }

        /// <summary>
        /// Removes the leading and trailing trivia associated with a syntax token.
        /// </summary>
        /// <param name="token">The syntax token to remove trivia from.</param>
        /// <returns>A copy of the input syntax token with leading and trailing trivia removed.</returns>
        public static SyntaxToken WithoutTrivia(this SyntaxToken token)
        {
            return token.WithLeadingTrivia(default(SyntaxTriviaList)).WithTrailingTrivia(default(SyntaxTriviaList));
        }
    }
}
