﻿// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.ReadabilityRules
{
    using System.Collections.Immutable;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using StyleCop.Analyzers.Helpers;

    internal sealed class RemoveRegionFixAllProvider : DocumentBasedFixAllProvider
    {
        protected override string CodeActionTitle => "Remove region";

        protected override async Task<SyntaxNode> FixAllInDocumentAsync(FixAllContext fixAllContext, Document document, ImmutableArray<Diagnostic> diagnostics)
        {
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
