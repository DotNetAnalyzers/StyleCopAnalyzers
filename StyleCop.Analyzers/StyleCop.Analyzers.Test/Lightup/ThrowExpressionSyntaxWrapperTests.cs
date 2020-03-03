// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.Lightup
{
    using System;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using StyleCop.Analyzers.Lightup;
    using Xunit;

    public class ThrowExpressionSyntaxWrapperTests
    {
        [Fact]
        public void TestNull()
        {
            var syntaxNode = default(SyntaxNode);
            var throwExpressionSyntax = (ThrowExpressionSyntaxWrapper)syntaxNode;
            Assert.Null(throwExpressionSyntax.SyntaxNode);
            Assert.Throws<NullReferenceException>(() => throwExpressionSyntax.Expression);
            Assert.Throws<NullReferenceException>(() => throwExpressionSyntax.ThrowKeyword);
            Assert.Throws<NullReferenceException>(() => throwExpressionSyntax.WithExpression(SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression)));
            Assert.Throws<NullReferenceException>(() => throwExpressionSyntax.WithThrowKeyword(SyntaxFactory.Token(SyntaxKind.ThrowKeyword)));
        }

        [Fact]
        public void TestIsInstance()
        {
            Assert.False(ThrowExpressionSyntaxWrapper.IsInstance(null));
            Assert.False(ThrowExpressionSyntaxWrapper.IsInstance(SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression)));
        }

        [Fact]
        public void TestConversionsNull()
        {
            var syntaxNode = default(SyntaxNode);
            var throwExpressionSyntax = (ThrowExpressionSyntaxWrapper)syntaxNode;

            ExpressionSyntax syntax = throwExpressionSyntax;
            Assert.Null(syntax);
        }

        [Fact]
        public void TestInvalidConversion()
        {
            var syntaxNode = SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression);
            Assert.Throws<InvalidCastException>(() => (ThrowExpressionSyntaxWrapper)syntaxNode);
        }
    }
}
