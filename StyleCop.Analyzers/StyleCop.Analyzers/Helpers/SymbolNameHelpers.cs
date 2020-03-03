// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Helpers
{
    using System.Text;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
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

        private const string TupleTypeOpen = "(";
        private const string TupleTypeClose = ")";
        private const string TupleElementSeparator = ", ";

        /// <summary>
        /// Generates the qualified name for the given symbol.
        /// </summary>
        /// <param name="symbol">The symbol to use.</param>
        /// <param name="name">The syntax node which resolves to the symbol.</param>
        /// <returns>The generated qualified name.</returns>
        public static string ToQualifiedString(this ISymbol symbol, NameSyntax name)
        {
            var builder = ObjectPools.StringBuilderPool.Allocate();
            AppendQualifiedSymbolName(builder, symbol, name);
            return ObjectPools.StringBuilderPool.ReturnAndFree(builder);
        }

        private static bool AppendQualifiedSymbolName(StringBuilder builder, ISymbol symbol, TypeSyntax type)
        {
            switch (symbol.Kind)
            {
            case SymbolKind.ArrayType:
                var arraySymbol = (IArrayTypeSymbol)symbol;
                AppendQualifiedSymbolName(builder, arraySymbol.ElementType, (type as ArrayTypeSyntax)?.ElementType);
                builder
                    .Append("[")
                    .Append(',', arraySymbol.Rank - 1)
                    .Append("]");
                return true;

            case SymbolKind.Namespace:
                var namespaceSymbol = (INamespaceSymbol)symbol;
                if (namespaceSymbol.IsGlobalNamespace)
                {
                    return false;
                }

                builder.Append(namespaceSymbol.ToDisplayString());
                return true;

            case SymbolKind.NamedType:
                var namedTypeSymbol = (INamedTypeSymbol)symbol;
                if (SpecialTypeHelper.TryGetPredefinedType(namedTypeSymbol.SpecialType, out var specialTypeSyntax))
                {
                    builder.Append(specialTypeSyntax.ToFullString());
                    return true;
                }
                else if (namedTypeSymbol.IsTupleType())
                {
                    if (TupleTypeSyntaxWrapper.IsInstance(type))
                    {
                        var tupleType = (TupleTypeSyntaxWrapper)type;

                        builder.Append(TupleTypeOpen);
                        var elements = namedTypeSymbol.TupleElements();
                        for (int i = 0; i < elements.Length; i++)
                        {
                            var field = elements[i];
                            var fieldType = tupleType.Elements.Count > i ? tupleType.Elements[i] : default;

                            if (i > 0)
                            {
                                builder.Append(TupleElementSeparator);
                            }

                            AppendQualifiedSymbolName(builder, field.Type, fieldType.Type);
                            if (field != field.CorrespondingTupleField())
                            {
                                builder.Append(" ").Append(field.Name);
                            }
                        }

                        builder.Append(TupleTypeClose);
                        return true;
                    }
                    else
                    {
                        return AppendQualifiedSymbolName(builder, namedTypeSymbol.TupleUnderlyingType(), type);
                    }
                }
                else if (namedTypeSymbol.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T)
                {
                    AppendQualifiedSymbolName(builder, namedTypeSymbol.TypeArguments[0], (type as NullableTypeSyntax)?.ElementType);
                    builder.Append("?");
                    return true;
                }
                else
                {
                    if (AppendQualifiedSymbolName(builder, symbol.ContainingSymbol, (type as QualifiedNameSyntax)?.Left))
                    {
                        builder.Append(".");
                    }

                    builder.Append(symbol.Name);
                    if (namedTypeSymbol.IsGenericType && !namedTypeSymbol.TypeArguments.IsEmpty)
                    {
                        builder.Append(GenericTypeParametersOpen);
                        var arguments = namedTypeSymbol.TypeArguments;
                        var argumentTypes = type is QualifiedNameSyntax qualifiedName
                            ? (qualifiedName.Right as GenericNameSyntax)?.TypeArgumentList
                            : (type as GenericNameSyntax)?.TypeArgumentList;

                        for (int i = 0; i < arguments.Length; i++)
                        {
                            var argument = arguments[i];
                            var argumentType = argumentTypes != null && argumentTypes.Arguments.Count > i ? argumentTypes.Arguments[i] : null;

                            if (i > 0)
                            {
                                builder.Append(GenericSeparator);
                            }

                            if (!argumentType.IsKind(SyntaxKind.OmittedTypeArgument))
                            {
                                AppendQualifiedSymbolName(builder, argument, argumentType);
                            }
                        }

                        builder.Append(GenericTypeParametersClose);
                    }

                    return true;
                }

            default:
                if (symbol != null)
                {
                    builder.Append(symbol.Name);
                    return true;
                }

                return false;
            }
        }
    }
}
