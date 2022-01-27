// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Helpers
{
    using System.Threading;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal static class PropertyAnalyzerHelper
    {
        /// <summary>
        /// Analyzes the indexer accessors.
        /// </summary>
        /// <param name="indexerDeclaration">The indexer declaration.</param>
        /// <param name="semanticModel">The semantic model.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The property data.</returns>
        public static PropertyData AnalyzeIndexerAccessors(
            IndexerDeclarationSyntax indexerDeclaration,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            return AnalyzePropertyAccessors(indexerDeclaration, indexerDeclaration.ExpressionBody != null, semanticModel, cancellationToken);
        }

        /// <summary>
        /// Analyzes the property accessors.
        /// </summary>
        /// <param name="propertyDeclaration">The property declaration.</param>
        /// <param name="semanticModel">The semantic model.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The property data.</returns>
        public static PropertyData AnalyzePropertyAccessors(
            PropertyDeclarationSyntax propertyDeclaration,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            return AnalyzePropertyAccessors(propertyDeclaration, propertyDeclaration.ExpressionBody != null, semanticModel, cancellationToken);
        }

        private static PropertyData AnalyzePropertyAccessors(
            BasePropertyDeclarationSyntax propertyDeclaration,
            bool hasExpressionBody,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            AccessorDeclarationSyntax getter = null;
            AccessorDeclarationSyntax setter = null;

            if (propertyDeclaration.AccessorList != null)
            {
                foreach (var accessor in propertyDeclaration.AccessorList.Accessors)
                {
                    switch (accessor.Keyword.Kind())
                    {
                    case SyntaxKind.GetKeyword:
                        getter = accessor;
                        break;

                    case SyntaxKind.SetKeyword:
                        setter = accessor;
                        break;
                    }
                }
            }

            bool getterVisible, setterVisible;
            if (getter != null && setter != null)
            {
                if (!getter.Modifiers.Any() && !setter.Modifiers.Any())
                {
                    // The getter and setter have the same declared accessibility
                    getterVisible = true;
                    setterVisible = true;
                }
                else if (getter.Modifiers.Any(SyntaxKind.PrivateKeyword))
                {
                    getterVisible = false;
                    setterVisible = true;
                }
                else if (setter.Modifiers.Any(SyntaxKind.PrivateKeyword))
                {
                    getterVisible = true;
                    setterVisible = false;
                }
                else
                {
                    var propertyAccessibility = propertyDeclaration.GetEffectiveAccessibility(semanticModel, cancellationToken);
                    bool propertyOnlyInternal = propertyAccessibility == Accessibility.Internal
                                                || propertyAccessibility == Accessibility.ProtectedAndInternal
                                                || propertyAccessibility == Accessibility.Private;
                    if (propertyOnlyInternal)
                    {
                        // Property only internal and no accessor is explicitly private
                        getterVisible = true;
                        setterVisible = true;
                    }
                    else
                    {
                        var getterAccessibility = getter.GetEffectiveAccessibility(semanticModel, cancellationToken);
                        var setterAccessibility = setter.GetEffectiveAccessibility(semanticModel, cancellationToken);

                        switch (getterAccessibility)
                        {
                        case Accessibility.Public:
                        case Accessibility.ProtectedOrInternal:
                        case Accessibility.Protected:
                            getterVisible = true;
                            break;

                        case Accessibility.Internal:
                        case Accessibility.ProtectedAndInternal:
                        case Accessibility.Private:
                        default:
                            // The property is externally accessible, so the setter must be more accessible.
                            getterVisible = false;
                            break;
                        }

                        switch (setterAccessibility)
                        {
                        case Accessibility.Public:
                        case Accessibility.ProtectedOrInternal:
                        case Accessibility.Protected:
                            setterVisible = true;
                            break;

                        case Accessibility.Internal:
                        case Accessibility.ProtectedAndInternal:
                        case Accessibility.Private:
                        default:
                            // The property is externally accessible, so the getter must be more accessible.
                            setterVisible = false;
                            break;
                        }
                    }
                }
            }
            else
            {
                if (getter != null || hasExpressionBody)
                {
                    getterVisible = true;
                    setterVisible = false;
                }
                else
                {
                    getterVisible = false;
                    setterVisible = setter != null;
                }
            }

            return new PropertyData(setter != null, setterVisible, getter != null, getterVisible);
        }

        public struct PropertyData
        {
            public PropertyData(
                bool hasSetter,
                bool setterVisible,
                bool hasGetter,
                bool getterVisible)
            {
                this.HasSetter = hasSetter;
                this.SetterVisible = setterVisible;
                this.HasGetter = hasGetter;
                this.GetterVisible = getterVisible;
            }

            public bool HasSetter { get; }

            public bool SetterVisible { get; }

            public bool HasGetter { get; }

            public bool GetterVisible { get; }
        }
    }
}
