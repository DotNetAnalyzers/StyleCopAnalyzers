// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Helpers
{
    using System;
    using System.Threading;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;

    internal static class SyntaxTreeHelpers
    {
        /// <summary>
        /// A cache of the result of computing whether a document has using alias directives.
        /// </summary>
        /// <remarks>
        /// <para>This allows many analyzers that run on every token in the file to avoid checking
        /// the same state in the document repeatedly.</para>
        /// </remarks>
        private static Tuple<WeakReference<Compilation>, UsingAliasCache> usingAliasCache
            = Tuple.Create(new WeakReference<Compilation>(null), default(UsingAliasCache));

        public static UsingAliasCache GetOrCreateUsingAliasCache(this Compilation compilation)
        {
            var cache = usingAliasCache;

            Compilation cachedCompilation;
            if (!cache.Item1.TryGetTarget(out cachedCompilation) || cachedCompilation != compilation)
            {
                var replacementCache = Tuple.Create(new WeakReference<Compilation>(compilation), new UsingAliasCache());
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
    }
}
