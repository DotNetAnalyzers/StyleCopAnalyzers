// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp7.Lightup
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using StyleCop.Analyzers.Lightup;
    using Xunit;

    public class SyntaxWrapperCSharp7UnitTests
    {
        [Fact]
        public void TestWrapSyntaxNode()
        {
            SyntaxNode syntaxNode = SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression);

            Assert.Same(syntaxNode, SyntaxWrapper<LiteralExpressionSyntax>.Default.Wrap(syntaxNode));
            Assert.Same(syntaxNode, SyntaxWrapper<LiteralExpressionSyntax>.Default.Unwrap((LiteralExpressionSyntax)syntaxNode));
        }

        [Fact]
        public void TestWrapSyntaxWrapperNode()
        {
            var syntaxNode = SyntaxFactory.DiscardDesignation();
            var syntaxWrapper = SyntaxWrapper<DiscardDesignationSyntaxWrapper>.Default;

            Assert.Same(syntaxNode, syntaxWrapper.Wrap(syntaxNode).SyntaxNode);
            Assert.Same(syntaxNode, syntaxWrapper.Unwrap((DiscardDesignationSyntaxWrapper)syntaxNode));
        }

        [Fact]
        public void TestWrapNullSyntaxWrapperNode()
        {
            var syntaxWrapper = SyntaxWrapper<DiscardDesignationSyntaxWrapper>.Default;

            Assert.Null(syntaxWrapper.Wrap(null).SyntaxNode);
            Assert.Null(syntaxWrapper.Unwrap(default));
        }

        [Fact]
        [WorkItem(3258, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3258")]
        public void TestExpressionOrPatternFallbackForPattern()
        {
            var pattern = SyntaxFactory.ConstantPattern(SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression));
            Assert.True(ExpressionOrPatternSyntaxWrapper.IsInstance(pattern));
            Assert.Same(pattern, ((ExpressionOrPatternSyntaxWrapper)pattern).SyntaxNode);
            ExpressionOrPatternSyntaxWrapper upcast = (PatternSyntaxWrapper)pattern;
            Assert.Same(pattern, upcast.SyntaxNode);
            Assert.Same(pattern, ((PatternSyntaxWrapper)upcast).SyntaxNode);
        }
    }
}
