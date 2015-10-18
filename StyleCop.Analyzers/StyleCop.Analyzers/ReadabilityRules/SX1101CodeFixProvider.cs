// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.ReadabilityRules
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Composition;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using StyleCop.Analyzers.Helpers;

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SX1101CodeFixProvider))]
    [Shared]
    internal class SX1101CodeFixProvider : CodeFixProvider
    {
        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds { get; } =
            ImmutableArray.Create(SX1101DoNotPrefixLocalMembersWithThis.DiagnosticId);

        /// <inheritdoc/>
        public override FixAllProvider GetFixAllProvider()
        {
            return FixAll.Instance;
        }

        /// <inheritdoc/>
        public override Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            foreach (var diagnostic in context.Diagnostics)
            {
                context.RegisterCodeFix(
                    CodeAction.Create(
                        ReadabilityResources.SX1101CodeFix,
                        cancellationToken => GetTransformedDocumentAsync(context.Document, diagnostic, cancellationToken),
                        nameof(SX1101CodeFixProvider)),
                    diagnostic);
            }

            return SpecializedTasks.CompletedTask;
        }

        private static async Task<Document> GetTransformedDocumentAsync(Document document, Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            var syntaxRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            var node = syntaxRoot.FindNode(diagnostic.Location.SourceSpan, getInnermostNodeForTie: true) as ThisExpressionSyntax;
            if (node == null)
            {
                return document;
            }

            var replacementNode = GenerateReplacementNode(node);
            var newSyntaxRoot = syntaxRoot.ReplaceNode(node.Parent, replacementNode);
            return document.WithSyntaxRoot(newSyntaxRoot);
        }

        private static SyntaxNode GenerateReplacementNode(ThisExpressionSyntax node)
        {
            var parent = (MemberAccessExpressionSyntax)node.Parent;
            return parent.Name.WithTriviaFrom(parent);
        }

        private class FixAll : DocumentBasedFixAllProvider
        {
            public static FixAllProvider Instance { get; } =
                   new FixAll();

            protected override string CodeActionTitle =>
                ReadabilityResources.SX1101CodeFix;

            protected override async Task<SyntaxNode> FixAllInDocumentAsync(FixAllContext fixAllContext, Document document)
            {
                var diagnostics = await fixAllContext.GetDocumentDiagnosticsAsync(document).ConfigureAwait(false);
                if (diagnostics.IsEmpty)
                {
                    return null;
                }

                SyntaxNode syntaxRoot = await document.GetSyntaxRootAsync(fixAllContext.CancellationToken).ConfigureAwait(false);

                var replaceMap = new Dictionary<SyntaxNode, SyntaxNode>();

                foreach (Diagnostic diagnostic in diagnostics)
                {
                    var node = syntaxRoot.FindNode(diagnostic.Location.SourceSpan, false, true) as ThisExpressionSyntax;
                    if (node == null || node.IsMissing)
                    {
                        continue;
                    }

                    replaceMap[node.Parent] = GenerateReplacementNode(node);
                }

                return syntaxRoot.ReplaceNodes(replaceMap.Keys, (originalNode, rewrittenNode) => replaceMap[originalNode]);
            }
        }
    }
}
