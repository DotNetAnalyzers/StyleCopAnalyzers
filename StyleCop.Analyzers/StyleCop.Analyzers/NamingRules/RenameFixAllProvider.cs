// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.NamingRules
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Helpers;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;

    internal class RenameFixAllProvider : FixAllProvider
    {
        private readonly string title;
        private readonly Func<ISymbol, string> getNewName;

        public RenameFixAllProvider(string title, Func<ISymbol, string> getNewName)
        {
            this.title = title;
            this.getNewName = getNewName;
        }

        public async override Task<CodeAction> GetFixAsync(FixAllContext fixAllContext)
        {
            switch (fixAllContext.Scope)
            {
            case FixAllScope.Document:
                return CodeAction.Create(this.title, async t => await GetActionAsync(fixAllContext.Solution, await fixAllContext.GetDocumentDiagnosticsAsync(fixAllContext.Document).ConfigureAwait(false), this.getNewName, t).ConfigureAwait(false));
            case FixAllScope.Project:
                return CodeAction.Create(this.title, async t => await GetActionAsync(fixAllContext.Solution, await fixAllContext.GetAllDiagnosticsAsync(fixAllContext.Project).ConfigureAwait(false), this.getNewName, t).ConfigureAwait(false));
            case FixAllScope.Solution:
                var diagnostics = new List<Diagnostic>();
                foreach (var item in fixAllContext.Solution.Projects)
                {
                    diagnostics.AddRange(await fixAllContext.GetAllDiagnosticsAsync(item).ConfigureAwait(false));
                }

                return CodeAction.Create(this.title, async t => await GetActionAsync(fixAllContext.Solution, diagnostics, this.getNewName, t).ConfigureAwait(false));
            case FixAllScope.Custom:
                break;
            default:
                break;
            }

            return null;
        }

        private static async Task<Solution> GetActionAsync(Solution solution, IEnumerable<Diagnostic> diagnostics, Func<ISymbol, string> getNewName, CancellationToken cancellationToken)
        {
            List<ISymbol> symbolsToRename = new List<ISymbol>();

            foreach (var grouping in diagnostics.Select(x => x.Location).GroupBy(y => y.SourceTree))
            {
                var document = solution.GetDocument(grouping.Key);
                var root = await document.GetSyntaxRootAsync().ConfigureAwait(false);
                var semanticModel = await document.GetSemanticModelAsync().ConfigureAwait(false);

                foreach (var location in grouping)
                {
                    var node = root.FindNode(location.SourceSpan, getInnermostNodeForTie: true);
                    var symbol = semanticModel.GetDeclaredSymbol(node) ?? semanticModel.GetSymbolInfo(node).Symbol;
                    if (symbol != null)
                    {
                        symbolsToRename.Add(symbol);
                    }
                }
            }

            return await RenameHelper.RenameAllSymbolsAsync(symbolsToRename.ToImmutableHashSet(), getNewName, solution, cancellationToken).ConfigureAwait(false);
        }
    }
}
