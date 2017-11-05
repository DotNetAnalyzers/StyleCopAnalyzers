// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp7.Lightup
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
        public void TestProperties()
        {
            var syntaxNode = SyntaxFactory.ThrowExpression(SyntaxFactory.Token(SyntaxKind.ThrowKeyword), SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression));
            Assert.True(syntaxNode.IsKind(SyntaxKind.ThrowExpression));
            Assert.True(syntaxNode.IsKind(SyntaxKindEx.ThrowExpression));

            var throwExpressionSyntax = (ThrowExpressionSyntaxWrapper)syntaxNode;
            Assert.Same(syntaxNode, throwExpressionSyntax.SyntaxNode);
            Assert.Same(syntaxNode.Expression, throwExpressionSyntax.Expression);
            Assert.True(syntaxNode.ThrowKeyword.IsEquivalentTo(throwExpressionSyntax.ThrowKeyword));

            var newExpression = SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(0)); // This does not make sense semantically, but it's good enough for this test
            var modifiedExpression = throwExpressionSyntax.WithExpression(newExpression);
            Assert.NotNull(modifiedExpression.SyntaxNode);
            Assert.NotSame(syntaxNode.Expression, modifiedExpression.Expression);
            Assert.Equal(SyntaxKind.NumericLiteralExpression, modifiedExpression.Expression.Kind());

            var newThrowKeyword = SyntaxFactory.Token(SyntaxKind.ThrowKeyword).WithLeadingTrivia(SyntaxFactory.Space);
            var modifiedThrowKeyword = throwExpressionSyntax.WithThrowKeyword(newThrowKeyword);
            Assert.NotNull(modifiedThrowKeyword.SyntaxNode);
            Assert.Single(modifiedThrowKeyword.ThrowKeyword.LeadingTrivia);
            Assert.Equal(" ", modifiedThrowKeyword.ThrowKeyword.LeadingTrivia.ToString());
        }

        [Fact]
        public void TestIsInstance()
        {
            Assert.False(ThrowExpressionSyntaxWrapper.IsInstance(null));
            Assert.False(ThrowExpressionSyntaxWrapper.IsInstance(SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression)));

            var syntaxNode = SyntaxFactory.ThrowExpression(SyntaxFactory.Token(SyntaxKind.ThrowKeyword), SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression));
            Assert.True(ThrowExpressionSyntaxWrapper.IsInstance(syntaxNode));
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
        public void TestConversions()
        {
            var syntaxNode = SyntaxFactory.ThrowExpression(SyntaxFactory.Token(SyntaxKind.ThrowKeyword), SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression));
            var throwExpressionSyntax = (ThrowExpressionSyntaxWrapper)syntaxNode;

            ExpressionSyntax syntax = throwExpressionSyntax;
            Assert.Same(syntaxNode, syntax);
        }

        [Fact]
        public void TestInvalidConversion()
        {
            var syntaxNode = SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression);
            Assert.Throws<InvalidCastException>(() => (ThrowExpressionSyntaxWrapper)syntaxNode);
        }
    }
}
