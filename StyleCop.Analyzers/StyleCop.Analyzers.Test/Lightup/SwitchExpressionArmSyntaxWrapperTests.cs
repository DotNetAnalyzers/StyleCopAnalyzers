// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.Lightup
{
    using System;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using StyleCop.Analyzers.Lightup;
    using Xunit;

    public class SwitchExpressionArmSyntaxWrapperTests
    {
        [Fact]
        public void TestNull()
        {
            var switchExpressionSyntax = (SwitchExpressionArmSyntaxWrapper)default(SyntaxNode);

            Assert.Null(switchExpressionSyntax.SyntaxNode);

            Assert.Throws<NullReferenceException>(() => switchExpressionSyntax.Pattern);
            Assert.Throws<NullReferenceException>(() => switchExpressionSyntax.WhenClause);
            Assert.Throws<NullReferenceException>(() => switchExpressionSyntax.Expression);
            Assert.Throws<NullReferenceException>(() => switchExpressionSyntax.EqualsGreaterThanToken);
            Assert.Throws<NullReferenceException>(() => switchExpressionSyntax.WithExpression(SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression)));
            Assert.Throws<NullReferenceException>(() => switchExpressionSyntax.WithEqualsGreaterThanToken(SyntaxFactory.Token(SyntaxKind.EqualsGreaterThanToken)));
        }

        [Fact]
        public void TestConversionNull()
        {
            var wrapper = (SwitchExpressionArmSyntaxWrapper)default(SyntaxNode);

            CSharpSyntaxNode syntax = wrapper;
            Assert.Null(syntax);
        }

        [Fact]
        public void TestInvalidConversion()
        {
            var syntaxNode = SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression);
            Assert.Throws<InvalidCastException>(() => (SwitchExpressionArmSyntaxWrapper)syntaxNode);
        }
    }
}
