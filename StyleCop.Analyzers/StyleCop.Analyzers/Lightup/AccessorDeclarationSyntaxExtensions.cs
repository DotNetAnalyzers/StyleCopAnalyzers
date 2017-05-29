// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal static class AccessorDeclarationSyntaxExtensions
    {
        private static readonly Func<AccessorDeclarationSyntax, ArrowExpressionClauseSyntax> ExpressionBodyAccessor;
        private static readonly Func<AccessorDeclarationSyntax, ArrowExpressionClauseSyntax, AccessorDeclarationSyntax> WithExpressionBodyAccessor;

        static AccessorDeclarationSyntaxExtensions()
        {
            if (LightupHelpers.SupportsCSharp7)
            {
                {
                    var expressionBodyProperty = typeof(AccessorDeclarationSyntax).GetTypeInfo().GetDeclaredProperty(nameof(ExpressionBody));
                    var syntaxParameter = Expression.Parameter(typeof(AccessorDeclarationSyntax), "syntax");
                    Expression<Func<AccessorDeclarationSyntax, ArrowExpressionClauseSyntax>> expression =
                        Expression.Lambda<Func<AccessorDeclarationSyntax, ArrowExpressionClauseSyntax>>(
                            Expression.Call(syntaxParameter, expressionBodyProperty.GetMethod),
                            syntaxParameter);
                    ExpressionBodyAccessor = expression.Compile();
                }

                {
                    var withExpressionBodyMethod = typeof(AccessorDeclarationSyntax).GetTypeInfo().GetDeclaredMethods(nameof(WithExpressionBody))
                        .Single(method => method.GetParameters().Length == 1 && method.GetParameters()[0].ParameterType == typeof(ArrowExpressionClauseSyntax));
                    var syntaxParameter = Expression.Parameter(typeof(AccessorDeclarationSyntax), "syntax");
                    var expressionBodyParameter = Expression.Parameter(typeof(ArrowExpressionClauseSyntax), "expressionBody");
                    Expression<Func<AccessorDeclarationSyntax, ArrowExpressionClauseSyntax, AccessorDeclarationSyntax>> expression =
                        Expression.Lambda<Func<AccessorDeclarationSyntax, ArrowExpressionClauseSyntax, AccessorDeclarationSyntax>>(
                            Expression.Call(syntaxParameter, withExpressionBodyMethod, expressionBodyParameter),
                            syntaxParameter,
                            expressionBodyParameter);
                    WithExpressionBodyAccessor = expression.Compile();
                }
            }
            else
            {
                ExpressionBodyAccessor = syntax => null;
                WithExpressionBodyAccessor =
                    (syntax, expressionBody) =>
                    {
                        throw new NotSupportedException("Expression-bodied accessors are only available in C# 7+.");
                    };
            }
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
