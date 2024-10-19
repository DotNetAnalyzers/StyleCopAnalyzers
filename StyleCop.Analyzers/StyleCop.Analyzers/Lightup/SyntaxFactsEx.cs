﻿// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;

    internal static class SyntaxFactsEx
    {
        private static readonly Func<SyntaxNode, string> TryGetInferredMemberNameAccessor;
        private static readonly Func<string, bool> IsReservedTupleElementNameAccessor;

        static SyntaxFactsEx()
        {
            string FallbackAccessor(SyntaxNode syntax)
            {
                if (syntax == null)
                {
                    // Unlike an extension method which would throw ArgumentNullException here, the light-up
                    // behavior needs to match behavior of the underlying property.
                    throw new NullReferenceException();
                }

                return null;
            }

            var tryGetInferredMemberNameMethod = typeof(SyntaxFacts).GetTypeInfo().GetDeclaredMethod(nameof(TryGetInferredMemberName));
            if (tryGetInferredMemberNameMethod is object)
            {
                var syntaxParameter = Expression.Parameter(typeof(SyntaxNode), "syntax");
                Expression<Func<SyntaxNode, string>> expression =
                    Expression.Lambda<Func<SyntaxNode, string>>(
                        Expression.Call(tryGetInferredMemberNameMethod, syntaxParameter),
                        syntaxParameter);
                TryGetInferredMemberNameAccessor = expression.Compile();
            }
            else
            {
                TryGetInferredMemberNameAccessor = FallbackAccessor;
            }

            var isReservedTupleElementNameMethod = typeof(SyntaxFacts).GetTypeInfo().GetDeclaredMethod(nameof(IsReservedTupleElementName));
            if (isReservedTupleElementNameMethod is object)
            {
                var elementNameParameter = Expression.Parameter(typeof(string), "elementName");
                Expression<Func<string, bool>> expression =
                    Expression.Lambda<Func<string, bool>>(
                        Expression.Call(isReservedTupleElementNameMethod, elementNameParameter),
                        elementNameParameter);
                IsReservedTupleElementNameAccessor = expression.Compile();
            }
            else
            {
                IsReservedTupleElementNameAccessor = _ => false;
            }
        }

        public static string TryGetInferredMemberName(this SyntaxNode syntax)
        {
            return TryGetInferredMemberNameAccessor(syntax);
        }

        public static bool IsReservedTupleElementName(string elementName)
        {
            return IsReservedTupleElementNameAccessor(elementName);
        }
    }
}
