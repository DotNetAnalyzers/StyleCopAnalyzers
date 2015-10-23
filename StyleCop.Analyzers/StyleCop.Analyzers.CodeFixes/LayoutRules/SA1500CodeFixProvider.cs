// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.LayoutRules
{
    using System;
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
    using Microsoft.CodeAnalysis.Text;

    /// <summary>
    /// Implements a code fix for <see cref="SA1500CurlyBracketsForMultiLineStatementsMustNotShareLine"/>.
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1500CodeFixProvider))]
    [Shared]
    internal class SA1500CodeFixProvider : CodeFixProvider
    {
        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds { get; } =
            ImmutableArray.Create(SA1500CurlyBracketsForMultiLineStatementsMustNotShareLine.DiagnosticId);

        /// <inheritdoc/>
        public override FixAllProvider GetFixAllProvider()
        {
            return FixAll.Instance;
        }

        /// <inheritdoc/>
        public override Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            foreach (Diagnostic diagnostic in context.Diagnostics)
            {
                context.RegisterCodeFix(
                    CodeAction.Create(
                        LayoutResources.SA1500CodeFix,
                        cancellationToken => GetTransformedDocumentAsync(context.Document, diagnostic, cancellationToken),
                        nameof(SA1500CodeFixProvider)),
                    diagnostic);
            }

            return SpecializedTasks.CompletedTask;
        }

        private static async Task<Document> GetTransformedDocumentAsync(Document document, Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            var syntaxRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            var braceToken = syntaxRoot.FindToken(diagnostic.Location.SourceSpan.Start);
            var tokenReplacements = GenerateBraceFixes(document, ImmutableArray.Create(braceToken));

            var newSyntaxRoot = syntaxRoot.ReplaceTokens(tokenReplacements.Keys, (originalToken, rewrittenToken) => tokenReplacements[originalToken]);
            return document.WithSyntaxRoot(newSyntaxRoot);
        }

        private static Dictionary<SyntaxToken, SyntaxToken> GenerateBraceFixes(Document document, ImmutableArray<SyntaxToken> braceTokens)
        {
            var tokenReplacements = new Dictionary<SyntaxToken, SyntaxToken>();

            foreach (var braceToken in braceTokens)
            {
                var braceLine = LocationHelpers.GetLineSpan(braceToken).StartLinePosition.Line;
                var braceReplacementToken = braceToken;

                var indentationOptions = IndentationOptions.FromDocument(document);
                var indentationSteps = DetermineIndentationSteps(indentationOptions, braceToken);

                var previousToken = braceToken.GetPreviousToken();
                var nextToken = braceToken.GetNextToken();

                if (IsAccessorWithSingleLineBlock(previousToken, braceToken))
                {
                    var newTrailingTrivia = previousToken.TrailingTrivia
                        .WithoutTrailingWhitespace()
                        .Add(SyntaxFactory.Space);

                    AddReplacement(tokenReplacements, previousToken, previousToken.WithTrailingTrivia(newTrailingTrivia));

                    braceReplacementToken = braceReplacementToken.WithLeadingTrivia(braceToken.LeadingTrivia.WithoutLeadingWhitespace());
                }
                else
                {
                    // Check if we need to apply a fix before the curly bracket
                    if (LocationHelpers.GetLineSpan(previousToken).StartLinePosition.Line == braceLine)
                    {
                        if (!braceTokens.Contains(previousToken))
                        {
                            var sharedTrivia = braceReplacementToken.LeadingTrivia.WithoutTrailingWhitespace();
                            var previousTokenNewTrailingTrivia = previousToken.TrailingTrivia
                            .WithoutTrailingWhitespace()
                            .AddRange(sharedTrivia)
                            .Add(SyntaxFactory.CarriageReturnLineFeed);

                            AddReplacement(tokenReplacements, previousToken, previousToken.WithTrailingTrivia(previousTokenNewTrailingTrivia));
                        }

                        braceReplacementToken = braceReplacementToken.WithLeadingTrivia(IndentationHelper.GenerateWhitespaceTrivia(indentationOptions, indentationSteps));
                    }

                    // Check if we need to apply a fix after the curly bracket
                    // if a closing curly bracket is followed by a semi-colon or closing paren, no fix is needed.
                    if ((LocationHelpers.GetLineSpan(nextToken).StartLinePosition.Line == braceLine) &&
                        (!braceToken.IsKind(SyntaxKind.CloseBraceToken) || !IsValidFollowingToken(nextToken)))
                    {
                        var sharedTrivia = nextToken.LeadingTrivia.WithoutTrailingWhitespace();
                        var newTrailingTrivia = braceReplacementToken.TrailingTrivia
                            .WithoutTrailingWhitespace()
                            .AddRange(sharedTrivia)
                            .Add(SyntaxFactory.CarriageReturnLineFeed);

                        if (!braceTokens.Contains(nextToken))
                        {
                            int newIndentationSteps;
                            if (braceToken.IsKind(SyntaxKind.OpenBraceToken))
                            {
                                newIndentationSteps = indentationSteps + 1;
                            }
                            else if (nextToken.IsKind(SyntaxKind.CloseBraceToken))
                            {
                                newIndentationSteps = Math.Max(0, indentationSteps - 1);
                            }
                            else
                            {
                                newIndentationSteps = indentationSteps;
                            }

                            AddReplacement(tokenReplacements, nextToken, nextToken.WithLeadingTrivia(IndentationHelper.GenerateWhitespaceTrivia(indentationOptions, newIndentationSteps)));
                        }

                        braceReplacementToken = braceReplacementToken.WithTrailingTrivia(newTrailingTrivia);
                    }
                }

                AddReplacement(tokenReplacements, braceToken, braceReplacementToken);
            }

            return tokenReplacements;
        }

        private static bool IsAccessorWithSingleLineBlock(SyntaxToken previousToken, SyntaxToken curlyBracketToken)
        {
            if (!curlyBracketToken.IsKind(SyntaxKind.OpenBraceToken))
            {
                return false;
            }

            switch (previousToken.Kind())
            {
            case SyntaxKind.GetKeyword:
            case SyntaxKind.SetKeyword:
            case SyntaxKind.AddKeyword:
            case SyntaxKind.RemoveKeyword:
                break;

            default:
                return false;
            }

            var token = curlyBracketToken;
            var depth = 1;

            while (depth > 0)
            {
                token = token.GetNextToken();
                switch (token.Kind())
                {
                case SyntaxKind.CloseBraceToken:
                    depth--;
                    break;

                case SyntaxKind.OpenBraceToken:
                    depth++;
                    break;
                }
            }

            return LocationHelpers.GetLineSpan(curlyBracketToken).StartLinePosition.Line == LocationHelpers.GetLineSpan(token).StartLinePosition.Line;
        }

        private static bool IsValidFollowingToken(SyntaxToken nextToken)
        {
            switch (nextToken.Kind())
            {
            case SyntaxKind.SemicolonToken:
            case SyntaxKind.CloseParenToken:
            case SyntaxKind.CommaToken:
                return true;

            default:
                return false;
            }
        }

        private static int DetermineIndentationSteps(IndentationOptions indentationOptions, SyntaxToken token)
        {
            // For a closing curly bracket use the indentation of the corresponding opening curly bracket
            if (token.IsKind(SyntaxKind.CloseBraceToken))
            {
                var depth = 1;

                while (depth > 0)
                {
                    token = token.GetPreviousToken();
                    switch (token.Kind())
                    {
                    case SyntaxKind.CloseBraceToken:
                        depth++;
                        break;

                    case SyntaxKind.OpenBraceToken:
                        depth--;
                        break;
                    }
                }
            }

            var startLine = GetTokenStartLinePosition(token).Line;

            while (!ContainsStartOfLine(token, startLine))
            {
                token = token.GetPreviousToken();
            }

            return IndentationHelper.GetIndentationSteps(indentationOptions, token);
        }

        private static bool ContainsStartOfLine(SyntaxToken token, int startLine)
        {
            var startLinePosition = GetTokenStartLinePosition(token);

            return (startLinePosition.Line < startLine) || (startLinePosition.Character == 0);
        }

        private static LinePosition GetTokenStartLinePosition(SyntaxToken token)
        {
            return token.SyntaxTree.GetLineSpan(token.FullSpan).StartLinePosition;
        }

        private static void AddReplacement(Dictionary<SyntaxToken, SyntaxToken> tokenReplacements, SyntaxToken originalToken, SyntaxToken replacementToken)
        {
            tokenReplacements[originalToken] = replacementToken;
        }

        private class FixAll : DocumentBasedFixAllProvider
        {
            public static FixAllProvider Instance { get; } =
                new FixAll();

            protected override string CodeActionTitle =>
                LayoutResources.SA1500CodeFix;

            protected override async Task<SyntaxNode> FixAllInDocumentAsync(FixAllContext fixAllContext, Document document)
            {
                var diagnostics = await fixAllContext.GetDocumentDiagnosticsAsync(document).ConfigureAwait(false);
                if (diagnostics.IsEmpty)
                {
                    return null;
                }

                SyntaxNode syntaxRoot = await document.GetSyntaxRootAsync().ConfigureAwait(false);

                var tokens = diagnostics
                    .Select(diagnostic => syntaxRoot.FindToken(diagnostic.Location.SourceSpan.Start))
                    .OrderBy(token => token.SpanStart)
                    .ToImmutableArray();

                var tokenReplacements = GenerateBraceFixes(document, tokens);

                return syntaxRoot.ReplaceTokens(tokenReplacements.Keys, (originalToken, rewrittenToken) => tokenReplacements[originalToken]);
            }
        }
    }
}
