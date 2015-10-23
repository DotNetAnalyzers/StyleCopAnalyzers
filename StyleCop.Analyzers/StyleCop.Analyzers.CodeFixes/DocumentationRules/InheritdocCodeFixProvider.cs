// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.DocumentationRules
{
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
    using Microsoft.CodeAnalysis.Formatting;

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
                        cancellationToken => GetTransformedDocumentAsync(context.Document, diagnostic, root, identifierToken, cancellationToken),
                        nameof(InheritdocCodeFixProvider)),
                    diagnostic);
            }
        }

        private static async Task<Document> GetTransformedDocumentAsync(Document document, Diagnostic diagnostic, SyntaxNode root, SyntaxToken identifierToken, CancellationToken cancellationToken)
        {
            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);
            switch (identifierToken.Parent.Kind())
            {
            case SyntaxKind.PropertyDeclaration:
            case SyntaxKind.EventDeclaration:
                return GetTransformedDocumentForBasePropertyDeclaration(document, diagnostic, root, semanticModel, (BasePropertyDeclarationSyntax)identifierToken.Parent, cancellationToken);

            case SyntaxKind.MethodDeclaration:
                return GetTransformedDocumentForMethodDeclaration(document, diagnostic, root, semanticModel, (MethodDeclarationSyntax)identifierToken.Parent, cancellationToken);

            case SyntaxKind.VariableDeclarator:
                return GetTransformedDocumentForEventFieldDeclaration(document, diagnostic, root, semanticModel, (EventFieldDeclarationSyntax)identifierToken.Parent.Parent.Parent, cancellationToken);

            case SyntaxKind.IndexerDeclaration:
                return GetTransformedDocumentForIndexerDeclaration(document, diagnostic, root, semanticModel, (IndexerDeclarationSyntax)identifierToken.Parent, cancellationToken);

            default:
                return document;
            }
        }

        private static Document GetTransformedDocumentForBasePropertyDeclaration(Document document, Diagnostic diagnostic, SyntaxNode root, SemanticModel semanticModel, BasePropertyDeclarationSyntax basePropertyDeclaration, CancellationToken cancellationToken)
        {
            if (basePropertyDeclaration.ExplicitInterfaceSpecifier == null && !basePropertyDeclaration.Modifiers.Any(SyntaxKind.OverrideKeyword))
            {
                ISymbol declaredSymbol = semanticModel.GetDeclaredSymbol(basePropertyDeclaration, cancellationToken);
                if (declaredSymbol == null || !NamedTypeHelpers.IsImplementingAnInterfaceMember(declaredSymbol))
                {
                    return document;
                }
            }

            return InsertInheritdocComment(document, diagnostic, root, basePropertyDeclaration, cancellationToken);
        }

        private static Document GetTransformedDocumentForMethodDeclaration(Document document, Diagnostic diagnostic, SyntaxNode root, SemanticModel semanticModel, MethodDeclarationSyntax methodDeclaration, CancellationToken cancellationToken)
        {
            if (methodDeclaration.ExplicitInterfaceSpecifier == null && !methodDeclaration.Modifiers.Any(SyntaxKind.OverrideKeyword))
            {
                ISymbol declaredSymbol = semanticModel.GetDeclaredSymbol(methodDeclaration, cancellationToken);
                if (declaredSymbol == null || !NamedTypeHelpers.IsImplementingAnInterfaceMember(declaredSymbol))
                {
                    return document;
                }
            }

            return InsertInheritdocComment(document, diagnostic, root, methodDeclaration, cancellationToken);
        }

        private static Document GetTransformedDocumentForEventFieldDeclaration(Document document, Diagnostic diagnostic, SyntaxNode root, SemanticModel semanticModel, EventFieldDeclarationSyntax eventFieldDeclaration, CancellationToken cancellationToken)
        {
            if (!eventFieldDeclaration.Modifiers.Any(SyntaxKind.OverrideKeyword))
            {
                ISymbol declaredSymbol = null;
                VariableDeclaratorSyntax firstVariable = eventFieldDeclaration.Declaration?.Variables.FirstOrDefault();
                if (firstVariable != null)
                {
                    declaredSymbol = semanticModel.GetDeclaredSymbol(firstVariable, cancellationToken);
                }

                if (declaredSymbol == null || !NamedTypeHelpers.IsImplementingAnInterfaceMember(declaredSymbol))
                {
                    return document;
                }
            }

            return InsertInheritdocComment(document, diagnostic, root, eventFieldDeclaration, cancellationToken);
        }

        private static Document GetTransformedDocumentForIndexerDeclaration(Document document, Diagnostic diagnostic, SyntaxNode root, SemanticModel semanticModel, IndexerDeclarationSyntax indexerDeclaration, CancellationToken cancellationToken)
        {
            if (indexerDeclaration.ExplicitInterfaceSpecifier == null && !indexerDeclaration.Modifiers.Any(SyntaxKind.OverrideKeyword))
            {
                ISymbol declaredSymbol = semanticModel.GetDeclaredSymbol(indexerDeclaration, cancellationToken);
                if (declaredSymbol == null || !NamedTypeHelpers.IsImplementingAnInterfaceMember(declaredSymbol))
                {
                    return document;
                }
            }

            return InsertInheritdocComment(document, diagnostic, root, indexerDeclaration, cancellationToken);
        }

        private static Document InsertInheritdocComment(Document document, Diagnostic diagnostic, SyntaxNode root, SyntaxNode syntaxNode, CancellationToken cancellationToken)
        {
            SyntaxTriviaList leadingTrivia = syntaxNode.GetLeadingTrivia();
            int insertionIndex = leadingTrivia.Count;
            while (insertionIndex > 0 && !leadingTrivia[insertionIndex - 1].HasBuiltinEndLine())
            {
                insertionIndex--;
            }

            string newLineText = document.Project.Solution.Workspace.Options.GetOption(FormattingOptions.NewLine, LanguageNames.CSharp);
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
