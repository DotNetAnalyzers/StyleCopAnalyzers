﻿// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using Microsoft.CodeAnalysis;

    internal static class ITypeParameterSymbolExtensions
    {
        private static readonly Func<ITypeParameterSymbol, bool> HasUnmanagedTypeConstraintAccessor;

        static ITypeParameterSymbolExtensions()
        {
            HasUnmanagedTypeConstraintAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<ITypeParameterSymbol, bool>(typeof(ITypeParameterSymbol), nameof(HasUnmanagedTypeConstraint));
        }

        public static bool HasUnmanagedTypeConstraint(this ITypeParameterSymbol symbol)
        {
            return HasUnmanagedTypeConstraintAccessor(symbol);
        }
    }
}
