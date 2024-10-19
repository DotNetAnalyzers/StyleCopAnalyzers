// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.MaintainabilityRules
{
    using System.Collections.Immutable;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using StyleCop.Analyzers.Helpers;

    internal sealed class SA1412FixAllProvider : FixAllProvider
    {
        public override Task<CodeAction> GetFixAsync(FixAllContext fixAllContext)
        {
            string title = string.Format(MaintainabilityResources.SA1412CodeFix, fixAllContext.CodeActionEquivalenceKey.Substring(fixAllContext.CodeActionEquivalenceKey.IndexOf('.') + 1));

            CodeAction fixAction;
            switch (fixAllContext.Scope)
            {
            case FixAllScope.Document:
                fixAction = CodeAction.Create(
                    title,
                    cancellationToken => GetDocumentFixesAsync(fixAllContext.WithCancellationToken(cancellationToken)),
                    nameof(SA1412FixAllProvider));
                break;

            case FixAllScope.Project:
                fixAction = CodeAction.Create(
                    title,
                    cancellationToken => GetProjectFixesAsync(fixAllContext.WithCancellationToken(cancellationToken)),
                    nameof(SA1412FixAllProvider));
                break;

            case FixAllScope.Solution:
                fixAction = CodeAction.Create(
                    title,
                    cancellationToken => GetSolutionFixesAsync(fixAllContext.WithCancellationToken(cancellationToken)),
                    nameof(SA1412FixAllProvider));
                break;

            case FixAllScope.Custom:
            default:
                fixAction = null;
                break;
            }

            return Task.FromResult(fixAction);
        }

        private static async Task<Solution> FixDocumentAsync(Solution solution, DocumentId documentId, ImmutableArray<Diagnostic> diagnostics, string codeActionEquivalenceKey, CancellationToken cancellationToken)
        {
            if (diagnostics.IsEmpty)
            {
                return solution;
            }

            string equivalenceKey = nameof(SA1412CodeFixProvider) + "." + diagnostics[0].Properties[SA1412StoreFilesAsUtf8.EncodingProperty];
            if (codeActionEquivalenceKey != equivalenceKey)
            {
                return solution;
            }

            Document document = solution.GetDocument(documentId);
            return await SA1412CodeFixProvider.GetTransformedSolutionAsync(document, cancellationToken).ConfigureAwait(false);
        }

        private static async Task<Solution> GetDocumentFixesAsync(FixAllContext fixAllContext)
        {
            var documentDiagnosticsToFix = await FixAllContextHelper.GetDocumentDiagnosticsToFixAsync(fixAllContext).ConfigureAwait(false);
            ImmutableArray<Diagnostic> diagnostics;
            if (!documentDiagnosticsToFix.TryGetValue(fixAllContext.Document, out diagnostics))
            {
                return fixAllContext.Document.Project.Solution;
            }

            return await FixDocumentAsync(fixAllContext.Document.Project.Solution, fixAllContext.Document.Id, diagnostics, fixAllContext.CodeActionEquivalenceKey, fixAllContext.CancellationToken).ConfigureAwait(false);
        }

        private static async Task<Solution> GetSolutionFixesAsync(FixAllContext fixAllContext, ImmutableArray<Document> documents)
        {
            var documentDiagnosticsToFix = await FixAllContextHelper.GetDocumentDiagnosticsToFixAsync(fixAllContext).ConfigureAwait(false);

            Solution solution = fixAllContext.Solution;
            foreach (var document in documents)
            {
                ImmutableArray<Diagnostic> diagnostics;
                if (!documentDiagnosticsToFix.TryGetValue(document, out diagnostics))
                {
                    continue;
                }

                solution = await FixDocumentAsync(solution, document.Id, diagnostics, fixAllContext.CodeActionEquivalenceKey, fixAllContext.CancellationToken).ConfigureAwait(false);
            }

            return solution;
        }

        private static Task<Solution> GetProjectFixesAsync(FixAllContext fixAllContext)
        {
            return GetSolutionFixesAsync(fixAllContext, fixAllContext.Project.Documents.ToImmutableArray());
        }

        private static Task<Solution> GetSolutionFixesAsync(FixAllContext fixAllContext)
        {
            ImmutableArray<Document> documents = fixAllContext.Solution.Projects.SelectMany(i => i.Documents).ToImmutableArray();
            return GetSolutionFixesAsync(fixAllContext, documents);
        }
    }
}
