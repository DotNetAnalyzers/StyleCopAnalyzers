// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.SpacingRules
{
    using System;
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
            var transformations = new Dictionary<SyntaxToken, Func<SyntaxToken, SyntaxToken>>();
            UpdateReplacements(syntaxRoot, replacements, transformations, diagnostic);
            var newSyntaxRoot = syntaxRoot.ReplaceTokens(replacements.Keys, (original, maybeRewritten) => transformations[original](maybeRewritten));
            return document.WithSyntaxRoot(newSyntaxRoot);
        }

        private static void UpdateReplacements(
            SyntaxNode syntaxRoot,
            Dictionary<SyntaxToken, SyntaxToken> replacements,
            Dictionary<SyntaxToken, Func<SyntaxToken, SyntaxToken>> transformations,
            Diagnostic diagnostic)
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
                if (!HasWhitespaceBefore(precedingTokenForInsertion, token, replacements))
                {
                    UpdateReplacement(replacements, transformations, token, current => current.WithLeadingTrivia(current.LeadingTrivia.Add(SyntaxFactory.Space)));
                }

                break;

            case SA1003SymbolsMustBeSpacedCorrectly.InsertAfterTag:
                followingToken = token.GetNextToken();
                if (!HasWhitespaceAfter(token, followingToken, replacements))
                {
                    UpdateReplacement(replacements, transformations, token, current => current.WithTrailingTrivia(current.TrailingTrivia.Insert(0, SyntaxFactory.Space)));
                }

                break;

            case SA1003SymbolsMustBeSpacedCorrectly.RemoveBeforeTag:
                var precedingToken = token.GetPreviousToken();
                UpdateReplacement(
                    replacements,
                    transformations,
                    precedingToken,
                    current => current.WithTrailingTrivia(current.TrailingTrivia.WithoutTrailingWhitespace()));
                UpdateReplacement(
                    replacements,
                    transformations,
                    token,
                    current => current.WithLeadingTrivia(current.LeadingTrivia.WithoutLeadingWhitespace()));
                break;

            case SA1003SymbolsMustBeSpacedCorrectly.RemoveAfterTag:
                followingToken = token.GetNextToken();
                UpdateReplacement(
                    replacements,
                    transformations,
                    token,
                    current => current.WithTrailingTrivia(current.TrailingTrivia.WithoutLeadingWhitespace()));
                UpdateReplacement(
                    replacements,
                    transformations,
                    followingToken,
                    current => current.WithLeadingTrivia(current.LeadingTrivia.WithoutLeadingWhitespace()));
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
                        UpdateReplacement(
                            replacements,
                            transformations,
                            token,
                            current => current.WithTrailingTrivia(current.TrailingTrivia.WithoutTrailingWhitespace().Add(SyntaxFactory.Space)));
                    }
                    else
                    {
                        var constructorPrecedingToken = token.GetPreviousToken();
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

                        UpdateReplacement(
                            replacements,
                            transformations,
                            constructorPrecedingToken,
                            current => current.WithTrailingTrivia(current.TrailingTrivia.WithoutTrailingWhitespace().AddRange(endOfLineTrivia)));
                        UpdateReplacement(
                            replacements,
                            transformations,
                            token,
                            current => current
                                .WithLeadingTrivia(current.LeadingTrivia.WithoutLeadingWhitespace().WithoutTrailingWhitespace().AddRange(initializerIndentation))
                                .WithTrailingTrivia(current.TrailingTrivia.WithoutTrailingWhitespace().Add(SyntaxFactory.Space)));
                    }

                    UpdateReplacement(
                        replacements,
                        transformations,
                        followingToken,
                        current => current.WithLeadingTrivia(current.LeadingTrivia.WithoutLeadingWhitespace()));
                }
                else if (action == SA1003SymbolsMustBeSpacedCorrectly.RemoveEndOfLineWithTrailingSpaceTag)
                {
                    UpdateReplacement(
                        replacements,
                        transformations,
                        token,
                        current => current.WithTrailingTrivia(current.TrailingTrivia.WithoutTrailingWhitespace().Add(SyntaxFactory.Space)));
                    UpdateReplacement(
                        replacements,
                        transformations,
                        followingToken,
                        current => current.WithLeadingTrivia(current.LeadingTrivia.WithoutLeadingWhitespace()));
                }
                else
                {
                    UpdateReplacement(
                        replacements,
                        transformations,
                        token,
                        current => current.WithTrailingTrivia(current.TrailingTrivia.WithoutTrailingWhitespace()));
                    UpdateReplacement(
                        replacements,
                        transformations,
                        followingToken,
                        current => current.WithLeadingTrivia(current.LeadingTrivia.WithoutLeadingWhitespace()));
                }

                break;
            }
        }

        private static void UpdateReplacement(
            Dictionary<SyntaxToken, SyntaxToken> replacements,
            Dictionary<SyntaxToken, Func<SyntaxToken, SyntaxToken>> transformations,
            SyntaxToken token,
            Func<SyntaxToken, SyntaxToken> transformation)
        {
            replacements[token] = transformation(GetCurrentReplacement(replacements, token));
            Func<SyntaxToken, SyntaxToken> composedTransformation;
            if (transformations.TryGetValue(token, out composedTransformation))
            {
                transformations[token] = current => transformation(composedTransformation(current));
            }
            else
            {
                transformations[token] = transformation;
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
                var transformations = new Dictionary<SyntaxToken, Func<SyntaxToken, SyntaxToken>>();
                foreach (var diagnostic in diagnostics)
                {
                    fixAllContext.CancellationToken.ThrowIfCancellationRequested();
                    UpdateReplacements(syntaxRoot, replacements, transformations, diagnostic);
                }

                if (replacements.Count == 0)
                {
                    return null;
                }

                return syntaxRoot.ReplaceTokens(replacements.Keys, (original, maybeRewritten) => transformations[original](maybeRewritten));
            }
        }
    }
}
