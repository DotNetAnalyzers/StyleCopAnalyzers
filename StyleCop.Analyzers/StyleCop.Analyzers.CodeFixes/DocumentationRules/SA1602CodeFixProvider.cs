// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.DocumentationRules
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Composition;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Formatting;
    using StyleCop.Analyzers.Helpers;

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1602CodeFixProvider))]
    [Shared]
    internal class SA1602CodeFixProvider : CodeFixProvider
    {
        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds { get; } =
            ImmutableArray.Create(
                SA1602EnumerationItemsMustBeDocumented.DiagnosticId);

        /// <inheritdoc/>
        public override FixAllProvider GetFixAllProvider()
        {
            return CustomFixAllProviders.BatchFixer;
        }

        /// <inheritdoc/>
        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var document = context.Document;
            SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            foreach (var diagnostic in context.Diagnostics)
            {
                SyntaxToken identifier = root.FindToken(diagnostic.Location.SourceSpan.Start);
                if (identifier.IsMissingOrDefault())
                {
                    continue;
                }

                EnumMemberDeclarationSyntax declaration = identifier.Parent.FirstAncestorOrSelf<EnumMemberDeclarationSyntax>();

                context.RegisterCodeFix(
                    CodeAction.Create(
                        DocumentationResources.EnumMemberDocumentationCodeFix,
                        cancellationToken => GetEnumDocumentationTransformedDocumentAsync(
                            context.Document,
                            root,
                            declaration,
                            identifier,
                            cancellationToken),
                        nameof(SA1602CodeFixProvider)),
                    diagnostic);
            }
        }

        private static Task<Document> GetEnumDocumentationTransformedDocumentAsync(Document document, SyntaxNode root, EnumMemberDeclarationSyntax declaration, SyntaxToken identifier, CancellationToken cancellationToken)
        {
            string newLineText = GetNewLineText(document);
            var commentTrivia = declaration.GetDocumentationCommentTriviaSyntax();

            var summaryNode = CommonDocumentationHelper.CreateDefaultSummaryNode(identifier.ValueText, newLineText);
            if (commentTrivia != null)
            {
                return ReplaceExistingSummaryAsync(document, root, newLineText, commentTrivia, summaryNode);
            }

            commentTrivia = XmlSyntaxFactory.DocumentationComment(newLineText, summaryNode);
            return Task.FromResult(CreateCommentAndReplaceInDocument(document, root, declaration, commentTrivia));
        }

        private static Task<Document> ReplaceExistingSummaryAsync(Document document, SyntaxNode root, string newLineText, DocumentationCommentTriviaSyntax commentTrivia, XmlNodeSyntax summaryNode)
        {
            // HACK: The formatter isn't working when contents are added to an existing documentation comment, so we
            // manually apply the indentation from the last line of the existing comment to each new line of the
            // generated content.
            SyntaxTrivia exteriorTrivia = CommonDocumentationHelper.GetLastDocumentationCommentExteriorTrivia(commentTrivia);
            if (!exteriorTrivia.Token.IsMissing)
            {
                summaryNode = summaryNode.ReplaceExteriorTrivia(exteriorTrivia);
            }

            var originalSummeryNode = commentTrivia.Content.GetFirstXmlElement(XmlCommentHelper.SummaryXmlTag);
            return Task.FromResult(document.WithSyntaxRoot(root.ReplaceNode(originalSummeryNode, summaryNode)));
        }

        private static Document CreateCommentAndReplaceInDocument(
            Document document,
            SyntaxNode root,
            SyntaxNode declarationNode,
            DocumentationCommentTriviaSyntax documentationComment)
        {
            var leadingTrivia = declarationNode.GetLeadingTrivia();
            int insertionIndex = GetInsertionIndex(ref leadingTrivia);

            var trivia = SyntaxFactory.Trivia(documentationComment);

            SyntaxTriviaList newLeadingTrivia = leadingTrivia.Insert(insertionIndex, trivia);
            SyntaxNode newElement = declarationNode.WithLeadingTrivia(newLeadingTrivia);
            return document.WithSyntaxRoot(root.ReplaceNode(declarationNode, newElement));
        }

        private static int GetInsertionIndex(ref SyntaxTriviaList leadingTrivia)
        {
            int insertionIndex = leadingTrivia.Count;
            while (insertionIndex > 0 && !leadingTrivia[insertionIndex - 1].HasBuiltinEndLine())
            {
                insertionIndex--;
            }

            return insertionIndex;
        }

        private static string GetNewLineText(Document document)
        {
            return document.Project.Solution.Workspace.Options.GetOption(FormattingOptions.NewLine, LanguageNames.CSharp);
        }
    }
}
