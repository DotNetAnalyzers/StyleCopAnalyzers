// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.MaintainabilityRules;

using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using StyleCop.Analyzers.Helpers;

internal sealed class SX1412FixAllProvider : FixAllProvider
{
    public override Task<CodeAction?> GetFixAsync(FixAllContext fixAllContext)
    {
        CodeAction? fixAction;
        switch (fixAllContext.Scope)
        {
        case FixAllScope.Document:
            fixAction = CodeAction.Create(
                MaintainabilityResources.SX1412CodeFix,
                cancellationToken => GetDocumentFixesAsync(fixAllContext.WithCancellationToken(cancellationToken)),
                nameof(SX1412FixAllProvider));
            break;

        case FixAllScope.Project:
            fixAction = CodeAction.Create(
                MaintainabilityResources.SX1412CodeFix,
                cancellationToken => GetProjectFixesAsync(fixAllContext.WithCancellationToken(cancellationToken)),
                nameof(SX1412FixAllProvider));
            break;

        case FixAllScope.Solution:
            fixAction = CodeAction.Create(
                MaintainabilityResources.SX1412CodeFix,
                cancellationToken => GetSolutionFixesAsync(fixAllContext.WithCancellationToken(cancellationToken)),
                nameof(SX1412FixAllProvider));
            break;

        case FixAllScope.Custom:
        default:
            fixAction = null;
            break;
        }

        return Task.FromResult(fixAction);
    }

    private static async Task<Solution> FixDocumentAsync(Solution solution, DocumentId documentId, ImmutableArray<Diagnostic> diagnostics, CancellationToken cancellationToken)
    {
        if (diagnostics.IsEmpty)
        {
            return solution;
        }

        Document document = solution.GetDocument(documentId);
        return await SX1412CodeFixProvider.GetTransformedSolutionAsync(document, cancellationToken).ConfigureAwait(false);
    }

    private static async Task<Solution> GetDocumentFixesAsync(FixAllContext fixAllContext)
    {
        var documentDiagnosticsToFix = await FixAllContextHelper.GetDocumentDiagnosticsToFixAsync(fixAllContext).ConfigureAwait(false);
        if (!documentDiagnosticsToFix.TryGetValue(fixAllContext.Document, out ImmutableArray<Diagnostic> diagnostics))
        {
            return fixAllContext.Document.Project.Solution;
        }

        return await FixDocumentAsync(fixAllContext.Document.Project.Solution, fixAllContext.Document.Id, diagnostics, fixAllContext.CancellationToken).ConfigureAwait(false);
    }

    private static async Task<Solution> GetSolutionFixesAsync(FixAllContext fixAllContext, ImmutableArray<Document> documents)
    {
        var documentDiagnosticsToFix = await FixAllContextHelper.GetDocumentDiagnosticsToFixAsync(fixAllContext).ConfigureAwait(false);
        Solution solution = fixAllContext.Solution;
        foreach (Document document in documents)
        {
            if (!documentDiagnosticsToFix.TryGetValue(document, out ImmutableArray<Diagnostic> diagnostics))
            {
                continue;
            }

            solution = await FixDocumentAsync(solution, document.Id, diagnostics, fixAllContext.CancellationToken).ConfigureAwait(false);
        }

        return solution;
    }

    private static Task<Solution> GetProjectFixesAsync(FixAllContext fixAllContext) =>
        GetSolutionFixesAsync(fixAllContext, fixAllContext.Project.Documents.ToImmutableArray());

    private static Task<Solution> GetSolutionFixesAsync(FixAllContext fixAllContext) =>
        GetSolutionFixesAsync(fixAllContext, fixAllContext.Solution.Projects.SelectMany(project => project.Documents).ToImmutableArray());
}
