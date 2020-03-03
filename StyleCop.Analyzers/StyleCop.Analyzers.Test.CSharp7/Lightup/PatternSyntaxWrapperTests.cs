// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp7.Lightup
{
    using System;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using StyleCop.Analyzers.Lightup;
    using Xunit;

    public class PatternSyntaxWrapperTests
    {
        [Fact]
        public void TestNull()
        {
            var syntaxNode = default(SyntaxNode);
            var patternSyntax = (PatternSyntaxWrapper)syntaxNode;
            Assert.Null(patternSyntax.SyntaxNode);
        }

        [Fact]
        public void TestNonNull()
        {
            var syntaxNode = SyntaxFactory.ConstantPattern(SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression));
            var patternSyntax = (PatternSyntaxWrapper)syntaxNode;
            Assert.Same(syntaxNode, patternSyntax.SyntaxNode);
        }

        [Fact]
        public void TestIsInstance()
        {
            Assert.False(PatternSyntaxWrapper.IsInstance(null));
            Assert.False(PatternSyntaxWrapper.IsInstance(SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression)));

            var constantPatternSyntax = SyntaxFactory.ConstantPattern(SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression));
            Assert.True(PatternSyntaxWrapper.IsInstance(constantPatternSyntax));

            var declarationPatternSyntax = SyntaxFactory.DeclarationPattern(
                SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword)),
                SyntaxFactory.DiscardDesignation());
            Assert.True(PatternSyntaxWrapper.IsInstance(declarationPatternSyntax));
        }

        [Fact]
        public void TestConversionsNull()
        {
            var syntaxNode = default(SyntaxNode);
            var patternSyntax = (PatternSyntaxWrapper)syntaxNode;

            SyntaxNode syntax = patternSyntax;
            Assert.Null(syntax);
        }

        [Fact]
        public void TestConversions()
        {
            var syntaxNode = SyntaxFactory.ConstantPattern(SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression));
            var patternSyntax = (PatternSyntaxWrapper)syntaxNode;

            SyntaxNode syntax = patternSyntax;
            Assert.Same(syntaxNode, syntax);
        }

        [Fact]
        public void TestInvalidConversion()
        {
            var syntaxNode = SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression);
            Assert.Throws<InvalidCastException>(() => (PatternSyntaxWrapper)syntaxNode);
        }
    }
}
