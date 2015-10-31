// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.ReadabilityRules
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Composition;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Helpers;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;

    /// <summary>
    /// Implements a code fix for <see cref="SA1120CommentsMustContainText"/>.
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1120CodeFixProvider))]
    [Shared]
    internal class SA1120CodeFixProvider : CodeFixProvider
    {
        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds { get; } =
            ImmutableArray.Create(SA1120CommentsMustContainText.DiagnosticId);

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
                        ReadabilityResources.SA1120CodeFix,
                        cancellationToken => GetTransformedDocumentAsync(context.Document, diagnostic, cancellationToken),
                        nameof(SA1120CodeFixProvider)),
                    diagnostic);
            }

            return SpecializedTasks.CompletedTask;
        }

        private static async Task<Document> GetTransformedDocumentAsync(Document document, Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            var trivia = root.FindTrivia(diagnostic.Location.SourceSpan.Start, true);

            int diagnosticIndex = 0;
            var triviaList = TriviaHelper.GetContainingTriviaList(trivia, out diagnosticIndex);

            var triviaToRemove = new List<SyntaxTrivia>();
            triviaToRemove.Add(trivia);

            bool hasTrailingContent = TriviaHasTrailingContentOnLine(root, trivia);
            if (!hasTrailingContent && diagnosticIndex > 0)
            {
                var previousTrivia = triviaList[diagnosticIndex - 1];
                if (previousTrivia.IsKind(SyntaxKind.WhitespaceTrivia))
                {
                    triviaToRemove.Add(previousTrivia);
                }
            }

            bool hasLeadingContent = TriviaHasLeadingContentOnLine(root, trivia);
            if (!hasLeadingContent && diagnosticIndex < triviaList.Count - 1)
            {
                var nextTrivia = triviaList[diagnosticIndex + 1];
                if (nextTrivia.IsKind(SyntaxKind.EndOfLineTrivia))
                {
                    triviaToRemove.Add(nextTrivia);
                }
            }

            // Replace all roots with an empty node
            var newRoot = root.ReplaceTrivia(triviaToRemove, (original, rewritten) => default(SyntaxTrivia));

            Document updatedDocument = document.WithSyntaxRoot(newRoot);
            return updatedDocument;
        }

        private static bool TriviaHasLeadingContentOnLine(SyntaxNode root, SyntaxTrivia commentTrivia)
        {
            if (commentTrivia.SpanStart == 0)
            {
                // It is impossible to have leading content at the start of the file.
                return false;
            }

            var nodeBeforeStart = commentTrivia.SpanStart - 1;
            var nodeBefore = root.FindNode(new Microsoft.CodeAnalysis.Text.TextSpan(nodeBeforeStart, 1));

            return nodeBefore.GetEndLine() == commentTrivia.GetLine() && !nodeBefore.GetLeadingTrivia().Contains(commentTrivia);
        }

        private static bool TriviaHasTrailingContentOnLine(SyntaxNode root, SyntaxTrivia commentTrivia)
        {
            if (commentTrivia.Span.End == root.Span.End)
            {
                // It is impossible to have trailing content at the end of the file.
                return false;
            }

            var nodeAfterTriviaStart = commentTrivia.Span.End + 1;
            var nodeAfterTrivia = root.FindNode(new Microsoft.CodeAnalysis.Text.TextSpan(nodeAfterTriviaStart, 1));

            return nodeAfterTrivia.GetLine() == commentTrivia.GetEndLine() && !nodeAfterTrivia.GetTrailingTrivia().Contains(commentTrivia);
        }
    }
}
