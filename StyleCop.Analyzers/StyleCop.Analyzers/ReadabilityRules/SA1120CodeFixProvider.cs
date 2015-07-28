namespace StyleCop.Analyzers.ReadabilityRules
{
    using System.Composition;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CodeActions;
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
                context.RegisterCodeFix(CodeAction.Create(ReadabilityResources.SA1120CodeFix, token => GetTransformedDocumentAsync(context.Document, diagnostic, token)), diagnostic);
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

            // If the comment is the first non-whitespace item in the trivia list, we are going to remove all of the leading whitespace.
            var firstIndex = TriviaHelper.IndexOfFirstNonWhitespaceTrivia(triviaList);

            bool removedLeadingWhitespace = false;

            if (firstIndex == diagnosticIndex)
            {
                removedLeadingWhitespace = true;
                for (int i = diagnosticIndex; i >= 0; i--)
                {
                    var start = diagnostic.Location.SourceSpan.Start - (triviaList[diagnosticIndex].SpanStart - triviaList[i].SpanStart);
                    var prevNode = root.FindTrivia(start, true);
                    nodesToRemove.Add(prevNode);
                }
            }

            var lastIndex = TriviaHelper.IndexOfTrailingWhitespace(triviaList);

            // If the comment is the last non-whitespace item in the trivia list and it also wasn't the first non-whitespace item in the trivia list, then we remove the trailing whitespace.
            if (diagnosticIndex+node.Span.Length==lastIndex && !removedLeadingWhitespace)
            {
                for (int i = diagnosticIndex; i<triviaList.Count; i++)
                {
                    var start = diagnostic.Location.SourceSpan.Start + (triviaList[i].SpanStart- triviaList[diagnosticIndex].SpanStart);
                    var nextNodes = root.FindTrivia(start, true);
                    nodesToRemove.Add(nextNodes);
                }
            }

            var newRoot = root.ReplaceTrivia(nodesToRemove, (a, b) =>
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
    }
}
