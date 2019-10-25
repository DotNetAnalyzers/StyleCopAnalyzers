// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp8.Lightup
{
    using System;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using StyleCop.Analyzers.Lightup;
    using Xunit;

    public class SwitchExpressionArmSyntaxWrapperTests
    {
        [Fact]
        public void TestNull()
        {
            var syntaxNode = default(SyntaxNode);
            var switchExpressionSyntax = (SwitchExpressionArmSyntaxWrapper)syntaxNode;
            Assert.Null(switchExpressionSyntax.SyntaxNode);
            Assert.Throws<NullReferenceException>(() => switchExpressionSyntax.Pattern);
            Assert.Throws<NullReferenceException>(() => switchExpressionSyntax.WhenClause);
            Assert.Throws<NullReferenceException>(() => switchExpressionSyntax.Expression);
            Assert.Throws<NullReferenceException>(() => switchExpressionSyntax.EqualsGreaterThanToken);

            var patternSyntax = SyntaxFactory.ConstantPattern(SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression));
            Assert.Throws<NullReferenceException>(() => switchExpressionSyntax.WithPattern((PatternSyntaxWrapper)patternSyntax));

            var whenClause = SyntaxFactory.WhenClause(SyntaxFactory.LiteralExpression(SyntaxKind.TrueLiteralExpression));
            Assert.Throws<NullReferenceException>(() => switchExpressionSyntax.WithWhenClause((WhenClauseSyntaxWrapper)whenClause));

            Assert.Throws<NullReferenceException>(() => switchExpressionSyntax.WithExpression(SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression)));
            Assert.Throws<NullReferenceException>(() => switchExpressionSyntax.WithEqualsGreaterThanToken(SyntaxFactory.Token(SyntaxKind.EqualsGreaterThanToken)));
        }

        [Fact]
        public void TestProperties()
        {
            var syntaxNode = this.CreateSwitchExpressionArm();

            Assert.True(syntaxNode.IsKind(SyntaxKind.SwitchExpressionArm));
            Assert.True(syntaxNode.IsKind(SyntaxKindEx.SwitchExpressionArm));

            var wrapper = (SwitchExpressionArmSyntaxWrapper)syntaxNode;
            Assert.Same(syntaxNode, wrapper.SyntaxNode);
            Assert.Same(syntaxNode.Pattern, wrapper.Pattern.SyntaxNode);
            Assert.Same(syntaxNode.WhenClause, wrapper.WhenClause.SyntaxNode);
            Assert.Same(syntaxNode.Expression, wrapper.Expression);
            Assert.Equal(syntaxNode.EqualsGreaterThanToken, wrapper.EqualsGreaterThanToken);

            // Pattern
            var newPattern = SyntaxFactory.ConstantPattern(SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression));
            var wrapperWithModifiedPattern = wrapper.WithPattern((PatternSyntaxWrapper)newPattern);
            Assert.NotNull(wrapperWithModifiedPattern.SyntaxNode);
            Assert.NotSame(syntaxNode.Pattern, wrapperWithModifiedPattern.Pattern.SyntaxNode);
            Assert.Equal(SyntaxKind.ConstantPattern, wrapperWithModifiedPattern.Pattern.SyntaxNode.Kind());

            // When Clause
            var newWhenClause = SyntaxFactory.WhenClause(SyntaxFactory.LiteralExpression(SyntaxKind.TrueLiteralExpression));
            var wrapperWithModifiedWhenClause = wrapper.WithWhenClause((WhenClauseSyntaxWrapper)newWhenClause);
            Assert.NotNull(wrapperWithModifiedWhenClause.SyntaxNode);
            Assert.NotSame(syntaxNode.WhenClause, wrapperWithModifiedWhenClause.WhenClause.SyntaxNode);
            Assert.Equal(SyntaxKind.WhenClause, wrapperWithModifiedWhenClause.WhenClause.SyntaxNode.Kind());
            Assert.Equal(SyntaxKind.TrueLiteralExpression, wrapperWithModifiedWhenClause.WhenClause.Condition.Kind());

            // Expression
            var newExpression = SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression);
            var wrapperWithModifiedExpression = wrapper.WithExpression(newExpression);
            Assert.NotNull(wrapperWithModifiedExpression.SyntaxNode);
            Assert.NotSame(syntaxNode.Expression, wrapperWithModifiedExpression.Expression);
            Assert.Equal(SyntaxKind.NullLiteralExpression, wrapperWithModifiedExpression.Expression.Kind());

            // EqualsGreaterThanToken
            Assert.Throws<ArgumentException>(() => wrapper.WithEqualsGreaterThanToken(SyntaxFactory.Token(SyntaxKind.EqualsEqualsToken)));
        }

        [Fact]
        public void TestIsInstance()
        {
            Assert.False(SwitchExpressionArmSyntaxWrapper.IsInstance(null));
            Assert.False(SwitchExpressionArmSyntaxWrapper.IsInstance(SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression)));

            Assert.True(SwitchExpressionArmSyntaxWrapper.IsInstance(this.CreateSwitchExpressionArm()));
        }

        [Fact]
        public void TestConversionsNull()
        {
            var syntaxNode = default(SyntaxNode);
            var wrapper = (SwitchExpressionArmSyntaxWrapper)syntaxNode;

            CSharpSyntaxNode syntax = wrapper;
            Assert.Null(syntax);
        }

        [Fact]
        public void TestConversions()
        {
            var syntaxNode = this.CreateSwitchExpressionArm();
            var wrapper = (SwitchExpressionArmSyntaxWrapper)syntaxNode;

            CSharpSyntaxNode syntax = wrapper;
            Assert.Same(syntaxNode, syntax);
        }

        [Fact]
        public void TestInvalidConversion()
        {
            var syntaxNode = SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression);
            Assert.Throws<InvalidCastException>(() => (SwitchExpressionArmSyntaxWrapper)syntaxNode);
        }

        private SwitchExpressionArmSyntax CreateSwitchExpressionArm()
            => SyntaxFactory.SwitchExpressionArm(
                SyntaxFactory.DiscardPattern(),
                SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression));
    }
}
