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
    /// Implements a code fix for <see cref="SA1511WhileDoFooterMustNotBePrecededByBlankLine"/>.
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1511CodeFixProvider))]
    [Shared]
    public class SA1511CodeFixProvider : CodeFixProvider
    {
        private static readonly ImmutableArray<string> FixableDiagnostics =
            ImmutableArray.Create(SA1511WhileDoFooterMustNotBePrecededByBlankLine.DiagnosticId);

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
                context.RegisterCodeFix(CodeAction.Create(LayoutResources.SA1511CodeFix, token => GetTransformedDocumentAsync(context.Document, diagnostic, token)), diagnostic);
            }

            return Task.FromResult(true);
        }

        private static async Task<Document> GetTransformedDocumentAsync(Document document, Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            var syntaxRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            var whileToken = syntaxRoot.FindToken(diagnostic.Location.SourceSpan.Start);

            var triviaList = whileToken.LeadingTrivia;
            var leadingWhitespaceStart = triviaList.Count - 1;

            // skip leading whitespace in front of the while keyword
            while ((leadingWhitespaceStart > 0) && triviaList[leadingWhitespaceStart - 1].IsKind(SyntaxKind.WhitespaceTrivia))
            {
                leadingWhitespaceStart--;
            }

            var blankLinesStart = leadingWhitespaceStart - 1;
            var done = false;
            while (!done && (blankLinesStart >= 0))
            {
                switch (triviaList[blankLinesStart].Kind())
                {
                case SyntaxKind.WhitespaceTrivia:
                case SyntaxKind.EndOfLineTrivia:
                    blankLinesStart--;
                    break;

                case SyntaxKind.IfDirectiveTrivia:
                case SyntaxKind.ElifDirectiveTrivia:
                case SyntaxKind.ElseDirectiveTrivia:
                case SyntaxKind.EndIfDirectiveTrivia:
                    // directives include an embedded end of line
                    blankLinesStart++;
                    done = true;
                    break;

                default:
                    // include the first end of line (as it is part of the non blank line trivia)
                    while (!triviaList[blankLinesStart].IsKind(SyntaxKind.EndOfLineTrivia))
                    {
                        blankLinesStart++;
                    }

                    blankLinesStart++;
                    done = true;
                    break;
                }
            }

            var newLeadingTrivia = SyntaxFactory.TriviaList(triviaList.Take(blankLinesStart).Concat(triviaList.Skip(leadingWhitespaceStart)));
            var newSyntaxRoot = syntaxRoot.ReplaceToken(whileToken, whileToken.WithLeadingTrivia(newLeadingTrivia));
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
