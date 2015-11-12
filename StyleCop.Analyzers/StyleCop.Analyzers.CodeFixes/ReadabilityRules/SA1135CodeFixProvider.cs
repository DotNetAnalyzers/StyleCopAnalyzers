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
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using StyleCop.Analyzers.Helpers;

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1135CodeFixProvider))]
    [Shared]
    internal class SA1135CodeFixProvider : CodeFixProvider
    {
        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds { get; } =
            ImmutableArray.Create(SA1135UsingDirectivesMustBeQualified.DiagnosticId);

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
                        ReadabilityResources.SA1135CodeFix,
                        cancellationToken => GetTransformedDocumentAsync(context.Document, diagnostic, cancellationToken),
                        nameof(SA1135CodeFixProvider)),
                    diagnostic);
            }

            return SpecializedTasks.CompletedTask;
        }

        private static async Task<Document> GetTransformedDocumentAsync(Document document, Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            var syntaxRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            var node = syntaxRoot.FindNode(diagnostic.Location.SourceSpan) as UsingDirectiveSyntax;
            if (node == null)
            {
                return document;
            }

            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);
            var replacementNode = GenerateReplacementNode(semanticModel, node, cancellationToken);
            var newSyntaxRoot = syntaxRoot.ReplaceNode(node, replacementNode);
            return document.WithSyntaxRoot(newSyntaxRoot);
        }

        private static SyntaxNode GenerateReplacementNode(SemanticModel semanticModel, UsingDirectiveSyntax node, CancellationToken cancellationToken)
        {
            SymbolInfo symbolInfo = semanticModel.GetSymbolInfo(node.Name, cancellationToken);
            return node.WithName(SyntaxFactory.ParseName(symbolInfo.Symbol.ToString()));
        }

        private class FixAll : DocumentBasedFixAllProvider
        {
            public static FixAllProvider Instance { get; } =
                   new FixAll();

            protected override string CodeActionTitle =>
                ReadabilityResources.SA1135CodeFix;

            protected override async Task<SyntaxNode> FixAllInDocumentAsync(FixAllContext fixAllContext, Document document)
            {
                var diagnostics = await fixAllContext.GetDocumentDiagnosticsAsync(document).ConfigureAwait(false);
                if (diagnostics.IsEmpty)
                {
                    return null;
                }

                SyntaxNode syntaxRoot = await document.GetSyntaxRootAsync(fixAllContext.CancellationToken).ConfigureAwait(false);
                SemanticModel semanticModel = await document.GetSemanticModelAsync(fixAllContext.CancellationToken).ConfigureAwait(false);

                var replaceMap = new Dictionary<SyntaxNode, SyntaxNode>();

                foreach (Diagnostic diagnostic in diagnostics)
                {
                    var node = syntaxRoot.FindNode(diagnostic.Location.SourceSpan, false, true) as UsingDirectiveSyntax;
                    if (node == null || node.IsMissing)
                    {
                        continue;
                    }

                    replaceMap[node] = GenerateReplacementNode(semanticModel, node, fixAllContext.CancellationToken);
                }

                return syntaxRoot.ReplaceNodes(replaceMap.Keys, (originalNode, rewrittenNode) => replaceMap[originalNode]);
            }
        }
    }
}