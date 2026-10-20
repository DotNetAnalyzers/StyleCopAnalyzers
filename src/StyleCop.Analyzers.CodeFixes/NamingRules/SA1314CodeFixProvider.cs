// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.NamingRules
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
    using Microsoft.CodeAnalysis.Text;
    using StyleCop.Analyzers.Helpers;

    /// <summary>
    /// Implements a code fix for <see cref="SA1314TypeParameterNamesMustBeginWithT"/>.
    /// </summary>
    /// <remarks>
    /// <para>To fix a violation of this rule, add the capital letter T to the front of the type parameter name.</para>
    /// </remarks>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1314CodeFixProvider))]
    [Shared]
    internal class SA1314CodeFixProvider : CodeFixProvider
    {
        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds { get; } =
            ImmutableArray.Create(SA1314TypeParameterNamesMustBeginWithT.DiagnosticId);

        /// <inheritdoc/>
        public override FixAllProvider? GetFixAllProvider()
        {
            // Fix All is not yet supported
            return null;
        }

        /// <inheritdoc/>
        public override Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            foreach (var diagnostic in context.Diagnostics)
            {
                context.RegisterCodeFix(
                    CodeAction.Create(
                        NamingResources.SA1314CodeFix,
                        cancellationToken => CreateChangedSolutionAsync(context.Document, diagnostic, cancellationToken),
                        nameof(SA1314CodeFixProvider)),
                    diagnostic);
            }

            return SpecializedTasks.CompletedTask;
        }

        private static async Task<Solution> CreateChangedSolutionAsync(Document document, Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            var token = root.FindToken(diagnostic.Location.SourceSpan.Start);
            var baseName = "T" + token.ValueText;
            var index = 0;
            var newName = baseName;

            var semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);
            var declaredSymbol = semanticModel.GetDeclaredSymbol(token.Parent, cancellationToken);
            HashSet<string>? crefPlaceholderNames = null;
            if (declaredSymbol is ITypeParameterSymbol typeParameter
                && typeParameter.ContainingSymbol is INamedTypeSymbol containingType)
            {
                crefPlaceholderNames = await GetCrefPlaceholderNamesAsync(
                    document.Project.Solution,
                    containingType,
                    typeParameter,
                    cancellationToken).ConfigureAwait(false);
            }

            while (!await RenameHelper.IsValidNewMemberNameAsync(semanticModel, declaredSymbol, newName, cancellationToken).ConfigureAwait(false)
                || crefPlaceholderNames?.Contains(newName) == true)
            {
                index++;
                newName = baseName + index;
            }

            if (declaredSymbol is ITypeParameterSymbol typeParameter2
                && typeParameter2.ContainingSymbol is INamedTypeSymbol containingType2)
            {
                var declarationAnnotation = new SyntaxAnnotation();
                var solution = await RenameTypeParameterReferencesInCrefsAsync(
                    document.Project.Solution,
                    document.Id,
                    token,
                    declarationAnnotation,
                    containingType2,
                    typeParameter2,
                    newName,
                    cancellationToken).ConfigureAwait(false);
                document = solution.GetDocument(document.Id);
                root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
                token = root.GetAnnotatedTokens(declarationAnnotation).Single();
            }

            return await RenameHelper.RenameSymbolAsync(document, root, token, newName, cancellationToken).ConfigureAwait(false);
        }

        private static bool IsShadowedTypeParameterReference(SyntaxNode? documentationOwner, SyntaxNode declaration, string name)
        {
            for (var scope = documentationOwner; scope != null && scope != declaration; scope = scope.Parent)
            {
                if (scope is TypeDeclarationSyntax typeDeclaration
                    && typeDeclaration.TypeParameterList?.Parameters.Any(parameter => parameter.Identifier.ValueText == name) == true)
                {
                    return true;
                }

                if (scope is MethodDeclarationSyntax methodDeclaration
                    && methodDeclaration.TypeParameterList?.Parameters.Any(parameter => parameter.Identifier.ValueText == name) == true)
                {
                    return true;
                }

                if (scope is DelegateDeclarationSyntax delegateDeclaration
                    && delegateDeclaration.TypeParameterList?.Parameters.Any(parameter => parameter.Identifier.ValueText == name) == true)
                {
                    return true;
                }
            }

            return false;
        }

        private static async Task<HashSet<string>> GetCrefPlaceholderNamesAsync(
            Solution solution,
            INamedTypeSymbol containingType,
            ITypeParameterSymbol typeParameter,
            CancellationToken cancellationToken)
        {
            var placeholderNames = new HashSet<string>();
            foreach (var declarationReference in containingType.DeclaringSyntaxReferences)
            {
                var declaration = await declarationReference.GetSyntaxAsync(cancellationToken).ConfigureAwait(false);
                var declarationDocument = solution.GetDocument(declaration.SyntaxTree);
                if (declarationDocument == null || solution.GetDocument(declarationDocument.Id) == null)
                {
                    continue;
                }

                var semanticModel = await declarationDocument.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);
                foreach (var trivia in declaration.DescendantTrivia(descendIntoTrivia: true))
                {
                    if (!(trivia.GetStructure() is DocumentationCommentTriviaSyntax documentationComment))
                    {
                        continue;
                    }

                    var documentationOwner = documentationComment.ParentTrivia.Token.Parent;
                    if (IsShadowedTypeParameterReference(documentationOwner, declaration, typeParameter.Name))
                    {
                        continue;
                    }

                    foreach (var genericName in documentationComment.DescendantNodes()
                        .OfType<CrefSyntax>()
                        .SelectMany(cref => cref.DescendantNodes().OfType<GenericNameSyntax>()))
                    {
                        if (!(semanticModel.GetSymbolInfo(genericName, cancellationToken).Symbol is INamedTypeSymbol referencedType)
                            || !Equals(referencedType.OriginalDefinition, containingType.OriginalDefinition))
                        {
                            continue;
                        }

                        for (var i = 0; i < genericName.TypeArgumentList.Arguments.Count; i++)
                        {
                            if (i != typeParameter.Ordinal
                                && genericName.TypeArgumentList.Arguments[i] is IdentifierNameSyntax identifierName)
                            {
                                placeholderNames.Add(identifierName.Identifier.ValueText);
                            }
                        }
                    }
                }
            }

            return placeholderNames;
        }

        private static async Task<Solution> RenameTypeParameterReferencesInCrefsAsync(
            Solution solution,
            DocumentId declarationDocumentId,
            SyntaxToken declarationToken,
            SyntaxAnnotation declarationAnnotation,
            INamedTypeSymbol containingType,
            ITypeParameterSymbol typeParameter,
            string newName,
            CancellationToken cancellationToken)
        {
            var referencesByDocument = new Dictionary<DocumentId, List<TextSpan>>();
            foreach (var declarationReference in containingType.DeclaringSyntaxReferences)
            {
                var declaration = await declarationReference.GetSyntaxAsync(cancellationToken).ConfigureAwait(false);
                var declarationDocument = solution.GetDocument(declaration.SyntaxTree);
                if (declarationDocument == null || solution.GetDocument(declarationDocument.Id) == null)
                {
                    continue;
                }

                var semanticModel = await declarationDocument.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

                foreach (var trivia in declaration.DescendantTrivia(descendIntoTrivia: true))
                {
                    if (!(trivia.GetStructure() is DocumentationCommentTriviaSyntax documentationComment))
                    {
                        continue;
                    }

                    var documentationOwner = documentationComment.ParentTrivia.Token.Parent;
                    if (IsShadowedTypeParameterReference(documentationOwner, declaration, typeParameter.Name))
                    {
                        continue;
                    }

                    foreach (var identifierName in documentationComment.DescendantNodes()
                        .OfType<CrefSyntax>()
                        .SelectMany(cref => cref.DescendantNodes().OfType<IdentifierNameSyntax>()))
                    {
                        var genericName = identifierName.Ancestors().OfType<GenericNameSyntax>().FirstOrDefault();
                        if (!(identifierName.Parent is TypeArgumentListSyntax typeArgumentList)
                            || genericName == null
                            || typeArgumentList.Arguments.IndexOf(identifierName) != typeParameter.Ordinal
                            || identifierName.Identifier.ValueText != typeParameter.Name
                            || !(semanticModel.GetSymbolInfo(genericName, cancellationToken).Symbol is INamedTypeSymbol referencedType)
                            || !Equals(referencedType.OriginalDefinition, containingType.OriginalDefinition))
                        {
                            continue;
                        }

                        if (!referencesByDocument.TryGetValue(declarationDocument.Id, out var identifiers))
                        {
                            identifiers = new List<TextSpan>();
                            referencesByDocument.Add(declarationDocument.Id, identifiers);
                        }

                        identifiers.Add(identifierName.Span);
                    }
                }
            }

            if (!referencesByDocument.ContainsKey(declarationDocumentId))
            {
                referencesByDocument.Add(declarationDocumentId, new List<TextSpan>());
            }

            foreach (var reference in referencesByDocument)
            {
                var referenceDocument = solution.GetDocument(reference.Key);
                var root = await referenceDocument.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
                if (reference.Key == declarationDocumentId)
                {
                    var token = root.FindToken(declarationToken.SpanStart);
                    root = root.ReplaceToken(token, token.WithAdditionalAnnotations(declarationAnnotation));
                }

                if (reference.Value.Count > 0)
                {
                    var spans = new HashSet<TextSpan>(reference.Value);
                    var identifiers = root.DescendantNodes(descendIntoTrivia: true)
                        .OfType<IdentifierNameSyntax>()
                        .Where(identifier => spans.Contains(identifier.Span));
                    root = root.ReplaceNodes(
                        identifiers,
                        (original, _) => original.WithIdentifier(SyntaxFactory.Identifier(newName).WithTriviaFrom(original.Identifier)));
                }

                solution = solution.WithDocumentSyntaxRoot(reference.Key, root);
            }

            return solution;
        }
    }
}
