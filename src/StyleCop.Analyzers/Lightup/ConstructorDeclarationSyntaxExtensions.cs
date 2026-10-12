// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal static class ConstructorDeclarationSyntaxExtensions
    {
        private static readonly Func<ConstructorDeclarationSyntax, ArrowExpressionClauseSyntax, ConstructorDeclarationSyntax> WithExpressionBodyAccessor;

        static ConstructorDeclarationSyntaxExtensions()
        {
            WithExpressionBodyAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<ConstructorDeclarationSyntax, ArrowExpressionClauseSyntax>(typeof(ConstructorDeclarationSyntax), nameof(BaseMethodDeclarationSyntaxExtensions.ExpressionBody));
        }

        public static ConstructorDeclarationSyntax WithExpressionBody(this ConstructorDeclarationSyntax syntax, ArrowExpressionClauseSyntax expressionBody)
        {
            return WithExpressionBodyAccessor(syntax, expressionBody);
        }
    }
}
