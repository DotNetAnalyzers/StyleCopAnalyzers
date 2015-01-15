namespace StyleCop.Analyzers.Helpers
{
    using System;
    using Microsoft.CodeAnalysis;

    internal static class NamedTypeHelpers
    {
        internal static bool IsNativeMethodsClass(INamedTypeSymbol type)
        {
            if(type == null || type.TypeKind != TypeKind.Class)
            {
                return false;
            }

            if(type.Name != null && type.Name.EndsWith("NativeMethods", StringComparison.Ordinal))
            {
                return true;
            }

            return false;
        }

        internal static bool IsContainedInNativeMethodsClass(INamedTypeSymbol type)
        {
            if(IsNativeMethodsClass(type))
            {
                return true;
            }

            INamedTypeSymbol typeSymbol = type;
            while((typeSymbol = typeSymbol.ContainingType) != null)
            {
                if(IsNativeMethodsClass(typeSymbol))
                {
                    return true;
                }

                
            }

            return false;
        }
    }
}
