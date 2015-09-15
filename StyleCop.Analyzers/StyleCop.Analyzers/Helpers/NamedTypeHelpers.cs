// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Helpers
{
    using System;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal static class NamedTypeHelpers
    {
        internal static bool IsNativeMethodsClass(INamedTypeSymbol type)
        {
            if (type == null || type.TypeKind != TypeKind.Class)
            {
                return false;
            }

            if (type.Name != null && type.Name.EndsWith("NativeMethods", StringComparison.Ordinal))
            {
                return true;
            }

            return false;
        }

        internal static bool IsNativeMethodsClass(ClassDeclarationSyntax syntax)
        {
            string name = syntax?.Identifier.ValueText;
            if (name == null)
            {
                return false;
            }

            return name.EndsWith("NativeMethods", StringComparison.Ordinal);
        }

        internal static bool IsContainedInNativeMethodsClass(INamedTypeSymbol type)
        {
            if (type == null)
            {
                return false;
            }

            if (IsNativeMethodsClass(type))
            {
                return true;
            }

            INamedTypeSymbol typeSymbol = type;
            while ((typeSymbol = typeSymbol.ContainingType) != null)
            {
                if (IsNativeMethodsClass(typeSymbol))
                {
                    return true;
                }
            }

            return false;
        }

        internal static bool IsContainedInNativeMethodsClass(SyntaxNode syntax)
        {
            while (syntax != null)
            {
                ClassDeclarationSyntax classDeclarationSyntax = syntax.FirstAncestorOrSelf<ClassDeclarationSyntax>();
                if (IsNativeMethodsClass(classDeclarationSyntax))
                {
                    return true;
                }

                syntax = syntax?.Parent;
            }

            return false;
        }

        internal static Location GetNameOrIdentifierLocation(SyntaxNode member)
        {
            Location location = null;
            location = location ?? (member as PropertyDeclarationSyntax)?.Identifier.GetLocation();
            location = location ?? (member as FieldDeclarationSyntax)?.Declaration?.Variables.FirstOrDefault()?.Identifier.GetLocation();
            location = location ?? (member as MethodDeclarationSyntax)?.Identifier.GetLocation();
            location = location ?? (member as ConstructorDeclarationSyntax)?.Identifier.GetLocation();
            location = location ?? (member as DestructorDeclarationSyntax)?.Identifier.GetLocation();
            location = location ?? (member as BaseTypeDeclarationSyntax)?.Identifier.GetLocation();
            location = location ?? (member as NamespaceDeclarationSyntax)?.Name.GetLocation();
            location = location ?? (member as UsingDirectiveSyntax)?.Name.GetLocation();
            location = location ?? (member as ExternAliasDirectiveSyntax)?.Identifier.GetLocation();
            location = location ?? (member as AccessorDeclarationSyntax)?.Keyword.GetLocation();
            location = location ?? (member as DelegateDeclarationSyntax)?.Identifier.GetLocation();
            location = location ?? (member as EventDeclarationSyntax)?.Identifier.GetLocation();
            location = location ?? (member as IndexerDeclarationSyntax)?.ThisKeyword.GetLocation();
            location = location ?? member.GetLocation();
            return location;
        }

        /// <summary>
        /// Returns whether or not a member is implementing an interface member.
        /// </summary>
        /// <remarks>
        /// This method does only check the interfaces the containing type is implementing directly.
        /// If a derived class is implementing an interface and this member is required for it
        /// this method will still return false.
        /// </remarks>
        /// <param name="memberSymbol">The member symbol that should be analyzed.</param>
        /// <returns>true if the member is implementing an interface member, otherwise false.</returns>
        internal static bool IsImplementingAnInterfaceMember(ISymbol memberSymbol)
        {
            if (memberSymbol.IsStatic)
            {
                return false;
            }

            IMethodSymbol methodSymbol;
            IPropertySymbol propertySymbol;
            IEventSymbol eventSymbol;
            bool isImplementingExplicitly;

            // Only methods, properties and events can implement an interface member
            if ((methodSymbol = memberSymbol as IMethodSymbol) != null)
            {
                // Check if the member is implementing an interface explicitly
                isImplementingExplicitly = methodSymbol.ExplicitInterfaceImplementations.Any();
            }
            else if ((propertySymbol = memberSymbol as IPropertySymbol) != null)
            {
                // Check if the member is implementing an interface explicitly
                isImplementingExplicitly = propertySymbol.ExplicitInterfaceImplementations.Any();
            }
            else if ((eventSymbol = memberSymbol as IEventSymbol) != null)
            {
                // Check if the member is implementing an interface explicitly
                isImplementingExplicitly = eventSymbol.ExplicitInterfaceImplementations.Any();
            }
            else
            {
                return false;
            }

            if (isImplementingExplicitly)
            {
                return true;
            }

            var typeSymbol = memberSymbol.ContainingType;

            return typeSymbol != null && typeSymbol.AllInterfaces
                .SelectMany(m => m.GetMembers(memberSymbol.Name))
                .Select(typeSymbol.FindImplementationForInterfaceMember)
                .Any(x => memberSymbol.Equals(x));
        }
    }
}
