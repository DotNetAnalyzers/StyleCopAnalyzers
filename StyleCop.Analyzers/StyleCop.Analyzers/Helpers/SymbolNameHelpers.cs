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

                if (namedTypeSymbol.IsGenericType)
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
            }
            else
            {
                AppendQualifiedSymbolName(builder, symbol);
            }

            return ObjectPools.StringBuilderPool.ReturnAndFree(builder);
        }

        private static void AppendQualifiedSymbolName(StringBuilder builder, ISymbol symbol)
        {
            switch (symbol)
            {
            case IArrayTypeSymbol arraySymbol:
                builder
                    .Append(arraySymbol.ElementType.ContainingNamespace.ToDisplayString())
                    .Append(".")
                    .Append(arraySymbol.ElementType.Name)
                    .Append("[")
                    .Append(',', arraySymbol.Rank - 1)
                    .Append("]");
                break;

            default:
                builder
                    .Append(symbol.ContainingNamespace.ToDisplayString())
                    .Append(".")
                    .Append(symbol.Name);
                break;
            }
        }
    }
}
