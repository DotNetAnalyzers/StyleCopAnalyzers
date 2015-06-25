namespace StyleCop.Analyzers.LayoutRules
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
    /// Implements a code fix for <see cref="SA1505OpeningCurlyBracketsMustNotBeFollowedByBlankLine"/>.
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1505CodeFixProvider))]
    [Shared]
    public class SA1505CodeFixProvider : CodeFixProvider
    {
        private static readonly ImmutableArray<string> FixableDiagnostics =
            ImmutableArray.Create(SA1505OpeningCurlyBracketsMustNotBeFollowedByBlankLine.DiagnosticId);

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
                context.RegisterCodeFix(CodeAction.Create(LayoutResources.SA1505CodeFix, token => GetTransformedDocumentAsync(context.Document, diagnostic, token)), diagnostic);
            }

            return SpecializedTasks.CompletedTask;
        }

        private static async Task<Document> GetTransformedDocumentAsync(Document document, Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            var syntaxRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            var openBraceToken = syntaxRoot.FindToken(diagnostic.Location.SourceSpan.Start);
            var nextToken = openBraceToken.GetNextToken();

            var triviaList = openBraceToken.TrailingTrivia.AddRange(nextToken.LeadingTrivia);

            var firstEndOfLineIndex = triviaList.IndexOf(SyntaxKind.EndOfLineTrivia);
            var lastEndOfLineIndex = -1;

            var done = false;
            for (var i = firstEndOfLineIndex + 1; !done && (i < triviaList.Count); i++)
            {
                switch (triviaList[i].Kind())
                {
                case SyntaxKind.WhitespaceTrivia:
                    break;
                case SyntaxKind.EndOfLineTrivia:
                    lastEndOfLineIndex = i;
                    break;
                default:
                    done = true;
                    break;
                }
            }

            var replaceMap = new Dictionary<SyntaxToken, SyntaxToken>()
            {
                [openBraceToken] = openBraceToken.WithTrailingTrivia(triviaList.Take(firstEndOfLineIndex + 1)),
                [nextToken] = nextToken.WithLeadingTrivia(triviaList.Skip(lastEndOfLineIndex + 1))
            };

            var newSyntaxRoot = syntaxRoot.ReplaceTokens(replaceMap.Keys, (t1, t2) => replaceMap[t1]);
            return document.WithSyntaxRoot(newSyntaxRoot);
        }
    }
}
