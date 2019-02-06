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

    public class RefTypeSyntaxWrapperTests
    {
        [Fact]
        public void TestNull()
        {
            var syntaxNode = default(SyntaxNode);
            var wrapper = (RefTypeSyntaxWrapper)syntaxNode;
            Assert.Null(wrapper.SyntaxNode);
            Assert.Throws<NullReferenceException>(() => wrapper.Type);
            Assert.Throws<NullReferenceException>(() => wrapper.RefKeyword);
            Assert.Throws<NullReferenceException>(() => wrapper.ReadOnlyKeyword);
            Assert.Throws<NullReferenceException>(() => wrapper.WithType(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword))));
            Assert.Throws<NullReferenceException>(() => wrapper.WithRefKeyword(SyntaxFactory.Token(SyntaxKind.RefKeyword)));
            Assert.Throws<NullReferenceException>(() => wrapper.WithReadOnlyKeyword(SyntaxFactory.Token(SyntaxKind.ReadOnlyKeyword)));
        }

        [Fact]
        public void TestIsInstance()
        {
            Assert.False(RefTypeSyntaxWrapper.IsInstance(null));
            Assert.False(RefTypeSyntaxWrapper.IsInstance(SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression)));
        }

        [Fact]
        public void TestConversionsNull()
        {
            var syntaxNode = default(SyntaxNode);
            var wrapper = (RefTypeSyntaxWrapper)syntaxNode;

            TypeSyntax syntax = wrapper;
            Assert.Null(syntax);
        }

        [Fact]
        public void TestInvalidConversion()
        {
            var syntaxNode = SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression);
            Assert.Throws<InvalidCastException>(() => (RefTypeSyntaxWrapper)syntaxNode);
        }
    }
}
