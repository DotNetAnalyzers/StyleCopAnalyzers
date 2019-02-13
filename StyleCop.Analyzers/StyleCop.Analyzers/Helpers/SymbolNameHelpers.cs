// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Helpers
{
    using System.Text;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using StyleCop.Analyzers.Lightup;

    /// <summary>
    /// Contains helper methods to work with symbol names consistently over different C# versions.
    /// </summary>
    internal static class SymbolNameHelpers
    {
        private const string GenericTypeParametersOpen = "<";
        private const string GenericTypeParametersClose = ">";
        private const string GenericSeparator = ", ";

        /// <summary>
        /// Generates the qualified name for the given symbol.
        /// </summary>
        /// <param name="symbol">The symbol to use.</param>
        /// <returns>The generated qualified name.</returns>
        public static string ToQualifiedString(this ISymbol symbol)
        {
            var builder = ObjectPools.StringBuilderPool.Allocate();

            if (symbol is INamedTypeSymbol namedTypeSymbol)
            {
                if (SpecialTypeHelper.TryGetPredefinedType(namedTypeSymbol.SpecialType, out PredefinedTypeSyntax specialTypeSyntax))
                {
                    return specialTypeSyntax.ToFullString();
                }

                if (namedTypeSymbol.IsTupleType())
                {
                    namedTypeSymbol = namedTypeSymbol.TupleUnderlyingType();
                }

                AppendQualifiedSymbolName(builder, namedTypeSymbol);
            }
            else
            {
                AppendQualifiedSymbolName(builder, symbol);
            }

            return ObjectPools.StringBuilderPool.ReturnAndFree(builder);
        }

        private static bool AppendQualifiedSymbolName(StringBuilder builder, ISymbol symbol)
        {
            switch (symbol)
            {
            case IArrayTypeSymbol arraySymbol:
                AppendQualifiedSymbolName(builder, arraySymbol.ElementType);
                builder
                    .Append("[")
                    .Append(',', arraySymbol.Rank - 1)
                    .Append("]");
                return true;

            case INamespaceSymbol namespaceSymbol:
                if (namespaceSymbol.IsGlobalNamespace)
                {
                    return false;
                }

                builder.Append(namespaceSymbol.ToDisplayString());
                return true;

            case INamedTypeSymbol namedTypeSymbol:
                if (AppendQualifiedSymbolName(builder, symbol.ContainingSymbol))
                {
                    builder.Append(".");
                }

                builder.Append(symbol.Name);
                if (namedTypeSymbol.IsGenericType && !namedTypeSymbol.TypeArguments.IsEmpty)
                {
                    builder.Append(GenericTypeParametersOpen);

                    foreach (var typeArgument in namedTypeSymbol.TypeArguments)
                    {
                        if (typeArgument is INamedTypeSymbol namedTypeArgument && typeArgument.IsTupleType())
                        {
                            builder.Append(namedTypeArgument.TupleUnderlyingType().ToQualifiedString());
                        }
                        else
                        {
                            builder.Append(typeArgument.ToQualifiedString());
                        }

                        builder.Append(GenericSeparator);
                    }

                    builder.Remove(builder.Length - GenericSeparator.Length, GenericSeparator.Length);
                    builder.Append(GenericTypeParametersClose);
                }

                return true;

            default:
                builder.Append(symbol.Name);
                return true;
            }
        }
    }
}
