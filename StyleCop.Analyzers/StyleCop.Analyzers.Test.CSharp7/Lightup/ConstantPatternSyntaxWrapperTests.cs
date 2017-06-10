// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp7.Lightup
{
    using System;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using StyleCop.Analyzers.Helpers;
    using StyleCop.Analyzers.Lightup;
    using Xunit;

    public class ConstantPatternSyntaxWrapperTests
    {
        [Fact]
        public void TestNull()
        {
            var syntaxNode = default(SyntaxNode);
            var constantPatternSyntax = (ConstantPatternSyntaxWrapper)syntaxNode;
            Assert.Null(constantPatternSyntax.SyntaxNode);
            Assert.Throws<NullReferenceException>(() => constantPatternSyntax.Expression);
            Assert.Throws<NullReferenceException>(() => constantPatternSyntax.WithExpression(SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression)));
        }

        [Fact]
        public void TestExpression()
        {
            var syntaxNode = SyntaxFactory.ConstantPattern(SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression));
            Assert.True(syntaxNode.IsKind(SyntaxKind.ConstantPattern));
            Assert.True(syntaxNode.IsKind(SyntaxKindEx.ConstantPattern));

            var constantPatternSyntax = (ConstantPatternSyntaxWrapper)syntaxNode;
            Assert.Same(syntaxNode, constantPatternSyntax.SyntaxNode);
            Assert.Same(syntaxNode.Expression, constantPatternSyntax.Expression);

            constantPatternSyntax = constantPatternSyntax.WithExpression(SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(1)));
            Assert.NotNull(constantPatternSyntax.SyntaxNode);
            Assert.NotSame(syntaxNode, constantPatternSyntax.SyntaxNode);
            Assert.Equal(SyntaxKind.NumericLiteralExpression, constantPatternSyntax.Expression.Kind());
        }

        [Fact]
        public void TestIsInstance()
        {
            Assert.False(ConstantPatternSyntaxWrapper.IsInstance(null));
            Assert.False(ConstantPatternSyntaxWrapper.IsInstance(SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression)));

            var syntaxNode = SyntaxFactory.ConstantPattern(SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression));
            Assert.True(ConstantPatternSyntaxWrapper.IsInstance(syntaxNode));
        }

        [Fact]
        public void TestConversionsNull()
        {
            var syntaxNode = default(SyntaxNode);
            var constantPatternSyntax = (ConstantPatternSyntaxWrapper)syntaxNode;

            PatternSyntaxWrapper patternSyntax = constantPatternSyntax;
            Assert.Null(patternSyntax.SyntaxNode);

            constantPatternSyntax = (ConstantPatternSyntaxWrapper)patternSyntax;
            Assert.Null(constantPatternSyntax.SyntaxNode);

            SyntaxNode syntax = constantPatternSyntax;
            Assert.Null(syntax);
        }

        [Fact]
        public void TestConversions()
        {
            var syntaxNode = SyntaxFactory.ConstantPattern(SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression));
            var constantPatternSyntax = (ConstantPatternSyntaxWrapper)syntaxNode;

            PatternSyntaxWrapper patternSyntax = constantPatternSyntax;
            Assert.Same(syntaxNode, patternSyntax.SyntaxNode);

            constantPatternSyntax = (ConstantPatternSyntaxWrapper)patternSyntax;
            Assert.Same(syntaxNode, constantPatternSyntax.SyntaxNode);

            SyntaxNode syntax = constantPatternSyntax;
            Assert.Same(syntaxNode, syntax);
        }

        [Fact]
        public void TestInvalidConversion()
        {
            var syntaxNode = SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression);
            Assert.Throws<InvalidCastException>(() => (ConstantPatternSyntaxWrapper)syntaxNode);
        }
    }
}
