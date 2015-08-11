namespace StyleCop.Analyzers.LayoutRules
{
    using System.Collections.Immutable;
    using System.Composition;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using StyleCop.Analyzers.Helpers;
    using StyleCop.Analyzers.SpacingRules;

    /// <summary>
    /// Implements a code fix for <see cref="SA1501StatementMustNotBeOnASingleLine"/>.
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1501CodeFixProvider))]
    [Shared]
    public class SA1501CodeFixProvider : CodeFixProvider
    {
        private static readonly ImmutableArray<string> FixableDiagnostics =
            ImmutableArray.Create(SA1501StatementMustNotBeOnASingleLine.DiagnosticId);

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
            foreach (Diagnostic diagnostic in context.Diagnostics.Where(d => FixableDiagnostics.Contains(d.Id)))
            {
                context.RegisterCodeFix(CodeAction.Create(LayoutResources.SA1501CodeFix, cancellationToken => GetTransformedDocumentAsync(context.Document, diagnostic, cancellationToken), equivalenceKey: nameof(SA1501CodeFixProvider)), diagnostic);
            }

            return SpecializedTasks.CompletedTask;
        }

        private static async Task<Document> GetTransformedDocumentAsync(Document document, Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            var syntaxRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            var block = syntaxRoot.FindNode(diagnostic.Location.SourceSpan, getInnermostNodeForTie: true) as BlockSyntax;
            if (block == null)
            {
                return document;
            }

            var newSyntaxRoot = ReformatBlockAndParent(document, syntaxRoot, block);
            var newDocument = document.WithSyntaxRoot(newSyntaxRoot);

            return newDocument;
        }

        private static SyntaxNode ReformatBlockAndParent(Document document, SyntaxNode syntaxRoot, BlockSyntax block)
        {
            var parentLastToken = block.OpenBraceToken.GetPreviousToken();

            var parentEndLine = parentLastToken.GetLineSpan().EndLinePosition.Line;
            var blockStartLine = block.OpenBraceToken.GetLineSpan().StartLinePosition.Line;

            var newParentLastToken = parentLastToken;
            if (parentEndLine == blockStartLine)
            {
                var newTrailingTrivia = parentLastToken.TrailingTrivia
                    .WithoutTrailingWhitespace()
                    .Add(SyntaxFactory.CarriageReturnLineFeed);

                newParentLastToken = newParentLastToken.WithTrailingTrivia(newTrailingTrivia);
            }

            var parentNextToken = block.CloseBraceToken.GetNextToken();

            var nextTokenLine = parentNextToken.GetLineSpan().StartLinePosition.Line;
            var blockCloseLine = block.CloseBraceToken.GetLineSpan().EndLinePosition.Line;

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

        private static SyntaxNode GetStatementParent(SyntaxNode node)
        {
            while ((node != null) && !(node is StatementSyntax))
            {
                node = node.Parent;
            }

            return node;
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
