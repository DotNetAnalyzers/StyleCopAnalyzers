// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Helpers
{
    using System.Reflection;
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

        /// <summary>
        /// Generates the fully qualified System.ValueTuple based name for the given tuple type.
        /// </summary>
        /// <param name="tupleSymbol">The tuple symbol.</param>
        /// <returns>The generated fully qualified display string.</returns>
        public static string ToFullyQualifiedValueTupleDisplayString(this INamedTypeSymbol tupleSymbol)
        {
            var tupleElements = tupleSymbol.TupleElements();
            if (tupleElements.IsDefault)
            {
                // If the tuple elements API is not available, the default formatting will produce System.ValueTuple and not the C# tuple format.
                return tupleSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            }
            else
            {
                // workaround for SymbolDisplayCompilerInternalOptions.UseValueTuple not being available to us.
                var builder = ObjectPools.StringBuilderPool.Allocate();

                builder.Append("global::System.ValueTuple<");

                for (var i = 0; i < tupleElements.Length; i++)
                {
                    if (i > 0)
                    {
                        builder.Append(", ");
                    }

                    builder.Append(tupleElements[i].Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat));
                }

                builder.Append(">");

                return ObjectPools.StringBuilderPool.ReturnAndFree(builder);
            }
        }

        private static bool AppendQualifiedSymbolName(StringBuilder builder, ISymbol symbol, TypeSyntax type)
        {
            switch (symbol.Kind)
            {
            case SymbolKind.ArrayType:
                var arraySymbol = (IArrayTypeSymbol)symbol;
                AppendQualifiedSymbolName(builder, arraySymbol.ElementType, GetElementSyntax(type));
                builder
                    .Append("[")
                    .Append(',', arraySymbol.Rank - 1)
                    .Append("]");

                AppendNullableSuffixIfNeeded(builder, type);
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

                if (SpecialTypeHelper.TryGetPredefinedType(namedTypeSymbol.SpecialType, out var specialTypeSyntax) &&
                    (type?.IsKind(SyntaxKind.PredefinedType) == true
                    || (type is NullableTypeSyntax nullable && nullable.ElementType.IsKind(SyntaxKind.PredefinedType))))
                {
                    // This handles these cases: int, int?, object, object?
                    // But not these cases: System.Int32, System.Int32?, System.Object, System.Object?
                    builder.Append(specialTypeSyntax.ToFullString());
                    AppendNullableSuffixIfNeeded(builder, type);
                    return true;
                }
                else if (namedTypeSymbol.IsTupleType())
                {
                    return AppendTupleType(builder, namedTypeSymbol, type);
                }
                else if (namedTypeSymbol.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T &&
                    type?.IsKind(SyntaxKind.NullableType) == true)
                {
                    // This handles the case '(int, int)?' but not 'System.Nullable<(int, int)>'
                    AppendQualifiedSymbolName(builder, namedTypeSymbol.TypeArguments[0], GetElementSyntax(type));
                    builder.Append("?");
                    return true;
                }
                else
                {
                    return AppendNamedType(builder, namedTypeSymbol, type);
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

        private static bool AppendNamedType(StringBuilder builder, INamedTypeSymbol namedTypeSymbol, TypeSyntax type)
        {
            if (AppendQualifiedSymbolName(builder, namedTypeSymbol.ContainingSymbol, (type as QualifiedNameSyntax)?.Left))
            {
                builder.Append(".");
            }

            builder.Append(namedTypeSymbol.Name);
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

            AppendNullableSuffixIfNeeded(builder, type);
            return true;
        }

        private static bool AppendTupleType(StringBuilder builder, INamedTypeSymbol namedTypeSymbol, TypeSyntax type)
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
                AppendNullableSuffixIfNeeded(builder, type);
                return true;
            }
            else
            {
				return AppendNamedType(builder, namedTypeSymbol.TupleUnderlyingTypeOrSelf(), type);
            }
        }

        private static TypeSyntax GetElementSyntax(TypeSyntax typeSyntax)
        {
            return typeSyntax switch
            {
                ArrayTypeSyntax array => array.ElementType,

                NullableTypeSyntax nullable =>
                    nullable.ElementType switch
                    {
                        ArrayTypeSyntax array => array.ElementType,
                        _ => nullable.ElementType,
                    },

                _ => null,
            };
        }

        private static void AppendNullableSuffixIfNeeded(StringBuilder builder, TypeSyntax type)
        {
            if (type?.IsKind(SyntaxKind.NullableType) == true)
            {
                builder.Append("?");
            }
        }
    }
}
