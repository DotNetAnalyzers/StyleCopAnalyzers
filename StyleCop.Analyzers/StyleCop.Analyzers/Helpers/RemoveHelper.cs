namespace StyleCop.Analyzers.Helpers
{
    using Microsoft.CodeAnalysis;
    
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    internal static class RemoveHelper
    {
        public static async Task<Solution> RemoveSymbolsAsync(Document document, SyntaxNode root, IEnumerable<SyntaxNode> nodeToRemove, SyntaxRemoveOptions removeOptions, CancellationToken cancellationToken)
        {
            var annotatedRoot = root.RemoveNodes(nodeToRemove, removeOptions);
            var annotatedSolution = document.Project.Solution.WithDocumentSyntaxRoot(document.Id, annotatedRoot);
            var annotatedDocument = annotatedSolution.GetDocument(document.Id);
            
            return annotatedSolution;
        }
    }
}
