// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Helpers
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Threading;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Text;

    internal static class SyntaxTreeHelpers
    {
        /// <summary>
        /// A cache of the result of computing whether a document has using alias directives.
        /// </summary>
        /// <remarks>
        /// This allows many analyzers that run on every token in the file to avoid checking
        /// the same state in the document repeatedly.
        /// </remarks>
        private static Tuple<WeakReference<Compilation>, ConcurrentDictionary<SyntaxTree, bool>> usingAliasCache
            = Tuple.Create(new WeakReference<Compilation>(null), default(ConcurrentDictionary<SyntaxTree, bool>));

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

        /// <summary>
        /// Locates all spans within the <see cref="SyntaxNode.FullSpan"/> of a syntax node which should be excluded
        /// from formatting operations.
        /// </summary>
        /// <remarks>
        /// <para>Spans are excluded from formatting operations any time they have a direct impact on the semantics of
        /// the compiled application. Spans are also excluded in cases where the impact is unknown.</para>
        /// <list type="bullet">
        /// <item>Spans disabled due to preprocessor directives (<see cref="SyntaxKind.DisabledTextTrivia"/>).</item>
        /// <item>The contents of commented code (comments starting with <c>////</c>).</item>
        /// <item>Content in a CDATA section of an XML comment (<see cref="SyntaxKind.XmlCDataSection"/>).</item>
        /// <item>Content in a character literal (<see cref="SyntaxKind.CharacterLiteralToken"/>).</item>
        /// <item>Content in a string literal (<see cref="SyntaxKind.StringLiteralToken"/>).</item>
        /// <item>Literal content in an interpolated string literal (<see cref="SyntaxKind.InterpolatedStringTextToken"/>).</item>
        /// </list>
        /// </remarks>
        /// <param name="root">The syntax node to examine.</param>
        /// <returns>
        /// A collection of <see cref="TextSpan"/> instances indicating the spans to exclude from formatting operations.
        /// The collection is ordered according to the start position of spans, and does not contain any overlapping or
        /// adjacent spans.
        /// </returns>
        internal static ImmutableArray<TextSpan> GetExcludedSpans(SyntaxNode root)
        {
            ImmutableArray<TextSpan>.Builder builder = ImmutableArray.CreateBuilder<TextSpan>();

            // Locate disabled text
            foreach (var trivia in root.DescendantTrivia(descendIntoTrivia: true))
            {
                if (trivia.IsKind(SyntaxKind.DisabledTextTrivia))
                {
                    builder.Add(trivia.Span);
                }
                else if (trivia.IsKind(SyntaxKind.SingleLineCommentTrivia))
                {
                    if (trivia.ToString().StartsWith("////"))
                    {
                        // Exclude comments starting with //// because they could contain commented code which contains
                        // string or character literals, and we don't want to change the contents of those strings.
                        builder.Add(trivia.Span);
                    }
                }
            }

            // Locate string literals
            foreach (var token in root.DescendantTokens(descendIntoTrivia: true))
            {
                switch (token.Kind())
                {
                case SyntaxKind.XmlTextLiteralToken:
                    if (token.Parent.IsKind(SyntaxKind.XmlCDataSection))
                    {
                        builder.Add(token.Span);
                    }

                    break;

                case SyntaxKind.CharacterLiteralToken:
                case SyntaxKind.StringLiteralToken:
                case SyntaxKind.InterpolatedStringTextToken:
                    builder.Add(token.Span);
                    break;

                default:
                    break;
                }
            }

            // Sort the results
            builder.Sort();

            // Combine adjacent and overlapping spans
            ReduceTextSpans(builder);

            return builder.ToImmutable();
        }

        private static void ReduceTextSpans(ImmutableArray<TextSpan>.Builder sortedTextSpans)
        {
            if (sortedTextSpans.Count == 0)
            {
                return;
            }

            int currentIndex = 0;
            for (int nextIndex = 1; nextIndex < sortedTextSpans.Count; nextIndex++)
            {
                TextSpan current = sortedTextSpans[currentIndex];
                TextSpan next = sortedTextSpans[nextIndex];
                if (current.End < next.Start)
                {
                    // Increment currentIndex this iteration
                    currentIndex++;

                    // Only increment nextIndex this iteration if necessary to ensure nextIndex > currentIndex on the
                    // next iteration. At this point we already incremented currentIndex, but haven't incremented
                    // nextIndex.
                    if (currentIndex > nextIndex)
                    {
                        nextIndex--;
                    }

                    continue;
                }

                // Since sortedTextSpans is sorted, we already know current and next overlap
                sortedTextSpans[currentIndex] = TextSpan.FromBounds(current.Start, next.End);
            }

            sortedTextSpans.Count = currentIndex + 1;
        }

        private static bool ContainsUsingAliasNoCache(SyntaxTree tree)
        {
            var nodes = tree.GetRoot().DescendantNodes(node => node.IsKind(SyntaxKind.CompilationUnit) || node.IsKind(SyntaxKind.NamespaceDeclaration));

            return nodes.OfType<UsingDirectiveSyntax>().Any(x => x.Alias != null);
        }
    }
}
