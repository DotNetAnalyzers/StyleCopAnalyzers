// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal static class ParenthesizedLambdaExpressionSyntaxExtensions
    {
        private static readonly Func<ParenthesizedLambdaExpressionSyntax, TypeSyntax> ReturnTypeAccessor;
        private static readonly Func<ParenthesizedLambdaExpressionSyntax, TypeSyntax, ParenthesizedLambdaExpressionSyntax> WithReturnTypeAccessor;

        static ParenthesizedLambdaExpressionSyntaxExtensions()
        {
            ReturnTypeAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<ParenthesizedLambdaExpressionSyntax, TypeSyntax>(typeof(ParenthesizedLambdaExpressionSyntax), nameof(ReturnType));
            WithReturnTypeAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<ParenthesizedLambdaExpressionSyntax, TypeSyntax>(typeof(ParenthesizedLambdaExpressionSyntax), nameof(ReturnType));
        }

        public static TypeSyntax ReturnType(this ParenthesizedLambdaExpressionSyntax syntax)
        {
            return ReturnTypeAccessor(syntax);
        }

        public static ParenthesizedLambdaExpressionSyntax WithReturnType(this ParenthesizedLambdaExpressionSyntax syntax, TypeSyntax returnType)
        {
            return WithReturnTypeAccessor(syntax, returnType);
        }
    }
}
