namespace StyleCop.Analyzers.Helpers
{
    using Microsoft.CodeAnalysis;

    /// <summary>
    /// Provides helper methods for working with source file locations.
    /// </summary>
    internal static class LocationHelpers
    {
        /// <summary>
        /// Gets the line on which the given token occurs.
        /// </summary>
        /// <param name="token">The token to use.</param>
        /// <returns>The line on which the given token occurs.</returns>
        internal static int GetLine(this SyntaxToken token)
        {
            return token.GetLocation().GetLineSpan().StartLinePosition.Line;
        }

        /// <summary>
        /// Get a value indicating whether the given node span multiple source text lines.
        /// </summary>
        /// <param name="node">The node to check.</param>
        /// <returns>True, if the node spans multiple source text lines.</returns>
        internal static bool SpansMultipleLines(this SyntaxNode node)
        {
            var lineSpan = node.GetLocation().GetLineSpan();

            return lineSpan.StartLinePosition.Line < lineSpan.EndLinePosition.Line;
        }
    }
}
