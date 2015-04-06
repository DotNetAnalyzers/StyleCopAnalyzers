namespace StyleCop.Analyzers.Helpers
{
    using System;
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
    }
}
