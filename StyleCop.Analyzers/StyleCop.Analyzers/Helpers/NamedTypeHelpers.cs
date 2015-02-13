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
                return false;

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
                    return true;

                syntax = syntax?.Parent;
            }

            return false;
        }

        internal static string GetTypeName(this TypeDeclarationSyntax typeDeclaration)
        {
            if (typeDeclaration is ClassDeclarationSyntax)
            {
                return ((ClassDeclarationSyntax)typeDeclaration).Identifier.ToString();
            }
            if (typeDeclaration is StructDeclarationSyntax)
            {
                return ((StructDeclarationSyntax)typeDeclaration).Identifier.ToString();
            }
            if (typeDeclaration is InterfaceDeclarationSyntax)
            {
                return ((InterfaceDeclarationSyntax)typeDeclaration).Identifier.ToString();
            }

            return null;
        }
    }
}
