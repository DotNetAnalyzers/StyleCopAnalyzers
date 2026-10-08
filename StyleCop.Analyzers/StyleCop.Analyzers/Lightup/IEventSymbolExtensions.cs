// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using Microsoft.CodeAnalysis;

    internal static class IEventSymbolExtensions
    {
        private static readonly Func<IEventSymbol, IEventSymbol> PartialDefinitionPartAccessor;
        private static readonly Func<IEventSymbol, IEventSymbol> PartialImplementationPartAccessor;

        static IEventSymbolExtensions()
        {
            PartialDefinitionPartAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<IEventSymbol, IEventSymbol>(typeof(IEventSymbol), nameof(PartialDefinitionPart));
            PartialImplementationPartAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<IEventSymbol, IEventSymbol>(typeof(IEventSymbol), nameof(PartialImplementationPart));
        }

        public static IEventSymbol PartialDefinitionPart(this IEventSymbol symbol)
        {
            return PartialDefinitionPartAccessor(symbol);
        }

        public static IEventSymbol PartialImplementationPart(this IEventSymbol symbol)
        {
            return PartialImplementationPartAccessor(symbol);
        }
    }
}
