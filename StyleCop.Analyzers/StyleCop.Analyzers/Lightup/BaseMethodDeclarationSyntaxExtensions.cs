// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal static class BaseMethodDeclarationSyntaxExtensions
    {
        private static readonly Func<BaseMethodDeclarationSyntax, ArrowExpressionClauseSyntax> ExpressionBodyAccessor;

        static BaseMethodDeclarationSyntaxExtensions()
        {
            if (LightupHelpers.SupportsCSharp7)
            {
                var expressionBodyProperty = typeof(BaseMethodDeclarationSyntax).GetTypeInfo().GetDeclaredProperty(nameof(ExpressionBody));
                var syntaxParameter = Expression.Parameter(typeof(BaseMethodDeclarationSyntax), "syntax");
                Expression<Func<BaseMethodDeclarationSyntax, ArrowExpressionClauseSyntax>> expression =
                    Expression.Lambda<Func<BaseMethodDeclarationSyntax, ArrowExpressionClauseSyntax>>(
                        Expression.Call(syntaxParameter, expressionBodyProperty.GetMethod),
                        syntaxParameter);
                ExpressionBodyAccessor = expression.Compile();
            }
            else
            {
                ExpressionBodyAccessor = syntax => null;
            }
        }

        public static ArrowExpressionClauseSyntax ExpressionBody(this BaseMethodDeclarationSyntax syntax)
        {
            return ExpressionBodyAccessor(syntax);
        }
    }
}
