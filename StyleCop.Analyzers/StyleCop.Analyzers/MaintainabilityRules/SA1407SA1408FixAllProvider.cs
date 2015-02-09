namespace StyleCop.Analyzers.MaintainabilityRules
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using SpacingRules;

    internal sealed class SA1407SA1408FixAllProvider : FixAllProvider
    {
        private static readonly SyntaxAnnotation NeedsParenthesisAnnotation = new SyntaxAnnotation("StyleCop.NeedsParenthesis");

        public override async Task<CodeAction> GetFixAsync(FixAllContext fixAllContext)
        {
            switch (fixAllContext.Scope)
            {
            case FixAllScope.Document:
                var newRoot = await this.FixAllInDocumentAsync(fixAllContext, fixAllContext.Document);
                return CodeAction.Create("Add parentheses", fixAllContext.Document.WithSyntaxRoot(newRoot));

            case FixAllScope.Project:
                Solution solution = await this.GetProjectFixesAsync(fixAllContext, fixAllContext.Project);
                return CodeAction.Create("Add parentheses", solution);

            case FixAllScope.Solution:
                var newSolution = fixAllContext.Solution;
                var projectIds = newSolution.ProjectIds;
                for (int i = 0; i < projectIds.Count; i++)
                {
                    newSolution = await this.GetProjectFixesAsync(fixAllContext, newSolution.GetProject(projectIds[i]));
                }
                return CodeAction.Create("Add parentheses", newSolution);

            case FixAllScope.Custom:
            default:
                return null;
            }
        }

        private async Task<Solution> GetProjectFixesAsync(FixAllContext fixAllContext, Project project)
        {
            Solution solution = project.Solution;
            var oldDocuments = project.Documents.AsImmutable();
            List<Task<SyntaxNode>> newDocuments = new List<Task<SyntaxNode>>(oldDocuments.Length);
            foreach (var document in oldDocuments)
            {
                newDocuments.Add(this.FixAllInDocumentAsync(fixAllContext, document));
            }
            for (int i = 0; i < oldDocuments.Length; i++)
            {
                var newDocumentRoot = await newDocuments[i];
                solution = solution.WithDocumentSyntaxRoot(oldDocuments[i].Id, newDocumentRoot);
            }

            return solution;
        }

        private async Task<SyntaxNode> FixAllInDocumentAsync(FixAllContext fixAllContext, Document document)
        {
            var diagnostics = await fixAllContext.GetDiagnosticsAsync(document);

            var newDocument = document;

            var root = await newDocument.GetSyntaxRootAsync(fixAllContext.CancellationToken).ConfigureAwait(false);

            // First annotate all expressions that need parenthesis with a temporary annotation.
            // With this annotation we can find the nodes that need parenthesis even if
            // the source span changes.
            foreach (var diagnostic in diagnostics)
            {
                SyntaxNode node = root.FindNode(diagnostic.Location.SourceSpan);
                if (node.IsMissing)
                    continue;

                root = root.ReplaceNode(node, node.WithAdditionalAnnotations(NeedsParenthesisAnnotation));
            }

            return root.ReplaceNodes(root.GetAnnotatedNodes(NeedsParenthesisAnnotation), this.AddParentheses);
        }

        private SyntaxNode AddParentheses(SyntaxNode originalNode, SyntaxNode rewrittenNode)
        {
            BinaryExpressionSyntax syntax = rewrittenNode as BinaryExpressionSyntax;
            if (syntax == null)
                return rewrittenNode;

            BinaryExpressionSyntax trimmedSyntax = syntax
                .WithoutTrivia()
                .WithoutAnnotations(NeedsParenthesisAnnotation.Kind);

            return SyntaxFactory.ParenthesizedExpression(trimmedSyntax)
                .WithTriviaFrom(syntax)
                .WithoutFormatting();
        }
    }
}
