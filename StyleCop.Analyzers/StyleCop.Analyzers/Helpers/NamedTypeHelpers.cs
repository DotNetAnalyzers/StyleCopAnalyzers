namespace StyleCop.Analyzers.Helpers
{
    using System;
    using Microsoft.CodeAnalysis;

    internal class NamedTypeHelpers
    {
        internal bool IsNativeMethodsClass(INamedTypeSymbol type)
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

        internal bool IsContainedInNativeMethodsClass(INamedTypeSymbol type)
        {
            if (type == null)
            {
                return false;
            }

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
