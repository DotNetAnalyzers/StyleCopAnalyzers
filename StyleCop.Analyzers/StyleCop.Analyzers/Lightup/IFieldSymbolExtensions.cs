// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using Microsoft.CodeAnalysis;

    internal static class IFieldSymbolExtensions
    {
        private static readonly Func<IFieldSymbol, IFieldSymbol> CorrespondingTupleFieldAccessor;

        static IFieldSymbolExtensions()
        {
            CorrespondingTupleFieldAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<IFieldSymbol, IFieldSymbol>(typeof(IFieldSymbol), nameof(CorrespondingTupleField));
        }

        public static IFieldSymbol CorrespondingTupleField(this IFieldSymbol symbol)
        {
            return CorrespondingTupleFieldAccessor(symbol);
        }
    }
}
