// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.MaintainabilityRules;

using System.Collections.Immutable;
using System.Composition;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using StyleCop.Analyzers.Helpers;

/// <summary>
/// Implements a code fix for <see cref="SA1415DoNotUsePublicOnMembersOfInternalTypes"/>.
/// </summary>
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1415CodeFixProvider))]
[Shared]
internal sealed class SA1415CodeFixProvider : CodeFixProvider
{
    /// <inheritdoc/>
    public override ImmutableArray<string> FixableDiagnosticIds { get; } =
        ImmutableArray.Create(SA1415DoNotUsePublicOnMembersOfInternalTypes.DiagnosticId);

    /// <inheritdoc/>
    public override FixAllProvider GetFixAllProvider() => CustomFixAllProviders.BatchFixer;

    /// <inheritdoc/>
    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
        if (root is null)
        {
            return;
        }

        foreach (var diagnostic in context.Diagnostics)
        {
            var token = root.FindToken(diagnostic.Location.SourceSpan.Start);
            if (!token.IsKind(SyntaxKind.PublicKeyword))
            {
                continue;
            }

            context.RegisterCodeFix(
                CodeAction.Create(
                    MaintainabilityResources.SA1415CodeFix,
                    cancellationToken => ReplacePublicWithInternalAsync(context.Document, root, token),
                    nameof(SA1415CodeFixProvider)),
                diagnostic);
        }
    }

    private static Task<Document> ReplacePublicWithInternalAsync(Document document, SyntaxNode root, SyntaxToken publicToken)
    {
        var internalToken = SyntaxFactory.Token(publicToken.LeadingTrivia, SyntaxKind.InternalKeyword, publicToken.TrailingTrivia);
        return Task.FromResult(document.WithSyntaxRoot(root.ReplaceToken(publicToken, internalToken)));
    }
}
