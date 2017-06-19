// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.Lightup
{
    using System;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using StyleCop.Analyzers.Lightup;
    using Xunit;

    public class TupleTypeSyntaxWrapperTests
    {
        [Fact]
        public void TestNull()
        {
            var syntaxNode = default(SyntaxNode);
            var wrapper = (TupleTypeSyntaxWrapper)syntaxNode;
            Assert.Null(wrapper.SyntaxNode);
            Assert.Throws<NullReferenceException>(() => wrapper.OpenParenToken);
            Assert.Throws<NullReferenceException>(() => wrapper.Elements);
            Assert.Throws<NullReferenceException>(() => wrapper.CloseParenToken);
            Assert.Throws<NullReferenceException>(() => wrapper.WithOpenParenToken(SyntaxFactory.Token(SyntaxKind.OpenParenToken)));
            Assert.Throws<NullReferenceException>(() => wrapper.WithElements(SeparatedSyntaxListWrapper<TupleElementSyntaxWrapper>.UnsupportedEmpty));
            Assert.Throws<NullReferenceException>(() => wrapper.AddElements());
            Assert.Throws<NullReferenceException>(() => wrapper.WithCloseParenToken(SyntaxFactory.Token(SyntaxKind.CloseParenToken)));
        }

        [Fact]
        public void TestIsInstance()
        {
            Assert.False(TupleTypeSyntaxWrapper.IsInstance(null));
            Assert.False(TupleTypeSyntaxWrapper.IsInstance(SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression)));
        }

        [Fact]
        public void TestConversionsNull()
        {
            var syntaxNode = default(SyntaxNode);
            var wrapper = (TupleTypeSyntaxWrapper)syntaxNode;

            TypeSyntax syntax = wrapper;
            Assert.Null(syntax);
        }

        [Fact]
        public void TestInvalidConversion()
        {
            var syntaxNode = SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression);
            Assert.Throws<InvalidCastException>(() => (TupleTypeSyntaxWrapper)syntaxNode);
        }
    }
}
