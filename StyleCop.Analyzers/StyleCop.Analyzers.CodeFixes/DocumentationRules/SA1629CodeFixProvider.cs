// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.DocumentationRules
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

    /// <summary>
    /// Implements a code fix for <see cref="SA1629DocumentationTextMustEndWithAPeriod"/>.
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1629CodeFixProvider))]
    [Shared]
    internal class SA1629CodeFixProvider : CodeFixProvider
    {
        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds { get; }
            = ImmutableArray.Create(SA1629DocumentationTextMustEndWithAPeriod.DiagnosticId);

        /// <inheritdoc/>
        public override FixAllProvider GetFixAllProvider()
        {
            return CustomFixAllProviders.BatchFixer;
        }

        /// <inheritdoc/>
        public override Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            foreach (Diagnostic diagnostic in context.Diagnostics)
            {
                if (!diagnostic.Properties.ContainsKey(SA1629DocumentationTextMustEndWithAPeriod.NoCodeFixKey))
                {
                    context.RegisterCodeFix(
                    CodeAction.Create(
                        DocumentationResources.SA1629CodeFix,
                        cancellationToken => GetTransformedDocumentAsync(context.Document, diagnostic, cancellationToken),
                        nameof(SA1629CodeFixProvider)),
                    diagnostic);
                }
            }

            return SpecializedTasks.CompletedTask;
        }

        private static async Task<Document> GetTransformedDocumentAsync(Document document, Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            var text = await document.GetTextAsync(cancellationToken).ConfigureAwait(false);
            bool replaceChar = diagnostic.Properties.ContainsKey(SA1629DocumentationTextMustEndWithAPeriod.ReplaceCharKey);
            var newText = text.WithChanges(new TextChange(new TextSpan(diagnostic.Location.SourceSpan.Start, replaceChar ? 1 : 0), "."));

            return document.WithText(newText);
        }
    }
}
