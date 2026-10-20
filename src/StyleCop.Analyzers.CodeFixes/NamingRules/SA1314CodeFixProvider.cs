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
            while (!await RenameHelper.IsValidNewMemberNameAsync(semanticModel, declaredSymbol, newName, cancellationToken).ConfigureAwait(false))
            {
                index++;
                newName = baseName + index;
            }

            if (declaredSymbol is ITypeParameterSymbol typeParameter
                && typeParameter.ContainingSymbol is INamedTypeSymbol containingType)
            {
                var declarationAnnotation = new SyntaxAnnotation();
                var solution = await RenameTypeParameterReferencesInCrefsAsync(
                    document.Project.Solution,
                    document.Id,
                    token,
                    declarationAnnotation,
                    containingType,
                    typeParameter,
                    newName,
                    cancellationToken).ConfigureAwait(false);
                document = solution.GetDocument(document.Id);
                root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
                token = root.GetAnnotatedTokens(declarationAnnotation).Single();
            }

            return await RenameHelper.RenameSymbolAsync(document, root, token, newName, cancellationToken).ConfigureAwait(false);
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
                var semanticModel = await declarationDocument.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

                foreach (var trivia in declaration.DescendantTrivia(descendIntoTrivia: true))
                {
                    if (!(trivia.GetStructure() is DocumentationCommentTriviaSyntax documentationComment))
                    {
                        continue;
                    }

                    var documentationOwner = documentationComment.ParentTrivia.Token.Parent;
                    var ownerType = documentationOwner?.FirstAncestorOrSelf<TypeDeclarationSyntax>();
                    if (ownerType == null
                        || ownerType.SyntaxTree != declaration.SyntaxTree
                        || ownerType.Span != declaration.Span
                        || documentationOwner?.FirstAncestorOrSelf<MethodDeclarationSyntax>()?.TypeParameterList?.Parameters
                            .Any(parameter => parameter.Identifier.ValueText == typeParameter.Name) == true
                        || documentationOwner?.FirstAncestorOrSelf<DelegateDeclarationSyntax>()?.TypeParameterList?.Parameters
                            .Any(parameter => parameter.Identifier.ValueText == typeParameter.Name) == true)
                    {
                        continue;
                    }

                    foreach (var identifierName in documentationComment.DescendantNodes()
                        .OfType<CrefSyntax>()
                        .SelectMany(cref => cref.DescendantNodes().OfType<IdentifierNameSyntax>()))
                    {
                        if (!(semanticModel.GetSymbolInfo(identifierName, cancellationToken).Symbol is ITypeParameterSymbol referencedTypeParameter)
                            || referencedTypeParameter.Name != typeParameter.Name
                            || referencedTypeParameter.Ordinal != typeParameter.Ordinal)
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
