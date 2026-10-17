// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.ReadabilityRules;

using System.Collections.Immutable;
using System.Composition;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using StyleCop.Analyzers.Helpers;

/// <summary>
/// Implements a code fix for <see cref="SA1115ParameterMustFollowComma"/>.
/// </summary>
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1115CodeFixProvider))]
[Shared]
internal class SA1115CodeFixProvider : CodeFixProvider
{
    /// <inheritdoc/>
    public override ImmutableArray<string> FixableDiagnosticIds { get; } =
        ImmutableArray.Create(SA1115ParameterMustFollowComma.DiagnosticId);

    /// <inheritdoc/>
    public override FixAllProvider GetFixAllProvider()
    {
        return CustomFixAllProviders.BatchFixer;
    }

    /// <inheritdoc/>
    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
        foreach (var diagnostic in context.Diagnostics)
        {
            var token = root.FindToken(diagnostic.Location.SourceSpan.Start);
            if (token.LeadingTrivia.WithoutBlankLines().Count == token.LeadingTrivia.Count)
            {
                continue;
            }

            context.RegisterCodeFix(
                CodeAction.Create(
                    ReadabilityResources.SA1115CodeFix,
                    cancellationToken => GetTransformedDocumentAsync(context.Document, diagnostic, cancellationToken),
                    nameof(SA1115CodeFixProvider)),
                diagnostic);
        }
    }

    private static async Task<Document> GetTransformedDocumentAsync(Document document, Diagnostic diagnostic, CancellationToken cancellationToken)
    {
        var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        var token = root.FindToken(diagnostic.Location.SourceSpan.Start);
        var newToken = token.WithLeadingTrivia(token.LeadingTrivia.WithoutBlankLines());
        return document.WithSyntaxRoot(root.ReplaceToken(token, newToken));
    }
}
