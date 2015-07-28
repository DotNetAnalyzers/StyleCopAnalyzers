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
    /// Implements a code fix for the opening and closing spacing diagnostics.
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(OpenCloseSpacingCodeFixProvider))]
    [Shared]
    public class OpenCloseSpacingCodeFixProvider : CodeFixProvider
    {
        internal const string LocationKey = "location";
        internal const string ActionKey = "action";
        internal const string LocationPreceding = "preceding";
        internal const string LocationFollowing = "following";
        internal const string ActionInsert = "insert";
        internal const string ActionRemove = "remove";

        private static readonly ImmutableArray<string> FixableDiagnostics =
            ImmutableArray.Create(
                SA1009ClosingParenthesisMustBeSpacedCorrectly.DiagnosticId,
                SA1011ClosingSquareBracketsMustBeSpacedCorrectly.DiagnosticId,
                SA1012OpeningCurlyBracketsMustBeSpacedCorrectly.DiagnosticId,
                SA1013ClosingCurlyBracketsMustBeSpacedCorrectly.DiagnosticId,
                SA1014OpeningGenericBracketsMustBeSpacedCorrectly.DiagnosticId,
                SA1015ClosingGenericBracketsMustBeSpacedCorrectly.DiagnosticId,
                SA1019MemberAccessSymbolsMustBeSpacedCorrectly.DiagnosticId,
                SA1020IncrementDecrementSymbolsMustBeSpacedCorrectly.DiagnosticId);

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
                context.RegisterCodeFix(CodeAction.Create(SpacingResources.OpenCloseSpacingCodeFix, token => GetTransformedDocumentAsync(context.Document, diagnostic, token), equivalenceKey: nameof(OpenCloseSpacingCodeFixProvider)), diagnostic);
            }

            return SpecializedTasks.CompletedTask;
        }

        private static async Task<Document> GetTransformedDocumentAsync(Document document, Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            var syntaxRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            var token = syntaxRoot.FindToken(diagnostic.Location.SourceSpan.Start);

            string location;
            if (!diagnostic.Properties.TryGetValue(LocationKey, out location))
            {
                return document;
            }

            string action;
            if (!diagnostic.Properties.TryGetValue(ActionKey, out action))
            {
                return document;
            }

            var replaceMap = new Dictionary<SyntaxToken, SyntaxToken>();
            SyntaxTriviaList triviaList;
            switch (location)
            {
            case LocationPreceding:
                switch (action)
                {
                case ActionInsert:
                    replaceMap[token] = token.WithLeadingTrivia(token.LeadingTrivia.Add(SyntaxFactory.Space));
                    break;

                case ActionRemove:
                    var prevToken = token.GetPreviousToken();
                    triviaList = prevToken.TrailingTrivia.AddRange(token.LeadingTrivia);

                    replaceMap[prevToken] = prevToken.WithTrailingTrivia();
                    replaceMap[token] = token.WithLeadingTrivia(triviaList.WithoutTrailingWhitespace());
                    break;

                default:
                    return document;
                }

                break;

            case LocationFollowing:
                switch (action)
                {
                case ActionInsert:
                    replaceMap[token] = token.WithTrailingTrivia(token.TrailingTrivia.Insert(0, SyntaxFactory.Space));
                    break;

                case ActionRemove:
                    var nextToken = token.GetNextToken();
                    triviaList = token.TrailingTrivia.AddRange(nextToken.LeadingTrivia);

                    replaceMap[token] = token.WithTrailingTrivia();
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
