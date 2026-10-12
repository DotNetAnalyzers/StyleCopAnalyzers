// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.Lightup
{
    using System;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using StyleCop.Analyzers.Lightup;
    using Xunit;

    public class WhenClauseSyntaxWrapperTests
    {
        [Fact]
        public void TestNull()
        {
            var syntaxNode = default(SyntaxNode);
            var wrapper = (WhenClauseSyntaxWrapper)syntaxNode;
            Assert.Null(wrapper.SyntaxNode);
            Assert.Throws<NullReferenceException>(() => wrapper.Condition);
            Assert.Throws<NullReferenceException>(() => wrapper.WhenKeyword);
            Assert.Throws<NullReferenceException>(() => wrapper.WithCondition(SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression)));
            Assert.Throws<NullReferenceException>(() => wrapper.WithWhenKeyword(SyntaxFactory.Token(SyntaxKind.IsKeyword)));
        }

        [Fact]
        public void TestIsInstance()
        {
            Assert.False(WhenClauseSyntaxWrapper.IsInstance(null));
            Assert.False(WhenClauseSyntaxWrapper.IsInstance(SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression)));
        }

        [Fact]
        public void TestConversionsNull()
        {
            var syntaxNode = default(SyntaxNode);
            var wrapper = (WhenClauseSyntaxWrapper)syntaxNode;

            CSharpSyntaxNode syntax = wrapper;
            Assert.Null(syntax);
        }

        [Fact]
        public void TestInvalidConversion()
        {
            var syntaxNode = SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression);
            Assert.Throws<InvalidCastException>(() => (WhenClauseSyntaxWrapper)syntaxNode);
        }
    }
}
