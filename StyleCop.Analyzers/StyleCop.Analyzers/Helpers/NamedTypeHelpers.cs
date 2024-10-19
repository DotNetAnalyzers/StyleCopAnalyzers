﻿// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Helpers
{
    using System;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    using StyleCop.Analyzers.Lightup;

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

        internal static string GetNameOrIdentifier(MemberDeclarationSyntax member)
        {
            switch (member.Kind())
            {
            case SyntaxKind.ClassDeclaration:
            case SyntaxKind.InterfaceDeclaration:
            case SyntaxKind.StructDeclaration:
            case SyntaxKindEx.RecordDeclaration:
            case SyntaxKindEx.RecordStructDeclaration:
                return ((TypeDeclarationSyntax)member).Identifier.Text;

            case SyntaxKind.EnumDeclaration:
                return ((EnumDeclarationSyntax)member).Identifier.Text;

            case SyntaxKind.DelegateDeclaration:
                return ((DelegateDeclarationSyntax)member).Identifier.Text;

            default:
                throw new ArgumentException("Unhandled declaration kind: " + member.Kind());
            }
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

        internal static bool IsPartialDeclaration(MemberDeclarationSyntax declaration)
        {
            if (declaration is TypeDeclarationSyntax typeDeclaration)
            {
                return typeDeclaration.Modifiers.Any(SyntaxKind.PartialKeyword);
            }

            return false;
        }

        /// <summary>
        /// Returns whether or not a member is implementing an interface member.
        /// </summary>
        /// <remarks>
        /// <para>This method does only check the interfaces the containing type is implementing directly.
        /// If a derived class is implementing an interface and this member is required for it
        /// this method will still return false.</para>
        /// </remarks>
        /// <param name="memberSymbol">The member symbol that should be analyzed.</param>
        /// <returns>true if the member is implementing an interface member, otherwise false.</returns>
        internal static bool IsImplementingAnInterfaceMember(ISymbol memberSymbol)
        {
            bool isImplementingExplicitly;

            // Only methods, properties and events can implement an interface member
            if (memberSymbol is IMethodSymbol methodSymbol)
            {
                // Check if the member is implementing an interface explicitly
                isImplementingExplicitly = methodSymbol.ExplicitInterfaceImplementations.Any();
            }
            else if (memberSymbol is IPropertySymbol propertySymbol)
            {
                // Check if the member is implementing an interface explicitly
                isImplementingExplicitly = propertySymbol.ExplicitInterfaceImplementations.Any();
            }
            else if (memberSymbol is IEventSymbol eventSymbol)
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

        internal static INamedTypeSymbol TupleUnderlyingTypeOrSelf(this INamedTypeSymbol tupleSymbol)
            => tupleSymbol.TupleUnderlyingType() ?? tupleSymbol;
    }
}
