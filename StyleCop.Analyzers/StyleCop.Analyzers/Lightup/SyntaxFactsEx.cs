// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System.Linq;
    using System.Reflection;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;

    internal static class SyntaxFactsEx
    {
        private static readonly System.Reflection.TypeInfo SyntaxFactsTypeInfo = typeof(SyntaxFacts).GetTypeInfo();

        public static string TryGetInferredMemberName(this SyntaxNode syntax)
        {
            var methodInfo = SyntaxFactsTypeInfo.GetDeclaredMethods("TryGetInferredMemberName")
                .Single();

            return (string)methodInfo.Invoke(null, new object[] { syntax });
        }

        public static bool IsReservedTupleElementName(string elementName)
        {
            var methodInfo = SyntaxFactsTypeInfo.GetDeclaredMethods("IsReservedTupleElementName")
                .Single();

            return (bool)methodInfo.Invoke(null, new object[] { elementName });
        }
    }
}
