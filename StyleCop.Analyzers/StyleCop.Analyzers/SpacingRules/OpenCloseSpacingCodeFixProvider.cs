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
                SA1020IncrementDecrementSymbolsMustBeSpacedCorrectly.DiagnosticId,
                SA1023DereferenceAndAccessOfSymbolsMustBeSpacedCorrectly.DiagnosticId);

        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds => FixableDiagnostics;

        /// <inheritdoc/>
        public override FixAllProvider GetFixAllProvider()
        {
            return CustomFixAllProviders.BatchFixer;
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
                    if (triviaList.All(i => i.IsKind(SyntaxKind.WhitespaceTrivia) || i.IsKind(SyntaxKind.EndOfLineTrivia)))
                    {
                        replaceMap[token] = token.WithLeadingTrivia();
                    }
                    else if (token.IsFirstInLine() && token.IsLastInLine())
                    {
                        /* This block covers the case where `token` is the only non-trivia token on its line. However,
                         * the line may still contain non-whitespace trivia which we want the removal process to
                         * preserve. This code fix only removes the whitespace surrounding `token` if it is the only
                         * non-whitespace token on the line.
                         */
                        int lastNewLineLeading = token.LeadingTrivia.LastIndexOf(SyntaxKind.EndOfLineTrivia);
                        int firstNewLineFollowing = token.TrailingTrivia.IndexOf(SyntaxKind.EndOfLineTrivia);
                        bool onlyWhitespace = true;
                        for (int i = lastNewLineLeading + 1; i < token.LeadingTrivia.Count; i++)
                        {
                            onlyWhitespace &= token.LeadingTrivia[i].IsKind(SyntaxKind.WhitespaceTrivia);
                        }

                        firstNewLineFollowing = firstNewLineFollowing == -1 ? token.TrailingTrivia.Count : firstNewLineFollowing;
                        for (int i = 0; i < firstNewLineFollowing; i++)
                        {
                            onlyWhitespace &= token.TrailingTrivia[i].IsKind(SyntaxKind.WhitespaceTrivia);
                        }

                        if (onlyWhitespace)
                        {
                            // Move the token, and remove the other tokens from its line. Keep all other surrounding
                            // trivia. Keep the last newline that precedes token, but not the first that follows it.
                            SyntaxTriviaList trailingTrivia = prevToken.TrailingTrivia;
                            if (lastNewLineLeading >= 0)
                            {
                                trailingTrivia = trailingTrivia.AddRange(token.LeadingTrivia.Take(lastNewLineLeading + 1));
                            }

                            // firstNewLineFollowing was adjusted above to account for the missing case.
                            trailingTrivia = trailingTrivia.AddRange(token.TrailingTrivia.Take(firstNewLineFollowing));

                            replaceMap[token] = token.WithLeadingTrivia().WithTrailingTrivia(trailingTrivia);
                        }
                        else
                        {
                            // Just move the token and keep all surrounding trivia.
                            SyntaxTriviaList trailingTrivia = triviaList.AddRange(token.TrailingTrivia);
                            replaceMap[token] = token.WithLeadingTrivia().WithTrailingTrivia(trailingTrivia);
                        }
                    }
                    else
                    {
                        SyntaxTriviaList trailingTrivia = triviaList.AddRange(token.TrailingTrivia);
                        replaceMap[token] = token.WithLeadingTrivia().WithTrailingTrivia(trailingTrivia);
                    }

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
