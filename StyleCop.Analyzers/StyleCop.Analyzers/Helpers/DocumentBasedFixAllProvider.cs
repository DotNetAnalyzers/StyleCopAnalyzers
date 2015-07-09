namespace StyleCop.Analyzers.Helpers
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;

    /// <summary>
    /// Provides a base class to write a <see cref="FixAllProvider"/> that fixes documents independently.
    /// </summary>
    internal abstract class DocumentBasedFixAllProvider : FixAllProvider
    {
        protected abstract string CodeActionTitle { get; }

        public override async Task<CodeAction> GetFixAsync(FixAllContext fixAllContext)
        {
            switch (fixAllContext.Scope)
            {
            case FixAllScope.Document:
                var newRoot = await this.FixAllInDocumentAsync(fixAllContext, fixAllContext.Document).ConfigureAwait(false);
                return CodeAction.Create(this.CodeActionTitle, token => Task.FromResult(fixAllContext.Document.WithSyntaxRoot(newRoot)));

            case FixAllScope.Project:
                Solution solution = await this.GetProjectFixesAsync(fixAllContext, fixAllContext.Project).ConfigureAwait(false);
                return CodeAction.Create(this.CodeActionTitle, token => Task.FromResult(solution));

            case FixAllScope.Solution:
                var newSolution = fixAllContext.Solution;
                var projectIds = newSolution.ProjectIds;
                for (int i = 0; i < projectIds.Count; i++)
                {
                    newSolution = await this.GetProjectFixesAsync(fixAllContext, newSolution.GetProject(projectIds[i])).ConfigureAwait(false);
                }

                return CodeAction.Create(this.CodeActionTitle, token => Task.FromResult(newSolution));

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

        protected abstract Task<SyntaxNode> FixAllInDocumentAsync(FixAllContext fixAllContext, Document document);
    }
}
