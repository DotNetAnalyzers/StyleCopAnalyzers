// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Helpers
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Threading;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using StyleCop.Analyzers.Lightup;
    using StyleCop.Analyzers.Settings.ObjectModel;

    internal static class SyntaxTreeHelpers
    {
        /// <summary>
        /// A cache of the result of computing whether a document has using alias directives.
        /// </summary>
        /// <remarks>
        /// <para>This allows many analyzers that run on every token in the file to avoid checking
        /// the same state in the document repeatedly.</para>
        /// </remarks>
        private static Tuple<WeakReference<Compilation>, ConcurrentDictionary<SyntaxTree, bool>> usingAliasCache
            = Tuple.Create(new WeakReference<Compilation>(null), default(ConcurrentDictionary<SyntaxTree, bool>));

        /// <summary>
        /// A cache of the StyleCopSettings for a document.
        /// </summary>
        /// <remarks>
        /// <para>This allows analyzers that needs the settings to not all need to re-create them during compilation of a project.</para>
        /// </remarks>
        private static Tuple<WeakReference<Compilation>, ConcurrentDictionary<SyntaxTree, Lazy<StyleCopSettings>>> styleCopSettingsCache
            = Tuple.Create(new WeakReference<Compilation>(null), default(ConcurrentDictionary<SyntaxTree, Lazy<StyleCopSettings>>));

        public static ConcurrentDictionary<SyntaxTree, bool> GetOrCreateUsingAliasCache(this Compilation compilation)
        {
            var cache = usingAliasCache;

            Compilation cachedCompilation;
            if (!cache.Item1.TryGetTarget(out cachedCompilation) || cachedCompilation != compilation)
            {
                var replacementCache = Tuple.Create(new WeakReference<Compilation>(compilation), new ConcurrentDictionary<SyntaxTree, bool>());
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

        public static ConcurrentDictionary<SyntaxTree, Lazy<StyleCopSettings>> GetOrCreateStyleCopSettingsCache(this Compilation compilation)
        {
            var cache = styleCopSettingsCache;

            Compilation cachedCompilation;
            if (!cache.Item1.TryGetTarget(out cachedCompilation) || cachedCompilation != compilation)
            {
                var replacementCache = Tuple.Create(new WeakReference<Compilation>(compilation), new ConcurrentDictionary<SyntaxTree, Lazy<StyleCopSettings>>());
                while (true)
                {
                    var prior = Interlocked.CompareExchange(ref styleCopSettingsCache, replacementCache, cache);
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

        internal static bool ContainsUsingAlias(this SyntaxTree tree, ConcurrentDictionary<SyntaxTree, bool> cache)
        {
            if (tree == null)
            {
                return false;
            }

            bool result;
            if (cache.TryGetValue(tree, out result))
            {
                return result;
            }

            bool generated = ContainsUsingAliasNoCache(tree);
            cache.TryAdd(tree, generated);
            return generated;
        }

        private static bool ContainsUsingAliasNoCache(SyntaxTree tree)
        {
            var nodes = tree.GetRoot().DescendantNodes(node => node.IsKind(SyntaxKind.CompilationUnit) || node.IsKind(SyntaxKind.NamespaceDeclaration) || node.IsKind(SyntaxKindEx.FileScopedNamespaceDeclaration));

            return nodes.OfType<UsingDirectiveSyntax>().Any(x => x.Alias != null);
        }
    }
}
