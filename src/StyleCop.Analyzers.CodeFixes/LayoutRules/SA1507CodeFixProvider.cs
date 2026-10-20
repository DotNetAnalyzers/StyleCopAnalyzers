// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.LayoutRules
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
    /// Implements a code fix for <see cref="SA1507CodeMustNotContainMultipleBlankLinesInARow"/>.
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1507CodeFixProvider))]
    [Shared]
    internal class SA1507CodeFixProvider : CodeFixProvider
    {
        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds { get; } =
            ImmutableArray.Create(SA1507CodeMustNotContainMultipleBlankLinesInARow.DiagnosticId);

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
                        LayoutResources.SA1507CodeFix,
                        cancellationToken => GetTransformedDocumentAsync(context.Document, diagnostic, cancellationToken),
                        nameof(SA1507CodeFixProvider)),
                    diagnostic);
            }

            return SpecializedTasks.CompletedTask;
        }

        private static async Task<Document> GetTransformedDocumentAsync(Document document, Diagnostic diagnostic, CancellationToken token)
        {
            var sourceText = await document.GetTextAsync(token).ConfigureAwait(false);
            var syntaxRoot = await document.GetSyntaxRootAsync(token).ConfigureAwait(false);
            var syntaxToken = syntaxRoot.FindToken(diagnostic.Location.SourceSpan.Start);
            var newLine = FormattingHelper.GetEndOfLineForCodeFix(syntaxToken, sourceText, document.Project.Solution.Workspace.Options).ToFullString();
            var textChange = new TextChange(diagnostic.Location.SourceSpan, newLine);

            return document.WithText(sourceText.WithChanges(textChange));
        }

        private class FixAll : DocumentBasedFixAllProvider
        {
            public static FixAllProvider Instance { get; } =
                new FixAll();

            protected override string CodeActionTitle =>
                LayoutResources.SA1507CodeFix;

            protected override async Task<SyntaxNode?> FixAllInDocumentAsync(FixAllContext fixAllContext, Document document, ImmutableArray<Diagnostic> diagnostics)
            {
                if (diagnostics.IsEmpty)
                {
                    return null;
                }

                var text = await document.GetTextAsync(fixAllContext.CancellationToken).ConfigureAwait(false);
                var syntaxTree = await document.GetSyntaxTreeAsync(fixAllContext.CancellationToken).ConfigureAwait(false);
                var syntaxRoot = await syntaxTree.GetRootAsync(fixAllContext.CancellationToken).ConfigureAwait(false);
                var options = document.Project.Solution.Workspace.Options;

                List<TextChange> changes = new List<TextChange>();

                foreach (var diagnostic in diagnostics)
                {
                    var token = syntaxRoot.FindToken(diagnostic.Location.SourceSpan.Start);
                    var newLine = FormattingHelper.GetEndOfLineForCodeFix(token, text, options).ToFullString();
                    changes.Add(new TextChange(diagnostic.Location.SourceSpan, newLine));
                }

                changes.Sort((left, right) => left.Span.Start.CompareTo(right.Span.Start));

                return await syntaxTree.WithChangedText(text.WithChanges(changes)).GetRootAsync(fixAllContext.CancellationToken).ConfigureAwait(false);
            }
        }
    }
}
