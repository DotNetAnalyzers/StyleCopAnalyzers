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
    /// Implements a code fix for <see cref="SA1008OpeningParenthesisMustBeSpacedCorrectly"/>.
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1008CodeFixProvider))]
    [Shared]
    public class SA1008CodeFixProvider : CodeFixProvider
    {
        private static readonly ImmutableArray<string> FixableDiagnostics =
            ImmutableArray.Create(SA1008OpeningParenthesisMustBeSpacedCorrectly.DiagnosticId);

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
                context.RegisterCodeFix(CodeAction.Create(SpacingResources.SA1008CodeFix, token => GetTransformedDocumentAsync(context.Document, diagnostic, token)), diagnostic);
            }

            return Task.FromResult(true);
        }

        private static async Task<Document> GetTransformedDocumentAsync(Document document, Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            var syntaxRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            var openParenToken = syntaxRoot.FindToken(diagnostic.Location.SourceSpan.Start);

            string location;
            if (!diagnostic.Properties.TryGetValue(SA1008OpeningParenthesisMustBeSpacedCorrectly.LocationKey, out location))
            {
                return document;
            }

            string action;
            if (!diagnostic.Properties.TryGetValue(SA1008OpeningParenthesisMustBeSpacedCorrectly.ActionKey, out action))
            {
                return document;
            }

            var replaceMap = new Dictionary<SyntaxToken, SyntaxToken>();
            SyntaxTriviaList triviaList;
            switch (location)
            {
                case SA1008OpeningParenthesisMustBeSpacedCorrectly.LocationPreceding:
                    switch (action)
                    {
                        case SA1008OpeningParenthesisMustBeSpacedCorrectly.ActionInsert:
                            replaceMap[openParenToken] = openParenToken.WithLeadingTrivia(openParenToken.LeadingTrivia.Add(SyntaxFactory.Space));
                            break;

                        case SA1008OpeningParenthesisMustBeSpacedCorrectly.ActionRemove:
                            var prevToken = openParenToken.GetPreviousToken();
                            triviaList = prevToken.TrailingTrivia.AddRange(openParenToken.LeadingTrivia);

                            replaceMap[prevToken] = prevToken.WithTrailingTrivia();
                            replaceMap[openParenToken] = openParenToken.WithLeadingTrivia(triviaList.WithoutTrailingWhitespace());
                            break;

                        default:
                            return document;
                    }

                    break;

                case SA1008OpeningParenthesisMustBeSpacedCorrectly.LocationFollowing:
                    switch (action)
                    {
                        case SA1008OpeningParenthesisMustBeSpacedCorrectly.ActionInsert:
                            replaceMap[openParenToken] = openParenToken.WithTrailingTrivia(openParenToken.TrailingTrivia.Insert(0, SyntaxFactory.Space));
                            break;

                        case SA1008OpeningParenthesisMustBeSpacedCorrectly.ActionRemove:
                            var nextToken = openParenToken.GetNextToken();
                            triviaList = openParenToken.TrailingTrivia.AddRange(nextToken.LeadingTrivia);

                            replaceMap[openParenToken] = openParenToken.WithTrailingTrivia();
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
