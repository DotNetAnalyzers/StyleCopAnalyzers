// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.CSharp7.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using StyleCop.Analyzers.Lightup;
    using Xunit;

    public class TupleExpressionSyntaxWrapperTests
    {
        [Fact]
        public void TestNull()
        {
            var syntaxNode = default(SyntaxNode);
            var wrapper = (TupleExpressionSyntaxWrapper)syntaxNode;
            Assert.Null(wrapper.SyntaxNode);
            Assert.Throws<NullReferenceException>(() => wrapper.OpenParenToken);
            Assert.Throws<NullReferenceException>(() => wrapper.Arguments);
            Assert.Throws<NullReferenceException>(() => wrapper.CloseParenToken);
            Assert.Throws<NullReferenceException>(() => wrapper.WithOpenParenToken(SyntaxFactory.Token(SyntaxKind.OpenParenToken)));
            Assert.Throws<NullReferenceException>(() => wrapper.WithArguments(SyntaxFactory.SeparatedList(
                ImmutableArray.Create(
                    SyntaxFactory.Argument(SyntaxFactory.LiteralExpression(SyntaxKind.TrueLiteralExpression)),
                    SyntaxFactory.Argument(SyntaxFactory.LiteralExpression(SyntaxKind.FalseLiteralExpression))))));
            Assert.Throws<NullReferenceException>(() => wrapper.AddArguments());
            Assert.Throws<NullReferenceException>(() => wrapper.WithCloseParenToken(SyntaxFactory.Token(SyntaxKind.CloseParenToken)));
        }

        [Fact]
        public void TestProperties()
        {
            var syntaxNode = this.CreateTupleExpression();
            Assert.True(syntaxNode.IsKind(SyntaxKind.TupleExpression));
            Assert.True(syntaxNode.IsKind(SyntaxKindEx.TupleExpression));

            var wrapper = (TupleExpressionSyntaxWrapper)syntaxNode;
            Assert.Same(syntaxNode, wrapper.SyntaxNode);
            Assert.True(syntaxNode.OpenParenToken.IsEquivalentTo(wrapper.OpenParenToken));
            Assert.Same(syntaxNode.Arguments[0], wrapper.Arguments[0]);
            Assert.True(syntaxNode.CloseParenToken.IsEquivalentTo(wrapper.CloseParenToken));

            var newOpenParenToken = SyntaxFactory.Token(SyntaxKind.OpenParenToken).WithLeadingTrivia(SyntaxFactory.Space);
            var wrapperWithModifiedOpenParenToken = wrapper.WithOpenParenToken(newOpenParenToken);
            Assert.NotNull(wrapperWithModifiedOpenParenToken.SyntaxNode);
            Assert.Single(wrapperWithModifiedOpenParenToken.OpenParenToken.LeadingTrivia);
            Assert.Equal(" ", wrapperWithModifiedOpenParenToken.OpenParenToken.LeadingTrivia.ToString());

            var newArguments = wrapper.Arguments.Replace(wrapper.Arguments[0], SyntaxFactory.Argument(SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression)));
            var wrapperWithModifiedArguments = wrapper.WithArguments(newArguments);
            Assert.NotNull(wrapperWithModifiedArguments.SyntaxNode);
            Assert.NotSame(syntaxNode.Arguments[0], wrapperWithModifiedArguments.Arguments[0]);
            Assert.Equal(SyntaxKind.NullLiteralExpression, wrapperWithModifiedArguments.Arguments[0].Expression.Kind());

            var wrapperWithAddedArguments = wrapper.AddArguments(SyntaxFactory.Argument(SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(42))));
            Assert.NotNull(wrapperWithAddedArguments.SyntaxNode);
            Assert.Equal(wrapper.Arguments.Count + 1, wrapperWithAddedArguments.Arguments.Count);
            Assert.Equal(SyntaxKind.NumericLiteralExpression, wrapperWithAddedArguments.Arguments.Last().Expression.Kind());
            Assert.Equal("42", wrapperWithAddedArguments.Arguments.Last().ToString());

            var newCloseParenToken = SyntaxFactory.Token(SyntaxKind.CloseParenToken).WithLeadingTrivia(SyntaxFactory.Space);
            var wrapperWithModifiedCloseParenToken = wrapper.WithCloseParenToken(newCloseParenToken);
            Assert.NotNull(wrapperWithModifiedCloseParenToken.SyntaxNode);
            Assert.Single(wrapperWithModifiedCloseParenToken.CloseParenToken.LeadingTrivia);
            Assert.Equal(" ", wrapperWithModifiedCloseParenToken.CloseParenToken.LeadingTrivia.ToString());
        }

        [Fact]
        public void TestIsInstance()
        {
            Assert.False(TupleExpressionSyntaxWrapper.IsInstance(null));
            Assert.False(TupleExpressionSyntaxWrapper.IsInstance(SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression)));

            var syntaxNode = this.CreateTupleExpression();
            Assert.True(TupleExpressionSyntaxWrapper.IsInstance(syntaxNode));
        }

        [Fact]
        public void TestConversionsNull()
        {
            var syntaxNode = default(SyntaxNode);
            var wrapper = (TupleExpressionSyntaxWrapper)syntaxNode;

            ExpressionSyntax syntax = wrapper;
            Assert.Null(syntax);
        }

        [Fact]
        public void TestConversions()
        {
            var syntaxNode = this.CreateTupleExpression();
            var wrapper = (TupleExpressionSyntaxWrapper)syntaxNode;

            ExpressionSyntax syntax = wrapper;
            Assert.Same(syntaxNode, syntax);
        }

        [Fact]
        public void TestInvalidConversion()
        {
            var syntaxNode = SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression);
            Assert.Throws<InvalidCastException>(() => (TupleExpressionSyntaxWrapper)syntaxNode);
        }

        private TupleExpressionSyntax CreateTupleExpression()
        {
            return SyntaxFactory.TupleExpression(
                SyntaxFactory.SeparatedList(
                    ImmutableArray.Create(
                        SyntaxFactory.Argument(SyntaxFactory.LiteralExpression(SyntaxKind.TrueLiteralExpression)),
                        SyntaxFactory.Argument(SyntaxFactory.LiteralExpression(SyntaxKind.FalseLiteralExpression)))));
        }
    }
}
