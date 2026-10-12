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

    public class WhenClauseSyntaxWrapperCSharp7UnitTests
    {
        [Fact]
        public void TestNull()
        {
            var syntaxNode = default(SyntaxNode);
            var wrapper = (WhenClauseSyntaxWrapper)syntaxNode;
            Assert.Null(wrapper.SyntaxNode);
            Assert.Throws<NullReferenceException>(() => wrapper.WhenKeyword);
            Assert.Throws<NullReferenceException>(() => wrapper.Condition);
            Assert.Throws<NullReferenceException>(() => wrapper.WithCondition(SyntaxFactory.LiteralExpression(SyntaxKind.TrueLiteralExpression)));
            Assert.Throws<NullReferenceException>(() => wrapper.WithWhenKeyword(SyntaxFactory.Token(SyntaxKind.WhenKeyword)));
        }

        [Fact]
        public void TestProperties()
        {
            var syntaxNode = this.CreateWhenClause();
            Assert.True(syntaxNode.IsKind(SyntaxKind.WhenClause));
            Assert.True(syntaxNode.IsKind(SyntaxKindEx.WhenClause));

            var wrapper = (WhenClauseSyntaxWrapper)syntaxNode;
            Assert.Same(syntaxNode, wrapper.SyntaxNode);
            Assert.Same(syntaxNode.Condition, wrapper.Condition);
            Assert.True(syntaxNode.WhenKeyword.IsEquivalentTo(wrapper.WhenKeyword));

            var newCondition = SyntaxFactory.LiteralExpression(SyntaxKind.FalseLiteralExpression);
            var wrapperWithModifiedCondition = wrapper.WithCondition(newCondition);
            Assert.NotNull(wrapperWithModifiedCondition.SyntaxNode);
            Assert.NotSame(syntaxNode.Condition, wrapperWithModifiedCondition.Condition);
            Assert.Equal(SyntaxKind.FalseLiteralExpression, wrapperWithModifiedCondition.Condition.Kind());

            var newWhenKeyword = SyntaxFactory.Token(SyntaxKind.WhenKeyword).WithLeadingTrivia(SyntaxFactory.Space);
            var wrapperWithModifiedWhenKeyword = wrapper.WithWhenKeyword(newWhenKeyword);
            Assert.NotNull(wrapperWithModifiedWhenKeyword.SyntaxNode);
            Assert.Single(wrapperWithModifiedWhenKeyword.WhenKeyword.LeadingTrivia);
            Assert.Equal(" ", wrapperWithModifiedWhenKeyword.WhenKeyword.LeadingTrivia.ToString());
        }

        [Fact]
        public void TestIsInstance()
        {
            Assert.False(WhenClauseSyntaxWrapper.IsInstance(null));
            Assert.False(WhenClauseSyntaxWrapper.IsInstance(SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression)));

            var syntaxNode = this.CreateWhenClause();
            Assert.True(WhenClauseSyntaxWrapper.IsInstance(syntaxNode));
        }

        [Fact]
        public void TestConversionsNull()
        {
            var syntaxNode = default(SyntaxNode);
            var wrapper = (WhenClauseSyntaxWrapper)syntaxNode;

            CSharpSyntaxNode syntax = wrapper;
            Assert.Null(syntax);
        }

        [Fact]
        public void TestConversions()
        {
            var syntaxNode = this.CreateWhenClause();
            var wrapper = (WhenClauseSyntaxWrapper)syntaxNode;

            CSharpSyntaxNode syntax = wrapper;
            Assert.Same(syntaxNode, syntax);
        }

        [Fact]
        public void TestInvalidConversion()
        {
            var syntaxNode = SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression);
            Assert.Throws<InvalidCastException>(() => (WhenClauseSyntaxWrapper)syntaxNode);
        }

        private WhenClauseSyntax CreateWhenClause()
        {
            return SyntaxFactory.WhenClause(SyntaxFactory.Token(SyntaxKind.WhenKeyword), SyntaxFactory.LiteralExpression(SyntaxKind.TrueLiteralExpression));
        }
    }
}
