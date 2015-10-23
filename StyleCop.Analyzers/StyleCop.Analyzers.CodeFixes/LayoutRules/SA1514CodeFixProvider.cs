// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.LayoutRules
{
    using System.Collections.Immutable;
    using System.Composition;
    using System.Threading;
    using System.Threading.Tasks;
    using Helpers;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;

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
            return CustomFixAllProviders.BatchFixer;
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
            var syntaxRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            var documentationHeader = syntaxRoot.FindTrivia(diagnostic.Location.SourceSpan.Start);
            var triviaList = documentationHeader.Token.LeadingTrivia;
            var documentationHeaderIndex = triviaList.IndexOf(documentationHeader);

            // Keep any leading whitespace with the documentation header
            var index = documentationHeaderIndex - 1;
            while ((index >= 0) && triviaList[index].IsKind(SyntaxKind.WhitespaceTrivia))
            {
                index--;
            }

            var newLeadingTrivia = documentationHeader.Token.LeadingTrivia.Insert(index + 1, SyntaxFactory.CarriageReturnLineFeed);
            var newSyntaxRoot = syntaxRoot.ReplaceToken(documentationHeader.Token, documentationHeader.Token.WithLeadingTrivia(newLeadingTrivia));
            return document.WithSyntaxRoot(newSyntaxRoot);
        }
    }
}
