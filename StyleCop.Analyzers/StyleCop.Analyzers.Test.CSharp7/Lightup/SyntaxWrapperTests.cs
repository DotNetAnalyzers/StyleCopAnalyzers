// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp7.Lightup
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using StyleCop.Analyzers.Lightup;
    using Xunit;

    public class SyntaxWrapperTests
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
    }
}
