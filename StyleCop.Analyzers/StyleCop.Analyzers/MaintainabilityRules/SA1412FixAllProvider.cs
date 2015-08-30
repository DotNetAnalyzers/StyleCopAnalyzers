namespace StyleCop.Analyzers.MaintainabilityRules
{
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;

    internal sealed class SA1412FixAllProvider : FixAllProvider
    {
        public override async Task<CodeAction> GetFixAsync(FixAllContext fixAllContext)
        {
            Solution newSolution;

            switch (fixAllContext.Scope)
            {
            case FixAllScope.Document:
                newSolution = await FixDocumentAsync(fixAllContext, fixAllContext.Document).ConfigureAwait(false);
                break;

            case FixAllScope.Project:
                newSolution = await GetProjectFixesAsync(fixAllContext, fixAllContext.Project).ConfigureAwait(false);
                break;

            case FixAllScope.Solution:
                newSolution = fixAllContext.Solution;
                var projectIds = newSolution.ProjectIds;
                for (int i = 0; i < projectIds.Count; i++)
                {
                    newSolution = await GetProjectFixesAsync(fixAllContext, newSolution.GetProject(projectIds[i])).ConfigureAwait(false);
                }

                break;

            case FixAllScope.Custom:
            default:
                return null;
            }

            return CodeAction.Create(string.Format(MaintainabilityResources.SA1412CodeFix, await SA1412CodeFixProvider.GetEncodingNameForDocumentAsync(fixAllContext.Document).ConfigureAwait(false)), token => Task.FromResult(newSolution));
        }

        private static async Task<Solution> FixDocumentAsync(FixAllContext fixAllContext, Document document)
        {
            Solution solution = document.Project.Solution;
            var diagnostics = await fixAllContext.GetDocumentDiagnosticsAsync(document).ConfigureAwait(false);

            if (diagnostics.Length == 0 || fixAllContext.CodeActionEquivalenceKey != await SA1412CodeFixProvider.GetEquivalenceKeyForDocumentAsync(document).ConfigureAwait(false))
            {
                return solution;
            }

            return await SA1412CodeFixProvider.GetTransformedSolutionAsync(document).ConfigureAwait(false);
        }

        private static async Task<Solution> GetProjectFixesAsync(FixAllContext fixAllContext, Project project)
        {
            Solution solution = project.Solution;

            var documentIds = project.DocumentIds;

            foreach (var documentId in documentIds)
            {
                solution = await FixDocumentAsync(fixAllContext, solution.GetDocument(documentId)).ConfigureAwait(false);
            }

            return solution;
        }
    }
}
