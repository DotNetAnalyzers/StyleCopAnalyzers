// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp7.Lightup
{
    using System;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using StyleCop.Analyzers.Lightup;
    using Xunit;

    public class RefExpressionSyntaxWrapperTests
    {
        [Fact]
        public void TestNull()
        {
            var syntaxNode = default(SyntaxNode);
            var wrapper = (RefExpressionSyntaxWrapper)syntaxNode;
            Assert.Null(wrapper.SyntaxNode);
            Assert.Throws<NullReferenceException>(() => wrapper.RefKeyword);
            Assert.Throws<NullReferenceException>(() => wrapper.Expression);
            Assert.Throws<NullReferenceException>(() => wrapper.WithExpression(SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression)));
            Assert.Throws<NullReferenceException>(() => wrapper.WithRefKeyword(SyntaxFactory.Token(SyntaxKind.RefKeyword)));
        }

        [Fact]
        public void TestProperties()
        {
            var syntaxNode = this.CreateRefExpression();
            Assert.True(syntaxNode.IsKind(SyntaxKind.RefExpression));
            Assert.True(syntaxNode.IsKind(SyntaxKindEx.RefExpression));

            var wrapper = (RefExpressionSyntaxWrapper)syntaxNode;
            Assert.Same(syntaxNode, wrapper.SyntaxNode);
            Assert.Same(syntaxNode.Expression, wrapper.Expression);
            Assert.True(syntaxNode.RefKeyword.IsEquivalentTo(wrapper.RefKeyword));

            var newExpression = SyntaxFactory.LiteralExpression(SyntaxKind.TrueLiteralExpression);
            var wrapperWithModifiedExpression = wrapper.WithExpression(newExpression);
            Assert.NotNull(wrapperWithModifiedExpression.SyntaxNode);
            Assert.NotSame(syntaxNode.Expression, wrapperWithModifiedExpression.Expression);
            Assert.Equal(SyntaxKind.TrueLiteralExpression, wrapperWithModifiedExpression.Expression.Kind());

            var newRefKeyword = SyntaxFactory.Token(SyntaxKind.RefKeyword).WithLeadingTrivia(SyntaxFactory.Space);
            var wrapperWithModifiedRefKeyword = wrapper.WithRefKeyword(newRefKeyword);
            Assert.NotNull(wrapperWithModifiedRefKeyword.SyntaxNode);
            Assert.Single(wrapperWithModifiedRefKeyword.RefKeyword.LeadingTrivia);
            Assert.Equal(" ", wrapperWithModifiedRefKeyword.RefKeyword.LeadingTrivia.ToString());
        }

        [Fact]
        public void TestIsInstance()
        {
            Assert.False(RefExpressionSyntaxWrapper.IsInstance(null));
            Assert.False(RefExpressionSyntaxWrapper.IsInstance(SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression)));

            var syntaxNode = this.CreateRefExpression();
            Assert.True(RefExpressionSyntaxWrapper.IsInstance(syntaxNode));
        }

        [Fact]
        public void TestConversionsNull()
        {
            var syntaxNode = default(SyntaxNode);
            var wrapper = (RefExpressionSyntaxWrapper)syntaxNode;

            ExpressionSyntax syntax = wrapper;
            Assert.Null(syntax);
        }

        [Fact]
        public void TestConversions()
        {
            var syntaxNode = this.CreateRefExpression();
            var wrapper = (RefExpressionSyntaxWrapper)syntaxNode;

            ExpressionSyntax syntax = wrapper;
            Assert.Same(syntaxNode, syntax);
        }

        [Fact]
        public void TestInvalidConversion()
        {
            var syntaxNode = SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression);
            Assert.Throws<InvalidCastException>(() => (RefExpressionSyntaxWrapper)syntaxNode);
        }

        private RefExpressionSyntax CreateRefExpression()
        {
            return SyntaxFactory.RefExpression(SyntaxFactory.Token(SyntaxKind.RefKeyword), SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression));
        }
    }
}
