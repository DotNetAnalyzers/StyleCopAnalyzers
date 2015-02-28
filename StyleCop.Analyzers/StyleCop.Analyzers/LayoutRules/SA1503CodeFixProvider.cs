namespace StyleCop.Analyzers.LayoutRules
{
    using System.Composition;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Collections.Immutable;

    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// Implements a code fix for <see cref="SA1503CurlyBracketsMustNotBeOmitted"/>.
    /// </summary>
    /// <remarks>
    /// <para>To fix a violation of this rule, the violating statement will be converted to a block statement.</para>
    /// </remarks>
    [ExportCodeFixProvider(nameof(SA1503CodeFixProvider), LanguageNames.CSharp)]
    [Shared]
    public class SA1503CodeFixProvider : CodeFixProvider
    {
        private static readonly ImmutableArray<string> FixableDiagnostics =
            ImmutableArray.Create(SA1503CurlyBracketsMustNotBeOmitted.DiagnosticId);

        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return FixableDiagnostics; }
        }

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
                var node = syntaxRoot.FindNode(diagnostic.Location.SourceSpan, true, true);
                if (!node.IsMissing)
                {
                    SyntaxNode newSyntaxRoot = null;

                    switch (node.Parent.Kind())
                    {
                        case SyntaxKind.IfStatement:
                            newSyntaxRoot = this.FixIfStatement(syntaxRoot, node.Parent);
                            break;
                        case SyntaxKind.ElseClause:
                            newSyntaxRoot = this.FixElseStatement(syntaxRoot, node.Parent);
                            break;
                        case SyntaxKind.WhileStatement:
                            newSyntaxRoot = this.FixWhileStatement(syntaxRoot, node.Parent);
                            break;
                        case SyntaxKind.ForStatement:
                            newSyntaxRoot = this.FixForStatement(syntaxRoot, node.Parent);
                            break;
                        case SyntaxKind.ForEachStatement:
                            newSyntaxRoot = this.FixForEachStatement(syntaxRoot, node.Parent);
                            break;
                    }

                    if (newSyntaxRoot != null)
                    {
                        var newDocument = context.Document.WithSyntaxRoot(newSyntaxRoot);
                        context.RegisterCodeFix(CodeAction.Create("Wrap with curly brackets", token => Task.FromResult(newDocument)), diagnostic);
                    }
                }
            }
        }

        private SyntaxNode FixIfStatement(SyntaxNode root, SyntaxNode parent)
        {
            var ifStatement = (IfStatementSyntax)parent;
            return this.ReplaceStatementWithWrappedStatement(root, ifStatement, ifStatement.CloseParenToken, ifStatement.Statement);
        }

        private SyntaxNode FixElseStatement(SyntaxNode root, SyntaxNode parent)
        {
            var elseStatement = (ElseClauseSyntax)parent;
            return this.ReplaceStatementWithWrappedStatement(root, elseStatement, elseStatement.ElseKeyword, elseStatement.Statement);
        }

        private SyntaxNode FixWhileStatement(SyntaxNode root, SyntaxNode parent)
        {
            var whileStatement = (WhileStatementSyntax)parent;
            return this.ReplaceStatementWithWrappedStatement(root, whileStatement, whileStatement.CloseParenToken, whileStatement.Statement);
        }

        private SyntaxNode FixForStatement(SyntaxNode root, SyntaxNode parent)
        {
            var forStatement = (ForStatementSyntax)parent;
            return this.ReplaceStatementWithWrappedStatement(root, forStatement, forStatement.CloseParenToken, forStatement.Statement);
        }

        private SyntaxNode FixForEachStatement(SyntaxNode root, SyntaxNode parent)
        {
            var foreachStatement = (ForEachStatementSyntax)parent;
            return this.ReplaceStatementWithWrappedStatement(root, foreachStatement, foreachStatement.CloseParenToken, foreachStatement.Statement);
        }

        private SyntaxNode ReplaceStatementWithWrappedStatement(SyntaxNode root, SyntaxNode parent, SyntaxToken parentEndToken, StatementSyntax statement)
        {
            var newStatement = SyntaxFactory.Block(statement);
            return root.ReplaceNode(statement, newStatement);
        }
    }
}
