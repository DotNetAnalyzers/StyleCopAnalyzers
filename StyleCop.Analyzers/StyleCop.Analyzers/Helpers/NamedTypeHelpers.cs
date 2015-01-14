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
    }
}
