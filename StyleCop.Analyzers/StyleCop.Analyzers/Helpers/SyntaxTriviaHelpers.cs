namespace StyleCop.Analyzers.Helpers
{
    using System.Linq;
    using System.Runtime.CompilerServices;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal static class SyntaxTreeHelpers
    {
        /// <summary>
        /// A cache of the result of computing whether a document has using alias directives.
        /// </summary>
        /// <remarks>
        /// This allows many analyzers that run on every token in the file to avoid checking
        /// the same state in the document repeatedly.
        /// </remarks>
        private static readonly ConditionalWeakTable<SyntaxTree, StrongBox<bool?>> UsingAliasPresentCheck
            = new ConditionalWeakTable<SyntaxTree, StrongBox<bool?>>();

        internal static bool ContainsUsingAlias(this SyntaxTree tree)
        {
            if (tree == null)
            {
                return false;
            }

            StrongBox<bool?> cachedResult = UsingAliasPresentCheck.GetOrCreateValue(tree);
            if (cachedResult.Value.HasValue)
            {
                return cachedResult.Value.Value;
            }

            bool hasUsingAlias = ContainsUsingAliasNoCache(tree);

            // Update the strongbox's value with our computed result.
            // This doesn't change the strongbox reference, and its presence in the
            // ConditionalWeakTable is already assured, so we're updating in-place.
            // In the event of a race condition with another thread that set the value,
            // we'll just be re-setting it to the same value.
            cachedResult.Value = hasUsingAlias;

            return hasUsingAlias;
        }

        private static bool ContainsUsingAliasNoCache(SyntaxTree tree)
        {
            var nodes = tree.GetRoot().DescendantNodes(node => node.IsKind(SyntaxKind.CompilationUnit) || node.IsKind(SyntaxKind.NamespaceDeclaration));

            return nodes.OfType<UsingDirectiveSyntax>().Any(x => x.Alias != null);
        }
    }
}
