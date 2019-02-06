// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal static class DestructorDeclarationSyntaxExtensions
    {
        private static readonly Func<DestructorDeclarationSyntax, ArrowExpressionClauseSyntax, DestructorDeclarationSyntax> WithExpressionBodyAccessor;

        static DestructorDeclarationSyntaxExtensions()
        {
            WithExpressionBodyAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<DestructorDeclarationSyntax, ArrowExpressionClauseSyntax>(typeof(DestructorDeclarationSyntax), nameof(BaseMethodDeclarationSyntaxExtensions.ExpressionBody));
        }

        public static DestructorDeclarationSyntax WithExpressionBody(this DestructorDeclarationSyntax syntax, ArrowExpressionClauseSyntax expressionBody)
        {
            return WithExpressionBodyAccessor(syntax, expressionBody);
        }
    }
}
