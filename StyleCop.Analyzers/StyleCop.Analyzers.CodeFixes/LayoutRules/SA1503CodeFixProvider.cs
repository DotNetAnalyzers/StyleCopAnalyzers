﻿// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.LayoutRules
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Composition;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using StyleCop.Analyzers.Helpers;

    /// <summary>
    /// Implements a code fix for <see cref="SA1503BracesMustNotBeOmitted"/>.
    /// </summary>
    /// <remarks>
    /// <para>To fix a violation of this rule, the violating statement will be converted to a block statement.</para>
    /// </remarks>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1503CodeFixProvider))]
    [Shared]
    internal class SA1503CodeFixProvider : CodeFixProvider
    {
        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds { get; } =
            ImmutableArray.Create(SA1503BracesMustNotBeOmitted.DiagnosticId, SA1519BracesMustNotBeOmittedFromMultiLineChildStatement.DiagnosticId, SA1520UseBracesConsistently.DiagnosticId);

        /// <inheritdoc/>
        public override FixAllProvider GetFixAllProvider()
        {
            return FixAll.Instance;
        }

        /// <inheritdoc/>
        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var syntaxRoot = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            foreach (Diagnostic diagnostic in context.Diagnostics)
            {
                if (!(syntaxRoot.FindNode(diagnostic.Location.SourceSpan, false, true) is StatementSyntax node) || node.IsMissing)
                {
                    continue;
                }

                // If the parent of the statement contains a conditional directive, stuff will be really hard to fix correctly, so don't offer a code fix.
                if (ContainsConditionalDirectiveTrivia(node.Parent))
                {
                    continue;
                }

                context.RegisterCodeFix(
                    CodeAction.Create(
                        LayoutResources.SA1503CodeFix,
                        cancellationToken => GetTransformedDocumentAsync(context.Document, syntaxRoot, node, cancellationToken),
                        nameof(SA1503CodeFixProvider)),
                    diagnostic);
            }
        }

        private static Task<Document> GetTransformedDocumentAsync(Document document, SyntaxNode root, StatementSyntax node, CancellationToken cancellationToken)
        {
            // Currently unused
            _ = cancellationToken;

            var newSyntaxRoot = root.ReplaceNode(node, SyntaxFactory.Block(node));
            return Task.FromResult(document.WithSyntaxRoot(newSyntaxRoot));
        }

        private static bool ContainsConditionalDirectiveTrivia(SyntaxNode node)
        {
            for (var currentDirective = node.GetFirstDirective(); currentDirective != null && node.Contains(currentDirective); currentDirective = currentDirective.GetNextDirective())
            {
                switch (currentDirective.Kind())
                {
                case SyntaxKind.IfDirectiveTrivia:
                case SyntaxKind.ElseDirectiveTrivia:
                case SyntaxKind.ElifDirectiveTrivia:
                case SyntaxKind.EndIfDirectiveTrivia:
                    return true;
                }
            }

            return false;
        }

        private class FixAll : DocumentBasedFixAllProvider
        {
            public static FixAllProvider Instance { get; } =
                new FixAll();

            protected override string CodeActionTitle =>
                LayoutResources.SA1503CodeFix;

            protected override async Task<SyntaxNode> FixAllInDocumentAsync(FixAllContext fixAllContext, Document document, ImmutableArray<Diagnostic> diagnostics)
            {
                if (diagnostics.IsEmpty)
                {
                    return null;
                }

                SyntaxNode syntaxRoot = await document.GetSyntaxRootAsync().ConfigureAwait(false);
                List<SyntaxNode> nodesNeedingBlocks = new List<SyntaxNode>(diagnostics.Length);

                foreach (Diagnostic diagnostic in diagnostics)
                {
                    if (!(syntaxRoot.FindNode(diagnostic.Location.SourceSpan, false, true) is StatementSyntax node) || node.IsMissing)
                    {
                        continue;
                    }

                    // If the parent of the statement contains a conditional directive, stuff will be really hard to fix correctly, so don't offer a code fix.
                    if (ContainsConditionalDirectiveTrivia(node.Parent))
                    {
                        continue;
                    }

                    nodesNeedingBlocks.Add(node);
                }

                return syntaxRoot.ReplaceNodes(nodesNeedingBlocks, (originalNode, rewrittenNode) => SyntaxFactory.Block((StatementSyntax)rewrittenNode));
            }
        }
    }
}
