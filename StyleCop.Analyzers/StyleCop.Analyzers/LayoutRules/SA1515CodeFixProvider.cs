namespace StyleCop.Analyzers.LayoutRules
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

    /// <summary>
    /// Implements a code fix for <see cref="SA1515SingleLineCommentMustBePrecededByBlankLine"/>.
    /// </summary>
    [ExportCodeFixProvider(nameof(SA1515CodeFixProvider), LanguageNames.CSharp)]
    [Shared]
    public class SA1515CodeFixProvider : CodeFixProvider
    {
        private static readonly ImmutableArray<string> FixableDiagnostics =
            ImmutableArray.Create(SA1515SingleLineCommentMustBePrecededByBlankLine.DiagnosticId);

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
                context.RegisterCodeFix(CodeAction.Create("Insert blank line before comment", token => GetTransformedDocumentAsync(context.Document, diagnostic, token)), diagnostic);
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

            index--;
            while (index >= 0)
            {
                switch (triviaList[index].Kind())
                {
                    case SyntaxKind.WhitespaceTrivia:
                        index--;
                        break;

                    default:
                        return syntaxRoot.ReplaceTrivia(triviaList[index], new[] { triviaList[index], SyntaxFactory.CarriageReturnLineFeed });
                }
            }

            return syntaxRoot.ReplaceTrivia(triviaList[0], new[] { SyntaxFactory.CarriageReturnLineFeed, triviaList[0] });
        }
    }
}
