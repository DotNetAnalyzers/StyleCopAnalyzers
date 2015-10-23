// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.LayoutRules
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Composition;
    using System.Threading;
    using System.Threading.Tasks;
    using Helpers;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// Implements a code fix for <see cref="SA1516ElementsMustBeSeparatedByBlankLine"/>.
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1516CodeFixProvider))]
    [Shared]
    internal class SA1516CodeFixProvider : CodeFixProvider
    {
        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds { get; } =
            ImmutableArray.Create(SA1516ElementsMustBeSeparatedByBlankLine.DiagnosticId);

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
                context.RegisterCodeFix(
                    CodeAction.Create(
                        LayoutResources.SA1516CodeFix,
                        cancellationToken => GetTransformedDocumentAsync(context.Document, syntaxRoot, diagnostic, context.CancellationToken),
                        nameof(SA1516CodeFixProvider)),
                    diagnostic);
            }
        }

        private static Task<Document> GetTransformedDocumentAsync(Document document, SyntaxNode syntaxRoot, Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            var node = syntaxRoot.FindNode(diagnostic.Location.SourceSpan, getInnermostNodeForTie: true);
            node = GetRelevantNode(node);

            if (node == null)
            {
                return Task.FromResult(document);
            }

            var leadingTrivia = node.GetLeadingTrivia();

            var newTriviaList = leadingTrivia;
            newTriviaList = newTriviaList.Insert(0, SyntaxFactory.CarriageReturnLineFeed);

            var newNode = node.WithLeadingTrivia(newTriviaList);
            var newSyntaxRoot = syntaxRoot.ReplaceNode(node, newNode);
            var newDocument = document.WithSyntaxRoot(newSyntaxRoot);

            return Task.FromResult(newDocument);
        }

        private static SyntaxNode GetRelevantNode(SyntaxNode innerNode)
        {
            SyntaxNode currentNode = innerNode;
            while (currentNode != null)
            {
                if (currentNode is BaseTypeDeclarationSyntax)
                {
                    return currentNode;
                }

                if (currentNode is NamespaceDeclarationSyntax)
                {
                    return currentNode;
                }

                if (currentNode is UsingDirectiveSyntax)
                {
                    return currentNode;
                }

                if (currentNode is MemberDeclarationSyntax)
                {
                    return currentNode;
                }

                if (currentNode is AccessorDeclarationSyntax)
                {
                    return currentNode;
                }

                currentNode = currentNode.Parent;
            }

            return null;
        }

        private class FixAll : DocumentBasedFixAllProvider
        {
            public static FixAllProvider Instance { get; } =
                new FixAll();

            protected override string CodeActionTitle =>
                LayoutResources.SA1516CodeFix;

            protected override async Task<SyntaxNode> FixAllInDocumentAsync(FixAllContext fixAllContext, Document document)
            {
                var diagnostics = await fixAllContext.GetDocumentDiagnosticsAsync(document).ConfigureAwait(false);
                if (diagnostics.IsEmpty)
                {
                    return null;
                }

                var syntaxRoot = await document.GetSyntaxRootAsync().ConfigureAwait(false);

                List<SyntaxNode> nodes = new List<SyntaxNode>();

                foreach (var diagnostic in diagnostics)
                {
                    var node = syntaxRoot.FindNode(diagnostic.Location.SourceSpan, getInnermostNodeForTie: true);
                    node = GetRelevantNode(node);

                    if (node != null)
                    {
                        nodes.Add(node);
                    }
                }

                return syntaxRoot.ReplaceNodes(nodes, (oldNode, newNode) =>
                {
                    var newTriviaList = newNode.GetLeadingTrivia();
                    newTriviaList = newTriviaList.Insert(0, SyntaxFactory.CarriageReturnLineFeed);

                    return newNode.WithLeadingTrivia(newTriviaList);
                });
            }
        }
    }
}
