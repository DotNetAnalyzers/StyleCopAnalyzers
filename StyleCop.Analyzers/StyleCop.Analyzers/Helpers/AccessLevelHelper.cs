// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// Contains helper methods for determining an element's access level.
    /// </summary>
    internal static class AccessLevelHelper
    {
        private static readonly Dictionary<AccessLevel, string> AccessLevelNames = new Dictionary<AccessLevel, string>
        {
            [AccessLevel.NotSpecified] = "unspecified access",
            [AccessLevel.Public] = "public",
            [AccessLevel.Internal] = "internal",
            [AccessLevel.ProtectedInternal] = "protected internal",
            [AccessLevel.Protected] = "protected",
            [AccessLevel.Private] = "private"
        };

        /// <summary>Determines the access level for the given <paramref name="modifiers"/>.</summary>
        /// <param name="modifiers">The modifiers.</param>
        /// <returns>A <see cref="AccessLevel"/> value representing the access level.</returns>
        internal static AccessLevel GetAccessLevel(SyntaxTokenList modifiers)
        {
            bool isProtected = false;
            bool isInternal = false;
            foreach (var modifier in modifiers)
            {
                switch (modifier.Kind())
                {
                case SyntaxKind.PublicKeyword:
                    return AccessLevel.Public;
                case SyntaxKind.PrivateKeyword:
                    return AccessLevel.Private;
                case SyntaxKind.InternalKeyword:
                    if (isProtected)
                    {
                        return AccessLevel.ProtectedInternal;
                    }
                    else
                    {
                        isInternal = true;
                    }

                    break;
                case SyntaxKind.ProtectedKeyword:
                    if (isInternal)
                    {
                        return AccessLevel.ProtectedInternal;
                    }
                    else
                    {
                        isProtected = true;
                    }

                    break;
                }
            }

            if (isProtected)
            {
                return AccessLevel.Protected;
            }
            else if (isInternal)
            {
                return AccessLevel.Internal;
            }

            return AccessLevel.NotSpecified;
        }

        /// <summary>Gets the name for a given access level.</summary>
        /// <param name="accessLevel">The access level.</param>
        /// <returns>The name for a given access level.</returns>
        internal static string GetName(AccessLevel accessLevel)
        {
            return AccessLevelNames[accessLevel];
        }

        /// <summary>
        /// Gets the <see cref="Accessibility"/> corresponding to a specified <see cref="AccessLevel"/> value.
        /// </summary>
        /// <param name="accessLevel">The <see cref="AccessLevel"/> to convert.</param>
        /// <returns>
        /// The <see cref="Accessibility"/> associated with the specified <see cref="AccessLevel"/> value.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// If <paramref name="accessLevel"/> is <see cref="AccessLevel.NotSpecified"/> or does not map directly to any
        /// <see cref="Accessibility"/> value.
        /// </exception>
        internal static Accessibility ToAccessibility(this AccessLevel accessLevel)
        {
            switch (accessLevel)
            {
            case AccessLevel.Public:
                return Accessibility.Public;

            case AccessLevel.Internal:
                return Accessibility.Internal;

            case AccessLevel.ProtectedInternal:
                return Accessibility.ProtectedOrInternal;

            case AccessLevel.Protected:
                return Accessibility.Protected;

            case AccessLevel.Private:
                return Accessibility.Private;

            case AccessLevel.NotSpecified:
            default:
                throw new ArgumentException($"'AccessLevel.{accessLevel}' does not have a corresponding 'Accessibility' value.");
            }
        }

        internal static Accessibility GetDeclaredAccessibility(this BaseTypeDeclarationSyntax syntax, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            Requires.NotNull(syntax, nameof(syntax));
            Requires.NotNull(semanticModel, nameof(semanticModel));

            AccessLevel accessLevel = GetAccessLevel(syntax.Modifiers);
            if (accessLevel != AccessLevel.NotSpecified)
            {
                return accessLevel.ToAccessibility();
            }

            if (!syntax.Modifiers.Any(SyntaxKind.PartialKeyword))
            {
                BaseTypeDeclarationSyntax enclosingType = syntax.Parent as BaseTypeDeclarationSyntax;
                return enclosingType == null ? Accessibility.Internal : Accessibility.Private;
            }

            INamedTypeSymbol declaredSymbol = semanticModel.GetDeclaredSymbol(syntax, cancellationToken);
            return declaredSymbol?.DeclaredAccessibility ?? Accessibility.NotApplicable;
        }

        internal static Accessibility GetDeclaredAccessibility(this BaseMethodDeclarationSyntax syntax, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            Requires.NotNull(syntax, nameof(syntax));
            Requires.NotNull(semanticModel, nameof(semanticModel));

            AccessLevel accessLevel = GetAccessLevel(syntax.Modifiers);
            if (accessLevel != AccessLevel.NotSpecified)
            {
                return accessLevel.ToAccessibility();
            }

            if (syntax.Modifiers.Any(SyntaxKind.PartialKeyword))
            {
                return Accessibility.Private;
            }

            MethodDeclarationSyntax methodDeclarationSyntax = syntax as MethodDeclarationSyntax;
            if (methodDeclarationSyntax != null)
            {
                if (methodDeclarationSyntax.ExplicitInterfaceSpecifier == null)
                {
                    return Accessibility.Private;
                }
                else
                {
                    return Accessibility.Public;
                }
            }

            if (syntax.IsKind(SyntaxKind.ConstructorDeclaration))
            {
                return Accessibility.Private;
            }

            IMethodSymbol declaredSymbol = semanticModel.GetDeclaredSymbol(syntax, cancellationToken);
            return declaredSymbol?.DeclaredAccessibility ?? Accessibility.NotApplicable;
        }

        internal static Accessibility GetDeclaredAccessibility(this BasePropertyDeclarationSyntax syntax, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            Requires.NotNull(syntax, nameof(syntax));
            Requires.NotNull(semanticModel, nameof(semanticModel));

            AccessLevel accessLevel = GetAccessLevel(syntax.Modifiers);
            if (accessLevel != AccessLevel.NotSpecified)
            {
                return accessLevel.ToAccessibility();
            }

            PropertyDeclarationSyntax propertyDeclarationSyntax = syntax as PropertyDeclarationSyntax;
            if (propertyDeclarationSyntax != null)
            {
                if (propertyDeclarationSyntax.ExplicitInterfaceSpecifier == null)
                {
                    return Accessibility.Private;
                }
                else
                {
                    return Accessibility.Public;
                }
            }

            IndexerDeclarationSyntax indexerDeclarationSyntax = syntax as IndexerDeclarationSyntax;
            if (indexerDeclarationSyntax != null)
            {
                if (indexerDeclarationSyntax.ExplicitInterfaceSpecifier == null)
                {
                    return Accessibility.Private;
                }
                else
                {
                    return Accessibility.Public;
                }
            }

            EventDeclarationSyntax eventDeclarationSyntax = syntax as EventDeclarationSyntax;
            if (eventDeclarationSyntax != null)
            {
                if (eventDeclarationSyntax.ExplicitInterfaceSpecifier == null)
                {
                    return Accessibility.Private;
                }
                else
                {
                    return Accessibility.Public;
                }
            }

            ISymbol declaredSymbol = semanticModel.GetDeclaredSymbol(syntax, cancellationToken);
            return declaredSymbol?.DeclaredAccessibility ?? Accessibility.NotApplicable;
        }

        internal static Accessibility GetDeclaredAccessibility(this BaseFieldDeclarationSyntax syntax, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            Requires.NotNull(syntax, nameof(syntax));
            Requires.NotNull(semanticModel, nameof(semanticModel));

            AccessLevel accessLevel = GetAccessLevel(syntax.Modifiers);
            if (accessLevel != AccessLevel.NotSpecified)
            {
                return accessLevel.ToAccessibility();
            }

            if (syntax.IsKind(SyntaxKind.FieldDeclaration) || syntax.IsKind(SyntaxKind.EventFieldDeclaration))
            {
                return Accessibility.Private;
            }

            VariableDeclaratorSyntax firstVariable = syntax.Declaration?.Variables.FirstOrDefault();
            if (firstVariable == null)
            {
                return Accessibility.NotApplicable;
            }

            ISymbol declaredSymbol = semanticModel.GetDeclaredSymbol(firstVariable, cancellationToken);
            return declaredSymbol?.DeclaredAccessibility ?? Accessibility.NotApplicable;
        }

        internal static Accessibility GetDeclaredAccessibility(this EnumMemberDeclarationSyntax syntax)
        {
            return Accessibility.Public;
        }

        internal static Accessibility GetDeclaredAccessibility(this DelegateDeclarationSyntax syntax, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            Requires.NotNull(syntax, nameof(syntax));
            Requires.NotNull(semanticModel, nameof(semanticModel));

            AccessLevel accessLevel = GetAccessLevel(syntax.Modifiers);
            if (accessLevel != AccessLevel.NotSpecified)
            {
                return accessLevel.ToAccessibility();
            }

            BaseTypeDeclarationSyntax enclosingType = syntax.Parent as BaseTypeDeclarationSyntax;
            return enclosingType == null ? Accessibility.Internal : Accessibility.Private;
        }

        internal static Accessibility GetEffectiveAccessibility(this BaseTypeDeclarationSyntax syntax, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            Requires.NotNull(syntax, nameof(syntax));
            Requires.NotNull(semanticModel, nameof(semanticModel));

            Accessibility declaredAccessibility = syntax.GetDeclaredAccessibility(semanticModel, cancellationToken);
            if (declaredAccessibility <= Accessibility.Private)
            {
                return declaredAccessibility;
            }

            BaseTypeDeclarationSyntax enclosingType = syntax.Parent as BaseTypeDeclarationSyntax;
            if (enclosingType == null)
            {
                return declaredAccessibility;
            }

            Accessibility enclosingAccessibility = enclosingType.GetEffectiveAccessibility(semanticModel, cancellationToken);
            return CombineEffectiveAccessibility(declaredAccessibility, enclosingAccessibility);
        }

        internal static Accessibility GetEffectiveAccessibility(this BaseMethodDeclarationSyntax syntax, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            Requires.NotNull(syntax, nameof(syntax));
            Requires.NotNull(semanticModel, nameof(semanticModel));

            Accessibility declaredAccessibility = syntax.GetDeclaredAccessibility(semanticModel, cancellationToken);
            if (declaredAccessibility <= Accessibility.Private)
            {
                return declaredAccessibility;
            }

            BaseTypeDeclarationSyntax enclosingType = syntax.Parent as BaseTypeDeclarationSyntax;
            if (enclosingType == null)
            {
                return declaredAccessibility;
            }

            Accessibility enclosingAccessibility = enclosingType.GetEffectiveAccessibility(semanticModel, cancellationToken);
            return CombineEffectiveAccessibility(declaredAccessibility, enclosingAccessibility);
        }

        internal static Accessibility GetEffectiveAccessibility(this BasePropertyDeclarationSyntax syntax, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            Requires.NotNull(syntax, nameof(syntax));
            Requires.NotNull(semanticModel, nameof(semanticModel));

            Accessibility declaredAccessibility = syntax.GetDeclaredAccessibility(semanticModel, cancellationToken);
            if (declaredAccessibility <= Accessibility.Private)
            {
                return declaredAccessibility;
            }

            BaseTypeDeclarationSyntax enclosingType = syntax.Parent as BaseTypeDeclarationSyntax;
            if (enclosingType == null)
            {
                return declaredAccessibility;
            }

            Accessibility enclosingAccessibility = enclosingType.GetEffectiveAccessibility(semanticModel, cancellationToken);
            return CombineEffectiveAccessibility(declaredAccessibility, enclosingAccessibility);
        }

        internal static Accessibility GetEffectiveAccessibility(this BaseFieldDeclarationSyntax syntax, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            Requires.NotNull(syntax, nameof(syntax));
            Requires.NotNull(semanticModel, nameof(semanticModel));

            Accessibility declaredAccessibility = syntax.GetDeclaredAccessibility(semanticModel, cancellationToken);
            if (declaredAccessibility <= Accessibility.Private)
            {
                return declaredAccessibility;
            }

            BaseTypeDeclarationSyntax enclosingType = syntax.Parent as BaseTypeDeclarationSyntax;
            if (enclosingType == null)
            {
                return declaredAccessibility;
            }

            Accessibility enclosingAccessibility = enclosingType.GetEffectiveAccessibility(semanticModel, cancellationToken);
            return CombineEffectiveAccessibility(declaredAccessibility, enclosingAccessibility);
        }

        internal static Accessibility GetEffectiveAccessibility(this EnumMemberDeclarationSyntax syntax, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            Requires.NotNull(syntax, nameof(syntax));
            Requires.NotNull(semanticModel, nameof(semanticModel));

            Accessibility declaredAccessibility = Accessibility.Public;

            BaseTypeDeclarationSyntax enclosingType = (BaseTypeDeclarationSyntax)syntax.Parent;
            Accessibility enclosingAccessibility = enclosingType.GetEffectiveAccessibility(semanticModel, cancellationToken);
            return CombineEffectiveAccessibility(declaredAccessibility, enclosingAccessibility);
        }

        internal static Accessibility GetEffectiveAccessibility(this DelegateDeclarationSyntax syntax, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            Requires.NotNull(syntax, nameof(syntax));
            Requires.NotNull(semanticModel, nameof(semanticModel));

            Accessibility declaredAccessibility = syntax.GetDeclaredAccessibility(semanticModel, cancellationToken);
            if (declaredAccessibility <= Accessibility.Private)
            {
                return declaredAccessibility;
            }

            BaseTypeDeclarationSyntax enclosingType = syntax.Parent as BaseTypeDeclarationSyntax;
            if (enclosingType == null)
            {
                return Accessibility.Internal;
            }

            Accessibility enclosingAccessibility = enclosingType.GetEffectiveAccessibility(semanticModel, cancellationToken);
            return CombineEffectiveAccessibility(declaredAccessibility, enclosingAccessibility);
        }

        internal static Accessibility CombineEffectiveAccessibility(Accessibility declaredAccessibility, Accessibility enclosingAccessibility)
        {
            switch (enclosingAccessibility)
            {
            case Accessibility.NotApplicable:
            case Accessibility.Private:
                return enclosingAccessibility;

            case Accessibility.ProtectedAndInternal:
                switch (declaredAccessibility)
                {
                case Accessibility.NotApplicable:
                case Accessibility.Private:
                    return declaredAccessibility;

                case Accessibility.ProtectedAndInternal:
                case Accessibility.Internal:
                case Accessibility.Protected:
                case Accessibility.ProtectedOrInternal:
                case Accessibility.Public:
                default:
                    return Accessibility.ProtectedAndInternal;
                }

            case Accessibility.Protected:
                switch (declaredAccessibility)
                {
                case Accessibility.NotApplicable:
                case Accessibility.Private:
                    return declaredAccessibility;

                case Accessibility.ProtectedAndInternal:
                case Accessibility.Internal:
                    return Accessibility.ProtectedAndInternal;

                case Accessibility.Protected:
                case Accessibility.ProtectedOrInternal:
                case Accessibility.Public:
                default:
                    return Accessibility.Protected;
                }

            case Accessibility.Internal:
                switch (declaredAccessibility)
                {
                case Accessibility.NotApplicable:
                case Accessibility.Private:
                    return declaredAccessibility;

                case Accessibility.ProtectedAndInternal:
                case Accessibility.Protected:
                    return Accessibility.ProtectedAndInternal;

                case Accessibility.Internal:
                case Accessibility.ProtectedOrInternal:
                case Accessibility.Public:
                default:
                    return Accessibility.Internal;
                }

            case Accessibility.ProtectedOrInternal:
                switch (declaredAccessibility)
                {
                case Accessibility.NotApplicable:
                case Accessibility.Private:
                case Accessibility.ProtectedAndInternal:
                case Accessibility.Protected:
                case Accessibility.Internal:
                case Accessibility.ProtectedOrInternal:
                    return declaredAccessibility;

                case Accessibility.Public:
                default:
                    return Accessibility.ProtectedOrInternal;
                }

            case Accessibility.Public:
            default:
                return declaredAccessibility;
            }
        }
    }
}
