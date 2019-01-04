// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System.Linq;
    using System.Reflection;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal static class SyntaxFactoryEx
    {
        private static readonly System.Reflection.TypeInfo SyntaxFactoryTypeInfo = typeof(SyntaxFactory).GetTypeInfo();

        public static TupleElementSyntaxWrapper TupleElement(TypeSyntax type, SyntaxToken identifier)
        {
            var methodInfo = SyntaxFactoryTypeInfo.GetDeclaredMethods("TupleElement")
                .Single(m => m.IsStatic && m.GetParameters().Length == 2);

            var result = (SyntaxNode)methodInfo.Invoke(null, new object[] { type, identifier });
            return (TupleElementSyntaxWrapper)result;
        }

        internal static TupleElementSyntaxWrapper TupleElement(TypeSyntax type)
        {
            return TupleElement(type, default);
        }

        internal static TupleExpressionSyntaxWrapper TupleExpression(SeparatedSyntaxList<ArgumentSyntax> arguments = default)
        {
            var methodInfo = SyntaxFactoryTypeInfo.GetDeclaredMethods("TupleExpression")
                .Single(m => m.IsStatic && m.GetParameters().Length == 1 && m.GetParameters()[0].ParameterType.Equals(typeof(SeparatedSyntaxList<ArgumentSyntax>)));

            var result = (SyntaxNode)methodInfo.Invoke(null, new object[] { arguments });
            return (TupleExpressionSyntaxWrapper)result;
        }

        internal static TupleTypeSyntaxWrapper TupleType(SeparatedSyntaxListWrapper<TupleElementSyntaxWrapper> elements = default)
        {
            var methodInfo = SyntaxFactoryTypeInfo.GetDeclaredMethods("TupleType")
                .Single(m => m.IsStatic && m.GetParameters().Length == 1 && m.GetParameters()[0].ParameterType.Equals(LightupHelpers.SeparatedSyntaxListWithTupleElementType));

            var result = (SyntaxNode)methodInfo.Invoke(null, new object[] { elements.UnderlyingList });
            return (TupleTypeSyntaxWrapper)result;
        }
    }
}
