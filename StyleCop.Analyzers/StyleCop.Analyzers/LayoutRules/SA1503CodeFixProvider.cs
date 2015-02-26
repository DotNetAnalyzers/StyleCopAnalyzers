using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Immutable;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using System;

namespace StyleCop.Analyzers.LayoutRules
{
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
            return this.ReplaceStatementWithWrappedStatement(root, parent, ((IfStatementSyntax)parent).Statement);
        }

        private SyntaxNode FixElseStatement(SyntaxNode root, SyntaxNode parent)
        {
            return this.ReplaceStatementWithWrappedStatement(root, parent, ((ElseClauseSyntax)parent).Statement);
        }

        private SyntaxNode FixWhileStatement(SyntaxNode root, SyntaxNode parent)
        {
            return this.ReplaceStatementWithWrappedStatement(root, parent, ((WhileStatementSyntax)parent).Statement);
        }

        private SyntaxNode FixForStatement(SyntaxNode root, SyntaxNode parent)
        {
            return this.ReplaceStatementWithWrappedStatement(root, parent, ((ForStatementSyntax)parent).Statement);
        }

        private SyntaxNode FixForEachStatement(SyntaxNode root, SyntaxNode parent)
        {
            return this.ReplaceStatementWithWrappedStatement(root, parent, ((ForEachStatementSyntax)parent).Statement);
        }

        private SyntaxNode ReplaceStatementWithWrappedStatement(SyntaxNode root, SyntaxNode parent, StatementSyntax statement)
        {
            SyntaxTriviaList parentLeadingTrivia = parent.GetLeadingTrivia();

            var newOpenBraceToken = SyntaxFactory.Token(parentLeadingTrivia, SyntaxKind.OpenBraceToken, SyntaxFactory.TriviaList(SyntaxFactory.CarriageReturnLineFeed));
            var newCloseBraceToken = SyntaxFactory.Token(parentLeadingTrivia, SyntaxKind.CloseBraceToken, SyntaxFactory.TriviaList(SyntaxFactory.CarriageReturnLineFeed));
            var newStatementList = new SyntaxList<StatementSyntax>().Add(statement);

            var newStatement = SyntaxFactory.Block(newOpenBraceToken, newStatementList, newCloseBraceToken);

            return root.ReplaceNode(statement, newStatement);
        }
    }
}
