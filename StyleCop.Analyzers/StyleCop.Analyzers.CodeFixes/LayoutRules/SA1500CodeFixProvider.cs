// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.LayoutRules
{
    using System;
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
    using Microsoft.CodeAnalysis.Text;
    using StyleCop.Analyzers.Helpers;
    using StyleCop.Analyzers.Settings.ObjectModel;

    /// <summary>
    /// Implements a code fix for <see cref="SA1500BracesForMultiLineStatementsMustNotShareLine"/>.
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1500CodeFixProvider))]
    [Shared]
    internal class SA1500CodeFixProvider : CodeFixProvider
    {
        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds { get; } =
            ImmutableArray.Create(SA1500BracesForMultiLineStatementsMustNotShareLine.DiagnosticId);

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

            var settings = SettingsHelper.GetStyleCopSettings(document.Project.AnalyzerOptions, cancellationToken);
            var braceToken = syntaxRoot.FindToken(diagnostic.Location.SourceSpan.Start);
            var tokenReplacements = GenerateBraceFixes(settings, ImmutableArray.Create(braceToken));

            var newSyntaxRoot = syntaxRoot.ReplaceTokens(tokenReplacements.Keys, (originalToken, rewrittenToken) => tokenReplacements[originalToken]);
            return document.WithSyntaxRoot(newSyntaxRoot);
        }

        private static Dictionary<SyntaxToken, SyntaxToken> GenerateBraceFixes(StyleCopSettings settings, ImmutableArray<SyntaxToken> braceTokens)
        {
            var tokenReplacements = new Dictionary<SyntaxToken, SyntaxToken>();

            foreach (var braceToken in braceTokens)
            {
                var braceLine = LocationHelpers.GetLineSpan(braceToken).StartLinePosition.Line;
                var braceReplacementToken = braceToken;

                var indentationSteps = DetermineIndentationSteps(settings.Indentation, braceToken);

                var previousToken = braceToken.GetPreviousToken();

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
                    // Check if we need to apply a fix before the brace
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

                        braceReplacementToken = braceReplacementToken.WithLeadingTrivia(IndentationHelper.GenerateWhitespaceTrivia(settings.Indentation, indentationSteps));
                    }

                    // Check if we need to apply a fix after the brace. No fix is needed when:
                    // - The closing brace is followed by a semi-colon or closing paren
                    // - The closing brace is the last token in the file
                    // - The closing brace is followed by the while expression of a do/while loop and the
                    //   allowDoWhileOnClosingBrace setting is enabled.
                    var nextToken = braceToken.GetNextToken();
                    var nextTokenLine = nextToken.IsKind(SyntaxKind.None) ? -1 : LocationHelpers.GetLineSpan(nextToken).StartLinePosition.Line;
                    var isMultiDimensionArrayInitializer = braceToken.IsKind(SyntaxKind.OpenBraceToken) && braceToken.Parent.IsKind(SyntaxKind.ArrayInitializerExpression) && braceToken.Parent.Parent.IsKind(SyntaxKind.ArrayInitializerExpression);
                    var allowDoWhileOnClosingBrace = settings.LayoutRules.AllowDoWhileOnClosingBrace && nextToken.IsKind(SyntaxKind.WhileKeyword) && (braceToken.Parent?.IsKind(SyntaxKind.Block) ?? false) && (braceToken.Parent.Parent?.IsKind(SyntaxKind.DoStatement) ?? false);

                    if ((nextTokenLine == braceLine) &&
                        (!braceToken.IsKind(SyntaxKind.CloseBraceToken) || !IsValidFollowingToken(nextToken)) &&
                        !isMultiDimensionArrayInitializer &&
                        !allowDoWhileOnClosingBrace)
                    {
                        var sharedTrivia = nextToken.LeadingTrivia.WithoutTrailingWhitespace();
                        var newTrailingTrivia = braceReplacementToken.TrailingTrivia
                            .WithoutTrailingWhitespace()
                            .AddRange(sharedTrivia)
                            .Add(SyntaxFactory.CarriageReturnLineFeed);

                        if (!braceTokens.Contains(nextToken))
                        {
                            int newIndentationSteps = indentationSteps;
                            if (braceToken.IsKind(SyntaxKind.OpenBraceToken))
                            {
                                newIndentationSteps++;
                            }

                            if (nextToken.IsKind(SyntaxKind.CloseBraceToken))
                            {
                                newIndentationSteps = Math.Max(0, newIndentationSteps - 1);
                            }

                            AddReplacement(tokenReplacements, nextToken, nextToken.WithLeadingTrivia(IndentationHelper.GenerateWhitespaceTrivia(settings.Indentation, newIndentationSteps)));
                        }

                        braceReplacementToken = braceReplacementToken.WithTrailingTrivia(newTrailingTrivia);
                    }
                }

                AddReplacement(tokenReplacements, braceToken, braceReplacementToken);
            }

            return tokenReplacements;
        }

        private static bool IsAccessorWithSingleLineBlock(SyntaxToken previousToken, SyntaxToken braceToken)
        {
            if (!braceToken.IsKind(SyntaxKind.OpenBraceToken))
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

            var token = braceToken;
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

            return LocationHelpers.GetLineSpan(braceToken).StartLinePosition.Line == LocationHelpers.GetLineSpan(token).StartLinePosition.Line;
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

        private static int DetermineIndentationSteps(IndentationSettings indentationSettings, SyntaxToken token)
        {
            // For a closing brace use the indentation of the corresponding opening brace
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

            return IndentationHelper.GetIndentationSteps(indentationSettings, token);
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
            if (tokenReplacements.ContainsKey(originalToken))
            {
                // This will only happen when a single keyword (like else) has invalid brace tokens before and after it.
                tokenReplacements[originalToken] = tokenReplacements[originalToken].WithTrailingTrivia(replacementToken.TrailingTrivia);
            }
            else
            {
                tokenReplacements[originalToken] = replacementToken;
            }
        }

        private class FixAll : DocumentBasedFixAllProvider
        {
            public static FixAllProvider Instance { get; } =
                new FixAll();

            protected override string CodeActionTitle =>
                LayoutResources.SA1500CodeFix;

            protected override async Task<SyntaxNode> FixAllInDocumentAsync(FixAllContext fixAllContext, Document document, ImmutableArray<Diagnostic> diagnostics)
            {
                if (diagnostics.IsEmpty)
                {
                    return null;
                }

                SyntaxNode syntaxRoot = await document.GetSyntaxRootAsync().ConfigureAwait(false);

                var tokens = diagnostics
                    .Select(diagnostic => syntaxRoot.FindToken(diagnostic.Location.SourceSpan.Start))
                    .OrderBy(token => token.SpanStart)
                    .ToImmutableArray();

                var settings = SettingsHelper.GetStyleCopSettings(document.Project.AnalyzerOptions, fixAllContext.CancellationToken);

                var tokenReplacements = GenerateBraceFixes(settings, tokens);

                return syntaxRoot.ReplaceTokens(tokenReplacements.Keys, (originalToken, rewrittenToken) => tokenReplacements[originalToken]);
            }
        }
    }
}
