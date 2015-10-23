// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.LayoutRules
{
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
    using Microsoft.CodeAnalysis.CSharp.Syntax;

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
            return CustomFixAllProviders.BatchFixer;
        }

        /// <inheritdoc/>
        public override Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            foreach (Diagnostic diagnostic in context.Diagnostics)
            {
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
            var statement = syntaxRoot.FindNode(diagnostic.Location.SourceSpan, getInnermostNodeForTie: true) as StatementSyntax;
            if (statement == null)
            {
                return document;
            }

            SyntaxNode newSyntaxRoot;
            BlockSyntax block = statement as BlockSyntax;
            if (block != null)
            {
                newSyntaxRoot = ReformatBlockAndParent(document, syntaxRoot, block);
            }
            else
            {
                newSyntaxRoot = ReformatStatementAndParent(document, syntaxRoot, statement);
            }

            var newDocument = document.WithSyntaxRoot(newSyntaxRoot);
            return newDocument;
        }

        private static SyntaxNode ReformatBlockAndParent(Document document, SyntaxNode syntaxRoot, BlockSyntax block)
        {
            var parentLastToken = block.OpenBraceToken.GetPreviousToken();

            var parentEndLine = parentLastToken.GetEndLine();
            var blockStartLine = block.OpenBraceToken.GetLine();

            var newParentLastToken = parentLastToken;
            if (parentEndLine == blockStartLine)
            {
                var newTrailingTrivia = parentLastToken.TrailingTrivia
                    .WithoutTrailingWhitespace()
                    .Add(SyntaxFactory.CarriageReturnLineFeed);

                newParentLastToken = newParentLastToken.WithTrailingTrivia(newTrailingTrivia);
            }

            var parentNextToken = block.CloseBraceToken.GetNextToken();

            var nextTokenLine = parentNextToken.GetLine();
            var blockCloseLine = block.CloseBraceToken.GetEndLine();

            var newParentNextToken = parentNextToken;
            if (nextTokenLine == blockCloseLine)
            {
                newParentNextToken = newParentNextToken.WithLeadingTrivia(parentLastToken.LeadingTrivia);
            }

            var newBlock = ReformatBlock(document, block);
            var rewriter = new BlockRewriter(parentLastToken, newParentLastToken, block, newBlock, parentNextToken, newParentNextToken);

            var newSyntaxRoot = rewriter.Visit(syntaxRoot);
            return newSyntaxRoot.WithoutFormatting();
        }

        private static SyntaxNode ReformatStatementAndParent(Document document, SyntaxNode syntaxRoot, StatementSyntax statement)
        {
            var parentLastToken = statement.GetFirstToken().GetPreviousToken();

            var parentEndLine = parentLastToken.GetEndLine();
            var statementStartLine = statement.GetFirstToken().GetLine();

            var newParentLastToken = parentLastToken;
            if (parentEndLine == statementStartLine)
            {
                var newTrailingTrivia = parentLastToken.TrailingTrivia
                    .WithoutTrailingWhitespace()
                    .Add(SyntaxFactory.CarriageReturnLineFeed);

                newParentLastToken = newParentLastToken.WithTrailingTrivia(newTrailingTrivia);
            }

            var parentNextToken = statement.GetLastToken().GetNextToken();

            var nextTokenLine = parentNextToken.GetLine();
            var statementCloseLine = statement.GetLastToken().GetEndLine();

            var newParentNextToken = parentNextToken;
            if (nextTokenLine == statementCloseLine)
            {
                var indentationOptions = IndentationOptions.FromDocument(document);
                var parentIndentationLevel = IndentationHelper.GetIndentationSteps(indentationOptions, GetStatementParent(statement.Parent));
                var indentationString = IndentationHelper.GenerateIndentationString(indentationOptions, parentIndentationLevel);
                newParentNextToken = newParentNextToken.WithLeadingTrivia(SyntaxFactory.Whitespace(indentationString));
            }

            var newStatement = ReformatStatement(document, statement);
            var newSyntaxRoot = syntaxRoot.ReplaceSyntax(
                new[] { statement },
                (originalNode, rewrittenNode) => originalNode == statement ? newStatement : rewrittenNode,
                new[] { parentLastToken, parentNextToken },
                (originalToken, rewrittenToken) =>
                {
                    if (originalToken == parentLastToken)
                    {
                        return newParentLastToken;
                    }
                    else if (originalToken == parentNextToken)
                    {
                        return newParentNextToken;
                    }
                    else
                    {
                        return rewrittenToken;
                    }
                },
                Enumerable.Empty<SyntaxTrivia>(),
                (originalTrivia, rewrittenTrivia) => rewrittenTrivia);

            return newSyntaxRoot.WithoutFormatting();
        }

        private static BlockSyntax ReformatBlock(Document document, BlockSyntax block)
        {
            var indentationOptions = IndentationOptions.FromDocument(document);
            var parentIndentationLevel = IndentationHelper.GetIndentationSteps(indentationOptions, GetStatementParent(block.Parent));

            // use one additional step of indentation for lambdas / anonymous methods
            switch (block.Parent.Kind())
            {
            case SyntaxKind.AnonymousMethodExpression:
            case SyntaxKind.SimpleLambdaExpression:
            case SyntaxKind.ParenthesizedLambdaExpression:
                parentIndentationLevel++;
                break;
            }

            var indentationString = IndentationHelper.GenerateIndentationString(indentationOptions, parentIndentationLevel);
            var statementIndentationString = IndentationHelper.GenerateIndentationString(indentationOptions, parentIndentationLevel + 1);

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

            var openBraceToken = SyntaxFactory.Token(SyntaxKind.OpenBraceToken)
                .WithLeadingTrivia(newOpenBraceLeadingTrivia)
                .WithTrailingTrivia(newOpenBraceTrailingTrivia);

            var closeBraceToken = SyntaxFactory.Token(SyntaxKind.CloseBraceToken)
                .WithLeadingTrivia(newCloseBraceLeadingTrivia)
                .WithTrailingTrivia(newCloseBraceTrailingTrivia);

            var statements = SyntaxFactory.List<StatementSyntax>();
            foreach (var statement in block.Statements)
            {
                var newLeadingTrivia = statement.GetLeadingTrivia()
                    .WithoutTrailingWhitespace()
                    .Add(SyntaxFactory.Whitespace(statementIndentationString));

                var newTrailingTrivia = statement.GetTrailingTrivia()
                    .WithoutTrailingWhitespace()
                    .Add(SyntaxFactory.CarriageReturnLineFeed);

                var modifiedStatement = statement
                    .WithLeadingTrivia(newLeadingTrivia)
                    .WithTrailingTrivia(newTrailingTrivia);

                statements = statements.Add(modifiedStatement);
            }

            return SyntaxFactory.Block(openBraceToken, statements, closeBraceToken);
        }

        private static StatementSyntax ReformatStatement(Document document, StatementSyntax statement)
        {
            var indentationOptions = IndentationOptions.FromDocument(document);
            var parentIndentationLevel = IndentationHelper.GetIndentationSteps(indentationOptions, GetStatementParent(statement.Parent));

            // use one additional step of indentation for lambdas / anonymous methods
            switch (statement.Parent.Kind())
            {
            case SyntaxKind.AnonymousMethodExpression:
            case SyntaxKind.SimpleLambdaExpression:
            case SyntaxKind.ParenthesizedLambdaExpression:
                parentIndentationLevel++;
                break;
            }

            var statementIndentationString = IndentationHelper.GenerateIndentationString(indentationOptions, parentIndentationLevel + 1);

            var newFirstTokenLeadingTrivia = statement.GetFirstToken().LeadingTrivia
                .WithoutTrailingWhitespace()
                .Add(SyntaxFactory.Whitespace(statementIndentationString));

            var newLastTokenTrailingTrivia = statement.GetLastToken().TrailingTrivia
                .WithoutTrailingWhitespace()
                .Add(SyntaxFactory.CarriageReturnLineFeed);

            var firstToken = statement.GetFirstToken().WithLeadingTrivia(newFirstTokenLeadingTrivia);
            var lastToken = statement.GetLastToken().WithTrailingTrivia(newLastTokenTrailingTrivia);

            return statement.ReplaceTokens(
                new[] { statement.GetFirstToken(), statement.GetLastToken() },
                (originalToken, rewrittenToken) =>
                {
                    if (originalToken == statement.GetFirstToken())
                    {
                        return firstToken;
                    }
                    else if (originalToken == statement.GetLastToken())
                    {
                        return lastToken;
                    }
                    else
                    {
                        return rewrittenToken;
                    }
                });
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

        private class BlockRewriter : CSharpSyntaxRewriter
        {
            private readonly SyntaxToken parentToken;
            private readonly SyntaxToken newParentToken;
            private readonly BlockSyntax block;
            private readonly BlockSyntax newBlock;
            private readonly SyntaxToken nextToken;
            private readonly SyntaxToken newNextToken;

            public BlockRewriter(SyntaxToken parentToken, SyntaxToken newParentToken, BlockSyntax block, BlockSyntax newBlock, SyntaxToken nextToken, SyntaxToken newNextToken)
            {
                this.parentToken = parentToken;
                this.newParentToken = newParentToken;
                this.block = block;
                this.newBlock = newBlock;
                this.nextToken = nextToken;
                this.newNextToken = newNextToken;
            }

            public override SyntaxToken VisitToken(SyntaxToken token)
            {
                if (token == this.parentToken)
                {
                    return this.newParentToken;
                }

                if (token == this.nextToken)
                {
                    return this.newNextToken;
                }

                return base.VisitToken(token);
            }

            public override SyntaxNode VisitBlock(BlockSyntax node)
            {
                if (node == this.block)
                {
                    return this.newBlock;
                }

                return base.VisitBlock(node);
            }
        }
    }
}
