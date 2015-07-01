namespace StyleCop.Analyzers.Helpers
{
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis;

    internal static class DictionaryHelper
    {
        /// <summary>
        /// This helper method allow us avoid closure allocations when used with SyntaxNode.ReplaceTokens and etc.
        /// </summary>
        /// <param name="replacements">Dictionary with key value pairs {token to replace, replacement token}.</param>
        /// <param name="original">The token to replace. This argument used for getting replacement token from dictionary.</param>
        /// <param name="couldBeRewritten">The token to replace which could be rewritten. We ignore this argument.</param>
        /// <returns>Returns replacement token from dicionary.</returns>
        internal static SyntaxToken GetReplacementToken(
            this Dictionary<SyntaxToken, SyntaxToken> replacements,
            SyntaxToken original,
            SyntaxToken couldBeRewritten)
        {
            return replacements[original];
        }
    }
}
