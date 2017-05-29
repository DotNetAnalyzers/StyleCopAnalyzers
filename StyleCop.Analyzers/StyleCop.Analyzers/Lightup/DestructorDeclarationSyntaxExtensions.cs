// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal static class DestructorDeclarationSyntaxExtensions
    {
        private static readonly Func<DestructorDeclarationSyntax, ArrowExpressionClauseSyntax, DestructorDeclarationSyntax> WithExpressionBodyAccessor;

        static DestructorDeclarationSyntaxExtensions()
        {
            if (LightupHelpers.SupportsCSharp7)
            {
                var withExpressionBodyMethod = typeof(DestructorDeclarationSyntax).GetTypeInfo().GetDeclaredMethods(nameof(WithExpressionBody))
                    .Single(method => method.GetParameters().Length == 1 && method.GetParameters()[0].ParameterType == typeof(ArrowExpressionClauseSyntax));
                var syntaxParameter = Expression.Parameter(typeof(DestructorDeclarationSyntax), "syntax");
                var expressionBodyParameter = Expression.Parameter(typeof(ArrowExpressionClauseSyntax), "expressionBody");
                Expression<Func<DestructorDeclarationSyntax, ArrowExpressionClauseSyntax, DestructorDeclarationSyntax>> expression =
                    Expression.Lambda<Func<DestructorDeclarationSyntax, ArrowExpressionClauseSyntax, DestructorDeclarationSyntax>>(
                        Expression.Call(syntaxParameter, withExpressionBodyMethod, expressionBodyParameter),
                        syntaxParameter,
                        expressionBodyParameter);
                WithExpressionBodyAccessor = expression.Compile();
            }
            else
            {
                WithExpressionBodyAccessor =
                    (syntax, expressionBody) =>
                    {
                        throw new NotSupportedException("Expression-bodied destructors are only available in C# 7+.");
                    };
            }
        }

        public static DestructorDeclarationSyntax WithExpressionBody(this DestructorDeclarationSyntax syntax, ArrowExpressionClauseSyntax expressionBody)
        {
            return WithExpressionBodyAccessor(syntax, expressionBody);
        }
    }
}
