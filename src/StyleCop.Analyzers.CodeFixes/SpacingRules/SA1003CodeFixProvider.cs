// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.SpacingRules
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Composition;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using StyleCop.Analyzers.Helpers;

    /// <summary>
    /// Implements a code fix for <see cref="SA1003SymbolsMustBeSpacedCorrectly"/>.
    /// </summary>
    /// <remarks>
    /// <para>To fix a violation of this rule, ensure that the spacing around the symbol follows the rule described in
    /// <see cref="SA1003SymbolsMustBeSpacedCorrectly"/>.</para>
    /// </remarks>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1003CodeFixProvider))]
    [Shared]
    internal class SA1003CodeFixProvider : CodeFixProvider
    {
        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds { get; } =
            ImmutableArray.Create(SA1003SymbolsMustBeSpacedCorrectly.DiagnosticId);

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
                if (diagnostic.Properties.ContainsKey(SA1003SymbolsMustBeSpacedCorrectly.CodeFixAction))
                {
                    context.RegisterCodeFix(
                        CodeAction.Create(
                            SpacingResources.SA1003CodeFix,
                            cancellationToken => GetTransformedDocumentAsync(context.Document, diagnostic, cancellationToken),
                            nameof(SA1003CodeFixProvider)),
                        diagnostic);
                }
            }

            return SpecializedTasks.CompletedTask;
        }

        private static async Task<Document> GetTransformedDocumentAsync(Document document, Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            var syntaxRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            var replacements = new Dictionary<SyntaxToken, SyntaxToken>();
            UpdateReplacements(syntaxRoot, replacements, diagnostic);
            var newSyntaxRoot = syntaxRoot.ReplaceTokens(replacements.Keys, (original, maybeRewritten) => replacements[original]);
            return document.WithSyntaxRoot(newSyntaxRoot);
        }

        private static void UpdateReplacements(SyntaxNode syntaxRoot, Dictionary<SyntaxToken, SyntaxToken> replacements, Diagnostic diagnostic)
        {
            string action;
            if (!diagnostic.Properties.TryGetValue(SA1003SymbolsMustBeSpacedCorrectly.CodeFixAction, out action))
            {
                return;
            }

            var token = syntaxRoot.FindToken(diagnostic.Location.SourceSpan.Start, findInsideTrivia: true);
            SyntaxToken followingToken;

            switch (action)
            {
            case SA1003SymbolsMustBeSpacedCorrectly.InsertBeforeTag:
                var precedingTokenForInsertion = token.GetPreviousToken();
                var tokenToInsertBefore = GetCurrentReplacement(replacements, token);
                if (!HasWhitespaceBefore(precedingTokenForInsertion, token, replacements))
                {
                    replacements[token] = tokenToInsertBefore.WithLeadingTrivia(tokenToInsertBefore.LeadingTrivia.Add(SyntaxFactory.Space));
                }

                break;

            case SA1003SymbolsMustBeSpacedCorrectly.InsertAfterTag:
                followingToken = token.GetNextToken();
                var tokenToInsertAfter = GetCurrentReplacement(replacements, token);
                if (!HasWhitespaceAfter(token, followingToken, replacements))
                {
                    replacements[token] = tokenToInsertAfter.WithTrailingTrivia(tokenToInsertAfter.TrailingTrivia.Insert(0, SyntaxFactory.Space));
                }

                break;

            case SA1003SymbolsMustBeSpacedCorrectly.RemoveBeforeTag:
                var precedingToken = token.GetPreviousToken();
                var precedingTokenReplacement = GetCurrentReplacement(replacements, precedingToken);
                var tokenReplacement = GetCurrentReplacement(replacements, token);
                replacements[precedingToken] = precedingTokenReplacement.WithTrailingTrivia(precedingTokenReplacement.TrailingTrivia.WithoutTrailingWhitespace());
                replacements[token] = tokenReplacement.WithLeadingTrivia(tokenReplacement.LeadingTrivia.WithoutLeadingWhitespace());
                break;

            case SA1003SymbolsMustBeSpacedCorrectly.RemoveAfterTag:
                followingToken = token.GetNextToken();
                var tokenToRemoveAfter = GetCurrentReplacement(replacements, token);
                var followingTokenReplacement = GetCurrentReplacement(replacements, followingToken);
                replacements[token] = tokenToRemoveAfter.WithTrailingTrivia(tokenToRemoveAfter.TrailingTrivia.WithoutLeadingWhitespace());
                replacements[followingToken] = followingTokenReplacement.WithLeadingTrivia(followingTokenReplacement.LeadingTrivia.WithoutLeadingWhitespace());
                break;

            case SA1003SymbolsMustBeSpacedCorrectly.RemoveEndOfLineTag:
            case SA1003SymbolsMustBeSpacedCorrectly.RemoveEndOfLineWithTrailingSpaceTag:
                followingToken = token.GetNextToken();
                if (token.Parent is ConstructorInitializerSyntax constructorInitializer
                    && constructorInitializer.ColonToken == token)
                {
                    var constructorTokenReplacement = GetCurrentReplacement(replacements, token);
                    if (constructorTokenReplacement.LeadingTrivia.Any(SyntaxKind.SingleLineCommentTrivia))
                    {
                        replacements[token] = constructorTokenReplacement
                            .WithTrailingTrivia(constructorTokenReplacement.TrailingTrivia.WithoutTrailingWhitespace().Add(SyntaxFactory.Space));
                    }
                    else
                    {
                        var constructorPrecedingToken = token.GetPreviousToken();
                        var precedingTokenForInitializer = GetCurrentReplacement(replacements, constructorPrecedingToken);
                        var followingTokenForInitializer = GetCurrentReplacement(replacements, followingToken);
                        var initializerIndentation = SyntaxFactory.TriviaList();
                        var endOfLineTrivia = SyntaxFactory.TriviaList();
                        foreach (var trivia in followingTokenForInitializer.LeadingTrivia)
                        {
                            if (trivia.IsKind(SyntaxKind.WhitespaceTrivia))
                            {
                                initializerIndentation = initializerIndentation.Add(trivia);
                            }
                        }

                        foreach (var trivia in constructorTokenReplacement.TrailingTrivia)
                        {
                            if (trivia.IsKind(SyntaxKind.EndOfLineTrivia))
                            {
                                endOfLineTrivia = endOfLineTrivia.Add(trivia);
                            }
                        }

                        replacements[constructorPrecedingToken] = precedingTokenForInitializer.WithTrailingTrivia(
                            precedingTokenForInitializer.TrailingTrivia.WithoutTrailingWhitespace().AddRange(endOfLineTrivia));
                        replacements[token] = constructorTokenReplacement
                            .WithLeadingTrivia(
                                constructorTokenReplacement.LeadingTrivia.WithoutLeadingWhitespace().WithoutTrailingWhitespace().AddRange(initializerIndentation))
                            .WithTrailingTrivia(constructorTokenReplacement.TrailingTrivia.WithoutTrailingWhitespace().Add(SyntaxFactory.Space));
                    }

                    var nextTokenReplacement = GetCurrentReplacement(replacements, followingToken);
                    replacements[followingToken] = nextTokenReplacement.WithLeadingTrivia(nextTokenReplacement.LeadingTrivia.WithoutLeadingWhitespace());
                }
                else if (action == SA1003SymbolsMustBeSpacedCorrectly.RemoveEndOfLineWithTrailingSpaceTag)
                {
                    var tokenToEndLine = GetCurrentReplacement(replacements, token);
                    var followingTokenToEndLine = GetCurrentReplacement(replacements, followingToken);
                    replacements[token] = tokenToEndLine.WithTrailingTrivia(tokenToEndLine.TrailingTrivia.WithoutTrailingWhitespace().Add(SyntaxFactory.Space));
                    replacements[followingToken] = followingTokenToEndLine.WithLeadingTrivia(followingTokenToEndLine.LeadingTrivia.WithoutLeadingWhitespace());
                }
                else
                {
                    var tokenToEndLine = GetCurrentReplacement(replacements, token);
                    var followingTokenToEndLine = GetCurrentReplacement(replacements, followingToken);
                    replacements[token] = tokenToEndLine.WithTrailingTrivia(tokenToEndLine.TrailingTrivia.WithoutTrailingWhitespace());
                    replacements[followingToken] = followingTokenToEndLine.WithLeadingTrivia(followingTokenToEndLine.LeadingTrivia.WithoutLeadingWhitespace());
                }

                break;
            }
        }

        private static SyntaxToken GetCurrentReplacement(Dictionary<SyntaxToken, SyntaxToken> replacements, SyntaxToken token)
        {
            SyntaxToken replacement;
            if (replacements.TryGetValue(token, out replacement))
            {
                return replacement;
            }

            return token;
        }

        private static bool HasWhitespaceBefore(SyntaxToken precedingToken, SyntaxToken token, Dictionary<SyntaxToken, SyntaxToken> replacements)
        {
            var tokenReplacement = GetCurrentReplacement(replacements, token);
            if (tokenReplacement.LeadingTrivia.Count > 0)
            {
                return tokenReplacement.LeadingTrivia.Last().IsKind(SyntaxKind.WhitespaceTrivia);
            }

            var precedingTokenReplacement = GetCurrentReplacement(replacements, precedingToken);
            return precedingTokenReplacement.TrailingTrivia.Count > 0
                && precedingTokenReplacement.TrailingTrivia.Last().IsKind(SyntaxKind.WhitespaceTrivia);
        }

        private static bool HasWhitespaceAfter(SyntaxToken token, SyntaxToken followingToken, Dictionary<SyntaxToken, SyntaxToken> replacements)
        {
            var tokenReplacement = GetCurrentReplacement(replacements, token);
            if (tokenReplacement.TrailingTrivia.Count > 0)
            {
                var trailingTrivia = tokenReplacement.TrailingTrivia[0];
                return trailingTrivia.IsKind(SyntaxKind.WhitespaceTrivia) || trailingTrivia.IsKind(SyntaxKind.EndOfLineTrivia);
            }

            var followingTokenReplacement = GetCurrentReplacement(replacements, followingToken);
            if (followingTokenReplacement.LeadingTrivia.Count > 0)
            {
                var leadingTrivia = followingTokenReplacement.LeadingTrivia[0];
                return leadingTrivia.IsKind(SyntaxKind.WhitespaceTrivia) || leadingTrivia.IsKind(SyntaxKind.EndOfLineTrivia);
            }

            return false;
        }

        private class FixAll : DocumentBasedFixAllProvider
        {
            public static FixAllProvider Instance { get; } = new FixAll();

            protected override string CodeActionTitle => SpacingResources.SA1003CodeFix;

            protected override async Task<SyntaxNode?> FixAllInDocumentAsync(FixAllContext fixAllContext, Document document, ImmutableArray<Diagnostic> diagnostics)
            {
                if (diagnostics.IsEmpty)
                {
                    return null;
                }

                var syntaxRoot = await document.GetSyntaxRootAsync(fixAllContext.CancellationToken).ConfigureAwait(false);
                var replacements = new Dictionary<SyntaxToken, SyntaxToken>();
                foreach (var diagnostic in diagnostics)
                {
                    fixAllContext.CancellationToken.ThrowIfCancellationRequested();
                    UpdateReplacements(syntaxRoot, replacements, diagnostic);
                }

                if (replacements.Count == 0)
                {
                    return null;
                }

                return syntaxRoot.ReplaceTokens(replacements.Keys, (original, maybeRewritten) => replacements[original]);
            }
        }
    }
}
