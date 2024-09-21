// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Helpers
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
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

        /// <summary>
        /// Recursively descends through the tree structure, enumerating all Statement nodes encountered during the traversal.
        /// </summary>
        /// <param name="syntaxNode">The syntax node to recursively.</param>
        /// <returns>Enumerated StatementSyntax.</returns>
        public static IEnumerable<StatementSyntax> StatementDescendRecursively(this SyntaxNode syntaxNode)
        {
            foreach (var statement in syntaxNode.ChildNodes().OfType<StatementSyntax>())
            {
                yield return statement;

                foreach (var innerItem in StatementDescendRecursively(statement))
                {
                    yield return innerItem;
                }
            }
        }

        /// <summary>
        /// Recursively descends through the tree structure, enumerating all Expression nodes encountered during the traversal.
        /// </summary>
        /// <param name="syntaxNode">The syntax node to recursively.</param>
        /// <returns>Enumerated ExpressionSyntax.</returns>
        public static IEnumerable<ExpressionSyntax> ExpressionDescendRecursively(this SyntaxNode syntaxNode)
        {
            foreach (var node in syntaxNode.ChildNodes())
            {
                if (node is StatementSyntax statementSyntax)
                {
                    foreach (var expression in ExpressionDescendRecursively(statementSyntax))
                    {
                        yield return expression;
                    }
                }
                else if (node is ExpressionSyntax expressionSyntax)
                {
                    yield return expressionSyntax;

                    foreach (var expression in ExpressionDescendRecursively(expressionSyntax))
                    {
                        yield return expression;
                    }
                }
            }
        }

        /// <summary>
        /// Recursively descends through the tree structure, enumerating all Expression nodes encountered during the traversal.
        /// </summary>
        /// <param name="expressionSyntax">The expression syntax to recursively.</param>
        /// <returns>Enumerated ExpressionSyntax.</returns>
        public static IEnumerable<ExpressionSyntax> ExpressionDescendRecursively(this ExpressionSyntax expressionSyntax)
        {
            foreach (var inner in expressionSyntax.ChildNodes().OfType<ExpressionSyntax>())
            {
                yield return inner;

                foreach (var innerItem in ExpressionDescendRecursively(inner))
                {
                    yield return innerItem;
                }
            }
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
