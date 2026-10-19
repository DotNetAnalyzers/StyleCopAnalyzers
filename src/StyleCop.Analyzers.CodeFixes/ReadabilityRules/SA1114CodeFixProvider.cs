// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.ReadabilityRules;

using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using StyleCop.Analyzers.Helpers;

/// <summary>
/// Implements a code fix for <see cref="SA1114ParameterListMustFollowDeclaration"/>.
/// </summary>
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1114CodeFixProvider))]
[Shared]
internal class SA1114CodeFixProvider : CodeFixProvider
{
    /// <inheritdoc/>
    public override ImmutableArray<string> FixableDiagnosticIds { get; } =
        ImmutableArray.Create(SA1114ParameterListMustFollowDeclaration.DiagnosticId);

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
            if (token.LeadingTrivia.WithoutBlankLines().Count == token.LeadingTrivia.Count
                || (token.LeadingTrivia.Any(trivia => trivia.IsDirective) && IsArgumentListItem(token)))
            {
                continue;
            }

            context.RegisterCodeFix(
                CodeAction.Create(
                    ReadabilityResources.SA1114CodeFix,
                    cancellationToken => GetTransformedDocumentAsync(context.Document, diagnostic, cancellationToken),
                    nameof(SA1114CodeFixProvider)),
                diagnostic);
        }
    }

    private static bool IsArgumentListItem(SyntaxToken token)
    {
        for (var node = token.Parent; node != null; node = node.Parent)
        {
            if (node is BaseArgumentListSyntax or AttributeArgumentListSyntax or ArrayRankSpecifierSyntax)
            {
                return true;
            }

            if (node is BaseParameterListSyntax)
            {
                return false;
            }
        }

        return false;
    }

    private static async Task<Document> GetTransformedDocumentAsync(Document document, Diagnostic diagnostic, CancellationToken cancellationToken)
    {
        var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        var token = root.FindToken(diagnostic.Location.SourceSpan.Start);
        var newToken = token.WithLeadingTrivia(token.LeadingTrivia.WithoutBlankLines());
        return document.WithSyntaxRoot(root.ReplaceToken(token, newToken));
    }
}
