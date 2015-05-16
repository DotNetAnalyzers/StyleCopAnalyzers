namespace StyleCop.Analyzers.LayoutRules
{
    using System.Collections.Immutable;
    using System.Composition;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using StyleCop.Analyzers.Helpers;

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
            return WellKnownFixAllProviders.BatchFixer;
        }

        /// <inheritdoc/>
        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            foreach (Diagnostic diagnostic in context.Diagnostics.Where(d => FixableDiagnostics.Contains(d.Id)))
            {
                var syntaxRoot = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
                var block = syntaxRoot.FindNode(diagnostic.Location.SourceSpan, getInnermostNodeForTie: true) as BlockSyntax;

                if (block != null)
                {
                    var newSyntaxRoot = this.ReformatBlockAndParent(context, syntaxRoot, block);
                    var newDocument = context.Document.WithSyntaxRoot(newSyntaxRoot);

                    context.RegisterCodeFix(CodeAction.Create("Expand single line block", token => this.GetTransformedDocumentAsync(context, syntaxRoot, block)), diagnostic);
                }
            }
        }

        private Task<Document> GetTransformedDocumentAsync(CodeFixContext context, SyntaxNode syntaxRoot, BlockSyntax block)
        {
            var newSyntaxRoot = this.ReformatBlockAndParent(context, syntaxRoot, block);
            var newDocument = context.Document.WithSyntaxRoot(newSyntaxRoot);

            return Task.FromResult(newDocument);
        }

        private SyntaxNode ReformatBlockAndParent(CodeFixContext context, SyntaxNode syntaxRoot, BlockSyntax block)
        {
            var parentLastToken = block.OpenBraceToken.GetPreviousToken();

            var parentEndLine = parentLastToken.GetLocation().GetLineSpan().EndLinePosition.Line;
            var blockStartLine = block.OpenBraceToken.GetLocation().GetLineSpan().StartLinePosition.Line;

            var newParentLastToken = parentLastToken;
            if (parentEndLine == blockStartLine)
            {
                var newTrailingTrivia = parentLastToken.TrailingTrivia
                    .WithoutTrailingWhitespace()
                    .Add(SyntaxFactory.CarriageReturnLineFeed);

                newParentLastToken = newParentLastToken.WithTrailingTrivia(newTrailingTrivia);
            }

            var newBlock = this.ReformatBlock(context, block);
            var rewriter = new BlockRewriter(parentLastToken, newParentLastToken, block, newBlock);

            var newSyntaxRoot = rewriter.Visit(syntaxRoot);
            return newSyntaxRoot;
        }

        private BlockSyntax ReformatBlock(CodeFixContext context, BlockSyntax block)
        {
            var indentationOptions = IndentationOptions.FromDocument(context.Document);
            var parentIndentationLevel = IndentationHelper.GetIndentationSteps(indentationOptions, block.Parent);

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

            // only add an end-of-line to the close brace if there is none yet.
            if ((newCloseBraceTrailingTrivia.Count == 0) || !newCloseBraceTrailingTrivia.Last().IsKind(SyntaxKind.EndOfLineTrivia))
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

        private class BlockRewriter : CSharpSyntaxRewriter
        {
            private SyntaxToken parentToken;
            private SyntaxToken newParentToken;
            private BlockSyntax block;
            private BlockSyntax newBlock;

            public BlockRewriter(SyntaxToken parentToken, SyntaxToken newParentToken, BlockSyntax block, BlockSyntax newBlock)
            {
                this.parentToken = parentToken;
                this.newParentToken = newParentToken;
                this.block = block;
                this.newBlock = newBlock;
            }

            public override SyntaxToken VisitToken(SyntaxToken token)
            {
                if (token == this.parentToken)
                {
                    return this.newParentToken;
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
