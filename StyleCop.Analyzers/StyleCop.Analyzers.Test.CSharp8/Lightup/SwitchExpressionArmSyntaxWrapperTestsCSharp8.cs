// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp8.Lightup
{
    using System;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using StyleCop.Analyzers.Lightup;
    using StyleCop.Analyzers.Test.CSharp7.Lightup;
    using Xunit;

    public class SwitchExpressionArmSyntaxWrapperTestsCSharp8 : SwitchExpressionArmSyntaxWrapperTestsCSharp7
    {
        [Fact]
        public void TestSyntaxNodeProperty()
        {
            var syntaxNode = this.CreateSwitchExpressionArm();

            Assert.True(syntaxNode.IsKind(SyntaxKind.SwitchExpressionArm));
            Assert.True(syntaxNode.IsKind(SyntaxKindEx.SwitchExpressionArm));

            var wrapper = (SwitchExpressionArmSyntaxWrapper)syntaxNode;
            Assert.Same(syntaxNode, wrapper.SyntaxNode);
        }

        [Fact]
        public void TestPatternProperty()
        {
            var syntaxNode = this.CreateSwitchExpressionArm();

            var wrapper = (SwitchExpressionArmSyntaxWrapper)syntaxNode;
            Assert.Same(syntaxNode.Pattern, wrapper.Pattern.SyntaxNode);

            var newPattern = SyntaxFactory.ConstantPattern(SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression));
            var wrapperWithModifiedPattern = wrapper.WithPattern((PatternSyntaxWrapper)newPattern);
            Assert.NotNull(wrapperWithModifiedPattern.SyntaxNode);
            Assert.NotSame(syntaxNode.Pattern, wrapperWithModifiedPattern.Pattern.SyntaxNode);
            Assert.True(newPattern.IsEquivalentTo(wrapperWithModifiedPattern.Pattern));
        }

        [Fact]
        public void TestWhenClauseProperty()
        {
            var syntaxNode = this.CreateSwitchExpressionArm();

            var wrapper = (SwitchExpressionArmSyntaxWrapper)syntaxNode;
            Assert.Same(syntaxNode.WhenClause, wrapper.WhenClause.SyntaxNode);

            var newWhenClause = SyntaxFactory.WhenClause(SyntaxFactory.LiteralExpression(SyntaxKind.TrueLiteralExpression));
            var wrapperWithModifiedWhenClause = wrapper.WithWhenClause((WhenClauseSyntaxWrapper)newWhenClause);
            Assert.NotNull(wrapperWithModifiedWhenClause.SyntaxNode);
            Assert.NotSame(syntaxNode.WhenClause, wrapperWithModifiedWhenClause.WhenClause.SyntaxNode);
            Assert.Equal(SyntaxKind.WhenClause, wrapperWithModifiedWhenClause.WhenClause.SyntaxNode.Kind());
            Assert.Equal(SyntaxKind.TrueLiteralExpression, wrapperWithModifiedWhenClause.WhenClause.Condition.Kind());
        }

        [Fact]
        public void TestExpressionProperty()
        {
            var syntaxNode = this.CreateSwitchExpressionArm();

            var wrapper = (SwitchExpressionArmSyntaxWrapper)syntaxNode;
            Assert.Same(syntaxNode.Expression, wrapper.Expression);

            var newExpression = SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression);
            var wrapperWithModifiedExpression = wrapper.WithExpression(newExpression);
            Assert.NotNull(wrapperWithModifiedExpression.SyntaxNode);
            Assert.NotSame(syntaxNode.Expression, wrapperWithModifiedExpression.Expression);
            Assert.Equal(SyntaxKind.NullLiteralExpression, wrapperWithModifiedExpression.Expression.Kind());
        }

        [Fact]
        public void TestEqualsGreaterThanTokenProperty()
        {
            var syntaxNode = this.CreateSwitchExpressionArm();

            var wrapper = (SwitchExpressionArmSyntaxWrapper)syntaxNode;

            Assert.Equal(syntaxNode.EqualsGreaterThanToken, wrapper.EqualsGreaterThanToken);

            Assert.Throws<ArgumentException>(() => wrapper.WithEqualsGreaterThanToken(SyntaxFactory.Token(SyntaxKind.EqualsEqualsToken)));

            var wrapperWithModifiedToken = wrapper.WithEqualsGreaterThanToken(SyntaxFactory.Token(SyntaxKind.EqualsGreaterThanToken));
            Assert.Equal(SyntaxKind.EqualsGreaterThanToken, wrapperWithModifiedToken.EqualsGreaterThanToken.Kind());
        }

        [Fact]
        public void TestIsInstance()
        {
            Assert.False(SwitchExpressionArmSyntaxWrapper.IsInstance(null));
            Assert.False(SwitchExpressionArmSyntaxWrapper.IsInstance(SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression)));

            Assert.True(SwitchExpressionArmSyntaxWrapper.IsInstance(this.CreateSwitchExpressionArm()));
        }

        [Fact]
        public void TestConversions()
        {
            var syntaxNode = this.CreateSwitchExpressionArm();
            var wrapper = (SwitchExpressionArmSyntaxWrapper)syntaxNode;

            CSharpSyntaxNode syntax = wrapper;
            Assert.Same(syntaxNode, syntax);
        }

        private SwitchExpressionArmSyntax CreateSwitchExpressionArm()
            => SyntaxFactory.SwitchExpressionArm(
                SyntaxFactory.DiscardPattern(),
                SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression));
    }
}
