// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.DocumentationRules
{
    using System.Collections.Immutable;
    using System.Composition;
    using System.Threading;
    using System.Threading.Tasks;
    using Helpers;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// Implements a code fix for <see cref="SA1651DoNotUsePlaceholderElements"/>.
    /// </summary>
    /// <remarks>
    /// <para>To fix a violation of this rule, review the placeholder documentation for accuracy and remove the
    /// &lt;placeholder&gt; tags.</para>
    /// </remarks>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1651CodeFixProvider))]
    [Shared]
    internal class SA1651CodeFixProvider : CodeFixProvider
    {
        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds { get; } =
            ImmutableArray.Create(SA1651DoNotUsePlaceholderElements.DiagnosticId);

        /// <inheritdoc/>
        public override FixAllProvider GetFixAllProvider()
        {
            return CustomFixAllProviders.BatchFixer;
        }

        /// <inheritdoc/>
        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            foreach (var diagnostic in context.Diagnostics)
            {
                var documentRoot = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
                SyntaxNode syntax = documentRoot.FindNode(diagnostic.Location.SourceSpan, findInsideTrivia: true, getInnermostNodeForTie: true);
                if (syntax == null)
                {
                    continue;
                }

                XmlElementSyntax xmlElementSyntax = syntax as XmlElementSyntax;
                if (xmlElementSyntax == null)
                {
                    // We continue even for placeholders if they are empty elements (XmlEmptyElementSyntax)
                    continue;
                }

                if (string.IsNullOrWhiteSpace(xmlElementSyntax.Content.ToString()))
                {
                    // The placeholder hasn't been updated yet.
                    continue;
                }

                context.RegisterCodeFix(
                    CodeAction.Create(
                        DocumentationResources.SA1651CodeFix,
                        cancellationToken => this.GetTransformedDocumentAsync(context.Document, xmlElementSyntax, cancellationToken),
                        nameof(SA1651CodeFixProvider)),
                    diagnostic);
            }
        }

        private async Task<Document> GetTransformedDocumentAsync(Document document, XmlElementSyntax elementSyntax, CancellationToken cancellationToken)
        {
            SyntaxList<XmlNodeSyntax> content = elementSyntax.Content;
            if (content.Count == 0)
            {
                return document;
            }

            var leadingTrivia = elementSyntax.StartTag.GetLeadingTrivia();
            leadingTrivia = leadingTrivia.AddRange(elementSyntax.StartTag.GetTrailingTrivia());
            leadingTrivia = leadingTrivia.AddRange(content[0].GetLeadingTrivia());
            content = content.Replace(content[0], content[0].WithLeadingTrivia(leadingTrivia));

            var trailingTrivia = content[content.Count - 1].GetTrailingTrivia();
            trailingTrivia = trailingTrivia.AddRange(elementSyntax.EndTag.GetLeadingTrivia());
            trailingTrivia = trailingTrivia.AddRange(elementSyntax.EndTag.GetTrailingTrivia());
            content = content.Replace(content[content.Count - 1], content[content.Count - 1].WithTrailingTrivia(trailingTrivia));

            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            SyntaxNode newRoot = root.ReplaceNode(elementSyntax, content);
            return document.WithSyntaxRoot(newRoot);
        }
    }
}
