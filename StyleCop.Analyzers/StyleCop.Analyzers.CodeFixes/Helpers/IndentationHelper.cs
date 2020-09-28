// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Helpers
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using StyleCop.Analyzers.Helpers.ObjectPools;
    using StyleCop.Analyzers.Settings.ObjectModel;

    /// <summary>
    /// Provides helper methods to work with indentation.
    /// </summary>
    internal static class IndentationHelper
    {
        /// <summary>
        /// Gets the first token on the textline that the given token is on.
        /// </summary>
        /// <param name="token">The token used to determine the textline.</param>
        /// <returns>The first token on the textline of the given token.</returns>
        public static SyntaxToken GetFirstTokenOnTextLine(SyntaxToken token)
        {
            while (true)
            {
                var precedingToken = token.GetPreviousToken();
                if (precedingToken.IsKind(SyntaxKind.None))
                {
                    return token;
                }

                if (precedingToken.GetLine() < token.GetLine())
                {
                    return token;
                }

                token = precedingToken;
            }
        }

        /// <summary>
        /// Gets the number of steps that the given node is indented.
        /// </summary>
        /// <param name="indentationSettings">The indentation settings to use.</param>
        /// <param name="node">The node to inspect.</param>
        /// <returns>The number of steps that the node is indented.</returns>
        public static int GetIndentationSteps(IndentationSettings indentationSettings, SyntaxNode node)
        {
            return GetIndentationSteps(indentationSettings, node.SyntaxTree, node.GetLeadingTrivia());
        }

        /// <summary>
        /// Gets the number of steps that the given token is indented.
        /// </summary>
        /// <param name="indentationSettings">The indentation settings to use.</param>
        /// <param name="token">The token to inspect.</param>
        /// <returns>The number of steps that the token is indented.</returns>
        public static int GetIndentationSteps(IndentationSettings indentationSettings, SyntaxToken token)
        {
            // If the token does not belong to a syntax tree, it is a modified token and it is assumed that
            // the caller makes sure that the token is the first token on a line.
            return token.SyntaxTree != null
                ? GetIndentationSteps(indentationSettings, token.SyntaxTree, token.LeadingTrivia)
                : GetIndentationStepsUnchecked(indentationSettings, token.LeadingTrivia);
        }

        /// <summary>
        /// Generate a new indentation string.
        /// </summary>
        /// <param name="indentationSettings">The indentation settings to use.</param>
        /// <param name="indentationSteps">The number of indentation steps.</param>
        /// <returns>A string containing the amount of whitespace needed for the given indentation steps.</returns>
        public static string GenerateIndentationString(IndentationSettings indentationSettings, int indentationSteps)
        {
            string result;
            var indentationCount = indentationSteps * indentationSettings.IndentationSize;
            if (indentationSettings.UseTabs)
            {
                var tabCount = indentationCount / indentationSettings.TabSize;
                var spaceCount = indentationCount % indentationSettings.TabSize;
                result = new string('\t', tabCount) + new string(' ', spaceCount);
            }
            else
            {
                result = new string(' ', indentationCount);
            }

            return result;
        }

        /// <summary>
        /// Generates a whitespace trivia with the requested indentation.
        /// </summary>
        /// <param name="indentationSettings">The indentation settings to use.</param>
        /// <param name="indentationSteps">The amount of indentation steps.</param>
        /// <returns>A <see cref="SyntaxTrivia"/> containing the indentation whitespace.</returns>
        public static SyntaxTrivia GenerateWhitespaceTrivia(IndentationSettings indentationSettings, int indentationSteps)
        {
            return SyntaxFactory.Whitespace(GenerateIndentationString(indentationSettings, indentationSteps));
        }

        private static int GetIndentationSteps(IndentationSettings indentationSettings, SyntaxTree syntaxTree, SyntaxTriviaList leadingTrivia)
        {
            var triviaSpan = syntaxTree.GetLineSpan(leadingTrivia.FullSpan);

            // There is no indentation when the leading trivia doesn't begin at the start of the line.
            if ((triviaSpan.StartLinePosition == triviaSpan.EndLinePosition) && (triviaSpan.StartLinePosition.Character > 0))
            {
                return 0;
            }

            return GetIndentationStepsUnchecked(indentationSettings, leadingTrivia);
        }

        private static int GetIndentationStepsUnchecked(IndentationSettings indentationSettings, SyntaxTriviaList leadingTrivia)
        {
            var builder = StringBuilderPool.Allocate();

            foreach (SyntaxTrivia trivia in leadingTrivia.Reverse())
            {
                if (!trivia.IsKind(SyntaxKind.WhitespaceTrivia))
                {
                    break;
                }

                builder.Insert(0, trivia.ToFullString());
            }

            var tabSize = indentationSettings.TabSize;
            var indentationCount = 0;
            for (var i = 0; i < builder.Length; i++)
            {
                indentationCount += builder[i] == '\t' ? tabSize - (indentationCount % tabSize) : 1;
            }

            StringBuilderPool.ReturnAndFree(builder);

            return (indentationCount + (indentationSettings.IndentationSize / 2)) / indentationSettings.IndentationSize;
        }
    }
}
