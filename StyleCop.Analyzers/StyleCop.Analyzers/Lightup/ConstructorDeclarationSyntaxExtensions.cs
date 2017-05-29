// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal static class ConstructorDeclarationSyntaxExtensions
    {
        private static readonly Func<ConstructorDeclarationSyntax, ArrowExpressionClauseSyntax, ConstructorDeclarationSyntax> WithExpressionBodyAccessor;

        static ConstructorDeclarationSyntaxExtensions()
        {
            if (LightupHelpers.SupportsCSharp7)
            {
                var withExpressionBodyMethod = typeof(ConstructorDeclarationSyntax).GetTypeInfo().GetDeclaredMethods(nameof(WithExpressionBody))
                    .Single(method => method.GetParameters().Length == 1 && method.GetParameters()[0].ParameterType == typeof(ArrowExpressionClauseSyntax));
                var syntaxParameter = Expression.Parameter(typeof(ConstructorDeclarationSyntax), "syntax");
                var expressionBodyParameter = Expression.Parameter(typeof(ArrowExpressionClauseSyntax), "expressionBody");
                Expression<Func<ConstructorDeclarationSyntax, ArrowExpressionClauseSyntax, ConstructorDeclarationSyntax>> expression =
                    Expression.Lambda<Func<ConstructorDeclarationSyntax, ArrowExpressionClauseSyntax, ConstructorDeclarationSyntax>>(
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
                        throw new NotSupportedException("Expression-bodied constructors are only available in C# 7+.");
                    };
            }
        }

        public static ConstructorDeclarationSyntax WithExpressionBody(this ConstructorDeclarationSyntax syntax, ArrowExpressionClauseSyntax expressionBody)
        {
            return WithExpressionBodyAccessor(syntax, expressionBody);
        }
    }
}
