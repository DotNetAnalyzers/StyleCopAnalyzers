namespace StyleCop.Analyzers.Helpers
{
    using System.Linq;

    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;

    /// <summary>
    /// Provides helper methods to work with indentation.
    /// </summary>
    internal static class IndentationHelper
    {
        /// <summary>
        /// Gets the number of steps that the given node is indented.
        /// </summary>
        /// <param name="indentationOptions">The indentation options to use.</param>
        /// <param name="token">The token to inspect.</param>
        /// <returns>The number of steps that the token is indented.</returns>
        public static int GetIndentationSteps(IndentationOptions indentationOptions, SyntaxToken token)
        {
            var leadingTrivia = token.LeadingTrivia;
            var indentationString = string.Empty;

            for (var i = leadingTrivia.Count - 1; (i >= 0) && leadingTrivia[i].IsKind(SyntaxKind.WhitespaceTrivia); i--)
            {
                indentationString = string.Concat(leadingTrivia[i].ToFullString(), indentationString);
            }

            var indentationCount = indentationString.ToCharArray().Sum(c => IndentationAmount(c, indentationOptions));

            return indentationCount / indentationOptions.IndentationSize;
        }

        /// <summary>
        /// Generate a new indentation string.
        /// </summary>
        /// <param name="indentationOptions">The indentation options to use.</param>
        /// <param name="indentationSteps">The number of indentation steps.</param>
        /// <returns>A string containing the amount of whitespace needed for the given indentation steps.</returns>
        public static string GenerateIndentationString(IndentationOptions indentationOptions, int indentationSteps)
        {
            string result;
            var indentationCount = indentationSteps * indentationOptions.IndentationSize;

            if (indentationOptions.UseTabs)
            {
                var tabCount = indentationCount / indentationOptions.TabSize;
                var spaceCount = indentationCount % indentationOptions.TabSize;

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
        /// <param name="indentationOptions">The indentation options to use.</param>
        /// <param name="indentationSteps">The amount of indentation steps.</param>
        /// <returns>A <see cref="SyntaxTrivia"/> containing the indentation whitespace.</returns>
        public static SyntaxTrivia GenerateWhitespaceTrivia(IndentationOptions indentationOptions, int indentationSteps)
        {
            return SyntaxFactory.Whitespace(GenerateIndentationString(indentationOptions, indentationSteps));
        }

        private static int IndentationAmount(char c, IndentationOptions indentationOptions)
        {
            return c == '\t' ? indentationOptions.TabSize : 1;
        }
    }
}
