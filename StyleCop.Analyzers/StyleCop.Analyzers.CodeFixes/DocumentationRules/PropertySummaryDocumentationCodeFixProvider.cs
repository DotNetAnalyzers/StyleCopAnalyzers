// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.DocumentationRules
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
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using StyleCop.Analyzers.Helpers;
    using StyleCop.Analyzers.Helpers.ObjectPools;

    /// <summary>
    /// Implements the code fix for property summary documentation.
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(PropertySummaryDocumentationCodeFixProvider))]
    [Shared]
    public class PropertySummaryDocumentationCodeFixProvider : CodeFixProvider
    {
        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds { get; } =
            ImmutableArray.Create(
                PropertySummaryDocumentationAnalyzer.SA1623Descriptor.Id,
                PropertySummaryDocumentationAnalyzer.SA1624Descriptor.Id);

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
                if (!diagnostic.Properties.ContainsKey(PropertySummaryDocumentationAnalyzer.NoCodeFixKey))
                {
                    context.RegisterCodeFix(
                        CodeAction.Create(
                            DocumentationResources.PropertySummaryStartTextCodeFix,
                            cancellationToken => GetTransformedDocumentAsync(context.Document, diagnostic, cancellationToken),
                            nameof(PropertySummaryDocumentationCodeFixProvider)),
                        diagnostic);
                }
            }

            return SpecializedTasks.CompletedTask;
        }

        private static async Task<Document> GetTransformedDocumentAsync(Document document, Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            var syntaxRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            var node = syntaxRoot.FindNode(diagnostic.Location.SourceSpan);
            var documentation = node.GetDocumentationCommentTriviaSyntax();

            var summaryElement = (XmlElementSyntax)documentation.Content.GetFirstXmlElement(XmlCommentHelper.SummaryXmlTag);
            var textElement = XmlCommentHelper.TryGetFirstTextElementWithContent(summaryElement);
            if (textElement == null)
            {
                return document;
            }

            var textToken = textElement.TextTokens.First(token => token.IsKind(SyntaxKind.XmlTextLiteralToken));
            var text = textToken.ValueText;

            // preserve leading whitespace
            int index = 0;
            while (text.Length > index && char.IsWhiteSpace(text, index))
            {
                index++;
            }

            var preservedWhitespace = text.Substring(0, index);

            // process the current documentation string
            string modifiedText;
            string textToRemove;
            if (diagnostic.Properties.TryGetValue(PropertySummaryDocumentationAnalyzer.TextToRemoveKey, out textToRemove))
            {
                modifiedText = text.Substring(text.IndexOf(textToRemove) + textToRemove.Length).TrimStart();
            }
            else
            {
                modifiedText = text.Substring(index);
            }

            if (modifiedText.Length > 0)
            {
                modifiedText = char.ToLowerInvariant(modifiedText[0]) + modifiedText.Substring(1);
            }

            // create the new text string
            var textToAdd = diagnostic.Properties[PropertySummaryDocumentationAnalyzer.ExpectedTextKey];
            var newText = $"{preservedWhitespace}{textToAdd} {modifiedText}";

            // replace the token
            var newXmlTextLiteral = SyntaxFactory.XmlTextLiteral(textToken.LeadingTrivia, newText, newText, textToken.TrailingTrivia);
            var newTextTokens = textElement.TextTokens.Replace(textToken, newXmlTextLiteral);
            var newTextElement = textElement.WithTextTokens(newTextTokens);

            var newSyntaxRoot = syntaxRoot.ReplaceNode(textElement, newTextElement);
            var newDocument = document.WithSyntaxRoot(newSyntaxRoot);

            return newDocument;
        }
    }
}
