// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.Lightup
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using StyleCop.Analyzers.Lightup;
    using Xunit;

    public class SyntaxWrapperHelperTests
    {
        public static IEnumerable<object[]> SyntaxWrapperClasses
        {
            get
            {
                var wrapperTypes = typeof(ISyntaxWrapper<>).Assembly.GetTypes()
                    .Where(t => t.GetTypeInfo().ImplementedInterfaces.Any(i => i.IsGenericType && (i.GetGenericTypeDefinition() == typeof(ISyntaxWrapper<>))));

                foreach (var wrapperType in wrapperTypes)
                {
                    yield return new object[] { wrapperType };
                }
            }
        }

        [Theory]
        [MemberData(nameof(SyntaxWrapperClasses))]
        public void VerifyThatWrapperClassIsPresent(Type wrapperType)
        {
            var wrappedTypeName = $"Microsoft.CodeAnalysis.CSharp.Syntax.{wrapperType.Name.Substring(0, wrapperType.Name.Length - "Wrapper".Length)}";
            if (typeof(CSharpSyntaxNode).Assembly.GetType(wrappedTypeName) is { } expected)
            {
                var wrappedType = SyntaxWrapperHelper.GetWrappedType(wrapperType);
                Assert.Same(expected, wrappedType);
            }
            else if (wrapperType == typeof(CommonForEachStatementSyntaxWrapper))
            {
                // Special case for C# 6 analysis compatibility
                Assert.False(LightupHelpers.SupportsCSharp7);
                Assert.Same(typeof(ForEachStatementSyntax), SyntaxWrapperHelper.GetWrappedType(wrapperType));
            }
            else if (wrapperType == typeof(BaseObjectCreationExpressionSyntaxWrapper))
            {
                // Special case for C# 6-8 analysis compatibility
                Assert.False(LightupHelpers.SupportsCSharp9);
                Assert.Same(typeof(ObjectCreationExpressionSyntax), SyntaxWrapperHelper.GetWrappedType(wrapperType));
            }
            else
            {
                Assert.Null(SyntaxWrapperHelper.GetWrappedType(wrapperType));
            }
        }
    }
}
