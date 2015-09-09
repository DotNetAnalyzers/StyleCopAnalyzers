// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

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

            return CodeAction.Create(
                string.Format(MaintainabilityResources.SA1412CodeFix, fixAllContext.CodeActionEquivalenceKey.Substring(fixAllContext.CodeActionEquivalenceKey.IndexOf('.') + 1)),
                token => Task.FromResult(newSolution));
        }

        private static async Task<Solution> FixDocumentAsync(FixAllContext fixAllContext, Document document)
        {
            Solution solution = document.Project.Solution;
            var diagnostics = await fixAllContext.GetDocumentDiagnosticsAsync(document).ConfigureAwait(false);
            if (diagnostics.Length == 0)
            {
                return solution;
            }

            string equivalenceKey = nameof(SA1412CodeFixProvider) + "." + diagnostics[0].Properties[SA1412StoreFilesAsUtf8.EncodingProperty];
            if (fixAllContext.CodeActionEquivalenceKey != equivalenceKey)
            {
                return solution;
            }

            return await SA1412CodeFixProvider.GetTransformedSolutionAsync(document, fixAllContext.CancellationToken).ConfigureAwait(false);
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
