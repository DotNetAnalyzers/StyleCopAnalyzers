// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.Lightup
{
    using System;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using StyleCop.Analyzers.Lightup;
    using Xunit;

    public class TupleElementSyntaxWrapperTests
    {
        [Fact]
        public void TestNull()
        {
            var syntaxNode = default(SyntaxNode);
            var wrapper = (TupleElementSyntaxWrapper)syntaxNode;
            Assert.Null(wrapper.SyntaxNode);
            Assert.Throws<NullReferenceException>(() => wrapper.Type);
            Assert.Throws<NullReferenceException>(() => wrapper.Identifier);
            Assert.Throws<NullReferenceException>(() => wrapper.WithType(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.StringKeyword))));
            Assert.Throws<NullReferenceException>(() => wrapper.WithIdentifier(SyntaxFactory.Identifier("x")));
        }

        [Fact]
        public void TestIsInstance()
        {
            Assert.False(TupleElementSyntaxWrapper.IsInstance(null));
            Assert.False(TupleElementSyntaxWrapper.IsInstance(SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression)));
        }

        [Fact]
        public void TestConversionsNull()
        {
            var syntaxNode = default(SyntaxNode);
            var wrapper = (TupleElementSyntaxWrapper)syntaxNode;

            CSharpSyntaxNode syntax = wrapper;
            Assert.Null(syntax);
        }

        [Fact]
        public void TestInvalidConversion()
        {
            var syntaxNode = SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression);
            Assert.Throws<InvalidCastException>(() => (TupleElementSyntaxWrapper)syntaxNode);
        }
    }
}
