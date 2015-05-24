namespace StyleCop.Analyzers.LayoutRules
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

    /// <summary>
    /// Implements a code fix for <see cref="SA1512SingleLineCommentsMustNotBeFollowedByBlankLine"/>.
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1512CodeFixProvider))]
    [Shared]
    public class SA1512CodeFixProvider : CodeFixProvider
    {
        private static readonly ImmutableArray<string> FixableDiagnostics =
            ImmutableArray.Create(SA1512SingleLineCommentsMustNotBeFollowedByBlankLine.DiagnosticId);

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
            foreach (Diagnostic diagnostic in context.Diagnostics.Where(d => FixableDiagnostics.Contains(d.Id)))
            {
                context.RegisterCodeFix(CodeAction.Create(LayoutResources.SA1512CodeFix, token => GetTransformedDocumentAsync(context.Document, diagnostic, token)), diagnostic);
            }

            return Task.FromResult(true);
        }

        private static async Task<Document> GetTransformedDocumentAsync(Document document, Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            var syntaxRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            var singleLineComment = syntaxRoot.FindTrivia(diagnostic.Location.SourceSpan.Start);

            var newSyntaxRoot = ProcessTriviaList(syntaxRoot, singleLineComment.Token.LeadingTrivia, singleLineComment)
                                ?? ProcessTriviaList(syntaxRoot, singleLineComment.Token.TrailingTrivia, singleLineComment);

            return document.WithSyntaxRoot(newSyntaxRoot);
        }

        private static SyntaxNode ProcessTriviaList(SyntaxNode syntaxRoot, SyntaxTriviaList triviaList, SyntaxTrivia singleLineComment)
        {
            var index = triviaList.IndexOf(singleLineComment);
            if (index == -1)
            {
                return null;
            }

            do
            {
                index++;
            }
            while (!triviaList[index].IsKind(SyntaxKind.EndOfLineTrivia));

            var startOfBlankLine = index;

            do
            {
                index++;
            }
            while (!triviaList[index].IsKind(SyntaxKind.EndOfLineTrivia));

            return syntaxRoot.ReplaceTrivia(triviaList.Skip(startOfBlankLine).Take(index - startOfBlankLine), (t1, t2) => default(SyntaxTrivia));
        }
    }
}
