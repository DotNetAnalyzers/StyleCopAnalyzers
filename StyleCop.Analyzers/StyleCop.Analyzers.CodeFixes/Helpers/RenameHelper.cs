// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Helpers
{
    using System.Collections.Immutable;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.Rename;

    internal static class RenameHelper
    {
        public static async Task<Solution> RenameSymbolAsync(Document document, SyntaxNode root, SyntaxToken declarationToken, string newName, CancellationToken cancellationToken)
        {
            var annotatedRoot = root.ReplaceToken(declarationToken, declarationToken.WithAdditionalAnnotations(RenameAnnotation.Create()));
            var annotatedSolution = document.Project.Solution.WithDocumentSyntaxRoot(document.Id, annotatedRoot);
            var annotatedDocument = annotatedSolution.GetDocument(document.Id);

            annotatedRoot = await annotatedDocument.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            var annotatedToken = annotatedRoot.FindToken(declarationToken.SpanStart);

            var semanticModel = await annotatedDocument.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);
            var symbol = semanticModel.GetDeclaredSymbol(annotatedToken.Parent, cancellationToken);

            var newSolution = await Renamer.RenameSymbolAsync(annotatedSolution, symbol, newName, null, cancellationToken).ConfigureAwait(false);

            // TODO: return annotatedSolution instead of newSolution if newSolution contains any new errors (for any project)
            return newSolution;
        }

        public static bool IsValidNewMemberName(SemanticModel semanticModel, ISymbol symbol, string name)
        {
            if (symbol.Kind == SymbolKind.NamedType)
            {
                TypeKind typeKind = ((INamedTypeSymbol)symbol).TypeKind;

                // If the symbol is a class or struct, the name can't be the same as any of its members.
                if (typeKind == TypeKind.Class || typeKind == TypeKind.Struct)
                {
                    var members = (symbol as INamedTypeSymbol)?.GetMembers(name);
                    if (members.HasValue && !members.Value.IsDefaultOrEmpty)
                    {
                        return false;
                    }
                }
            }

            var containingSymbol = symbol.ContainingSymbol as INamespaceOrTypeSymbol;
            if (containingSymbol == null)
            {
                return true;
            }

            if (containingSymbol.Kind == SymbolKind.Namespace)
            {
                // Make sure to use the compilation namespace so interfaces in referenced assemblies are considered
                containingSymbol = semanticModel.Compilation.GetCompilationNamespace((INamespaceSymbol)containingSymbol);
            }
            else if (containingSymbol.Kind == SymbolKind.NamedType)
            {
                TypeKind typeKind = ((INamedTypeSymbol)containingSymbol).TypeKind;

                // If the containing type is a class or struct, the name can't be the same as the name of the containing
                // type.
                if ((typeKind == TypeKind.Class || typeKind == TypeKind.Struct)
                    && containingSymbol.Name == name)
                {
                    return false;
                }
            }

            // The name can't be the same as the name of an other member of the same type. At this point no special
            // consideration is given to overloaded methods.
            ImmutableArray<ISymbol> siblings = containingSymbol.GetMembers(name);
            if (!siblings.IsDefaultOrEmpty)
            {
                return false;
            }

            return true;
        }
    }
}
