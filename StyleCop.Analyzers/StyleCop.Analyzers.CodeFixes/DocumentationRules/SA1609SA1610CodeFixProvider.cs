// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.DocumentationRules
{
    using System;
    using System.Collections.Immutable;
    using System.Composition;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Formatting;
    using StyleCop.Analyzers.Helpers;

    /// <summary>
    /// Implements a code fix for <see cref="SA1609PropertyDocumentationMustHaveValue"/> and
    /// <see cref="SA1610PropertyDocumentationMustHaveValueText"/>.
    /// </summary>
    /// <remarks>
    /// <para>To fix a violation of this rule, fill-in a description of the value held by the property within the
    /// &lt;value&gt; tag.</para>
    /// </remarks>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1609SA1610CodeFixProvider))]
    [Shared]
    internal class SA1609SA1610CodeFixProvider : CodeFixProvider
    {
        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds { get; } =
            ImmutableArray.Create(SA1609PropertyDocumentationMustHaveValue.DiagnosticId, SA1610PropertyDocumentationMustHaveValueText.DiagnosticId);

        /// <inheritdoc/>
        public override FixAllProvider GetFixAllProvider()
        {
            return CustomFixAllProviders.BatchFixer;
        }

        /// <inheritdoc/>
        public override Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            foreach (var diagnostic in context.Diagnostics)
            {
                context.RegisterCodeFix(
                    CodeAction.Create(
                        DocumentationResources.SA1609SA1610CodeFix,
                        cancellationToken => this.GetTransformedDocumentAsync(context.Document, diagnostic, cancellationToken),
                        nameof(SA1609SA1610CodeFixProvider)),
                    diagnostic);
            }

            return SpecializedTasks.CompletedTask;
        }

        private static bool IsContentElement(XmlNodeSyntax syntax)
        {
            switch (syntax.Kind())
            {
            case SyntaxKind.XmlCDataSection:
            case SyntaxKind.XmlElement:
            case SyntaxKind.XmlEmptyElement:
            case SyntaxKind.XmlText:
                return true;

            default:
                return false;
            }
        }

        private static SyntaxTrivia GetLastDocumentationCommentExteriorTrivia(SyntaxNode node)
        {
            return node
                .DescendantTrivia(descendIntoTrivia: true)
                .LastOrDefault(trivia => trivia.IsKind(SyntaxKind.DocumentationCommentExteriorTrivia));
        }

        private async Task<Document> GetTransformedDocumentAsync(Document document, Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            var documentRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            SyntaxNode syntax = documentRoot.FindNode(diagnostic.Location.SourceSpan);
            if (syntax == null)
            {
                return document;
            }

            PropertyDeclarationSyntax propertyDeclarationSyntax = syntax.FirstAncestorOrSelf<PropertyDeclarationSyntax>();
            if (propertyDeclarationSyntax == null)
            {
                return document;
            }

            DocumentationCommentTriviaSyntax documentationComment = propertyDeclarationSyntax.GetDocumentationCommentTriviaSyntax();
            if (documentationComment == null)
            {
                return document;
            }

            XmlElementSyntax summaryElement = documentationComment.Content.GetFirstXmlElement(XmlCommentHelper.SummaryXmlTag) as XmlElementSyntax;
            if (summaryElement == null)
            {
                return document;
            }

            SyntaxList<XmlNodeSyntax> summaryContent = summaryElement.Content;
            if (!this.TryRemoveSummaryPrefix(ref summaryContent, "Gets or sets "))
            {
                if (!this.TryRemoveSummaryPrefix(ref summaryContent, "Gets "))
                {
                    this.TryRemoveSummaryPrefix(ref summaryContent, "Sets ");
                }
            }

            SyntaxList<XmlNodeSyntax> content = summaryContent.WithoutFirstAndLastNewlines();
            if (!string.IsNullOrWhiteSpace(content.ToFullString()))
            {
                // wrap the content in a <placeholder> element for review
                content = XmlSyntaxFactory.List(XmlSyntaxFactory.PlaceholderElement(content));
            }

            string newLineText = document.Project.Solution.Workspace.Options.GetOption(FormattingOptions.NewLine, LanguageNames.CSharp);

            XmlElementSyntax valueElement = XmlSyntaxFactory.MultiLineElement(XmlCommentHelper.ValueXmlTag, newLineText, content);

            XmlNodeSyntax leadingNewLine = XmlSyntaxFactory.NewLine(newLineText);

            // HACK: The formatter isn't working when contents are added to an existing documentation comment, so we
            // manually apply the indentation from the last line of the existing comment to each new line of the
            // generated content.
            SyntaxTrivia exteriorTrivia = GetLastDocumentationCommentExteriorTrivia(documentationComment);
            if (!exteriorTrivia.Token.IsMissing)
            {
                leadingNewLine = leadingNewLine.ReplaceExteriorTrivia(exteriorTrivia);
                valueElement = valueElement.ReplaceExteriorTrivia(exteriorTrivia);
            }

            // Try to replace an existing <value> element if the comment contains one. Otherwise, add it as a new element.
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            SyntaxNode newRoot;
            XmlNodeSyntax existingValue = documentationComment.Content.GetFirstXmlElement(XmlCommentHelper.ValueXmlTag);
            if (existingValue != null)
            {
                newRoot = root.ReplaceNode(existingValue, valueElement);
            }
            else
            {
                DocumentationCommentTriviaSyntax newDocumentationComment = documentationComment.WithContent(
                    documentationComment.Content.InsertRange(
                        documentationComment.Content.Count - 1,
                        XmlSyntaxFactory.List(leadingNewLine, valueElement)));

                newRoot = root.ReplaceNode(documentationComment, newDocumentationComment);
            }

            return document.WithSyntaxRoot(newRoot);
        }

        private bool TryRemoveSummaryPrefix(ref SyntaxList<XmlNodeSyntax> summaryContent, string prefix)
        {
            XmlNodeSyntax firstContent = summaryContent.FirstOrDefault(IsContentElement);
            XmlTextSyntax firstText = firstContent as XmlTextSyntax;
            if (firstText == null)
            {
                return false;
            }

            string firstTextContent = string.Concat(firstText.DescendantTokens());
            if (!firstTextContent.TrimStart().StartsWith(prefix, StringComparison.Ordinal))
            {
                return false;
            }

            // Find the token containing the prefix, such as "Gets or sets "
            SyntaxToken prefixToken = default(SyntaxToken);
            foreach (SyntaxToken textToken in firstText.TextTokens)
            {
                if (textToken.IsMissing)
                {
                    continue;
                }

                if (!textToken.Text.TrimStart().StartsWith(prefix, StringComparison.Ordinal))
                {
                    continue;
                }

                prefixToken = textToken;
                break;
            }

            if (prefixToken.IsMissingOrDefault())
            {
                return false;
            }

            string text = prefixToken.Text;
            string valueText = prefixToken.ValueText;
            int index = text.IndexOf(prefix);
            if (index >= 0)
            {
                bool additionalCharacters = index + prefix.Length < text.Length;
                text = text.Substring(0, index)
                    + (additionalCharacters ? char.ToUpperInvariant(text[index + prefix.Length]).ToString() : string.Empty)
                    + text.Substring(index + (additionalCharacters ? (prefix.Length + 1) : prefix.Length));
            }

            index = valueText.IndexOf(prefix);
            if (index >= 0)
            {
                valueText = valueText.Remove(index, prefix.Length);
            }

            SyntaxToken replaced = SyntaxFactory.Token(prefixToken.LeadingTrivia, prefixToken.Kind(), text, valueText, prefixToken.TrailingTrivia);
            summaryContent = summaryContent.Replace(firstText, firstText.ReplaceToken(prefixToken, replaced));
            return true;
        }
    }
}
