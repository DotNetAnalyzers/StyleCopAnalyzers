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
            return WellKnownFixAllProviders.BatchFixer;
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

            // If there is trailing content on the line, we don't want to remove the leading whitespace
            bool hasTrailingContent = TriviaHasTrailingContentOnLine(root, triviaList);

            if (diagnosticIndex > 0 && !hasTrailingContent)
            {
                var previousStart = triviaList[diagnosticIndex - 1].SpanStart;
                var previousNode = root.FindTrivia(previousStart, true);
                nodesToRemove.Add(previousNode);
            }

            // If there is leading content on the line, then we don't want to remove the trailing end of lines
            bool hasLeadingContent = TriviaHasLeadingContentOnLine(root, triviaList);

            if (diagnosticIndex < triviaList.Count - 1)
            {
                var nextStart = triviaList[diagnosticIndex + 1].SpanStart;
                var nextNode = root.FindTrivia(nextStart, true);

                if (nextNode.IsKind(SyntaxKind.EndOfLineTrivia) && !hasLeadingContent)
                {
                    nodesToRemove.Add(nextNode);
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

        private static bool IsSkippableWhitespace(SyntaxTrivia trivia)
        {
            switch (trivia.Kind())
            {
            case SyntaxKind.EndOfLineTrivia:
            case SyntaxKind.WhitespaceTrivia:
                return true;

            default:
                return false;
            }
        }

        private static bool TriviaHasLeadingContentOnLine(SyntaxNode root, IReadOnlyList<SyntaxTrivia> triviaList)
        {
            var nodeBeforeStart = triviaList[0].SpanStart - 1;
            var nodeBefore = root.FindNode(new Microsoft.CodeAnalysis.Text.TextSpan(nodeBeforeStart, 1));

            if (nodeBefore.GetLineSpan().EndLinePosition.Line == triviaList[0].GetLineSpan().StartLinePosition.Line)
            {
                return true;
            }

            return false;
        }

        private static bool TriviaHasTrailingContentOnLine(SyntaxNode root, IReadOnlyList<SyntaxTrivia> triviaList)
        {
            var nodeAfterTriviaStart = triviaList[triviaList.Count - 1].SpanStart - 1;
            var nodeAfterTrivia = root.FindNode(new Microsoft.CodeAnalysis.Text.TextSpan(nodeAfterTriviaStart, 1));

            if (nodeAfterTrivia.GetLineSpan().StartLinePosition.Line == triviaList[triviaList.Count - 1].GetLineSpan().EndLinePosition.Line)
            {
                return true;
            }

            return false;
        }
    }
}
