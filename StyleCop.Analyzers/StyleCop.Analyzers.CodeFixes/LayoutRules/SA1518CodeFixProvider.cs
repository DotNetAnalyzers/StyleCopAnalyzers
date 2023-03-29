// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.LayoutRules
{
    using System.Collections.Immutable;
    using System.Composition;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Text;
    using StyleCop.Analyzers.Helpers;
    using StyleCop.Analyzers.Settings.ObjectModel;

    /// <summary>
    /// Implements a code fix for <see cref="SA1518UseLineEndingsCorrectlyAtEndOfFile"/>.
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1518CodeFixProvider))]
    [Shared]
    internal class SA1518CodeFixProvider : CodeFixProvider
    {
        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds { get; } =
            ImmutableArray.Create(SA1518UseLineEndingsCorrectlyAtEndOfFile.DiagnosticId);

        /// <inheritdoc/>
        public override FixAllProvider GetFixAllProvider()
        {
            return FixAll.Instance;
        }

        /// <inheritdoc/>
        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var syntaxTree = await context.Document.GetSyntaxTreeAsync(context.CancellationToken).ConfigureAwait(false);
            var settings = SettingsHelper.GetStyleCopSettings(context.Document.Project.AnalyzerOptions, syntaxTree, context.CancellationToken);
            foreach (var diagnostic in context.Diagnostics)
            {
                context.RegisterCodeFix(
                    CodeAction.Create(
                        LayoutResources.SA1518CodeFix,
                        cancellationToken => FixEndOfFileAsync(context.Document, diagnostic, settings.LayoutRules.NewlineAtEndOfFile, cancellationToken),
                        nameof(SA1518CodeFixProvider)),
                    diagnostic);
            }
        }

        /// <summary>
        /// Fixes the whitespace at the end of a document.
        /// </summary>
        /// <param name="document">The document to be changed.</param>
        /// <param name="diagnostic">The diagnostic to fix.</param>
        /// <param name="newlineAtEndOfFile">A <see cref="OptionSetting"/> value indicating the desired behavior.</param>
        /// <param name="cancellationToken">The cancellation token associated with the fix action.</param>
        /// <returns>The transformed document.</returns>
        private static async Task<Document> FixEndOfFileAsync(Document document, Diagnostic diagnostic, OptionSetting newlineAtEndOfFile, CancellationToken cancellationToken)
        {
            var text = await document.GetTextAsync(cancellationToken).ConfigureAwait(false);
            string replacement = newlineAtEndOfFile == OptionSetting.Omit ? string.Empty : "\r\n";
            return document.WithText(text.WithChanges(new TextChange(diagnostic.Location.SourceSpan, replacement)));
        }

        private class FixAll : DocumentBasedFixAllProvider
        {
            public static FixAllProvider Instance { get; } =
                new FixAll();

            protected override string CodeActionTitle =>
                LayoutResources.SA1518CodeFix;

            protected override async Task<SyntaxNode> FixAllInDocumentAsync(FixAllContext fixAllContext, Document document, ImmutableArray<Diagnostic> diagnostics)
            {
                if (diagnostics.IsEmpty)
                {
                    return null;
                }

                var syntaxTree = await document.GetSyntaxTreeAsync(fixAllContext.CancellationToken).ConfigureAwait(false);
                var settings = SettingsHelper.GetStyleCopSettings(document.Project.AnalyzerOptions, syntaxTree, fixAllContext.CancellationToken);
                Document updatedDocument = await FixEndOfFileAsync(document, diagnostics[0], settings.LayoutRules.NewlineAtEndOfFile, fixAllContext.CancellationToken).ConfigureAwait(false);
                return await updatedDocument.GetSyntaxRootAsync(fixAllContext.CancellationToken).ConfigureAwait(false);
            }
        }
    }
}
