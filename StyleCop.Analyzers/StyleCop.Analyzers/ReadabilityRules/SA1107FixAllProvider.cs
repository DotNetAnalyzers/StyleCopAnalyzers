namespace StyleCop.Analyzers.ReadabilityRules
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Editing;

    internal class SA1107FixAllProvider : FixAllProvider
    {
        public override async Task<CodeAction> GetFixAsync(FixAllContext fixAllContext)
        {
            switch (fixAllContext.Scope)
            {
                case FixAllScope.Document:
                    var newRoot = await this.FixAllInDocumentAsync(fixAllContext, fixAllContext.Document).ConfigureAwait(false);
                    return CodeAction.Create(ReadabilityResources.SA1107CodeFix, token => Task.FromResult(fixAllContext.Document.WithSyntaxRoot(newRoot)));

                case FixAllScope.Project:
                    Solution solution = await this.GetProjectFixesAsync(fixAllContext, fixAllContext.Project).ConfigureAwait(false);
                    return CodeAction.Create(ReadabilityResources.SA1107CodeFix, token => Task.FromResult(solution));

                case FixAllScope.Solution:
                    var newSolution = fixAllContext.Solution;
                    var projectIds = newSolution.ProjectIds;
                    for (int i = 0; i < projectIds.Count; i++)
                    {
                        newSolution = await this.GetProjectFixesAsync(fixAllContext, newSolution.GetProject(projectIds[i])).ConfigureAwait(false);
                    }

                    return CodeAction.Create(ReadabilityResources.SA1107CodeFix, token => Task.FromResult(newSolution));

                case FixAllScope.Custom:
                default:
                    return null;
            }
        }

        private async Task<Solution> GetProjectFixesAsync(FixAllContext fixAllContext, Project project)
        {
            Solution solution = project.Solution;
            var oldDocuments = project.Documents.ToImmutableArray();
            List<Task<SyntaxNode>> newDocuments = new List<Task<SyntaxNode>>(oldDocuments.Length);
            foreach (var document in oldDocuments)
            {
                newDocuments.Add(this.FixAllInDocumentAsync(fixAllContext, document));
            }

            for (int i = 0; i < oldDocuments.Length; i++)
            {
                var newDocumentRoot = await newDocuments[i].ConfigureAwait(false);
                solution = solution.WithDocumentSyntaxRoot(oldDocuments[i].Id, newDocumentRoot);
            }

            return solution;
        }

        private async Task<SyntaxNode> FixAllInDocumentAsync(FixAllContext fixAllContext, Document document)
        {
            var diagnostics = await fixAllContext.GetDocumentDiagnosticsAsync(document).ConfigureAwait(false);

            DocumentEditor editor = await DocumentEditor.CreateAsync(document, fixAllContext.CancellationToken).ConfigureAwait(false);

            SyntaxNode root = editor.GetChangedRoot();

            ImmutableList<SyntaxNode> nodesToChange = ImmutableList.Create<SyntaxNode>();

            // Make sure all nodes we care about are tracked
            foreach (var diagnostic in await fixAllContext.GetDocumentDiagnosticsAsync(document).ConfigureAwait(false))
            {
                var location = diagnostic.Location;
                var syntaxNode = root.FindNode(location.SourceSpan);
                if (syntaxNode != null)
                {
                    editor.TrackNode(syntaxNode);
                    nodesToChange = nodesToChange.Add(syntaxNode);
                }
            }

            foreach (var node in nodesToChange)
            {
                editor.ReplaceNode(node, node.WithLeadingTrivia(SyntaxFactory.ElasticCarriageReturnLineFeed));
            }

            return editor.GetChangedRoot();
        }
    }
}
