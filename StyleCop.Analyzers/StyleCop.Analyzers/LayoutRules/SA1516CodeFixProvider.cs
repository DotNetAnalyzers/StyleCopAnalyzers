﻿namespace StyleCop.Analyzers.LayoutRules
{
    using System.Collections.Immutable;
    using System.Composition;
    using System.Linq;
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
    public class SA1516CodeFixProvider : CodeFixProvider
    {
        private static readonly ImmutableArray<string> FixableDiagnostics =
            ImmutableArray.Create(SA1516ElementsMustBeSeparatedByBlankLine.DiagnosticId);

        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds => FixableDiagnostics;

        /// <inheritdoc/>
        public override FixAllProvider GetFixAllProvider()
        {
            return CustomFixAllProviders.BatchFixer;
        }

        /// <inheritdoc/>
        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            foreach (Diagnostic diagnostic in context.Diagnostics.Where(d => FixableDiagnostics.Contains(d.Id)))
            {
                var syntaxRoot = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

                var node = syntaxRoot.FindNode(diagnostic.Location.SourceSpan, getInnermostNodeForTie: true);
                node = this.GetRelevantNode(node);
                var leadingTrivia = node?.GetLeadingTrivia();
                if (leadingTrivia != null)
                {
                    context.RegisterCodeFix(CodeAction.Create(LayoutResources.SA1516CodeFix, token => GetTransformedDocumentAsync(context, syntaxRoot, node, (SyntaxTriviaList)leadingTrivia), equivalenceKey: nameof(SA1516CodeFixProvider)), diagnostic);
                }
            }
        }

        private static Task<Document> GetTransformedDocumentAsync(CodeFixContext context, SyntaxNode syntaxRoot, SyntaxNode node, SyntaxTriviaList leadingTrivia)
        {
            var newTriviaList = leadingTrivia;
            newTriviaList = newTriviaList.Insert(0, SyntaxFactory.CarriageReturnLineFeed);

            var newNode = node.WithLeadingTrivia(newTriviaList);
            var newSyntaxRoot = syntaxRoot.ReplaceNode(node, newNode);
            var newDocument = context.Document.WithSyntaxRoot(newSyntaxRoot);

            return Task.FromResult(newDocument);
        }

        private SyntaxNode GetRelevantNode(SyntaxNode innerNode)
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
    }
}
