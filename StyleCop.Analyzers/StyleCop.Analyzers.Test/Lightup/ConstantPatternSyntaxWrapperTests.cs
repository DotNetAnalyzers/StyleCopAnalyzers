// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.Lightup
{
    using System;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
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
        public void TestIsInstance()
        {
            Assert.False(ConstantPatternSyntaxWrapper.IsInstance(null));
            Assert.False(ConstantPatternSyntaxWrapper.IsInstance(SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression)));
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
        public void TestInvalidConversion()
        {
            var syntaxNode = SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression);
            Assert.Throws<InvalidCastException>(() => (ConstantPatternSyntaxWrapper)syntaxNode);
        }
    }
}
