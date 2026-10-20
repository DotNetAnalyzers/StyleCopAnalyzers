// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.DocumentationRules
{
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
    /// Implements a code fix that will generate a documentation comment comprised of an empty
    /// <c>&lt;inheritdoc/&gt;</c> element.
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(InheritdocCodeFixProvider))]
    [Shared]
    internal class InheritdocCodeFixProvider : CodeFixProvider
    {
        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds { get; } =
            ImmutableArray.Create(
                "CS1591",
                SA1600ElementsMustBeDocumented.DiagnosticId);

        /// <inheritdoc/>
        public override FixAllProvider GetFixAllProvider()
        {
            return CustomFixAllProviders.BatchFixer;
        }

        /// <inheritdoc/>
        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            foreach (var diagnostic in context.Diagnostics)
            {
                SyntaxToken identifierToken = root.FindToken(diagnostic.Location.SourceSpan.Start);
                if (identifierToken.IsMissingOrDefault())
                {
                    continue;
                }

                switch (identifierToken.Parent.Kind())
                {
                case SyntaxKind.PropertyDeclaration:
                case SyntaxKind.EventDeclaration:
                    if (((BasePropertyDeclarationSyntax)identifierToken.Parent).Modifiers.Any(SyntaxKind.StaticKeyword))
                    {
                        continue;
                    }

                    break;

                case SyntaxKind.MethodDeclaration:
                    if (((MethodDeclarationSyntax)identifierToken.Parent).Modifiers.Any(SyntaxKind.StaticKeyword))
                    {
                        continue;
                    }

                    break;

                case SyntaxKind.VariableDeclarator:
                    if (!identifierToken.Parent.Parent.IsKind(SyntaxKind.VariableDeclaration)
                        || !identifierToken.Parent.Parent.Parent.IsKind(SyntaxKind.EventFieldDeclaration))
                    {
                        continue;
                    }

                    if (((EventFieldDeclarationSyntax)identifierToken.Parent.Parent.Parent).Modifiers.Any(SyntaxKind.StaticKeyword))
                    {
                        continue;
                    }

                    break;

                case SyntaxKind.IndexerDeclaration:
                    break;

                default:
                    continue;
                }

                context.RegisterCodeFix(
                    CodeAction.Create(
                        DocumentationResources.InheritdocCodeFix,
                        cancellationToken => GetTransformedDocumentAsync(context.Document, root, identifierToken, cancellationToken),
                        nameof(InheritdocCodeFixProvider)),
                    diagnostic);
            }
        }

        private static async Task<Document> GetTransformedDocumentAsync(Document document, SyntaxNode root, SyntaxToken identifierToken, CancellationToken cancellationToken)
        {
            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);
            switch (identifierToken.Parent.Kind())
            {
            case SyntaxKind.PropertyDeclaration:
            case SyntaxKind.EventDeclaration:
                return await GetTransformedDocumentForBasePropertyDeclarationAsync(document, root, semanticModel, (BasePropertyDeclarationSyntax)identifierToken.Parent, cancellationToken).ConfigureAwait(false);

            case SyntaxKind.MethodDeclaration:
                return await GetTransformedDocumentForMethodDeclarationAsync(document, root, semanticModel, (MethodDeclarationSyntax)identifierToken.Parent, cancellationToken).ConfigureAwait(false);

            case SyntaxKind.VariableDeclarator:
                return await GetTransformedDocumentForEventFieldDeclarationAsync(document, root, semanticModel, (EventFieldDeclarationSyntax)identifierToken.Parent.Parent.Parent, cancellationToken).ConfigureAwait(false);

            case SyntaxKind.IndexerDeclaration:
                return await GetTransformedDocumentForIndexerDeclarationAsync(document, root, semanticModel, (IndexerDeclarationSyntax)identifierToken.Parent, cancellationToken).ConfigureAwait(false);

            default:
                return document;
            }
        }

        private static async Task<Document> GetTransformedDocumentForBasePropertyDeclarationAsync(Document document, SyntaxNode root, SemanticModel semanticModel, BasePropertyDeclarationSyntax basePropertyDeclaration, CancellationToken cancellationToken)
        {
            if (basePropertyDeclaration.ExplicitInterfaceSpecifier == null && !basePropertyDeclaration.Modifiers.Any(SyntaxKind.OverrideKeyword))
            {
                ISymbol declaredSymbol = semanticModel.GetDeclaredSymbol(basePropertyDeclaration, cancellationToken);
                if (declaredSymbol == null || !NamedTypeHelpers.IsImplementingAnInterfaceMember(declaredSymbol))
                {
                    return document;
                }
            }

            return await InsertInheritdocCommentAsync(document, root, basePropertyDeclaration, cancellationToken).ConfigureAwait(false);
        }

        private static async Task<Document> GetTransformedDocumentForMethodDeclarationAsync(Document document, SyntaxNode root, SemanticModel semanticModel, MethodDeclarationSyntax methodDeclaration, CancellationToken cancellationToken)
        {
            if (methodDeclaration.ExplicitInterfaceSpecifier == null && !methodDeclaration.Modifiers.Any(SyntaxKind.OverrideKeyword))
            {
                ISymbol declaredSymbol = semanticModel.GetDeclaredSymbol(methodDeclaration, cancellationToken);
                if (declaredSymbol == null || !NamedTypeHelpers.IsImplementingAnInterfaceMember(declaredSymbol))
                {
                    return document;
                }
            }

            return await InsertInheritdocCommentAsync(document, root, methodDeclaration, cancellationToken).ConfigureAwait(false);
        }

        private static async Task<Document> GetTransformedDocumentForEventFieldDeclarationAsync(Document document, SyntaxNode root, SemanticModel semanticModel, EventFieldDeclarationSyntax eventFieldDeclaration, CancellationToken cancellationToken)
        {
            if (!eventFieldDeclaration.Modifiers.Any(SyntaxKind.OverrideKeyword))
            {
                ISymbol? declaredSymbol = null;
                VariableDeclaratorSyntax? firstVariable = eventFieldDeclaration.Declaration?.Variables.FirstOrDefault();
                if (firstVariable != null)
                {
                    declaredSymbol = semanticModel.GetDeclaredSymbol(firstVariable, cancellationToken);
                }

                if (declaredSymbol == null || !NamedTypeHelpers.IsImplementingAnInterfaceMember(declaredSymbol))
                {
                    return document;
                }
            }

            return await InsertInheritdocCommentAsync(document, root, eventFieldDeclaration, cancellationToken).ConfigureAwait(false);
        }

        private static async Task<Document> GetTransformedDocumentForIndexerDeclarationAsync(Document document, SyntaxNode root, SemanticModel semanticModel, IndexerDeclarationSyntax indexerDeclaration, CancellationToken cancellationToken)
        {
            if (indexerDeclaration.ExplicitInterfaceSpecifier == null && !indexerDeclaration.Modifiers.Any(SyntaxKind.OverrideKeyword))
            {
                ISymbol declaredSymbol = semanticModel.GetDeclaredSymbol(indexerDeclaration, cancellationToken);
                if (declaredSymbol == null || !NamedTypeHelpers.IsImplementingAnInterfaceMember(declaredSymbol))
                {
                    return document;
                }
            }

            return await InsertInheritdocCommentAsync(document, root, indexerDeclaration, cancellationToken).ConfigureAwait(false);
        }

        private static async Task<Document> InsertInheritdocCommentAsync(Document document, SyntaxNode root, SyntaxNode syntaxNode, CancellationToken cancellationToken)
        {
            SyntaxTriviaList leadingTrivia = syntaxNode.GetLeadingTrivia();
            int insertionIndex = leadingTrivia.Count;
            while (insertionIndex > 0 && !leadingTrivia[insertionIndex - 1].HasBuiltinEndLine())
            {
                insertionIndex--;
            }

            string newLineText = (await FormattingHelper.GetEndOfLineForCodeFixAsync(document, syntaxNode.GetFirstToken(), cancellationToken).ConfigureAwait(false)).ToFullString();
            var documentationComment =
                XmlSyntaxFactory.DocumentationComment(
                    newLineText,
                    XmlSyntaxFactory.EmptyElement(XmlCommentHelper.InheritdocXmlTag));
            var trivia = SyntaxFactory.Trivia(documentationComment);

            SyntaxTriviaList newLeadingTrivia = leadingTrivia.Insert(insertionIndex, trivia);
            SyntaxNode newElement = syntaxNode.WithLeadingTrivia(newLeadingTrivia);
            return document.WithSyntaxRoot(root.ReplaceNode(syntaxNode, newElement));
        }
    }
}
