// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.MaintainabilityRules
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using StyleCop.Analyzers.Helpers;

    internal sealed class SA1407SA1408FixAllProvider : DocumentBasedFixAllProvider
    {
        protected override string CodeActionTitle => MaintainabilityResources.SA1407SA1408CodeFix;

        protected override async Task<SyntaxNode> FixAllInDocumentAsync(FixAllContext fixAllContext, Document document, ImmutableArray<Diagnostic> diagnostics)
        {
            if (diagnostics.IsEmpty)
            {
                return null;
            }

            var root = await document.GetSyntaxRootAsync(fixAllContext.CancellationToken).ConfigureAwait(false);

            List<SyntaxNode> nodes = new List<SyntaxNode>();
            foreach (var diagnostic in diagnostics)
            {
                SyntaxNode node = root.FindNode(diagnostic.Location.SourceSpan);
                if (node.IsMissing)
                {
                    continue;
                }

                nodes.Add(node);
            }

            return root.ReplaceNodes(nodes, (originalNode, rewrittenNode) => AddParentheses(rewrittenNode));
        }

        private static SyntaxNode AddParentheses(SyntaxNode node)
        {
            if (!(node is BinaryExpressionSyntax syntax))
            {
                return node;
            }

            BinaryExpressionSyntax trimmedSyntax = syntax.WithoutTrivia();

            return SyntaxFactory.ParenthesizedExpression(trimmedSyntax)
                .WithTriviaFrom(syntax)
                .WithoutFormatting();
        }
    }
}
