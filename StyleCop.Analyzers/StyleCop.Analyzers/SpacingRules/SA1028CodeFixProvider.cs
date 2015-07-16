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
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Text;

    /// <summary>
    /// Implements a code fix for <see cref="SA1028CodeMustNotContainTrailingWhitespace"/>.
    /// </summary>
    /// <remarks>
    /// <para>To fix a violation of this rule, remove any whitespace at the end of a line of code.</para>
    /// </remarks>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1028CodeFixProvider))]
    [Shared]
    public class SA1028CodeFixProvider : CodeFixProvider
    {
        private static readonly ImmutableArray<string> FixableDiagnostics =
            ImmutableArray.Create(SA1028CodeMustNotContainTrailingWhitespace.DiagnosticId);

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
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
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
                            SpacingResources.SA1028CodeFix,
                            ct => RemoveWhitespaceAsync(context.Document, diagnostic, ct)),
                        diagnostic);
                    break;
                default:
                    var node = root.FindNode(diagnosticSpan, findInsideTrivia: true, getInnermostNodeForTie: true);
                    switch (node.Kind())
                    {
                    case SyntaxKind.XmlText:
                        context.RegisterCodeFix(
                            CodeAction.Create(
                                SpacingResources.SA1028CodeFix,
                                ct => RemoveWhitespaceAsync(context.Document, diagnostic, ct)),
                            diagnostic);
                        break;
                    }

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
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
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
                string oldTriviaContent = trivia.ToFullString();
                TextSpan diagnosticSpanWithinTrivia = TextSpan.FromBounds(diagnosticSpan.Start - trivia.Span.Start, diagnosticSpan.End - trivia.Span.Start);
                newTriviaContent = string.Concat(
                    oldTriviaContent.Substring(0, diagnosticSpanWithinTrivia.Start),
                    oldTriviaContent.Substring(diagnosticSpanWithinTrivia.End));
                newTrivia = SyntaxFactory.SyntaxTrivia(trivia.Kind(), newTriviaContent);
                newRoot = root.ReplaceTrivia(trivia, newTrivia);
                break;
            default:
                var node = root.FindNode(diagnosticSpan, findInsideTrivia: true, getInnermostNodeForTie: true);
                switch (node.Kind())
                {
                case SyntaxKind.XmlText:
                    var xmlNode = (XmlTextSyntax)node;
                    var newTextTokens = xmlNode.TextTokens;
                    foreach (var textToken in newTextTokens)
                    {
                        if (textToken.Span.IntersectsWith(diagnosticSpan))
                        {
                            diagnosticSpanWithinTrivia = TextSpan.FromBounds(diagnosticSpan.Start - textToken.Span.Start, diagnosticSpan.End - textToken.Span.Start);
                            oldTriviaContent = textToken.ValueText;
                            newTriviaContent = string.Concat(
                                oldTriviaContent.Substring(0, diagnosticSpanWithinTrivia.Start),
                                oldTriviaContent.Substring(diagnosticSpanWithinTrivia.End));
                            var newToken = SyntaxFactory.Token(textToken.LeadingTrivia, textToken.Kind(), newTriviaContent, newTriviaContent, textToken.TrailingTrivia);
                            newTextTokens = newTextTokens.Replace(textToken, newToken);
                            break;
                        }
                    }

                    var newNode = xmlNode.Update(newTextTokens);
                    newRoot = root.ReplaceNode(node, newNode);
                    break;
                default:
                    return document;
                }

                break;
            }

            var newDocument = document.WithSyntaxRoot(newRoot);
            return newDocument;
        }
    }
}
