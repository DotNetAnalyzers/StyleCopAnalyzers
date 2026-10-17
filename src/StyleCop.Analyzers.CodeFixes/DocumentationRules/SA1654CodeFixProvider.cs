// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.DocumentationRules
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Composition;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Text;
    using StyleCop.Analyzers.Helpers;

    /// <summary>
    /// Implements a code fix for <see cref="SA1654ProperlyEncodeGenericBrackets"/>.
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1654CodeFixProvider))]
    [Shared]
    internal class SA1654CodeFixProvider : CodeFixProvider
    {
        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds { get; } =
            ImmutableArray.Create(SA1654ProperlyEncodeGenericBrackets.DiagnosticId);

        /// <inheritdoc/>
        public override FixAllProvider GetFixAllProvider()
        {
            return FixAll.Instance;
        }

        /// <inheritdoc/>
        public override Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            foreach (var diagnostic in context.Diagnostics)
            {
                context.RegisterCodeFix(
                    CodeAction.Create(
                        DocumentationResources.SA1654CodeFix,
                        cancellationToken => GetTransformedDocumentAsync(context.Document, diagnostic, cancellationToken),
                        nameof(SA1654CodeFixProvider)),
                    diagnostic);
            }

            return SpecializedTasks.CompletedTask;
        }

        private static async Task<Document> GetTransformedDocumentAsync(Document document, Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            var text = await document.GetTextAsync(cancellationToken).ConfigureAwait(false);
            var textChange = GetTextChange(text, diagnostic);
            return document.WithText(text.WithChanges(textChange));
        }

        private static TextChange GetTextChange(SourceText text, Diagnostic diagnostic)
        {
            var span = diagnostic.Location.SourceSpan;
            string crefText = text.ToString(span);
            string fixedCrefText = crefText.Replace("&lt;", "{").Replace("&gt;", "}");
            return new TextChange(span, fixedCrefText);
        }

        private class FixAll : DocumentBasedFixAllProvider
        {
            public static FixAllProvider Instance { get; } = new FixAll();

            protected override string CodeActionTitle => DocumentationResources.SA1654CodeFix;

            protected override async Task<SyntaxNode?> FixAllInDocumentAsync(FixAllContext fixAllContext, Document document, ImmutableArray<Diagnostic> diagnostics)
            {
                if (diagnostics.IsEmpty)
                {
                    return null;
                }

                var text = await document.GetTextAsync(fixAllContext.CancellationToken).ConfigureAwait(false);
                var changes = new List<TextChange>();
                foreach (var diagnostic in diagnostics)
                {
                    changes.Add(GetTextChange(text, diagnostic));
                }

                changes.Sort((left, right) => left.Span.Start.CompareTo(right.Span.Start));

                var tree = await document.GetSyntaxTreeAsync(fixAllContext.CancellationToken).ConfigureAwait(false);
                return await tree.WithChangedText(text.WithChanges(changes)).GetRootAsync(fixAllContext.CancellationToken).ConfigureAwait(false);
            }
        }
    }
}
