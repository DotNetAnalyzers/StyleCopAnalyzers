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

    public class TupleTypeSyntaxWrapperCSharp7UnitTests
    {
        [Fact]
        public void TestNull()
        {
            var syntaxNode = default(SyntaxNode);
            var wrapper = (TupleTypeSyntaxWrapper)syntaxNode;
            Assert.Null(wrapper.SyntaxNode);
            Assert.Throws<NullReferenceException>(() => wrapper.OpenParenToken);
            Assert.Throws<NullReferenceException>(() => wrapper.Elements);
            Assert.Throws<NullReferenceException>(() => wrapper.CloseParenToken);
            Assert.Throws<NullReferenceException>(() => wrapper.WithOpenParenToken(SyntaxFactory.Token(SyntaxKind.OpenParenToken)));
            Assert.Throws<NullReferenceException>(() => wrapper.WithElements(SeparatedSyntaxListWrapper<TupleElementSyntaxWrapper>.UnsupportedEmpty));
            Assert.Throws<NullReferenceException>(() => wrapper.AddElements());
            Assert.Throws<NullReferenceException>(() => wrapper.WithCloseParenToken(SyntaxFactory.Token(SyntaxKind.CloseParenToken)));
        }

        [Fact]
        public void TestProperties()
        {
            var syntaxNode = this.CreateTupleType();
            Assert.True(syntaxNode.IsKind(SyntaxKind.TupleType));
            Assert.True(syntaxNode.IsKind(SyntaxKindEx.TupleType));

            var wrapper = (TupleTypeSyntaxWrapper)syntaxNode;
            Assert.Same(syntaxNode, wrapper.SyntaxNode);
            Assert.True(syntaxNode.OpenParenToken.IsEquivalentTo(wrapper.OpenParenToken));
            Assert.Same(syntaxNode.Elements[0], wrapper.Elements[0].SyntaxNode);
            Assert.True(syntaxNode.CloseParenToken.IsEquivalentTo(wrapper.CloseParenToken));

            var newOpenParenToken = SyntaxFactory.Token(SyntaxKind.OpenParenToken).WithLeadingTrivia(SyntaxFactory.Space);
            var wrapperWithModifiedOpenParenToken = wrapper.WithOpenParenToken(newOpenParenToken);
            Assert.NotNull(wrapperWithModifiedOpenParenToken.SyntaxNode);
            Assert.Single(wrapperWithModifiedOpenParenToken.OpenParenToken.LeadingTrivia);
            Assert.Equal(" ", wrapperWithModifiedOpenParenToken.OpenParenToken.LeadingTrivia.ToString());

            var newElements = wrapper.Elements.Replace(wrapper.Elements[0], (TupleElementSyntaxWrapper)SyntaxFactory.TupleElement(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.StringKeyword))));
            var wrapperWithModifiedElements = wrapper.WithElements(newElements);
            Assert.NotNull(wrapperWithModifiedElements.SyntaxNode);
            Assert.NotEqual(syntaxNode.Elements[0], (TupleElementSyntax)wrapperWithModifiedElements.Elements[0]);
            Assert.Equal(SyntaxKind.StringKeyword, ((PredefinedTypeSyntax)wrapperWithModifiedElements.Elements[0].Type).Keyword.Kind());

            var wrapperWithAddedElements = wrapper.AddElements((TupleElementSyntaxWrapper)SyntaxFactory.TupleElement(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.ShortKeyword))));
            Assert.NotNull(wrapperWithAddedElements.SyntaxNode);
            Assert.Equal(wrapper.Elements.Count + 1, wrapperWithAddedElements.Elements.Count);
            Assert.Equal(SyntaxKind.PredefinedType, wrapperWithAddedElements.Elements.Last().Type.Kind());
            Assert.Equal(SyntaxKind.ShortKeyword, ((PredefinedTypeSyntax)wrapperWithAddedElements.Elements.Last().Type).Keyword.Kind());

            var newCloseParenToken = SyntaxFactory.Token(SyntaxKind.CloseParenToken).WithLeadingTrivia(SyntaxFactory.Space);
            var wrapperWithModifiedCloseParenToken = wrapper.WithCloseParenToken(newCloseParenToken);
            Assert.NotNull(wrapperWithModifiedCloseParenToken.SyntaxNode);
            Assert.Single(wrapperWithModifiedCloseParenToken.CloseParenToken.LeadingTrivia);
            Assert.Equal(" ", wrapperWithModifiedCloseParenToken.CloseParenToken.LeadingTrivia.ToString());
        }

        [Fact]
        public void TestIsInstance()
        {
            Assert.False(TupleTypeSyntaxWrapper.IsInstance(null));
            Assert.False(TupleTypeSyntaxWrapper.IsInstance(SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression)));

            var syntaxNode = this.CreateTupleType();
            Assert.True(TupleTypeSyntaxWrapper.IsInstance(syntaxNode));
        }

        [Fact]
        public void TestConversionsNull()
        {
            var syntaxNode = default(SyntaxNode);
            var wrapper = (TupleTypeSyntaxWrapper)syntaxNode;

            TypeSyntax syntax = wrapper;
            Assert.Null(syntax);
        }

        [Fact]
        public void TestConversions()
        {
            var syntaxNode = this.CreateTupleType();
            var wrapper = (TupleTypeSyntaxWrapper)syntaxNode;

            TypeSyntax syntax = wrapper;
            Assert.Same(syntaxNode, syntax);
        }

        [Fact]
        public void TestInvalidConversion()
        {
            var syntaxNode = SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression);
            Assert.Throws<InvalidCastException>(() => (TupleTypeSyntaxWrapper)syntaxNode);
        }

        private TupleTypeSyntax CreateTupleType()
        {
            return SyntaxFactory.TupleType(
                SyntaxFactory.SeparatedList(ImmutableArray.Create(
                    SyntaxFactory.TupleElement(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword))),
                    SyntaxFactory.TupleElement(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.UIntKeyword))))));
        }
    }
}
