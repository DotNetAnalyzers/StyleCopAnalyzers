// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.Lightup
{
    using System;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using StyleCop.Analyzers.Lightup;
    using Xunit;

    public class SwitchExpressionSyntaxWrapperTests
    {
        [Fact]
        public void TestNull()
        {
            var syntaxNode = default(ExpressionSyntax);
            var switchExpressionSyntaxWrapper = (SwitchExpressionSyntaxWrapper)syntaxNode;

            Assert.Null(switchExpressionSyntaxWrapper.SyntaxNode);
            Assert.Throws<NullReferenceException>(() => switchExpressionSyntaxWrapper.Arms);
            Assert.Throws<NullReferenceException>(() => switchExpressionSyntaxWrapper.CloseBraceToken);
            Assert.Throws<NullReferenceException>(() => switchExpressionSyntaxWrapper.GoverningExpression);
            Assert.Throws<NullReferenceException>(() => switchExpressionSyntaxWrapper.OpenBraceToken);
            Assert.Throws<NullReferenceException>(() => switchExpressionSyntaxWrapper.AddArms((SwitchExpressionArmSyntaxWrapper)null));
            Assert.Throws<NullReferenceException>(() => switchExpressionSyntaxWrapper.WithArms(SeparatedSyntaxListWrapper<SwitchExpressionArmSyntaxWrapper>.UnsupportedEmpty));
            Assert.Throws<NullReferenceException>(() => switchExpressionSyntaxWrapper.WithCloseBraceToken(SyntaxFactory.Token(SyntaxKind.CloseBracketToken)));
            Assert.Throws<NullReferenceException>(() => switchExpressionSyntaxWrapper.WithOpenBraceToken(SyntaxFactory.Token(SyntaxKind.OpenBraceToken)));
            Assert.Throws<NullReferenceException>(() => switchExpressionSyntaxWrapper.WithSwitchKeyword(SyntaxFactory.Token(SyntaxKind.SwitchKeyword)));
        }

        [Fact]
        public void TestIsInstance()
        {
            Assert.False(SwitchExpressionSyntaxWrapper.IsInstance(null));
            Assert.False(SwitchExpressionSyntaxWrapper.IsInstance(SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression)));
        }

        [Fact]
        public void TestConversionsNull()
        {
            var syntaxNode = default(ExpressionSyntax);
            var switchExpressionSyntaxWrapper = (SwitchExpressionSyntaxWrapper)syntaxNode;

            Assert.Null(switchExpressionSyntaxWrapper.SyntaxNode);
            Assert.Null((ExpressionSyntax)switchExpressionSyntaxWrapper);
        }

        [Fact]
        public void TestInvalidConversion()
        {
            var syntaxNode = SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression);
            Assert.Throws<InvalidCastException>(() => (SwitchExpressionSyntaxWrapper)syntaxNode);
        }
    }
}
