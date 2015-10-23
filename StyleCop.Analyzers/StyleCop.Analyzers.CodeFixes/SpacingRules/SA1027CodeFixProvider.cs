// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.SpacingRules
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Composition;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Helpers;
    using Helpers.ObjectPools;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Text;

    /// <summary>
    /// Implements a code fix for <see cref="SA1027TabsMustNotBeUsed"/>.
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1027CodeFixProvider))]
    [Shared]
    internal class SA1027CodeFixProvider : CodeFixProvider
    {
        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds { get; } =
            ImmutableArray.Create(SA1027TabsMustNotBeUsed.DiagnosticId);

        /// <inheritdoc/>
        public override FixAllProvider GetFixAllProvider()
        {
            return FixAll.Instance;
        }

        /// <inheritdoc/>
        public override Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            foreach (Diagnostic diagnostic in context.Diagnostics)
            {
                context.RegisterCodeFix(
                    CodeAction.Create(
                        SpacingResources.SA1027CodeFix,
                        cancellationToken => GetTransformedDocumentAsync(context.Document, diagnostic, cancellationToken),
                        nameof(SA1027CodeFixProvider)),
                    diagnostic);
            }

            return SpecializedTasks.CompletedTask;
        }

        private static async Task<Document> GetTransformedDocumentAsync(Document document, Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            var indentationOptions = IndentationOptions.FromDocument(document);
            SourceText sourceText = await document.GetTextAsync(cancellationToken).ConfigureAwait(false);
            return document.WithText(sourceText.WithChanges(FixDiagnostic(indentationOptions, sourceText, diagnostic)));
        }

        private static TextChange FixDiagnostic(IndentationOptions indentationOptions, SourceText sourceText, Diagnostic diagnostic)
        {
            TextSpan span = diagnostic.Location.SourceSpan;

            TextLine startLine = sourceText.Lines.GetLineFromPosition(span.Start);
            string text = sourceText.ToString(TextSpan.FromBounds(startLine.Start, span.End));
            StringBuilder replacement = StringBuilderPool.Allocate();
            int column = 0;
            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                if (c == '\t')
                {
                    var offsetWithinTabColumn = column % indentationOptions.TabSize;
                    var spaceCount = indentationOptions.TabSize - offsetWithinTabColumn;

                    if (i >= span.Start - startLine.Start)
                    {
                        replacement.Append(' ', spaceCount);
                    }

                    column += spaceCount;
                }
                else
                {
                    if (i >= span.Start - startLine.Start)
                    {
                        replacement.Append(c);
                    }

                    if (c == '\n')
                    {
                        column = 0;
                    }
                    else
                    {
                        column++;
                    }
                }
            }

            return new TextChange(span, StringBuilderPool.ReturnAndFree(replacement));
        }

        private class FixAll : DocumentBasedFixAllProvider
        {
            public static FixAllProvider Instance { get; }
                = new FixAll();

            protected override string CodeActionTitle
                => SpacingResources.SA1027CodeFix;

            protected override async Task<SyntaxNode> FixAllInDocumentAsync(FixAllContext fixAllContext, Document document)
            {
                var diagnostics = await fixAllContext.GetDocumentDiagnosticsAsync(document).ConfigureAwait(false);
                if (diagnostics.IsEmpty)
                {
                    return null;
                }

                var indentationOptions = IndentationOptions.FromDocument(document);
                SourceText sourceText = await document.GetTextAsync(fixAllContext.CancellationToken).ConfigureAwait(false);

                List<TextChange> changes = new List<TextChange>();
                foreach (var diagnostic in diagnostics)
                {
                    changes.Add(FixDiagnostic(indentationOptions, sourceText, diagnostic));
                }

                changes.Sort((left, right) => left.Span.Start.CompareTo(right.Span.Start));

                SyntaxTree tree = await document.GetSyntaxTreeAsync(fixAllContext.CancellationToken).ConfigureAwait(false);
                return await tree.WithChangedText(sourceText.WithChanges(changes)).GetRootAsync(fixAllContext.CancellationToken).ConfigureAwait(false);
            }
        }
    }
}
