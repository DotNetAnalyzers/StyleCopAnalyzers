// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.LayoutRules
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
    using Settings.ObjectModel;
    using StyleCop.Analyzers.Helpers;

    /// <summary>
    /// Implements a code fix for <see cref="SA1501StatementMustNotBeOnASingleLine"/>.
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1501CodeFixProvider))]
    [Shared]
    internal class SA1501CodeFixProvider : CodeFixProvider
    {
        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds { get; } =
            ImmutableArray.Create(SA1501StatementMustNotBeOnASingleLine.DiagnosticId);

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
                if (diagnostic.Properties.GetValueOrDefault(SA1501StatementMustNotBeOnASingleLine.SuppressCodeFixKey) == SA1501StatementMustNotBeOnASingleLine.SuppressCodeFixValue)
                {
                    continue;
                }

                context.RegisterCodeFix(
                    CodeAction.Create(
                        LayoutResources.SA1501CodeFix,
                        cancellationToken => GetTransformedDocumentAsync(context.Document, diagnostic, cancellationToken),
                        nameof(SA1501CodeFixProvider)),
                    diagnostic);
            }

            return SpecializedTasks.CompletedTask;
        }

        private static async Task<Document> GetTransformedDocumentAsync(Document document, Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            var syntaxRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            var settings = SettingsHelper.GetStyleCopSettings(document.Project.AnalyzerOptions, cancellationToken);
            var statement = syntaxRoot.FindNode(diagnostic.Location.SourceSpan, getInnermostNodeForTie: true) as StatementSyntax;
            if (statement == null)
            {
                return document;
            }

            var tokenReplaceMap = new Dictionary<SyntaxToken, SyntaxToken>();

            ReformatStatementAndSurroundings(statement, settings.Indentation, tokenReplaceMap);

            var newSyntaxRoot = syntaxRoot.ReplaceTokens(tokenReplaceMap.Keys, (original, rewritten) => tokenReplaceMap[original]);
            var newDocument = document.WithSyntaxRoot(newSyntaxRoot.WithoutFormatting());
            return newDocument;
        }

        private static void ReformatStatementAndSurroundings(StatementSyntax statement, IndentationSettings indentationSettings, Dictionary<SyntaxToken, SyntaxToken> tokenReplaceMap)
        {
            var block = statement as BlockSyntax;

            var previousToken = statement.GetFirstToken().GetPreviousToken();
            var previousTokenEndLine = previousToken.GetEndLine();
            var statementStartLine = statement.GetFirstToken().GetLine();

            if (previousTokenEndLine == statementStartLine)
            {
                var newTrailingTrivia = previousToken.TrailingTrivia
                    .WithoutTrailingWhitespace()
                    .Add(SyntaxFactory.CarriageReturnLineFeed);

                AddToReplaceMap(tokenReplaceMap, previousToken, previousToken.WithTrailingTrivia(newTrailingTrivia));
            }

            if (block != null)
            {
                ReformatBlock(indentationSettings, block, tokenReplaceMap);
            }
            else
            {
                ReformatStatement(indentationSettings, statement, tokenReplaceMap);
            }

            var nextToken = statement.GetLastToken().GetNextToken();
            if ((block != null) && nextToken.IsKind(SyntaxKind.SemicolonToken))
            {
                // skip trailing semicolon tokens for blocks
                nextToken = nextToken.GetNextToken();
            }

            var nextTokenStartLine = nextToken.GetLine();
            var statementEndLine = statement.GetLastToken().GetEndLine();

            if (nextTokenStartLine == statementEndLine)
            {
                var indentationLevel = DetermineIndentationLevel(indentationSettings, tokenReplaceMap, statement);
                var indentationTrivia = IndentationHelper.GenerateWhitespaceTrivia(indentationSettings, indentationLevel);

                AddToReplaceMap(tokenReplaceMap, nextToken, nextToken.WithLeadingTrivia(indentationTrivia));
            }
        }

        private static int DetermineIndentationLevel(IndentationSettings indentationSettings, Dictionary<SyntaxToken, SyntaxToken> tokenReplaceMap, StatementSyntax statement)
        {
            var parent = GetStatementParent(statement.Parent);
            int parentIndentationLevel;

            SyntaxToken replacementToken;
            if (tokenReplaceMap.TryGetValue(parent.GetFirstToken(), out replacementToken))
            {
                // if the parent is being modified, use the new leading trivia from the parent for determining the indentation
                parentIndentationLevel = IndentationHelper.GetIndentationSteps(indentationSettings, replacementToken);
            }
            else
            {
                parentIndentationLevel = IndentationHelper.GetIndentationSteps(indentationSettings, GetFirstOnLineParent(parent));
            }

            return parentIndentationLevel;
        }

        private static void ReformatBlock(IndentationSettings indentationSettings, BlockSyntax block, Dictionary<SyntaxToken, SyntaxToken> tokenReplaceMap)
        {
            var parentIndentationLevel = IndentationHelper.GetIndentationSteps(indentationSettings, GetStatementParent(block.Parent));

            // use one additional step of indentation for lambdas / anonymous methods
            switch (block.Parent.Kind())
            {
            case SyntaxKind.AnonymousMethodExpression:
            case SyntaxKind.SimpleLambdaExpression:
            case SyntaxKind.ParenthesizedLambdaExpression:
                parentIndentationLevel++;
                break;
            }

            var indentationString = IndentationHelper.GenerateIndentationString(indentationSettings, parentIndentationLevel);
            var statementIndentationString = IndentationHelper.GenerateIndentationString(indentationSettings, parentIndentationLevel + 1);

            var newOpenBraceLeadingTrivia = block.OpenBraceToken.LeadingTrivia
                .WithoutTrailingWhitespace()
                .Add(SyntaxFactory.Whitespace(indentationString));

            var newOpenBraceTrailingTrivia = block.OpenBraceToken.TrailingTrivia
                .WithoutTrailingWhitespace()
                .Add(SyntaxFactory.CarriageReturnLineFeed);

            var newCloseBraceLeadingTrivia = block.CloseBraceToken.LeadingTrivia
                .WithoutTrailingWhitespace()
                .Add(SyntaxFactory.Whitespace(indentationString));

            var newCloseBraceTrailingTrivia = block.CloseBraceToken.TrailingTrivia
                .WithoutTrailingWhitespace();

            bool addNewLineAfterCloseBrace;
            switch (block.CloseBraceToken.GetNextToken().Kind())
            {
            case SyntaxKind.CloseParenToken:
            case SyntaxKind.CommaToken:
            case SyntaxKind.SemicolonToken:
                addNewLineAfterCloseBrace = false;
                break;
            default:
                addNewLineAfterCloseBrace = (newCloseBraceTrailingTrivia.Count == 0) || !newCloseBraceTrailingTrivia.Last().IsKind(SyntaxKind.EndOfLineTrivia);
                break;
            }

            if (addNewLineAfterCloseBrace)
            {
                newCloseBraceTrailingTrivia = newCloseBraceTrailingTrivia.Add(SyntaxFactory.CarriageReturnLineFeed);
            }

            AddToReplaceMap(tokenReplaceMap, block.OpenBraceToken, block.OpenBraceToken.WithLeadingTrivia(newOpenBraceLeadingTrivia).WithTrailingTrivia(newOpenBraceTrailingTrivia));
            AddToReplaceMap(tokenReplaceMap, block.CloseBraceToken, block.CloseBraceToken.WithLeadingTrivia(newCloseBraceLeadingTrivia).WithTrailingTrivia(newCloseBraceTrailingTrivia));

            foreach (var statement in block.Statements)
            {
                var firstToken = statement.GetFirstToken();
                var lastToken = statement.GetLastToken();

                var newLeadingTrivia = firstToken.LeadingTrivia
                    .WithoutTrailingWhitespace()
                    .Add(SyntaxFactory.Whitespace(statementIndentationString));

                var newTrailingTrivia = lastToken.TrailingTrivia
                    .WithoutTrailingWhitespace()
                    .Add(SyntaxFactory.CarriageReturnLineFeed);

                AddToReplaceMap(tokenReplaceMap, firstToken, firstToken.WithLeadingTrivia(newLeadingTrivia));
                AddToReplaceMap(tokenReplaceMap, lastToken, lastToken.WithTrailingTrivia(newTrailingTrivia));
            }
        }

        private static void ReformatStatement(IndentationSettings indentationSettings, StatementSyntax statement, Dictionary<SyntaxToken, SyntaxToken> tokenReplaceMap)
        {
            var indentationLevel = DetermineIndentationLevel(indentationSettings, tokenReplaceMap, statement);

            // use one additional step of indentation for lambdas / anonymous methods
            switch (statement.Parent.Kind())
            {
            case SyntaxKind.AnonymousMethodExpression:
            case SyntaxKind.SimpleLambdaExpression:
            case SyntaxKind.ParenthesizedLambdaExpression:
                indentationLevel++;
                break;
            }

            var statementIndentationTrivia = IndentationHelper.GenerateWhitespaceTrivia(indentationSettings, indentationLevel + 1);

            var newFirstTokenLeadingTrivia = statement.GetFirstToken().LeadingTrivia
                .WithoutTrailingWhitespace()
                .Add(statementIndentationTrivia);

            var newLastTokenTrailingTrivia = statement.GetLastToken().TrailingTrivia
                .WithoutTrailingWhitespace()
                .Add(SyntaxFactory.CarriageReturnLineFeed);

            AddToReplaceMap(tokenReplaceMap, statement.GetFirstToken(), statement.GetFirstToken().WithLeadingTrivia(newFirstTokenLeadingTrivia));
            AddToReplaceMap(tokenReplaceMap, statement.GetLastToken(), statement.GetLastToken().WithTrailingTrivia(newLastTokenTrailingTrivia));
        }

        private static void AddToReplaceMap(Dictionary<SyntaxToken, SyntaxToken> tokenReplaceMap, SyntaxToken original, SyntaxToken replacement)
        {
            SyntaxToken existingReplacement;
            SyntaxToken reprocessedReplacement = replacement;

            // Check if there is already a replacement for the token. If so -> merge the replacements.
            // This assumes that the overlapping token replacements do not overlap in trivia replacements.
            if (tokenReplaceMap.TryGetValue(original, out existingReplacement))
            {
                reprocessedReplacement = AreTriviaEqual(original.LeadingTrivia, existingReplacement.LeadingTrivia) ? replacement : existingReplacement;
                reprocessedReplacement = reprocessedReplacement.WithTrailingTrivia(AreTriviaEqual(original.TrailingTrivia, existingReplacement.TrailingTrivia) ? replacement.TrailingTrivia : existingReplacement.TrailingTrivia);
            }

            tokenReplaceMap[original] = reprocessedReplacement;
        }

        private static bool AreTriviaEqual(SyntaxTriviaList left, SyntaxTriviaList right)
        {
            return string.Equals(left.ToString(), right.ToString(), StringComparison.Ordinal);
        }

        private static SyntaxNode GetStatementParent(SyntaxNode node)
        {
            StatementSyntax statementSyntax = node.FirstAncestorOrSelf<StatementSyntax>();
            if (statementSyntax == null)
            {
                return null;
            }

            if (statementSyntax.IsKind(SyntaxKind.IfStatement) && statementSyntax.Parent.IsKind(SyntaxKind.ElseClause))
            {
                return statementSyntax.Parent;
            }

            return statementSyntax;
        }

        private static SyntaxNode GetFirstOnLineParent(SyntaxNode parent)
        {
            // if the parent is not the first on a line, find the parent that is.
            // This mainly happens for 'else if' statements.
            while (!parent.GetFirstToken().IsFirstInLine())
            {
                parent = parent.Parent;
            }

            return parent;
        }

        private class FixAll : DocumentBasedFixAllProvider
        {
            public static FixAllProvider Instance { get; } =
                new FixAll();

            protected override string CodeActionTitle =>
                LayoutResources.SA1501CodeFixAll;

            protected override async Task<SyntaxNode> FixAllInDocumentAsync(FixAllContext fixAllContext, Document document, ImmutableArray<Diagnostic> diagnostics)
            {
                if (diagnostics.IsEmpty)
                {
                    return null;
                }

                var tokenReplaceMap = new Dictionary<SyntaxToken, SyntaxToken>();
                var settings = SettingsHelper.GetStyleCopSettings(document.Project.AnalyzerOptions, fixAllContext.CancellationToken);
                SyntaxNode syntaxRoot = await document.GetSyntaxRootAsync(fixAllContext.CancellationToken).ConfigureAwait(false);

                foreach (var diagnostic in diagnostics.Sort(DiagnosticComparer.Instance))
                {
                    var statement = syntaxRoot.FindNode(diagnostic.Location.SourceSpan, getInnermostNodeForTie: true) as StatementSyntax;
                    if (statement == null)
                    {
                        continue;
                    }

                    ReformatStatementAndSurroundings(statement, settings.Indentation, tokenReplaceMap);
                }

                var newSyntaxRoot = syntaxRoot.ReplaceTokens(tokenReplaceMap.Keys, (original, rewritten) => tokenReplaceMap[original]);
                return newSyntaxRoot.WithoutFormatting();
            }

            private class DiagnosticComparer : IComparer<Diagnostic>
            {
                public static DiagnosticComparer Instance { get; } = new DiagnosticComparer();

                /// <inheritdoc/>
                public int Compare(Diagnostic x, Diagnostic y)
                {
                    return x.Location.SourceSpan.Start - y.Location.SourceSpan.Start;
                }
            }
        }
    }
}
