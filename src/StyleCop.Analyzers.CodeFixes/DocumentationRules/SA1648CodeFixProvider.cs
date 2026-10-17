// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.DocumentationRules;

using System.Collections.Immutable;
using System.Composition;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using StyleCop.Analyzers.Helpers;

/// <summary>
/// Implements a code fix for <see cref="SA1648InheritDocMustBeUsedWithInheritingClass"/>.
/// </summary>
/// <remarks>
/// <para>To fix a violation of this rule, remove the invalid <c>&lt;inheritdoc/&gt;</c> element.</para>
/// </remarks>
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1648CodeFixProvider))]
[Shared]
internal class SA1648CodeFixProvider : CodeFixProvider
{
    /// <inheritdoc/>
    public override ImmutableArray<string> FixableDiagnosticIds { get; } =
        ImmutableArray.Create(SA1648InheritDocMustBeUsedWithInheritingClass.DiagnosticId);

    /// <inheritdoc/>
    public override FixAllProvider GetFixAllProvider()
    {
        return CustomFixAllProviders.BatchFixer;
    }

    /// <inheritdoc/>
    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
        foreach (Diagnostic diagnostic in context.Diagnostics)
        {
            if (root.FindNode(diagnostic.Location.SourceSpan, findInsideTrivia: true, getInnermostNodeForTie: true) is XmlEmptyElementSyntax xmlEmptyElement
                && xmlEmptyElement.Name.ToString() == "include")
            {
                continue;
            }

            context.RegisterCodeFix(
                CodeAction.Create(
                    DocumentationResources.SA1648CodeFix,
                    cancellationToken => RemoveInheritdocAsync(context.Document, diagnostic, cancellationToken),
                    nameof(SA1648CodeFixProvider)),
                diagnostic);
        }
    }

    private static async Task<Document> RemoveInheritdocAsync(Document document, Diagnostic diagnostic, CancellationToken cancellationToken)
    {
        SourceText sourceText = await document.GetTextAsync(cancellationToken).ConfigureAwait(false);
        TextSpan removalSpan = diagnostic.Location.SourceSpan;
        TextLine line = sourceText.Lines.GetLineFromPosition(removalSpan.Start);

        if (removalSpan.End <= line.End
            && sourceText.ToString(TextSpan.FromBounds(line.Start, removalSpan.Start)).Trim() == "///"
            && string.IsNullOrWhiteSpace(sourceText.ToString(TextSpan.FromBounds(removalSpan.End, line.End))))
        {
            removalSpan = line.SpanIncludingLineBreak;
        }

        return document.WithText(sourceText.WithChanges(new TextChange(removalSpan, string.Empty)));
    }
}
