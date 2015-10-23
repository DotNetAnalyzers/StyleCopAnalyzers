// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.MaintainabilityRules
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Helpers;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal sealed class SA1407SA1408FixAllProvider : DocumentBasedFixAllProvider
    {
        protected override string CodeActionTitle => MaintainabilityResources.SA1407SA1408CodeFix;

        protected override async Task<SyntaxNode> FixAllInDocumentAsync(FixAllContext fixAllContext, Document document)
        {
            var diagnostics = await fixAllContext.GetDocumentDiagnosticsAsync(document).ConfigureAwait(false);
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

            return root.ReplaceNodes(nodes, (originalNode, rewrittenNode) => AddParentheses(originalNode, rewrittenNode));
        }

        private static SyntaxNode AddParentheses(SyntaxNode originalNode, SyntaxNode rewrittenNode)
        {
            BinaryExpressionSyntax syntax = rewrittenNode as BinaryExpressionSyntax;
            if (syntax == null)
            {
                return rewrittenNode;
            }

            BinaryExpressionSyntax trimmedSyntax = syntax.WithoutTrivia();

            return SyntaxFactory.ParenthesizedExpression(trimmedSyntax)
                .WithTriviaFrom(syntax)
                .WithoutFormatting();
        }
    }
}
