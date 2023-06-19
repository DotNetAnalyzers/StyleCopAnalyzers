// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using StyleCop.Analyzers.Lightup;

    internal static class SyntaxTreeHelpers
    {
        /// <summary>
        /// A cache of the result of computing whether a document has using alias directives.
        /// </summary>
        /// <remarks>
        /// <para>This allows many analyzers that run on every token in the file to avoid checking
        /// the same state in the document repeatedly.</para>
        /// </remarks>
        private static Tuple<WeakReference<Compilation>, IReadOnlyDictionary<SyntaxTree, bool>> usingAliasCache
            = Tuple.Create(new WeakReference<Compilation>(null), default(IReadOnlyDictionary<SyntaxTree, bool>));

        public static IReadOnlyDictionary<SyntaxTree, bool> GetOrCreateUsingAliasCache(this Compilation compilation)
        {
            var cache = usingAliasCache;

            Compilation cachedCompilation;
            if (!cache.Item1.TryGetTarget(out cachedCompilation) || cachedCompilation != compilation)
            {
                var replacementDictionary = CreateDictionary(compilation);
                var replacementCache = Tuple.Create(new WeakReference<Compilation>(compilation), replacementDictionary);

                while (true)
                {
                    var prior = Interlocked.CompareExchange(ref usingAliasCache, replacementCache, cache);
                    if (prior == cache)
                    {
                        cache = replacementCache;
                        break;
                    }

                    cache = prior;
                    if (cache.Item1.TryGetTarget(out cachedCompilation) && cachedCompilation == compilation)
                    {
                        break;
                    }
                }
            }

            return cache.Item2;
        }

        /// <summary>
        /// Checks if a given <see cref="SyntaxTree"/> only contains whitespace. We don't want to analyze empty files.
        /// </summary>
        /// <param name="tree">The syntax tree to examine.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that the task will observe.</param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="tree"/> only contains whitespace; otherwise,
        /// <see langword="false"/>.
        /// </returns>
        public static bool IsWhitespaceOnly(this SyntaxTree tree, CancellationToken cancellationToken)
        {
            var root = tree.GetRoot(cancellationToken);
            var firstToken = root.GetFirstToken(includeZeroWidth: true);

            return firstToken.IsKind(SyntaxKind.EndOfFileToken)
                && TriviaHelper.IndexOfFirstNonWhitespaceTrivia(firstToken.LeadingTrivia) == -1;
        }

        internal static bool ContainsUsingAlias(this SyntaxTree tree, IReadOnlyDictionary<SyntaxTree, bool> cache)
        {
            if (tree == null)
            {
                return false;
            }

            if (cache == null)
            {
                // NOTE: This happens if any syntax tree in the compilation contains a global using alias
                return true;
            }

            if (cache.TryGetValue(tree, out var result))
            {
                return result;
            }

            Debug.Assert(false, "This should not happen. Syntax tree could not be found in cache!");
            return false;
        }

        private static IReadOnlyDictionary<SyntaxTree, bool> CreateDictionary(Compilation compilation)
        {
            var result = new Dictionary<SyntaxTree, bool>();

            foreach (var tree in compilation.SyntaxTrees)
            {
                CheckUsingAliases(tree, out var containsUsingAlias, out var containsGlobalUsingAlias);
                if (containsGlobalUsingAlias)
                {
                    return null;
                }

                result.Add(tree, containsUsingAlias);
            }

            return result;
        }

        private static void CheckUsingAliases(SyntaxTree tree, out bool containsUsingAlias, out bool containsGlobalUsingAlias)
        {
            var usingNodes = tree.GetRoot().DescendantNodes(node => node.IsKind(SyntaxKind.CompilationUnit) || node.IsKind(SyntaxKind.NamespaceDeclaration) || node.IsKind(SyntaxKindEx.FileScopedNamespaceDeclaration)).OfType<UsingDirectiveSyntax>();
            var usingAliasNodes = usingNodes.Where(x => x.Alias != null).ToList();
            containsUsingAlias = usingAliasNodes.Any();
            containsGlobalUsingAlias = usingAliasNodes.Any(x => !x.GlobalKeyword().IsKind(SyntaxKind.None));
        }
    }
}
