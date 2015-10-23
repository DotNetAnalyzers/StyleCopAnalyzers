// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.SpacingRules
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Composition;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Helpers;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using ReadabilityRules;

    /// <summary>
    /// Implements a code fix for the opening and closing spacing diagnostics.
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(TokenSpacingCodeFixProvider))]
    [Shared]
    internal class TokenSpacingCodeFixProvider : CodeFixProvider
    {
        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds { get; } =
            ImmutableArray.Create(
                SA1000KeywordsMustBeSpacedCorrectly.DiagnosticId,
                SA1001CommasMustBeSpacedCorrectly.DiagnosticId,
                SA1002SemicolonsMustBeSpacedCorrectly.DiagnosticId,
                SA1006PreprocessorKeywordsMustNotBePrecededBySpace.DiagnosticId,
                SA1007OperatorKeywordMustBeFollowedBySpace.DiagnosticId,
                SA1008OpeningParenthesisMustBeSpacedCorrectly.DiagnosticId,
                SA1009ClosingParenthesisMustBeSpacedCorrectly.DiagnosticId,
                SA1010OpeningSquareBracketsMustBeSpacedCorrectly.DiagnosticId,
                SA1011ClosingSquareBracketsMustBeSpacedCorrectly.DiagnosticId,
                SA1012OpeningCurlyBracketsMustBeSpacedCorrectly.DiagnosticId,
                SA1013ClosingCurlyBracketsMustBeSpacedCorrectly.DiagnosticId,
                SA1014OpeningGenericBracketsMustBeSpacedCorrectly.DiagnosticId,
                SA1015ClosingGenericBracketsMustBeSpacedCorrectly.DiagnosticId,
                SA1016OpeningAttributeBracketsMustBeSpacedCorrectly.DiagnosticId,
                SA1017ClosingAttributeBracketsMustBeSpacedCorrectly.DiagnosticId,
                SA1019MemberAccessSymbolsMustBeSpacedCorrectly.DiagnosticId,
                SA1020IncrementDecrementSymbolsMustBeSpacedCorrectly.DiagnosticId,
                SA1021NegativeSignsMustBeSpacedCorrectly.DiagnosticId,
                SA1022PositiveSignsMustBeSpacedCorrectly.DiagnosticId,
                SA1023DereferenceAndAccessOfSymbolsMustBeSpacedCorrectly.DiagnosticId,
                SA1024ColonsMustBeSpacedCorrectly.DiagnosticId,
                SA1026CodeMustNotContainSpaceAfterNewKeywordInImplicitlyTypedArrayAllocation.DiagnosticId,
                SA1110OpeningParenthesisMustBeOnDeclarationLine.DiagnosticId,
                SA1111ClosingParenthesisMustBeOnLineOfLastParameter.DiagnosticId,
                SA1112ClosingParenthesisMustBeOnLineOfOpeningParenthesis.DiagnosticId,
                SA1113CommaMustBeOnSameLineAsPreviousParameter.DiagnosticId);

        /// <inheritdoc/>
        public override FixAllProvider GetFixAllProvider()
        {
            return FixAll.Instance;
        }

        /// <inheritdoc/>
        public override Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            foreach (var diagnostic in context.Diagnostics)
            {
                context.RegisterCodeFix(
                    CodeAction.Create(
                        SpacingResources.TokenSpacingCodeFix,
                        cancellationToken => GetTransformedDocumentAsync(context.Document, diagnostic, cancellationToken),
                        nameof(TokenSpacingCodeFixProvider)),
                    diagnostic);
            }

            return SpecializedTasks.CompletedTask;
        }

        private static async Task<Document> GetTransformedDocumentAsync(Document document, Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            var syntaxRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            var replaceMap = new Dictionary<SyntaxToken, SyntaxToken>();
            var token = syntaxRoot.FindToken(diagnostic.Location.SourceSpan.Start, findInsideTrivia: true);

            UpdateReplaceMap(replaceMap, token, diagnostic);

            if (replaceMap.Any())
            {
                var newSyntaxRoot = syntaxRoot.ReplaceTokens(replaceMap.Keys, (t1, t2) => replaceMap[t1]);
                return document.WithSyntaxRoot(newSyntaxRoot);
            }

            return document;
        }

        private static void UpdateReplaceMap(Dictionary<SyntaxToken, SyntaxToken> replaceMap, SyntaxToken token, Diagnostic diagnostic)
        {
            string location;
            if (!diagnostic.Properties.TryGetValue(TokenSpacingProperties.LocationKey, out location))
            {
                return;
            }

            string action;
            if (!diagnostic.Properties.TryGetValue(TokenSpacingProperties.ActionKey, out action))
            {
                return;
            }

            string layout;
            if (!diagnostic.Properties.TryGetValue(TokenSpacingProperties.LayoutKey, out layout))
            {
                layout = TokenSpacingProperties.LayoutPack;
            }

            SyntaxTriviaList triviaList;
            switch (location)
            {
            case TokenSpacingProperties.LocationPreceding:
                var prevToken = token.GetPreviousToken();
                switch (action)
                {
                case TokenSpacingProperties.ActionInsert:
                    if (!replaceMap.ContainsKey(prevToken))
                    {
                        replaceMap[token] = token.WithLeadingTrivia(token.LeadingTrivia.Add(SyntaxFactory.Space));
                    }

                    break;

                case TokenSpacingProperties.ActionRemove:
                    bool tokenIsFirstInLine = token.IsFirstInLine();
                    bool preserveLayout = layout == TokenSpacingProperties.LayoutPreserve;
                    triviaList = prevToken.TrailingTrivia.AddRange(token.LeadingTrivia);
                    if (triviaList.Any(t => t.IsDirective))
                    {
                        break;
                    }

                    replaceMap[prevToken] = prevToken.WithTrailingTrivia();
                    if ((!preserveLayout || !tokenIsFirstInLine)
                        && triviaList.All(i => i.IsKind(SyntaxKind.WhitespaceTrivia) || i.IsKind(SyntaxKind.EndOfLineTrivia)))
                    {
                        replaceMap[token] = token.WithLeadingTrivia();
                    }
                    else if (tokenIsFirstInLine && token.IsLastInLine())
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
                        SyntaxTriviaList trailingTrivia = triviaList.AddRange(token.TrailingTrivia.WithoutLeadingWhitespace(endOfLineIsWhitespace: false));
                        replaceMap[token] = token.WithLeadingTrivia().WithTrailingTrivia(trailingTrivia);
                    }

                    break;

                case TokenSpacingProperties.ActionRemoveImmediate:
                    SyntaxTriviaList tokenLeadingTrivia = token.LeadingTrivia;
                    while (tokenLeadingTrivia.Any() && tokenLeadingTrivia.Last().IsKind(SyntaxKind.WhitespaceTrivia))
                    {
                        tokenLeadingTrivia = tokenLeadingTrivia.RemoveAt(tokenLeadingTrivia.Count - 1);
                    }

                    replaceMap[token] = token.WithLeadingTrivia(tokenLeadingTrivia);

                    if (!tokenLeadingTrivia.Any())
                    {
                        SyntaxTriviaList previousTrailingTrivia = prevToken.TrailingTrivia;
                        while (previousTrailingTrivia.Any() && previousTrailingTrivia.Last().IsKind(SyntaxKind.WhitespaceTrivia))
                        {
                            previousTrailingTrivia = previousTrailingTrivia.RemoveAt(previousTrailingTrivia.Count - 1);
                        }

                        replaceMap[prevToken] = prevToken.WithTrailingTrivia(previousTrailingTrivia);
                    }

                    break;
                }

                break;

            case TokenSpacingProperties.LocationFollowing:
                var nextToken = token.GetNextToken();
                switch (action)
                {
                case TokenSpacingProperties.ActionInsert:
                    if (!replaceMap.ContainsKey(nextToken))
                    {
                        replaceMap[token] = token.WithTrailingTrivia(token.TrailingTrivia.Insert(0, SyntaxFactory.Space));
                    }

                    break;

                case TokenSpacingProperties.ActionRemove:
                    triviaList = token.TrailingTrivia.AddRange(nextToken.LeadingTrivia);

                    replaceMap[token] = token.WithTrailingTrivia();
                    replaceMap[nextToken] = nextToken.WithLeadingTrivia(triviaList.WithoutLeadingWhitespace(true));
                    break;
                }

                break;
            }
        }

        private class FixAll : DocumentBasedFixAllProvider
        {
            public static FixAllProvider Instance { get; } = new FixAll();

            protected override string CodeActionTitle => SpacingResources.TokenSpacingCodeFix;

            protected override async Task<SyntaxNode> FixAllInDocumentAsync(FixAllContext fixAllContext, Document document)
            {
                var diagnostics = await fixAllContext.GetDocumentDiagnosticsAsync(document).ConfigureAwait(false);
                if (diagnostics.IsEmpty)
                {
                    return null;
                }

                var syntaxRoot = await document.GetSyntaxRootAsync().ConfigureAwait(false);

                var replaceMap = new Dictionary<SyntaxToken, SyntaxToken>();

                foreach (var diagnostic in diagnostics)
                {
                    var token = syntaxRoot.FindToken(diagnostic.Location.SourceSpan.Start, findInsideTrivia: true);
                    UpdateReplaceMap(replaceMap, token, diagnostic);
                }

                return syntaxRoot.ReplaceTokens(replaceMap.Keys, (t1, t2) => replaceMap[t1]);
            }
        }
    }
}
