// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Helpers
{
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Threading;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using StyleCop.Analyzers.Lightup;

    internal sealed class UsingAliasCache
    {
        /// <summary>
        /// Caches a value indicating whether the compilation has any global using aliases.
        /// </summary>
        /// <value>
        /// <para>One of the following:</para>
        ///
        /// <list type="bullet">
        /// <item><description>-1: if the value has not yet been computed</description></item>
        /// <item><description>0: if the compilation does not have any global using aliases</description></item>
        /// <item><description>1: if the compilation has one or more global using aliases</description></item>
        /// </list>
        /// </value>
        private int perCompilationCache = -1;

        /// <summary>
        /// A cache of individual syntax trees to a value indicating whether that syntax tree has any local using
        /// aliases.
        /// </summary>
        /// <remarks><para>This field is only initialized to a non-null value when <see cref="perCompilationCache"/> is
        /// 0 (i.e. the compilation is known to not have any global using aliases).</para></remarks>
        private ConcurrentDictionary<SyntaxTree, bool>? perSyntaxTreeCache;

        internal bool ContainsUsingAlias(SyntaxTree tree, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            if (tree == null)
            {
                return false;
            }

            if (this.ContainsGlobalUsingAlias(semanticModel, cancellationToken))
            {
                return true;
            }

            var cache = LazyInitializer.EnsureInitialized(ref this.perSyntaxTreeCache, static () => new ConcurrentDictionary<SyntaxTree, bool>())!;

            bool result;
            if (cache.TryGetValue(tree, out result))
            {
                return result;
            }

            bool generated = ContainsLocalUsingAliasNoCache(tree);
            cache.TryAdd(tree, generated);
            return generated;
        }

        private static bool ContainsLocalUsingAliasNoCache(SyntaxTree tree)
        {
            // Check for "local" using aliases
            var nodes = tree.GetRoot().DescendantNodes(node => node.IsKind(SyntaxKind.CompilationUnit) || node.IsKind(SyntaxKind.NamespaceDeclaration) || node.IsKind(SyntaxKindEx.FileScopedNamespaceDeclaration));
            return nodes.OfType<UsingDirectiveSyntax>().Any(x => x.Alias != null);
        }

        private bool ContainsGlobalUsingAlias(SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            if (this.perCompilationCache == -1)
            {
                // Check for global using aliases
                var scopes = semanticModel.GetImportScopes(0, cancellationToken);
                Interlocked.CompareExchange(ref this.perCompilationCache, scopes.Any(x => x.Aliases.Length > 0) ? 1 : 0, -1);
            }

            return this.perCompilationCache == 1;
        }
    }
}
