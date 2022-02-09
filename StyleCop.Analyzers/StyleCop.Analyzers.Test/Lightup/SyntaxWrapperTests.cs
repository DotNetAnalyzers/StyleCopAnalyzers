// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.Lightup
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
    }
}
