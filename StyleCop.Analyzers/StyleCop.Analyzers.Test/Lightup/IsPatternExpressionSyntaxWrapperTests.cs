// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.Lightup
{
    using System;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using StyleCop.Analyzers.Lightup;
    using Xunit;

    public class IsPatternExpressionSyntaxWrapperTests
    {
        [Fact]
        public void TestNull()
        {
            var syntaxNode = default(SyntaxNode);
            var isPatternExpressionSyntax = (IsPatternExpressionSyntaxWrapper)syntaxNode;
            Assert.Null(isPatternExpressionSyntax.SyntaxNode);
            Assert.Throws<NullReferenceException>(() => isPatternExpressionSyntax.Expression);
            Assert.Throws<NullReferenceException>(() => isPatternExpressionSyntax.IsKeyword);
            Assert.Throws<NullReferenceException>(() => isPatternExpressionSyntax.Pattern);
            Assert.Throws<NullReferenceException>(() => isPatternExpressionSyntax.WithExpression(SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression)));
            Assert.Throws<NullReferenceException>(() => isPatternExpressionSyntax.WithIsKeyword(SyntaxFactory.Token(SyntaxKind.IsKeyword)));
            Assert.Throws<NullReferenceException>(() => isPatternExpressionSyntax.WithPattern((PatternSyntaxWrapper)null));
        }

        [Fact]
        public void TestIsInstance()
        {
            Assert.False(IsPatternExpressionSyntaxWrapper.IsInstance(null));
            Assert.False(IsPatternExpressionSyntaxWrapper.IsInstance(SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression)));
        }

        [Fact]
        public void TestConversionsNull()
        {
            var syntaxNode = default(SyntaxNode);
            var isPatternExpressionSyntax = (IsPatternExpressionSyntaxWrapper)syntaxNode;

            ExpressionSyntax syntax = isPatternExpressionSyntax;
            Assert.Null(syntax);
        }

        [Fact]
        public void TestInvalidConversion()
        {
            var syntaxNode = SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression);
            Assert.Throws<InvalidCastException>(() => (IsPatternExpressionSyntaxWrapper)syntaxNode);
        }
    }
}
