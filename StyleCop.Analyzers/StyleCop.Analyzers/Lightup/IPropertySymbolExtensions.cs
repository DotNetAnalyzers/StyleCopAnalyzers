// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using Microsoft.CodeAnalysis;

    internal static class IPropertySymbolExtensions
    {
        private static readonly Func<IPropertySymbol, IPropertySymbol> PartialDefinitionPartAccessor;
        private static readonly Func<IPropertySymbol, IPropertySymbol> PartialImplementationPartAccessor;

        static IPropertySymbolExtensions()
        {
            PartialDefinitionPartAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<IPropertySymbol, IPropertySymbol>(typeof(IPropertySymbol), nameof(PartialDefinitionPart));
            PartialImplementationPartAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<IPropertySymbol, IPropertySymbol>(typeof(IPropertySymbol), nameof(PartialImplementationPart));
        }

        public static IPropertySymbol PartialDefinitionPart(this IPropertySymbol symbol)
        {
            return PartialDefinitionPartAccessor(symbol);
        }

        public static IPropertySymbol PartialImplementationPart(this IPropertySymbol symbol)
        {
            return PartialImplementationPartAccessor(symbol);
        }
    }
}
