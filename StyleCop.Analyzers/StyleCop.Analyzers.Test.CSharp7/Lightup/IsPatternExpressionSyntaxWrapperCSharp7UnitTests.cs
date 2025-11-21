// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.CSharp7.Lightup
{
    using System;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using StyleCop.Analyzers.Lightup;
    using Xunit;

    public class IsPatternExpressionSyntaxWrapperCSharp7UnitTests
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
            Assert.Throws<NullReferenceException>(() => isPatternExpressionSyntax.WithPattern((PatternSyntaxWrapper)SyntaxFactory.ConstantPattern(SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression))));
        }

        [Fact]
        public void TestProperties()
        {
            var syntaxNode = SyntaxFactory.IsPatternExpression(SyntaxFactory.DefaultExpression(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword))), SyntaxFactory.ConstantPattern(SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression)));
            Assert.True(syntaxNode.IsKind(SyntaxKind.IsPatternExpression));
            Assert.True(syntaxNode.IsKind(SyntaxKindEx.IsPatternExpression));

            var isPatternExpressionSyntax = (IsPatternExpressionSyntaxWrapper)syntaxNode;
            Assert.Same(syntaxNode, isPatternExpressionSyntax.SyntaxNode);
            Assert.Same(syntaxNode.Expression, isPatternExpressionSyntax.Expression);
            Assert.True(syntaxNode.IsKeyword.IsEquivalentTo(isPatternExpressionSyntax.IsKeyword));
            Assert.Same(syntaxNode.Pattern, isPatternExpressionSyntax.Pattern.SyntaxNode);
            Assert.Equal(SyntaxKind.NullLiteralExpression, ((ConstantPatternSyntax)isPatternExpressionSyntax.Pattern).Expression.Kind());

            var newExpression = SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(0));
            var modifiedExpression = isPatternExpressionSyntax.WithExpression(newExpression);
            Assert.NotNull(modifiedExpression.SyntaxNode);
            Assert.NotSame(syntaxNode.Expression, modifiedExpression.Expression);
            Assert.Equal(SyntaxKind.NumericLiteralExpression, modifiedExpression.Expression.Kind());

            var newIsKeyword = SyntaxFactory.Token(SyntaxKind.IsKeyword).WithLeadingTrivia(SyntaxFactory.Space);
            var modifiedIsKeyword = isPatternExpressionSyntax.WithIsKeyword(newIsKeyword);
            Assert.NotNull(modifiedIsKeyword.SyntaxNode);
            Assert.Single(modifiedIsKeyword.IsKeyword.LeadingTrivia);
            Assert.Equal(" ", modifiedIsKeyword.IsKeyword.LeadingTrivia.ToString());

            var newPattern = SyntaxFactory.ConstantPattern(newExpression);
            var modifiedPattern = isPatternExpressionSyntax.WithPattern((PatternSyntaxWrapper)newPattern);
            Assert.NotNull(modifiedPattern.SyntaxNode);
            Assert.Equal(SyntaxKind.DefaultExpression, modifiedPattern.Expression.Kind());
            Assert.Equal(SyntaxKind.NumericLiteralExpression, ((ConstantPatternSyntax)modifiedPattern.Pattern).Expression.Kind());
        }

        [Fact]
        public void TestIsInstance()
        {
            Assert.False(IsPatternExpressionSyntaxWrapper.IsInstance(null));
            Assert.False(IsPatternExpressionSyntaxWrapper.IsInstance(SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression)));

            var syntaxNode = SyntaxFactory.IsPatternExpression(SyntaxFactory.DefaultExpression(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword))), SyntaxFactory.ConstantPattern(SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression)));
            Assert.True(IsPatternExpressionSyntaxWrapper.IsInstance(syntaxNode));
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
        public void TestConversions()
        {
            var syntaxNode = SyntaxFactory.IsPatternExpression(SyntaxFactory.DefaultExpression(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword))), SyntaxFactory.ConstantPattern(SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression)));
            var isPatternExpressionSyntax = (IsPatternExpressionSyntaxWrapper)syntaxNode;

            ExpressionSyntax syntax = isPatternExpressionSyntax;
            Assert.Same(syntaxNode, syntax);
        }

        [Fact]
        public void TestInvalidConversion()
        {
            var syntaxNode = SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression);
            Assert.Throws<InvalidCastException>(() => (IsPatternExpressionSyntaxWrapper)syntaxNode);
        }
    }
}
