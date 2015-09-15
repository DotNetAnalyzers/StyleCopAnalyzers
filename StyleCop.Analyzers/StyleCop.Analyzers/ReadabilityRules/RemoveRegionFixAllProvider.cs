// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.ReadabilityRules
{
    using System.Linq;
    using System.Threading.Tasks;
    using Helpers;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal sealed class RemoveRegionFixAllProvider : DocumentBasedFixAllProvider
    {
        protected override string CodeActionTitle => "Remove region";

        protected override async Task<SyntaxNode> FixAllInDocumentAsync(FixAllContext fixAllContext, Document document)
        {
            var diagnostics = await fixAllContext.GetDocumentDiagnosticsAsync(document).ConfigureAwait(false);
            if (diagnostics.IsEmpty)
            {
                return null;
            }

            SyntaxNode root = await document.GetSyntaxRootAsync().ConfigureAwait(false);

            var nodesToRemove = diagnostics.Select(d => root.FindNode(d.Location.SourceSpan, findInsideTrivia: true))
                .Where(node => node != null && !node.IsMissing)
                .OfType<RegionDirectiveTriviaSyntax>()
                .SelectMany(node => node.GetRelatedDirectives())
                .Where(node => !node.IsMissing);

            return root.RemoveNodes(nodesToRemove, SyntaxRemoveOptions.AddElasticMarker);
        }
    }
}
