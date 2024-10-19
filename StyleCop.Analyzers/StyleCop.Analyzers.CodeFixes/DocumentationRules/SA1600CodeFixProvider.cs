// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.DocumentationRules
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
    using Microsoft.CodeAnalysis.Formatting;
    using Microsoft.CodeAnalysis.Simplification;
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
                            cancellationToken => GetConstructorOrDestructorDocumentationTransformedDocumentAsync(context.Document, root, (BaseMethodDeclarationSyntax)identifierToken.Parent, cancellationToken),
                            nameof(SA1600CodeFixProvider)),
                        diagnostic);
                    break;

                case SyntaxKind.MethodDeclaration:
                    MethodDeclarationSyntax methodDeclaration = (MethodDeclarationSyntax)identifierToken.Parent;
                    if (TaskHelper.IsTaskReturningMethod(semanticModel, methodDeclaration, context.CancellationToken) &&
                        !IsCoveredByInheritDoc(semanticModel, methodDeclaration, context.CancellationToken))
                    {
                        context.RegisterCodeFix(
                            CodeAction.Create(
                                DocumentationResources.MethodDocumentationCodeFix,
                                cancellationToken => GetMethodDocumentationTransformedDocumentAsync(context.Document, root, semanticModel, (MethodDeclarationSyntax)identifierToken.Parent, cancellationToken),
                                nameof(SA1600CodeFixProvider)),
                            diagnostic);
                    }

                    break;
                }
            }
        }

        private static bool IsCoveredByInheritDoc(SemanticModel semanticModel, MethodDeclarationSyntax methodDeclaration, CancellationToken cancellationToken)
        {
            if (methodDeclaration.ExplicitInterfaceSpecifier != null)
            {
                return true;
            }

            if (methodDeclaration.Modifiers.Any(SyntaxKind.OverrideKeyword))
            {
                return true;
            }

            ISymbol declaredSymbol = semanticModel.GetDeclaredSymbol(methodDeclaration, cancellationToken);
            return (declaredSymbol != null) && NamedTypeHelpers.IsImplementingAnInterfaceMember(declaredSymbol);
        }

        private static Task<Document> GetConstructorOrDestructorDocumentationTransformedDocumentAsync(Document document, SyntaxNode root, BaseMethodDeclarationSyntax declaration, CancellationToken cancellationToken)
        {
            SyntaxTriviaList leadingTrivia = declaration.GetLeadingTrivia();
            int insertionIndex = GetInsertionIndex(ref leadingTrivia);

            string newLineText = document.Project.Solution.Workspace.Options.GetOption(FormattingOptions.NewLine, LanguageNames.CSharp);

            var documentationNodes = new List<XmlNodeSyntax>();

            var typeDeclaration = declaration.FirstAncestorOrSelf<BaseTypeDeclarationSyntax>();
            var standardText = SA1642SA1643CodeFixProvider.GenerateStandardText(document, declaration, typeDeclaration, cancellationToken);
            var standardTextSyntaxList = SA1642SA1643CodeFixProvider.BuildStandardTextSyntaxList(typeDeclaration, newLineText, standardText[0], standardText[1]);

            // Remove the empty line generated by build standard text, as this is not needed with constructing a new summary element.
            standardTextSyntaxList = standardTextSyntaxList.RemoveAt(0);

            documentationNodes.Add(XmlSyntaxFactory.SummaryElement(newLineText, standardTextSyntaxList));

            if (declaration.ParameterList != null)
            {
                foreach (var parameter in declaration.ParameterList.Parameters)
                {
                    documentationNodes.Add(XmlSyntaxFactory.NewLine(newLineText));
                    documentationNodes.Add(XmlSyntaxFactory.ParamElement(parameter.Identifier.ValueText));
                }
            }

            var documentationComment =
                XmlSyntaxFactory.DocumentationComment(
                    newLineText,
                    documentationNodes.ToArray());
            var trivia = SyntaxFactory.Trivia(documentationComment);

            SyntaxTriviaList newLeadingTrivia = leadingTrivia.Insert(insertionIndex, trivia);
            SyntaxNode newElement = declaration.WithLeadingTrivia(newLeadingTrivia);
            return Task.FromResult(document.WithSyntaxRoot(root.ReplaceNode(declaration, newElement)));
        }

        private static Task<Document> GetMethodDocumentationTransformedDocumentAsync(Document document, SyntaxNode root, SemanticModel semanticModel, MethodDeclarationSyntax methodDeclaration, CancellationToken cancellationToken)
        {
            SyntaxTriviaList leadingTrivia = methodDeclaration.GetLeadingTrivia();
            int insertionIndex = GetInsertionIndex(ref leadingTrivia);

            string newLineText = document.Project.Solution.Workspace.Options.GetOption(FormattingOptions.NewLine, LanguageNames.CSharp);

            var documentationNodes = new List<XmlNodeSyntax>();

            documentationNodes.Add(XmlSyntaxFactory.SummaryElement(newLineText));

            if (methodDeclaration.TypeParameterList != null)
            {
                foreach (var typeParameter in methodDeclaration.TypeParameterList.Parameters)
                {
                    documentationNodes.Add(XmlSyntaxFactory.NewLine(newLineText));
                    documentationNodes.Add(XmlSyntaxFactory.TypeParamElement(typeParameter.Identifier.ValueText));
                }
            }

            if (methodDeclaration.ParameterList != null)
            {
                foreach (var parameter in methodDeclaration.ParameterList.Parameters)
                {
                    documentationNodes.Add(XmlSyntaxFactory.NewLine(newLineText));
                    documentationNodes.Add(XmlSyntaxFactory.ParamElement(parameter.Identifier.ValueText));
                }
            }

            TypeSyntax typeName;

            var typeSymbol = semanticModel.GetSymbolInfo(methodDeclaration.ReturnType, cancellationToken).Symbol as INamedTypeSymbol;
            if (typeSymbol.IsGenericType)
            {
                typeName = SyntaxFactory.ParseTypeName("global::System.Threading.Tasks.Task<TResult>");
            }
            else
            {
                typeName = SyntaxFactory.ParseTypeName("global::System.Threading.Tasks.Task");
            }

            XmlNodeSyntax[] returnContent =
            {
                XmlSyntaxFactory.Text(DocumentationResources.TaskReturnElementFirstPart),
                XmlSyntaxFactory.SeeElement(SyntaxFactory.TypeCref(typeName)).WithAdditionalAnnotations(Simplifier.Annotation),
                XmlSyntaxFactory.Text(DocumentationResources.TaskReturnElementSecondPart),
            };

            documentationNodes.Add(XmlSyntaxFactory.NewLine(newLineText));
            documentationNodes.Add(XmlSyntaxFactory.ReturnsElement(returnContent));

            var documentationComment =
                XmlSyntaxFactory.DocumentationComment(
                    newLineText,
                    documentationNodes.ToArray());
            var trivia = SyntaxFactory.Trivia(documentationComment);

            SyntaxTriviaList newLeadingTrivia = leadingTrivia.Insert(insertionIndex, trivia);
            SyntaxNode newElement = methodDeclaration.WithLeadingTrivia(newLeadingTrivia);
            return Task.FromResult(document.WithSyntaxRoot(root.ReplaceNode(methodDeclaration, newElement)));
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
    }
}
