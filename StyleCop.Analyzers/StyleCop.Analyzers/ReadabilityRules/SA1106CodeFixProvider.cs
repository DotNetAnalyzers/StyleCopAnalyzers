namespace StyleCop.Analyzers.ReadabilityRules
{
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
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using StyleCop.Analyzers.Helpers;
    using StyleCop.Analyzers.SpacingRules;

    /// <summary>
    /// This class provides a code fix for <see cref="SA1106CodeMustNotContainEmptyStatements"/>.
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1106CodeFixProvider))]
    [Shared]
    public class SA1106CodeFixProvider : CodeFixProvider
    {
        private static readonly ImmutableArray<string> FixableDiagnostics =
            ImmutableArray.Create(SA1106CodeMustNotContainEmptyStatements.DiagnosticId);

        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds => FixableDiagnostics;

        /// <inheritdoc/>
        public override FixAllProvider GetFixAllProvider()
        {
            return CustomFixAllProviders.BatchFixer;
        }

        /// <inheritdoc/>
        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            foreach (var diagnostic in context.Diagnostics)
            {
                if (!diagnostic.Id.Equals(SA1106CodeMustNotContainEmptyStatements.DiagnosticId))
                {
                    continue;
                }

                var node = root?.FindNode(diagnostic.Location.SourceSpan);
                var emptyStatement = node as EmptyStatementSyntax;
                if (emptyStatement != null)
                {
                    context.RegisterCodeFix(CodeAction.Create(ReadabilityResources.SA1106CodeFix, token => RemoveEmptyStatementAsync(context.Document, root, emptyStatement), nameof(SA1106CodeFixProvider)), diagnostic);
                    continue;
                }

                var typeDeclaration = node as TypeDeclarationSyntax;
                if (typeDeclaration != null && typeDeclaration.SemicolonToken != null)
                {
                    context.RegisterCodeFix(CodeAction.Create(ReadabilityResources.SA1106CodeFix, token => RemoveSemicolonFromTypeAsync(context.Document, root, typeDeclaration), nameof(SA1106CodeFixProvider)), diagnostic);
                }
            }
        }

        private static Task<Document> RemoveSemicolonFromTypeAsync(Document document, SyntaxNode root, TypeDeclarationSyntax typeDeclaration)
        {
            return Task.FromResult(document.WithSyntaxRoot(root.ReplaceNode(typeDeclaration, typeDeclaration.WithoutTrailingTrivia())));
        }

        private static Task<Document> RemoveEmptyStatementAsync(Document document, SyntaxNode root, EmptyStatementSyntax node)
        {
            var newRoot = root;

            if (node.Parent.IsKind(SyntaxKind.Block))
            {
                newRoot = root.RemoveNode(node, SyntaxRemoveOptions.KeepNoTrivia);
            }
            else if (node.Parent.IsKind(SyntaxKind.ForStatement))
            {
                var forNode = (ForStatementSyntax)node.Parent;
                newRoot = root.ReplaceNode(forNode, forNode.WithStatement(SyntaxFactory.Block()));
            }
            else if (node.Parent.IsKind(SyntaxKind.LabeledStatement))
            {
                var labeledStatement = (LabeledStatementSyntax)node.Parent;
                var parentBlock = labeledStatement.Parent as BlockSyntax;
                var index = parentBlock.Statements.IndexOf(labeledStatement);
                var newStatements = parentBlock.Statements.Take(index)
                    .Concat(new[] { labeledStatement.WithStatement(parentBlock.Statements.Skip(index + 1).FirstOrDefault()) })
                    .Concat(parentBlock.Statements.Skip(index + 2));
                var newParentBlock = parentBlock.WithStatements(SyntaxFactory.List(newStatements));
                newRoot = root.ReplaceNode(parentBlock, newParentBlock);
            }

            return Task.FromResult(document.WithSyntaxRoot(newRoot));
        }
    }
}