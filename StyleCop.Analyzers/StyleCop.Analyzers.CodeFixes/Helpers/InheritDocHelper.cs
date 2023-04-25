// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Helpers
{
    using System.Threading;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal static class InheritDocHelper
    {
        /// <summary>
        /// Determines whether is covered by inherit document.
        /// </summary>
        /// <param name="semanticModel">The semantic model.</param>
        /// <param name="methodDeclaration">The method declaration.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns> True if covered by inherit document.</returns>
        public static bool IsCoveredByInheritDoc(SemanticModel semanticModel, MethodDeclarationSyntax methodDeclaration, CancellationToken cancellationToken)
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

        /// <summary>
        /// Determines whether is covered by inherit document.
        /// </summary>
        /// <param name="semanticModel">The semantic model.</param>
        /// <param name="propertyDeclaration">The property declaration.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns> True if covered by inherit document.</returns>
        public static bool IsCoveredByInheritDoc(SemanticModel semanticModel, PropertyDeclarationSyntax propertyDeclaration, CancellationToken cancellationToken)
        {
            if (propertyDeclaration.ExplicitInterfaceSpecifier != null)
            {
                return true;
            }

            if (propertyDeclaration.Modifiers.Any(SyntaxKind.OverrideKeyword))
            {
                return true;
            }

            ISymbol declaredSymbol = semanticModel.GetDeclaredSymbol(propertyDeclaration, cancellationToken);
            return (declaredSymbol != null) && NamedTypeHelpers.IsImplementingAnInterfaceMember(declaredSymbol);
        }
    }
}
