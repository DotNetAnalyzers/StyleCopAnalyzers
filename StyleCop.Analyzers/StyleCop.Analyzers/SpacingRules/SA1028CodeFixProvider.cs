namespace StyleCop.Analyzers.SpacingRules
{
    using System.Collections.Immutable;
    using System.Composition;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Text;

    /// <summary>
    /// Implements a code fix for <see cref="SA1028NoTrailingWhitespace"/>.
    /// </summary>
    /// <remarks>
    /// <para>To fix a violation of this rule, remove any whitespace at the end of a line of code.</para>
    /// </remarks>
    [ExportCodeFixProvider("WhitespaceDiagnosticCodeFixProvider", LanguageNames.CSharp), Shared]
    public class SA1028CodeFixProvider : CodeFixProvider
    {
        private static readonly ImmutableArray<string> FixableDiagnostics =
            ImmutableArray.Create(SA1028NoTrailingWhitespace.DiagnosticId);

        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds => FixableDiagnostics;

        /// <inheritdoc/>
        public sealed override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        /// <inheritdoc/>
        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken);
            foreach (var diagnostic in context.Diagnostics)
            {
                var diagnosticSpan = diagnostic.Location.SourceSpan;
                var trivia = root.FindTrivia(diagnosticSpan.Start, findInsideTrivia: true);
                switch (trivia.Kind())
                {
                case SyntaxKind.WhitespaceTrivia:
                case SyntaxKind.SingleLineCommentTrivia:
                case SyntaxKind.MultiLineCommentTrivia:
                case SyntaxKind.MultiLineDocumentationCommentTrivia:
                    context.RegisterCodeFix(
                        CodeAction.Create(
                            "Remove trailing whitespace",
                            ct => RemoveWhitespaceAsync(context.Document, diagnostic, ct)),
                        diagnostic);
                    break;
                }
            }
        }

        /// <summary>
        /// Removes trailing whitespace.
        /// </summary>
        /// <param name="document">The document to be changed.</param>
        /// <param name="diagnostic">The diagnostic to fix.</param>
        /// <param name="cancellationToken">The cancellation token associated with the fix action.</param>
        /// <returns>The transformed document.</returns>
        private static async Task<Document> RemoveWhitespaceAsync(Document document, Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken);
            var diagnosticSpan = diagnostic.Location.SourceSpan;
            var trivia = root.FindTrivia(diagnosticSpan.Start, findInsideTrivia: true);
            SyntaxNode newRoot;
            switch (trivia.Kind())
            {
            case SyntaxKind.WhitespaceTrivia:
                newRoot = root.ReplaceTrivia(trivia, SyntaxTriviaList.Empty);
                break;
            case SyntaxKind.SingleLineCommentTrivia:
                string newTriviaContent = trivia.ToFullString().Substring(0, trivia.FullSpan.Length - diagnosticSpan.Length);
                var newTrivia = SyntaxFactory.SyntaxTrivia(SyntaxKind.SingleLineCommentTrivia, newTriviaContent);
                newRoot = root.ReplaceTrivia(trivia, newTrivia);
                break;
            case SyntaxKind.MultiLineCommentTrivia:
            case SyntaxKind.MultiLineDocumentationCommentTrivia:
                var triviaLocation = trivia.GetLocation();
                string oldTriviaContent = trivia.ToFullString();
                TextSpan diagnosticSpanWithinTrivia = TextSpan.FromBounds(diagnosticSpan.Start - triviaLocation.SourceSpan.Start, diagnosticSpan.End - triviaLocation.SourceSpan.Start);
                newTriviaContent = string.Concat(
                    oldTriviaContent.Substring(0, diagnosticSpanWithinTrivia.Start),
                    oldTriviaContent.Substring(diagnosticSpanWithinTrivia.End));
                newTrivia = SyntaxFactory.SyntaxTrivia(trivia.Kind(), newTriviaContent);
                newRoot = root.ReplaceTrivia(trivia, newTrivia);
                break;
            default:
                return document;
            }

            var newDocument = document.WithSyntaxRoot(newRoot);
            return newDocument;
        }
    }
}
