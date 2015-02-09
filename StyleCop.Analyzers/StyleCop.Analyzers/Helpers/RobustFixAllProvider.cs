namespace StyleCop.Analyzers.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using SpacingRules;

    internal sealed class RobustFixAllProvider : FixAllProvider
    {
        private static readonly SyntaxAnnotation Annotation = new SyntaxAnnotation("StyleCop.Annotations.FixAllAnnotation");

        private string codeActionText;

        private Func<SyntaxNode, SyntaxNode, SyntaxNode> replaceNode;

        public RobustFixAllProvider(string codeActionText, Func<SyntaxNode, SyntaxNode, SyntaxNode> replaceNode)
        {
            if (replaceNode == null)
            {
                throw new ArgumentNullException(nameof(replaceNode));
            }

            this.codeActionText = codeActionText;
            this.replaceNode = replaceNode;
        }

        public override async Task<CodeAction> GetFixAsync(FixAllContext fixAllContext)
        {
            switch (fixAllContext.Scope)
            {
            case FixAllScope.Document:
                var newRoot = await FixAllInDocumentAsync(fixAllContext, fixAllContext.Document);
                return CodeAction.Create(codeActionText, fixAllContext.Document.WithSyntaxRoot(newRoot));

            case FixAllScope.Project:
                Solution solution = await GetProjectFixesAsync(fixAllContext, fixAllContext.Project);
                return CodeAction.Create(codeActionText, solution);

            case FixAllScope.Solution:
                var newSolution = fixAllContext.Solution;
                var projectIds = newSolution.ProjectIds;
                for (int i = 0; i < projectIds.Count; i++)
                {
                    newSolution = await GetProjectFixesAsync(fixAllContext, newSolution.GetProject(projectIds[i]));
                }
                return CodeAction.Create(codeActionText, newSolution);

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
                newDocuments.Add(FixAllInDocumentAsync(fixAllContext, document));
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

            // First annotate all expressions that have to be replaced with a temporary annotation.
            // With this annotation we can find the nodes even if
            // the source span changes.
            foreach (var diagnostic in diagnostics)
            {
                SyntaxNode node = root.FindNode(diagnostic.Location.SourceSpan);
                if (node.IsMissing)
                    continue;

                root = root.ReplaceNode(node, node.WithAdditionalAnnotations(Annotation));
            }

            return root.ReplaceNodes(root.GetAnnotatedNodes(Annotation), (s1, s2) => replaceNode(s1.WithoutAnnotations(Annotation), s2.WithoutAnnotations(Annotation)));
        }
    }
}
