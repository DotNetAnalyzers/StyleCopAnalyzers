// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.ReadabilityRules;

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using StyleCop.Analyzers.Helpers;

/// <summary>
/// Implements a code fix for <see cref="SA1144ZeroWidthCharactersInLiteralsMustBeEscaped"/>.
/// </summary>
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1144CodeFixProvider))]
[Shared]
internal class SA1144CodeFixProvider : CodeFixProvider
{
    /// <inheritdoc/>
    public override ImmutableArray<string> FixableDiagnosticIds { get; } =
        ImmutableArray.Create(SA1144ZeroWidthCharactersInLiteralsMustBeEscaped.DiagnosticId);

    /// <inheritdoc/>
    public override FixAllProvider GetFixAllProvider()
        => CustomFixAllProviders.BatchFixer;

    /// <inheritdoc/>
    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
        foreach (var diagnostic in context.Diagnostics)
        {
            var token = root.FindToken(diagnostic.Location.SourceSpan.Start);
            if (token.Span != diagnostic.Location.SourceSpan)
            {
                continue;
            }

            if (SA1144ZeroWidthCharactersInLiteralsMustBeEscaped.SupportsEscapeSequences(token))
            {
                context.RegisterCodeFix(
                    CodeAction.Create(
                        ReadabilityResources.SA1144CodeFix,
                        cancellationToken => GetTransformedDocumentAsync(context.Document, diagnostic, escape: true, cancellationToken),
                        nameof(SA1144CodeFixProvider) + "Escape"),
                    diagnostic);
            }

            // Removing the only character of a character literal would leave an invalid literal.
            if (!token.IsKind(SyntaxKind.CharacterLiteralToken))
            {
                context.RegisterCodeFix(
                    CodeAction.Create(
                        ReadabilityResources.SA1144CodeFixRemove,
                        cancellationToken => GetTransformedDocumentAsync(context.Document, diagnostic, escape: false, cancellationToken),
                        nameof(SA1144CodeFixProvider) + "Remove"),
                    diagnostic);
            }
        }
    }

    private static async Task<Document> GetTransformedDocumentAsync(Document document, Diagnostic diagnostic, bool escape, CancellationToken cancellationToken)
    {
        var text = await document.GetTextAsync(cancellationToken).ConfigureAwait(false);
        var span = diagnostic.Location.SourceSpan;
        var changes = new List<TextChange>();
        for (var position = span.Start; position < span.End; position++)
        {
            var c = text[position];
            if (SA1144ZeroWidthCharactersInLiteralsMustBeEscaped.IsZeroWidthCharacter(c))
            {
                changes.Add(new TextChange(new TextSpan(position, 1), escape ? SA1144ZeroWidthCharactersInLiteralsMustBeEscaped.GetEscapeSequence(c) : string.Empty));
            }
        }

        return document.WithText(text.WithChanges(changes));
    }
}
