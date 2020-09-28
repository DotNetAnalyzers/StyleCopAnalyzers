// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp7.Lightup
{
    using System;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using StyleCop.Analyzers.Lightup;
    using Xunit;

    public class ImplicitStackAllocArrayCreationExpressionSyntaxWrapperTests
    {
        [Fact]
        public void TestNull()
        {
            var syntaxNode = default(SyntaxNode);
            var wrapper = (ImplicitStackAllocArrayCreationExpressionSyntaxWrapper)syntaxNode;
            Assert.Null(wrapper.SyntaxNode);
            Assert.Throws<NullReferenceException>(() => wrapper.StackAllocKeyword);
            Assert.Throws<NullReferenceException>(() => wrapper.OpenBracketToken);
            Assert.Throws<NullReferenceException>(() => wrapper.CloseBracketToken);
            Assert.Throws<NullReferenceException>(() => wrapper.Initializer);
            Assert.Throws<NullReferenceException>(() => wrapper.WithStackAllocKeyword(SyntaxFactory.Token(SyntaxKind.StackAllocKeyword)));
            Assert.Throws<NullReferenceException>(() => wrapper.WithOpenBracketToken(SyntaxFactory.Token(SyntaxKind.OpenBracketToken)));
            Assert.Throws<NullReferenceException>(() => wrapper.WithCloseBracketToken(SyntaxFactory.Token(SyntaxKind.CloseBracketToken)));
            Assert.Throws<NullReferenceException>(() => wrapper.WithInitializer(SyntaxFactory.InitializerExpression(SyntaxKind.ArrayInitializerExpression)));
            Assert.Throws<NullReferenceException>(() => wrapper.AddInitializerExpressions());
        }

        [Fact]
        public void TestProperties()
        {
            var syntaxNode = this.CreateImplicitStackAllocArrayCreationExpression();
            Assert.True(syntaxNode.IsKind(SyntaxKind.ImplicitStackAllocArrayCreationExpression));
            Assert.True(syntaxNode.IsKind(SyntaxKindEx.ImplicitStackAllocArrayCreationExpression));

            var wrapper = (ImplicitStackAllocArrayCreationExpressionSyntaxWrapper)syntaxNode;
            Assert.Same(syntaxNode, wrapper.SyntaxNode);
            Assert.Equal(syntaxNode.StackAllocKeyword, wrapper.StackAllocKeyword); // This is a struct, so we can't use Same()
            Assert.Equal(syntaxNode.OpenBracketToken, wrapper.OpenBracketToken); // This is a struct, so we can't use Same()
            Assert.Equal(syntaxNode.CloseBracketToken, wrapper.CloseBracketToken); // This is a struct, so we can't use Same()
            Assert.Same(syntaxNode.Initializer, wrapper.Initializer);

            var newStackAllocKeyword = SyntaxFactory.Token(SyntaxKind.StackAllocKeyword);
            var wrapperWithModifiedStackAllocKeyword = wrapper.WithStackAllocKeyword(newStackAllocKeyword);
            Assert.NotNull(wrapperWithModifiedStackAllocKeyword.SyntaxNode);
            Assert.NotEqual(syntaxNode.StackAllocKeyword, wrapperWithModifiedStackAllocKeyword.StackAllocKeyword);
            Assert.Equal(SyntaxKind.StackAllocKeyword, wrapperWithModifiedStackAllocKeyword.StackAllocKeyword.Kind());
            Assert.Equal("stackalloc", wrapperWithModifiedStackAllocKeyword.StackAllocKeyword.Text);

            var newOpenBracketToken = SyntaxFactory.Token(SyntaxKind.OpenBracketToken);
            var wrapperWithModifiedOpenBracketToken = wrapper.WithOpenBracketToken(newOpenBracketToken);
            Assert.NotNull(wrapperWithModifiedOpenBracketToken.SyntaxNode);
            Assert.NotEqual(syntaxNode.OpenBracketToken, wrapperWithModifiedOpenBracketToken.OpenBracketToken);
            Assert.Equal(SyntaxKind.OpenBracketToken, wrapperWithModifiedOpenBracketToken.OpenBracketToken.Kind());
            Assert.Equal("[", wrapperWithModifiedOpenBracketToken.OpenBracketToken.Text);

            var newCloseBracketToken = SyntaxFactory.Token(SyntaxKind.CloseBracketToken);
            var wrapperWithModifiedCloseBracketToken = wrapper.WithCloseBracketToken(newCloseBracketToken);
            Assert.NotNull(wrapperWithModifiedCloseBracketToken.SyntaxNode);
            Assert.NotEqual(syntaxNode.CloseBracketToken, wrapperWithModifiedCloseBracketToken.CloseBracketToken);
            Assert.Equal(SyntaxKind.CloseBracketToken, wrapperWithModifiedCloseBracketToken.CloseBracketToken.Kind());
            Assert.Equal("]", wrapperWithModifiedCloseBracketToken.CloseBracketToken.Text);

            var newInitializer = SyntaxFactory.InitializerExpression(SyntaxKind.ArrayInitializerExpression);
            var wrapperWithModifiedInitializer = wrapper.WithInitializer(newInitializer);
            Assert.NotNull(wrapperWithModifiedInitializer.SyntaxNode);
            Assert.NotSame(syntaxNode.Initializer, wrapperWithModifiedInitializer.Initializer);
            Assert.Empty(wrapperWithModifiedInitializer.Initializer.Expressions);

            var addedInitializerExpressions = new ExpressionSyntax[] { SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(2)) };
            var wrapperWithAddedInitializerExpressions = wrapper.AddInitializerExpressions(addedInitializerExpressions);
            Assert.NotNull(wrapperWithAddedInitializerExpressions.SyntaxNode);
            Assert.NotSame(syntaxNode.Initializer, wrapperWithAddedInitializerExpressions.Initializer);
            Assert.Equal(2, wrapperWithAddedInitializerExpressions.Initializer.Expressions.Count);
            Assert.Equal("1", wrapperWithAddedInitializerExpressions.Initializer.Expressions[0].ToString());
            Assert.Equal("2", wrapperWithAddedInitializerExpressions.Initializer.Expressions[1].ToString());
        }

        [Fact]
        public void TestIsInstance()
        {
            Assert.False(ImplicitStackAllocArrayCreationExpressionSyntaxWrapper.IsInstance(null));
            Assert.False(ImplicitStackAllocArrayCreationExpressionSyntaxWrapper.IsInstance(SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression)));

            var syntaxNode = this.CreateImplicitStackAllocArrayCreationExpression();
            Assert.True(ImplicitStackAllocArrayCreationExpressionSyntaxWrapper.IsInstance(syntaxNode));
        }

        [Fact]
        public void TestConversionsNull()
        {
            var syntaxNode = default(SyntaxNode);
            var wrapper = (ImplicitStackAllocArrayCreationExpressionSyntaxWrapper)syntaxNode;

            ExpressionSyntax syntax = wrapper;
            Assert.Null(syntax);
        }

        [Fact]
        public void TestConversions()
        {
            var syntaxNode = this.CreateImplicitStackAllocArrayCreationExpression();
            var wrapper = (ImplicitStackAllocArrayCreationExpressionSyntaxWrapper)syntaxNode;

            ExpressionSyntax syntax = wrapper;
            Assert.Same(syntaxNode, syntax);
        }

        [Fact]
        public void TestInvalidConversion()
        {
            var syntaxNode = SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression);
            Assert.Throws<InvalidCastException>(() => (ImplicitStackAllocArrayCreationExpressionSyntaxWrapper)syntaxNode);
        }

        private ImplicitStackAllocArrayCreationExpressionSyntax CreateImplicitStackAllocArrayCreationExpression()
        {
            return SyntaxFactory.ImplicitStackAllocArrayCreationExpression(
                stackAllocKeyword: SyntaxFactory.Token(SyntaxKind.StackAllocKeyword),
                openBracketToken: SyntaxFactory.Token(SyntaxKind.OpenBracketToken),
                closeBracketToken: SyntaxFactory.Token(SyntaxKind.CloseBracketToken),
                initializer: SyntaxFactory.InitializerExpression(
                    SyntaxKind.ArrayInitializerExpression,
                    SyntaxFactory.SingletonSeparatedList<ExpressionSyntax>(SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(1)))));
        }
    }
}
