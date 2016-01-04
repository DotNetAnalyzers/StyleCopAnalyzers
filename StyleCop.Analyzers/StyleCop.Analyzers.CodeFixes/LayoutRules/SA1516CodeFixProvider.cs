// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.LayoutRules
{
    using System;
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
                var insertBlankLine = DetermineCodeFixAction(diagnostic);
                if (insertBlankLine == null)
                {
                    continue;
                }

                context.RegisterCodeFix(
                    CodeAction.Create(
                        insertBlankLine.Value ? LayoutResources.SA1516CodeFixInsert : LayoutResources.SA1516CodeFixRemove,
                        cancellationToken => GetTransformedDocumentAsync(context.Document, syntaxRoot, diagnostic, insertBlankLine.Value, context.CancellationToken),
                        nameof(SA1516CodeFixProvider)),
                    diagnostic);
            }
        }

        private static bool? DetermineCodeFixAction(Diagnostic diagnostic)
        {
            string codeFixAction;

            if (!diagnostic.Properties.TryGetValue(SA1516ElementsMustBeSeparatedByBlankLine.CodeFixActionKey, out codeFixAction))
            {
                return null;
            }

            switch (codeFixAction)
            {
                case SA1516ElementsMustBeSeparatedByBlankLine.InsertBlankLineValue:
                    return true;

                case SA1516ElementsMustBeSeparatedByBlankLine.RemoveBlankLinesValue:
                    return false;

                default:
                    return null;
            }
        }

        private static Task<Document> GetTransformedDocumentAsync(Document document, SyntaxNode syntaxRoot, Diagnostic diagnostic, bool insertBlankLine, CancellationToken cancellationToken)
        {
            var node = syntaxRoot.FindNode(diagnostic.Location.SourceSpan, getInnermostNodeForTie: true);
            node = GetRelevantNode(node);

            if (node == null)
            {
                return Task.FromResult(document);
            }

            SyntaxNode newNode = ProcessNode(node, insertBlankLine);
            var newSyntaxRoot = syntaxRoot.ReplaceNode(node, newNode);
            var newDocument = document.WithSyntaxRoot(newSyntaxRoot);

            return Task.FromResult(newDocument);
        }

        private static SyntaxNode ProcessNode(SyntaxNode node, bool insertBlankLine)
        {
            var leadingTrivia = node.GetLeadingTrivia();
            SyntaxTriviaList newLeadingTrivia;

            if (insertBlankLine)
            {
                newLeadingTrivia = leadingTrivia.Insert(0, SyntaxFactory.CarriageReturnLineFeed);
            }
            else
            {
                newLeadingTrivia = leadingTrivia.WithoutBlankLines();
            }

            return node.WithLeadingTrivia(newLeadingTrivia);
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
                LayoutResources.SA1516CodeFixAll;

            protected override async Task<SyntaxNode> FixAllInDocumentAsync(FixAllContext fixAllContext, Document document, ImmutableArray<Diagnostic> diagnostics)
            {
                if (diagnostics.IsEmpty)
                {
                    return null;
                }

                var syntaxRoot = await document.GetSyntaxRootAsync().ConfigureAwait(false);

                Dictionary<SyntaxNode, SyntaxNode> replaceMap = new Dictionary<SyntaxNode, SyntaxNode>();

                foreach (var diagnostic in diagnostics)
                {
                    var insertBlankLine = DetermineCodeFixAction(diagnostic);
                    if (insertBlankLine == null)
                    {
                        continue;
                    }

                    var node = syntaxRoot.FindNode(diagnostic.Location.SourceSpan, getInnermostNodeForTie: true);
                    node = GetRelevantNode(node);

                    if (node != null)
                    {
                        replaceMap[node] = ProcessNode(node, insertBlankLine.Value);
                    }
                }

                return syntaxRoot.ReplaceNodes(replaceMap.Keys, (original, rewritten) => replaceMap[original]);
            }
        }
    }
}
