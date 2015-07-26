namespace StyleCop.Analyzers.SpacingRules
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
    /// Implements a code fix for <see cref="SA1009ClosingParenthesisMustBeSpacedCorrectly"/>.
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1009CodeFixProvider))]
    [Shared]
    public class SA1009CodeFixProvider : CodeFixProvider
    {
        private static readonly ImmutableArray<string> FixableDiagnostics =
            ImmutableArray.Create(SA1009ClosingParenthesisMustBeSpacedCorrectly.DiagnosticId);

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
                context.RegisterCodeFix(CodeAction.Create(SpacingResources.SA1009CodeFix, token => GetTransformedDocumentAsync(context.Document, diagnostic, token)), diagnostic);
            }

            return SpecializedTasks.CompletedTask;
        }

        private static async Task<Document> GetTransformedDocumentAsync(Document document, Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            var syntaxRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            var closeParenToken = syntaxRoot.FindToken(diagnostic.Location.SourceSpan.Start);

            string location;
            if (!diagnostic.Properties.TryGetValue(SA1009ClosingParenthesisMustBeSpacedCorrectly.LocationKey, out location))
            {
                return document;
            }

            string action;
            if (!diagnostic.Properties.TryGetValue(SA1009ClosingParenthesisMustBeSpacedCorrectly.ActionKey, out action))
            {
                return document;
            }

            var replaceMap = new Dictionary<SyntaxToken, SyntaxToken>();
            SyntaxTriviaList triviaList;
            switch (location)
            {
                case SA1009ClosingParenthesisMustBeSpacedCorrectly.LocationPreceding:
                    switch (action)
                    {
                        case SA1009ClosingParenthesisMustBeSpacedCorrectly.ActionInsert:
                            replaceMap[closeParenToken] = closeParenToken.WithLeadingTrivia(closeParenToken.LeadingTrivia.Add(SyntaxFactory.Space));
                            break;

                        case SA1009ClosingParenthesisMustBeSpacedCorrectly.ActionRemove:
                            var prevToken = closeParenToken.GetPreviousToken();
                            triviaList = prevToken.TrailingTrivia.AddRange(closeParenToken.LeadingTrivia);

                            replaceMap[prevToken] = prevToken.WithTrailingTrivia();
                            replaceMap[closeParenToken] = closeParenToken.WithLeadingTrivia(triviaList.WithoutTrailingWhitespace());
                            break;

                        default:
                            return document;
                    }

                    break;

                case SA1009ClosingParenthesisMustBeSpacedCorrectly.LocationFollowing:
                    switch (action)
                    {
                        case SA1009ClosingParenthesisMustBeSpacedCorrectly.ActionInsert:
                            replaceMap[closeParenToken] = closeParenToken.WithTrailingTrivia(closeParenToken.TrailingTrivia.Insert(0, SyntaxFactory.Space));
                            break;

                        case SA1009ClosingParenthesisMustBeSpacedCorrectly.ActionRemove:
                            var nextToken = closeParenToken.GetNextToken();
                            triviaList = closeParenToken.TrailingTrivia.AddRange(nextToken.LeadingTrivia);

                            replaceMap[closeParenToken] = closeParenToken.WithTrailingTrivia();
                            replaceMap[nextToken] = nextToken.WithLeadingTrivia(triviaList.WithoutLeadingWhitespace());
                            break;

                        default:
                            return document;
                    }

                    break;

                default:
                    return document;
            }

            var newSyntaxRoot = syntaxRoot.ReplaceTokens(replaceMap.Keys, (t1, t2) => replaceMap[t1]);
            return document.WithSyntaxRoot(newSyntaxRoot);
        }
    }
}
