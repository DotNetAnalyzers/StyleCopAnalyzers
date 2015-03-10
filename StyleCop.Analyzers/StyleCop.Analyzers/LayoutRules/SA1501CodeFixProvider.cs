namespace StyleCop.Analyzers.LayoutRules
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Composition;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Formatting;

    using StyleCop.Analyzers.Helpers;

    /// <summary>
    /// Implements a code fix for <see cref="SA1501StatementMustNotBeOnASingleLine"/>.
    /// </summary>
    [ExportCodeFixProvider(nameof(SA1501CodeFixProvider), LanguageNames.CSharp)]
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

                    context.RegisterCodeFix(CodeAction.Create("Expand single line block", token => Task.FromResult(newDocument)), diagnostic);
                }
            }
        }

        private SyntaxNode ReformatBlockAndParent(CodeFixContext context, SyntaxNode syntaxRoot, BlockSyntax block)
        {
            var prevToken = block.OpenBraceToken.GetPreviousToken();

            var parentEndLine = prevToken.GetLocation().GetLineSpan().EndLinePosition.Line;
            var blockStartLine = block.OpenBraceToken.GetLocation().GetLineSpan().StartLinePosition.Line;

            var newPrevToken = prevToken.WithoutTrailingWhitespace();
            if (parentEndLine == blockStartLine)
            {
                var newTrailingTrivia = newPrevToken.TrailingTrivia.Add(SyntaxFactory.CarriageReturnLineFeed);
                newPrevToken = newPrevToken.WithTrailingTrivia(newTrailingTrivia);
            }

            /* TODO: The ReplaceToken call below seems to do nothing! */
            var newSyntaxRoot = syntaxRoot.TrackNodes(block);
            newSyntaxRoot = newSyntaxRoot.ReplaceToken(prevToken, newPrevToken);
            newSyntaxRoot = newSyntaxRoot.ReplaceNode(newSyntaxRoot.GetCurrentNode(block), this.ReformatBlock(context, block));

            return newSyntaxRoot;
        }

        private SyntaxNode ReformatBlock(CodeFixContext context, BlockSyntax block)
        {
            var indentationOptions = new IndentationOptions(context.Document);
            var parentIndentationLevel = this.GetNodeIndentationLevel(indentationOptions, block.Parent);

            var indentationString = this.GenerateIndentationString(parentIndentationLevel, indentationOptions);
            var statementIndentationString = this.GenerateIndentationString(parentIndentationLevel + 1, indentationOptions);

            var openBraceToken = SyntaxFactory.Token(SyntaxKind.OpenBraceToken)
                .WithLeadingTrivia(SyntaxFactory.Whitespace(indentationString))
                .WithTrailingTrivia(SyntaxFactory.CarriageReturnLineFeed);

            var closeBraceToken = SyntaxFactory.Token(SyntaxKind.CloseBraceToken)
                .WithLeadingTrivia(SyntaxFactory.Whitespace(indentationString))
                .WithTrailingTrivia(SyntaxFactory.CarriageReturnLineFeed);

            var statements = SyntaxFactory.List<StatementSyntax>();
            foreach (var statement in block.Statements)
            {
                /* TODO: Take into account all forms of non-whitespace trivia */
                /* TODO: Strip trailing whitespace from the last parent token and all statement tokens */
                /* TODO: Add CRLF to last statement if there is none */
                statements = statements.Add(statement.WithLeadingTrivia(SyntaxFactory.Whitespace(statementIndentationString)));
            }

            return SyntaxFactory.Block(openBraceToken, statements, closeBraceToken);
        }

        private int GetNodeIndentationLevel(IndentationOptions indentationOptions, SyntaxNode node)
        {
            var leadingTrivia = node.GetLeadingTrivia();
            var indentationString = string.Empty;

            for (var i = leadingTrivia.Count - 1; (i >= 0) && leadingTrivia[i].IsKind(SyntaxKind.WhitespaceTrivia); i--)
            {
                indentationString = string.Concat(leadingTrivia[i].ToFullString(), indentationString);
            }

            var indentationCount = indentationString.ToCharArray().Sum(c => this.IndentationAmount(c, indentationOptions));

            return indentationCount / indentationOptions.IndentationSize;
        }

        private int IndentationAmount(char c, IndentationOptions indentationOptions)
        {
            return c == '\t' ? indentationOptions.TabSize : 1;
        }

        private string GenerateIndentationString(int indentationLevel, IndentationOptions indentationOptions)
        {
            string result;
            var indentationCount = indentationLevel * indentationOptions.IndentationSize;

            if (indentationOptions.UseTabs)
            {
                var tabCount = indentationCount / indentationOptions.TabSize;
                var spaceCount = indentationCount % indentationOptions.TabSize;

                result = new string('\t', tabCount) + new string(' ', spaceCount);
            }
            else
            {
                result = new string(' ', indentationCount);
            }

            return result;
        }

        private class IndentationOptions
        {
            public IndentationOptions(Document document)
            {
                var options = document.Project.Solution.Workspace.Options;

                this.IndentationSize = options.GetOption(FormattingOptions.IndentationSize, LanguageNames.CSharp);
                this.TabSize = options.GetOption(FormattingOptions.TabSize, LanguageNames.CSharp);
                this.UseTabs = options.GetOption(FormattingOptions.UseTabs, LanguageNames.CSharp);
            }

            /// <summary>
            /// Gets the indentation size.
            /// </summary>
            public int IndentationSize { get; private set; }

            /// <summary>
            /// Gets the tab size.
            /// </summary>
            public int TabSize { get; private set; }

            /// <summary>
            /// Gets a value indicating whether tabs should be used instead of spaces.
            /// </summary>
            public bool UseTabs { get; private set; }
        }
    }
}

