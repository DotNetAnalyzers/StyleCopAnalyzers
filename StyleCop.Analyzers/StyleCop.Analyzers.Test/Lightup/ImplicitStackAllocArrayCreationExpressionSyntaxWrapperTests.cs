// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.Lightup
{
    using System;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using StyleCop.Analyzers.Lightup;
    using Xunit;

    public class ImplicitStackAllocArrayCreationExpressionSyntaxWrapperTests
    {
        [Fact]
        public void TestNull()
        {
            var syntaxNode = default(SyntaxNode);
            var wrapper = (ImplicitStackAllocArrayCreationExpressionSyntaxWrapper)syntaxNode;
            Assert.Null(wrapper.SyntaxNode);
            Assert.Throws<NullReferenceException>(() => wrapper.StackAllocKeyword);
            Assert.Throws<NullReferenceException>(() => wrapper.OpenBracketToken);
            Assert.Throws<NullReferenceException>(() => wrapper.CloseBracketToken);
            Assert.Throws<NullReferenceException>(() => wrapper.Initializer);
            Assert.Throws<NullReferenceException>(() => wrapper.WithStackAllocKeyword(SyntaxFactory.Token(SyntaxKind.StackAllocKeyword)));
            Assert.Throws<NullReferenceException>(() => wrapper.WithOpenBracketToken(SyntaxFactory.Token(SyntaxKind.OpenBracketToken)));
            Assert.Throws<NullReferenceException>(() => wrapper.WithCloseBracketToken(SyntaxFactory.Token(SyntaxKind.CloseBracketToken)));
            Assert.Throws<NullReferenceException>(() => wrapper.WithInitializer(SyntaxFactory.InitializerExpression(SyntaxKind.ArrayInitializerExpression)));
            Assert.Throws<NullReferenceException>(() => wrapper.AddInitializerExpressions());
        }

        [Fact]
        public void TestIsInstance()
        {
            Assert.False(ImplicitStackAllocArrayCreationExpressionSyntaxWrapper.IsInstance(null));
            Assert.False(ImplicitStackAllocArrayCreationExpressionSyntaxWrapper.IsInstance(SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression)));
        }

        [Fact]
        public void TestConversionsNull()
        {
            var syntaxNode = default(SyntaxNode);
            var wrapper = (ImplicitStackAllocArrayCreationExpressionSyntaxWrapper)syntaxNode;

            ExpressionSyntax syntax = wrapper;
            Assert.Null(syntax);
        }

        [Fact]
        public void TestInvalidConversion()
        {
            var syntaxNode = SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression);
            Assert.Throws<InvalidCastException>(() => (ImplicitStackAllocArrayCreationExpressionSyntaxWrapper)syntaxNode);
        }
    }
}
