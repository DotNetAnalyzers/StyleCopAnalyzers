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

    public class RefTypeSyntaxWrapperTests
    {
        [Fact]
        public void TestNull()
        {
            var syntaxNode = default(SyntaxNode);
            var wrapper = (RefTypeSyntaxWrapper)syntaxNode;
            Assert.Null(wrapper.SyntaxNode);
            Assert.Throws<NullReferenceException>(() => wrapper.RefKeyword);
            Assert.Throws<NullReferenceException>(() => wrapper.ReadOnlyKeyword);
            Assert.Throws<NullReferenceException>(() => wrapper.Type);
            Assert.Throws<NullReferenceException>(() => wrapper.WithType(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword))));
            Assert.Throws<NullReferenceException>(() => wrapper.WithRefKeyword(SyntaxFactory.Token(SyntaxKind.RefKeyword)));
            Assert.Throws<NullReferenceException>(() => wrapper.WithReadOnlyKeyword(SyntaxFactory.Token(SyntaxKind.ReadOnlyKeyword)));
        }

        [Fact]
        public void TestProperties()
        {
            var syntaxNode = this.CreateRefType();
            Assert.True(syntaxNode.IsKind(SyntaxKind.RefType));
            Assert.True(syntaxNode.IsKind(SyntaxKindEx.RefType));

            var wrapper = (RefTypeSyntaxWrapper)syntaxNode;
            Assert.Same(syntaxNode, wrapper.SyntaxNode);
            Assert.Same(syntaxNode.Type, wrapper.Type);
            Assert.True(syntaxNode.RefKeyword.IsEquivalentTo(wrapper.RefKeyword));
            Assert.Equal(default, syntaxNode.ReadOnlyKeyword);

            var newType = SyntaxFactory.PointerType(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.BoolKeyword)));
            var wrapperWithModifiedType = wrapper.WithType(newType);
            Assert.NotNull(wrapperWithModifiedType.SyntaxNode);
            Assert.NotSame(syntaxNode.Type, wrapperWithModifiedType.Type);
            Assert.Equal(SyntaxKind.PointerType, wrapperWithModifiedType.Type.Kind());

            var newRefKeyword = SyntaxFactory.Token(SyntaxKind.RefKeyword).WithLeadingTrivia(SyntaxFactory.Space);
            var wrapperWithModifiedRefKeyword = wrapper.WithRefKeyword(newRefKeyword);
            Assert.NotNull(wrapperWithModifiedRefKeyword.SyntaxNode);
            Assert.Single(wrapperWithModifiedRefKeyword.RefKeyword.LeadingTrivia);
            Assert.Equal(" ", wrapperWithModifiedRefKeyword.RefKeyword.LeadingTrivia.ToString());

            var readOnlyKeyword = SyntaxFactory.Token(SyntaxKind.ReadOnlyKeyword).WithLeadingTrivia(SyntaxFactory.Space);
            var wrapperWithReadOnlyKeyword = wrapper.WithReadOnlyKeyword(readOnlyKeyword);
            Assert.NotNull(wrapperWithReadOnlyKeyword.SyntaxNode);
            Assert.Single(wrapperWithReadOnlyKeyword.ReadOnlyKeyword.LeadingTrivia);
            Assert.Equal(" ", wrapperWithReadOnlyKeyword.ReadOnlyKeyword.LeadingTrivia.ToString());
            Assert.True(wrapperWithReadOnlyKeyword.ReadOnlyKeyword.IsEquivalentTo(readOnlyKeyword));
        }

        [Fact]
        public void TestIsInstance()
        {
            Assert.False(RefTypeSyntaxWrapper.IsInstance(null));
            Assert.False(RefTypeSyntaxWrapper.IsInstance(SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression)));

            var syntaxNode = this.CreateRefType();
            Assert.True(RefTypeSyntaxWrapper.IsInstance(syntaxNode));
        }

        [Fact]
        public void TestConversionsNull()
        {
            var syntaxNode = default(SyntaxNode);
            var wrapper = (RefTypeSyntaxWrapper)syntaxNode;

            TypeSyntax syntax = wrapper;
            Assert.Null(syntax);
        }

        [Fact]
        public void TestConversions()
        {
            var syntaxNode = this.CreateRefType();
            var wrapper = (RefTypeSyntaxWrapper)syntaxNode;

            TypeSyntax syntax = wrapper;
            Assert.Same(syntaxNode, syntax);
        }

        [Fact]
        public void TestInvalidConversion()
        {
            var syntaxNode = SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression);
            Assert.Throws<InvalidCastException>(() => (RefTypeSyntaxWrapper)syntaxNode);
        }

        private RefTypeSyntax CreateRefType()
        {
            return SyntaxFactory.RefType(SyntaxFactory.Token(SyntaxKind.RefKeyword), SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword)));
        }
    }
}
