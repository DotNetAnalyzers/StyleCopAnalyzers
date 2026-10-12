// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal static class AnonymousFunctionExpressionSyntaxExtensions
    {
        private static readonly Func<AnonymousFunctionExpressionSyntax, SyntaxTokenList> ModifiersAccessor;
        private static readonly Func<AnonymousFunctionExpressionSyntax, SyntaxTokenList, AnonymousFunctionExpressionSyntax> WithModifiersAccessor;

        static AnonymousFunctionExpressionSyntaxExtensions()
        {
            ModifiersAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<AnonymousFunctionExpressionSyntax, SyntaxTokenList>(typeof(AnonymousFunctionExpressionSyntax), nameof(Modifiers));
            WithModifiersAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<AnonymousFunctionExpressionSyntax, SyntaxTokenList>(typeof(AnonymousFunctionExpressionSyntax), nameof(Modifiers));
        }

        public static SyntaxTokenList Modifiers(this AnonymousFunctionExpressionSyntax syntax)
        {
            return ModifiersAccessor(syntax);
        }

        public static AnonymousFunctionExpressionSyntax WithModifiers(this AnonymousFunctionExpressionSyntax syntax, SyntaxTokenList modifiers)
        {
            return WithModifiersAccessor(syntax, modifiers);
        }
    }
}
