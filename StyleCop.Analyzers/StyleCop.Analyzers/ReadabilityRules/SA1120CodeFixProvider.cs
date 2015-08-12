namespace StyleCop.Analyzers.ReadabilityRules
{
    using System.Collections.Generic;
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
    /// Implements a code fix for <see cref="SA1120CommentsMustContainText"/>.
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1120CodeFixProvider))]
    [Shared]
    public class SA1120CodeFixProvider : CodeFixProvider
    {
        private static readonly ImmutableArray<string> FixableDiagnostics =
            ImmutableArray.Create(SA1120CommentsMustContainText.DiagnosticId);

        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds => FixableDiagnostics;

        /// <inheritdoc/>
        public override FixAllProvider GetFixAllProvider()
        {
            return CustomFixAllProviders.BatchFixer;
        }

        /// <inheritdoc/>
        public override Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            foreach (var diagnostic in context.Diagnostics.Where(d => FixableDiagnostics.Contains(d.Id)))
            {
                context.RegisterCodeFix(CodeAction.Create(ReadabilityResources.SA1120CodeFix, token => GetTransformedDocumentAsync(context.Document, diagnostic, token), nameof(SA1120CodeFixProvider)), diagnostic);
            }

            return SpecializedTasks.CompletedTask;
        }

        private static async Task<Document> GetTransformedDocumentAsync(Document document, Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            var node = root.FindTrivia(diagnostic.Location.SourceSpan.Start, true);

            int diagnosticIndex = 0;
            var triviaList = TriviaHelper.GetContainingTriviaList(node, out diagnosticIndex);

            var nodesToRemove = new List<SyntaxTrivia>();
            nodesToRemove.Add(node);

            bool hasTrailingContent = TriviaHasTrailingContentOnLine(root, node);
            if (!hasTrailingContent && diagnosticIndex > 0)
            {
                var previousTrivia = triviaList[diagnosticIndex - 1];
                if (previousTrivia.IsKind(SyntaxKind.WhitespaceTrivia))
                {
                    nodesToRemove.Add(previousTrivia);
                }
            }

            bool hasLeadingContent = TriviaHasLeadingContentOnLine(root, node);
            if (!hasLeadingContent && diagnosticIndex < triviaList.Count - 1)
            {
                var nextTrivia = triviaList[diagnosticIndex + 1];
                if (nextTrivia.IsKind(SyntaxKind.EndOfLineTrivia))
                {
                    nodesToRemove.Add(nextTrivia);
                }
            }

            // Replace all roots with an empty node
            var newRoot = root.ReplaceTrivia(nodesToRemove, (original, rewritten) =>
            {
                return new SyntaxTrivia();
            });

            Document updatedDocument = document.WithSyntaxRoot(newRoot);
            return updatedDocument;
        }

        private static bool TriviaHasLeadingContentOnLine(SyntaxNode root, SyntaxTrivia commentTrivia)
        {
            var nodeBeforeStart = commentTrivia.SpanStart - 1;
            var nodeBefore = root.FindNode(new Microsoft.CodeAnalysis.Text.TextSpan(nodeBeforeStart, 1));

            if (nodeBefore.GetLineSpan().EndLinePosition.Line == commentTrivia.GetLineSpan().StartLinePosition.Line && !nodeBefore.GetLeadingTrivia().Contains(commentTrivia))
            {
                return true;
            }

            return false;
        }

        private static bool TriviaHasTrailingContentOnLine(SyntaxNode root, SyntaxTrivia commentTrivia)
        {
            var nodeAfterTriviaStart = commentTrivia.Span.End + 1;
            var nodeAfterTrivia = root.FindNode(new Microsoft.CodeAnalysis.Text.TextSpan(nodeAfterTriviaStart, 1));

            if (nodeAfterTrivia.GetLineSpan().StartLinePosition.Line == commentTrivia.GetLineSpan().EndLinePosition.Line && !nodeAfterTrivia.GetTrailingTrivia().Contains(commentTrivia))
            {
                return true;
            }

            return false;
        }
    }
}
