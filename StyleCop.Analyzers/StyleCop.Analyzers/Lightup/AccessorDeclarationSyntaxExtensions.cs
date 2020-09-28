// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal static class AccessorDeclarationSyntaxExtensions
    {
        private static readonly Func<AccessorDeclarationSyntax, ArrowExpressionClauseSyntax> ExpressionBodyAccessor;
        private static readonly Func<AccessorDeclarationSyntax, ArrowExpressionClauseSyntax, AccessorDeclarationSyntax> WithExpressionBodyAccessor;

        static AccessorDeclarationSyntaxExtensions()
        {
            ExpressionBodyAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<AccessorDeclarationSyntax, ArrowExpressionClauseSyntax>(typeof(AccessorDeclarationSyntax), nameof(ExpressionBody));
            WithExpressionBodyAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<AccessorDeclarationSyntax, ArrowExpressionClauseSyntax>(typeof(AccessorDeclarationSyntax), nameof(ExpressionBody));
        }

        public static ArrowExpressionClauseSyntax ExpressionBody(this AccessorDeclarationSyntax syntax)
        {
            return ExpressionBodyAccessor(syntax);
        }

        public static AccessorDeclarationSyntax WithExpressionBody(this AccessorDeclarationSyntax syntax, ArrowExpressionClauseSyntax expressionBody)
        {
            return WithExpressionBodyAccessor(syntax, expressionBody);
        }
    }
}
