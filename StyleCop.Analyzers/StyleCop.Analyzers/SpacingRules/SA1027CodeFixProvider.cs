// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.SpacingRules
{
    using System.Collections.Immutable;
    using System.Composition;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Text;
    using StyleCop.Analyzers.Helpers;

    /// <summary>
    /// Implements a code fix for <see cref="SA1027TabsMustNotBeUsed"/>.
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1027CodeFixProvider))]
    [Shared]
    public class SA1027CodeFixProvider : CodeFixProvider
    {
        private static readonly ImmutableArray<string> FixableDiagnostics =
            ImmutableArray.Create(SA1027TabsMustNotBeUsed.DiagnosticId);

        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds => FixableDiagnostics;

        /// <inheritdoc/>
        public override FixAllProvider GetFixAllProvider()
        {
            return CustomFixAllProviders.BatchFixer;
        }

        /// <inheritdoc/>
        public override Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            foreach (Diagnostic diagnostic in context.Diagnostics.Where(d => FixableDiagnostics.Contains(d.Id)))
            {
                context.RegisterCodeFix(CodeAction.Create(SpacingResources.SA1027CodeFix, token => GetTransformedDocumentAsync(context.Document, diagnostic, token), equivalenceKey: nameof(SA1027CodeFixProvider)), diagnostic);
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
            StringBuilder replacement = new StringBuilder();
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

            return new TextChange(span, replacement.ToString());
        }
    }
}
