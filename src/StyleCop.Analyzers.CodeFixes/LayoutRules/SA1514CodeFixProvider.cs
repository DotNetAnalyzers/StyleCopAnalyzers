// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.LayoutRules
{
    using System.Collections.Immutable;
    using System.Composition;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using StyleCop.Analyzers.Helpers;

    /// <summary>
    /// Implements a code fix for <see cref="SA1514ElementDocumentationHeaderMustBePrecededByBlankLine"/>.
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1514CodeFixProvider))]
    [Shared]
    internal class SA1514CodeFixProvider : CodeFixProvider
    {
        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds { get; } =
            ImmutableArray.Create(SA1514ElementDocumentationHeaderMustBePrecededByBlankLine.DiagnosticId);

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
                        LayoutResources.SA1514CodeFix,
                        cancellationToken => GetTransformedDocumentAsync(context.Document, diagnostic, cancellationToken),
                        nameof(SA1514CodeFixProvider)),
                    diagnostic);
            }

            return SpecializedTasks.CompletedTask;
        }

        private static async Task<Document> GetTransformedDocumentAsync(Document document, Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            var newRoot = await GetTransformedDocumentAsync(document, ImmutableArray.Create(diagnostic), cancellationToken).ConfigureAwait(false);
            return document.WithSyntaxRoot(newRoot);
        }

        private static async Task<SyntaxNode> GetTransformedDocumentAsync(Document document, ImmutableArray<Diagnostic> diagnostics, CancellationToken cancellationToken)
        {
            var syntaxRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            var documentationHeaders = diagnostics.Select(diagnostic => syntaxRoot.FindTrivia(diagnostic.Location.SourceSpan.Start)).ToArray();
            return syntaxRoot.ReplaceTokens(
                documentationHeaders.Select(header => header.Token),
                (originalToken, rewrittenToken) =>
                {
                    var triviaList = rewrittenToken.LeadingTrivia;
                    var documentationHeaderIndex = originalToken.LeadingTrivia.IndexOf(originalToken.LeadingTrivia.First(documentationHeaders.Contains));

                    // Keep any leading whitespace with the documentation header
                    var index = documentationHeaderIndex - 1;
                    while ((index >= 0) && triviaList[index].IsKind(SyntaxKind.WhitespaceTrivia))
                    {
                        index--;
                    }

                    var newLeadingTrivia = rewrittenToken.LeadingTrivia.Insert(index + 1, SyntaxFactory.CarriageReturnLineFeed);
                    return rewrittenToken.WithLeadingTrivia(newLeadingTrivia);
                });
        }

        private class FixAll : DocumentBasedFixAllProvider
        {
            public static FixAllProvider Instance { get; } =
                new FixAll();

            protected override string CodeActionTitle => LayoutResources.SA1514CodeFix;

            protected override async Task<SyntaxNode> FixAllInDocumentAsync(FixAllContext fixAllContext, Document document, ImmutableArray<Diagnostic> diagnostics)
                => await GetTransformedDocumentAsync(document, diagnostics, fixAllContext.CancellationToken).ConfigureAwait(false);
        }
    }
}
