// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.DocumentationRules
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Composition;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Formatting;
    using StyleCop.Analyzers.Helpers;

    /// <summary>
    /// Implements a code fix that will generate a documentation comment comprised of an empty
    /// <c>&lt;inheritdoc/&gt;</c> element.
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1600CodeFixProvider))]
    [Shared]
    internal class SA1600CodeFixProvider : CodeFixProvider
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
            SemanticModel semanticModel = await context.Document.GetSemanticModelAsync(context.CancellationToken).ConfigureAwait(false);

            foreach (var diagnostic in context.Diagnostics)
            {
                SyntaxToken identifierToken = root.FindToken(diagnostic.Location.SourceSpan.Start);
                if (identifierToken.IsMissingOrDefault())
                {
                    continue;
                }

                switch (identifierToken.Parent.Kind())
                {
                case SyntaxKind.ConstructorDeclaration:
                case SyntaxKind.DestructorDeclaration:
                    context.RegisterCodeFix(
                        CodeAction.Create(
                            DocumentationResources.ConstructorDocumentationCodeFix,
                            cancellationToken =>
                                GetConstructorOrDestructorDocumentationTransformedDocumentAsync(
                                    context.Document,
                                    root,
                                    (BaseMethodDeclarationSyntax)identifierToken.Parent,
                                    cancellationToken),
                            nameof(SA1600CodeFixProvider)),
                        diagnostic);
                    break;

                case SyntaxKind.MethodDeclaration:
                    MethodDeclarationSyntax methodDeclaration = (MethodDeclarationSyntax)identifierToken.Parent;
                    if (!InheritDocHelper.IsCoveredByInheritDoc(semanticModel, methodDeclaration, context.CancellationToken))
                    {
                        context.RegisterCodeFix(
                            CodeAction.Create(
                                DocumentationResources.MethodDocumentationCodeFix,
                                cancellationToken => GetMethodDocumentationTransformedDocumentAsync(
                                    context.Document,
                                    root,
                                    semanticModel,
                                    (MethodDeclarationSyntax)identifierToken.Parent,
                                    cancellationToken),
                                nameof(SA1600CodeFixProvider)),
                            diagnostic);
                    }

                    break;

                case SyntaxKind.DelegateDeclaration:
                    context.RegisterCodeFix(
                        CodeAction.Create(
                            DocumentationResources.DelegateDocumentationCodeFix,
                            cancellationToken => GetDelegateDocumentationTransformedDocumentAsync(
                                context.Document,
                                root,
                                semanticModel,
                                (DelegateDeclarationSyntax)identifierToken.Parent,
                                cancellationToken),
                            nameof(SA1600CodeFixProvider)),
                        diagnostic);

                    break;

                case SyntaxKind.PropertyDeclaration:
                    var propertyDeclaration = (PropertyDeclarationSyntax)identifierToken.Parent;
                    if (!InheritDocHelper.IsCoveredByInheritDoc(semanticModel, propertyDeclaration, context.CancellationToken))
                    {
                        context.RegisterCodeFix(
                            CodeAction.Create(
                                DocumentationResources.PropertyDocumentationCodeFix,
                                cancellationToken => GetPropertyDocumentationTransformedDocumentAsync(
                                    context.Document,
                                    root,
                                    semanticModel,
                                    (PropertyDeclarationSyntax)identifierToken.Parent,
                                    cancellationToken),
                                nameof(SA1600CodeFixProvider)),
                            diagnostic);
                    }

                    break;

                case SyntaxKind.ClassDeclaration:
                    context.RegisterCodeFix(
                        CodeAction.Create(
                            DocumentationResources.ClassDocumentationCodeFix,
                            _ => GetCommonGenericTypeDocumentationTransformedDocumentAsync(
                                context.Document,
                                root,
                                (TypeDeclarationSyntax)identifierToken.Parent,
                                ((BaseTypeDeclarationSyntax)identifierToken.Parent).Identifier),
                            nameof(SA1600CodeFixProvider)),
                        diagnostic);

                    break;

                case SyntaxKind.InterfaceDeclaration:
                    context.RegisterCodeFix(
                        CodeAction.Create(
                            DocumentationResources.InterfaceDocumentationCodeFix,
                            _ => GetCommonGenericTypeDocumentationTransformedDocumentAsync(
                                context.Document,
                                root,
                                (TypeDeclarationSyntax)identifierToken.Parent,
                                ((BaseTypeDeclarationSyntax)identifierToken.Parent).Identifier),
                            nameof(SA1600CodeFixProvider)),
                        diagnostic);

                    break;

                case SyntaxKind.StructDeclaration:
                    context.RegisterCodeFix(
                        CodeAction.Create(
                            DocumentationResources.StructDocumentationCodeFix,
                            _ => GetCommonGenericTypeDocumentationTransformedDocumentAsync(
                                context.Document,
                                root,
                                (TypeDeclarationSyntax)identifierToken.Parent,
                                ((BaseTypeDeclarationSyntax)identifierToken.Parent).Identifier),
                            nameof(SA1600CodeFixProvider)),
                        diagnostic);

                    break;

                case SyntaxKind.EnumDeclaration:
                    context.RegisterCodeFix(
                        CodeAction.Create(
                            DocumentationResources.EnumDocumentationCodeFix,
                            _ => GetCommonTypeDocumentationTransformedDocumentAsync(
                                context.Document,
                                root,
                                identifierToken.Parent,
                                ((BaseTypeDeclarationSyntax)identifierToken.Parent).Identifier),
                            nameof(SA1600CodeFixProvider)),
                        diagnostic);

                    break;

                case SyntaxKind.VariableDeclarator:
                    var fieldDeclaration = identifierToken.Parent.FirstAncestorOrSelf<FieldDeclarationSyntax>();
                    if (fieldDeclaration != null)
                    {
                        var declaration = fieldDeclaration;
                        context.RegisterCodeFix(
                            CodeAction.Create(
                                DocumentationResources.FieldDocumentationCodeFix,
                                _ => GetCommonTypeDocumentationTransformedDocumentAsync(
                                    context.Document,
                                    root,
                                    declaration,
                                    identifierToken.Parent.FirstAncestorOrSelf<VariableDeclaratorSyntax>().Identifier),
                                nameof(SA1600CodeFixProvider)),
                            diagnostic);
                    }

                    var eventDeclaration = identifierToken.Parent.FirstAncestorOrSelf<EventFieldDeclarationSyntax>();
                    if (eventDeclaration != null)
                    {
                        var declaration = eventDeclaration;
                        context.RegisterCodeFix(
                            CodeAction.Create(
                                DocumentationResources.EventDocumentationCodeFix,
                                _ => GetEventFieldDocumentationTransformedDocumentAsync(
                                    context.Document,
                                    root,
                                    declaration),
                                nameof(SA1600CodeFixProvider)),
                            diagnostic);
                    }

                    break;

                case SyntaxKind.IndexerDeclaration:
                    context.RegisterCodeFix(
                        CodeAction.Create(
                            DocumentationResources.IndexerDocumentationCodeFix,
                            cancellationToken => GetIndexerDocumentationTransformedDocumentAsync(
                                context.Document,
                                root,
                                semanticModel,
                                (IndexerDeclarationSyntax)identifierToken.Parent,
                                cancellationToken),
                            nameof(SA1600CodeFixProvider)),
                        diagnostic);

                    break;

                case SyntaxKind.EventDeclaration:
                    context.RegisterCodeFix(
                        CodeAction.Create(
                            DocumentationResources.EventDocumentationCodeFix,
                            _ => GetEventDocumentationTransformedDocumentAsync(
                                context.Document,
                                root,
                                (EventDeclarationSyntax)identifierToken.Parent),
                            nameof(SA1600CodeFixProvider)),
                        diagnostic);

                    break;
                }
            }
        }

        private static Task<Document> GetEventDocumentationTransformedDocumentAsync(
            Document document,
            SyntaxNode root,
            EventDeclarationSyntax eventDeclaration)
        {
            return GetEventDocumentationTransformedDocumentAsync(
                document,
                root,
                eventDeclaration,
                eventDeclaration.Identifier);
        }

        private static Task<Document> GetEventFieldDocumentationTransformedDocumentAsync(
            Document document,
            SyntaxNode root,
            EventFieldDeclarationSyntax eventDeclaration)
        {
            return GetEventDocumentationTransformedDocumentAsync(
                document,
                root,
                eventDeclaration,
                eventDeclaration.Declaration.Variables.First().Identifier);
        }

        private static Task<Document> GetEventDocumentationTransformedDocumentAsync(
            Document document,
            SyntaxNode root,
            SyntaxNode eventDeclaration,
            SyntaxToken identifier)
        {
            string newLineText = GetNewLineText(document);
            var documentationNode = EventDocumentationHelper.CreateEventDocumentation(identifier, newLineText);
            return CreateCommentAndReplaceInDocumentAsync(document, root, eventDeclaration, newLineText, documentationNode);
        }

        private static Task<Document> GetIndexerDocumentationTransformedDocumentAsync(
            Document document,
            SyntaxNode root,
            SemanticModel semanticModel,
            IndexerDeclarationSyntax indexerDeclaration,
            CancellationToken cancellationToken)
        {
            string newLineText = GetNewLineText(document);

            var documentationNodes = new List<XmlNodeSyntax>();
            documentationNodes.Add(PropertyDocumentationHelper.CreateIndexerSummeryNode(indexerDeclaration, semanticModel, cancellationToken, newLineText));
            documentationNodes.AddRange(MethodDocumentationHelper.CreateParametersDocumentation(newLineText, indexerDeclaration.ParameterList?.Parameters.ToArray()));
            documentationNodes.AddRange(MethodDocumentationHelper.CreateReturnDocumentation(newLineText, XmlSyntaxFactory.Text(DocumentationResources.IndexerReturnDocumentation)));

            return CreateCommentAndReplaceInDocumentAsync(document, root, indexerDeclaration, newLineText, documentationNodes.ToArray());
        }

        private static Task<Document> GetCommonTypeDocumentationTransformedDocumentAsync(
            Document document,
            SyntaxNode root,
            SyntaxNode declaration,
            SyntaxToken declarationIdentifier,
            params XmlNodeSyntax[] additionalDocumentation)
        {
            string newLineText = GetNewLineText(document);

            var documentationNods = new List<XmlNodeSyntax>();
            var documentationText = CommonDocumentationHelper.CreateCommonComment(declarationIdentifier.ValueText, declaration.Kind() == SyntaxKind.InterfaceDeclaration);
            documentationNods.Add(CommonDocumentationHelper.CreateSummaryNode(documentationText, newLineText));
            documentationNods.AddRange(additionalDocumentation);

            return CreateCommentAndReplaceInDocumentAsync(document, root, declaration, newLineText, documentationNods.ToArray());
        }

        private static Task<Document> GetCommonGenericTypeDocumentationTransformedDocumentAsync(
            Document document,
            SyntaxNode root,
            TypeDeclarationSyntax declaration,
            SyntaxToken declarationIdentifier)
        {
            var typeParamsDocumentation = MethodDocumentationHelper.CreateTypeParametersDocumentation(GetNewLineText(document), declaration.TypeParameterList?.Parameters.ToArray()).ToArray();
            return GetCommonTypeDocumentationTransformedDocumentAsync(document, root, declaration, declarationIdentifier, typeParamsDocumentation);
        }

        private static Task<Document> GetPropertyDocumentationTransformedDocumentAsync(
            Document document,
            SyntaxNode root,
            SemanticModel semanticModel,
            PropertyDeclarationSyntax propertyDeclaration,
            CancellationToken cancellationToken)
        {
            string newLineText = GetNewLineText(document);

            var propertyDocumentationNode = PropertyDocumentationHelper.CreatePropertySummeryComment(propertyDeclaration, semanticModel, cancellationToken, newLineText);
            return CreateCommentAndReplaceInDocumentAsync(document, root, propertyDeclaration, newLineText, propertyDocumentationNode);
        }

        private static Task<Document> GetConstructorOrDestructorDocumentationTransformedDocumentAsync(Document document, SyntaxNode root, BaseMethodDeclarationSyntax declaration, CancellationToken cancellationToken)
        {
            string newLineText = GetNewLineText(document);
            var documentationNodes = new List<XmlNodeSyntax>();

            var typeDeclaration = declaration.FirstAncestorOrSelf<BaseTypeDeclarationSyntax>();
            var standardText = SA1642SA1643CodeFixProvider.GenerateStandardText(document, declaration, typeDeclaration, cancellationToken);
            var standardTextSyntaxList = SA1642SA1643CodeFixProvider.BuildStandardTextSyntaxList(typeDeclaration, newLineText, standardText[0], standardText[1]);

            // Remove the empty line generated by build standard text, as this is not needed with constructing a new summary element.
            standardTextSyntaxList = standardTextSyntaxList.RemoveAt(0);

            documentationNodes.Add(XmlSyntaxFactory.SummaryElement(newLineText, standardTextSyntaxList));

            var parametersDocumentation = MethodDocumentationHelper.CreateParametersDocumentation(newLineText, declaration.ParameterList?.Parameters.ToArray());
            documentationNodes.AddRange(parametersDocumentation);

            return CreateCommentAndReplaceInDocumentAsync(document, root, declaration, newLineText, documentationNodes.ToArray());
        }

        private static Task<Document> GetMethodDocumentationTransformedDocumentAsync(
            Document document,
            SyntaxNode root,
            SemanticModel semanticModel,
            MethodDeclarationSyntax methodDeclaration,
            CancellationToken cancellationToken)
        {
            var throwStatements = MethodDocumentationHelper.CreateThrowDocumentation(methodDeclaration.Body, GetNewLineText(document)).ToArray();
            return GetMethodDocumentationTransformedDocumentAsync(
                document,
                root,
                semanticModel,
                methodDeclaration,
                methodDeclaration.Identifier,
                methodDeclaration.TypeParameterList,
                methodDeclaration.ParameterList,
                methodDeclaration.ReturnType,
                cancellationToken,
                throwStatements);
        }

        private static Task<Document> GetDelegateDocumentationTransformedDocumentAsync(
            Document document,
            SyntaxNode root,
            SemanticModel semanticModel,
            DelegateDeclarationSyntax delegateDeclaration,
            CancellationToken cancellationToken)
        {
            return GetMethodDocumentationTransformedDocumentAsync(
                document,
                root,
                semanticModel,
                delegateDeclaration,
                delegateDeclaration.Identifier,
                delegateDeclaration.TypeParameterList,
                delegateDeclaration.ParameterList,
                delegateDeclaration.ReturnType,
                cancellationToken);
        }

        private static Task<Document> GetMethodDocumentationTransformedDocumentAsync(
            Document document,
            SyntaxNode root,
            SemanticModel semanticModel,
            SyntaxNode declaration,
            SyntaxToken identifier,
            TypeParameterListSyntax typeParameterList,
            ParameterListSyntax parameterList,
            TypeSyntax returnType,
            CancellationToken cancellationToken,
            params XmlNodeSyntax[] additionalDocumentation)
        {
            string newLineText = GetNewLineText(document);

            var documentationNodes = new List<XmlNodeSyntax>();
            documentationNodes.Add(CommonDocumentationHelper.CreateDefaultSummaryNode(identifier.ValueText, newLineText));
            documentationNodes.AddRange(MethodDocumentationHelper.CreateTypeParametersDocumentation(newLineText, typeParameterList?.Parameters.ToArray()));
            documentationNodes.AddRange(MethodDocumentationHelper.CreateParametersDocumentation(newLineText, parameterList?.Parameters.ToArray()));
            documentationNodes.AddRange(MethodDocumentationHelper.CreateReturnDocumentation(semanticModel, returnType, cancellationToken, newLineText));
            documentationNodes.AddRange(additionalDocumentation);

            return CreateCommentAndReplaceInDocumentAsync(document, root, declaration, newLineText, documentationNodes.ToArray());
        }

        private static Task<Document> CreateCommentAndReplaceInDocumentAsync(
            Document document,
            SyntaxNode root,
            SyntaxNode declarationNode,
            string newLineText,
            params XmlNodeSyntax[] documentationNodes)
        {
            var leadingTrivia = declarationNode.GetLeadingTrivia();
            int insertionIndex = GetInsertionIndex(ref leadingTrivia);

            var documentationComment = XmlSyntaxFactory.DocumentationComment(newLineText, documentationNodes);
            var trivia = SyntaxFactory.Trivia(documentationComment);

            SyntaxTriviaList newLeadingTrivia = leadingTrivia.Insert(insertionIndex, trivia);
            SyntaxNode newElement = declarationNode.WithLeadingTrivia(newLeadingTrivia);
            return Task.FromResult(document.WithSyntaxRoot(root.ReplaceNode(declarationNode, newElement)));
        }

        private static int GetInsertionIndex(ref SyntaxTriviaList leadingTrivia)
        {
            int insertionIndex = leadingTrivia.Count;
            while (insertionIndex > 0 && !leadingTrivia[insertionIndex - 1].HasBuiltinEndLine())
            {
                insertionIndex--;
            }

            return insertionIndex;
        }

        private static string GetNewLineText(Document document)
        {
            return document.Project.Solution.Workspace.Options.GetOption(FormattingOptions.NewLine, LanguageNames.CSharp);
        }
    }
}
