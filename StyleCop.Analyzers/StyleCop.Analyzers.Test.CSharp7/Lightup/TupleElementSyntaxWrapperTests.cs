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

    public class TupleElementSyntaxWrapperTests
    {
        [Fact]
        public void TestNull()
        {
            var syntaxNode = default(SyntaxNode);
            var wrapper = (TupleElementSyntaxWrapper)syntaxNode;
            Assert.Null(wrapper.SyntaxNode);
            Assert.Throws<NullReferenceException>(() => wrapper.Type);
            Assert.Throws<NullReferenceException>(() => wrapper.Identifier);
            Assert.Throws<NullReferenceException>(() => wrapper.WithType(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.StringKeyword))));
            Assert.Throws<NullReferenceException>(() => wrapper.WithIdentifier(SyntaxFactory.Identifier("x")));
        }

        [Fact]
        public void TestProperties()
        {
            var syntaxNode = this.CreateTupleElement();
            Assert.True(syntaxNode.IsKind(SyntaxKind.TupleElement));
            Assert.True(syntaxNode.IsKind(SyntaxKindEx.TupleElement));

            var wrapper = (TupleElementSyntaxWrapper)syntaxNode;
            Assert.Same(syntaxNode, wrapper.SyntaxNode);
            Assert.Same(syntaxNode.Type, wrapper.Type);
            Assert.True(syntaxNode.Identifier.IsEquivalentTo(wrapper.Identifier));

            var newType = SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.UIntKeyword));
            var wrapperWithModifiedType = wrapper.WithType(newType);
            Assert.NotNull(wrapperWithModifiedType.SyntaxNode);
            Assert.NotSame(syntaxNode.Type, wrapperWithModifiedType.Type);
            Assert.Equal(SyntaxKind.UIntKeyword, ((PredefinedTypeSyntax)wrapperWithModifiedType.Type).Keyword.Kind());

            var newIdentifier = SyntaxFactory.Identifier("y").WithLeadingTrivia(SyntaxFactory.Space);
            var wrapperWithModifiedIdentifier = wrapper.WithIdentifier(newIdentifier);
            Assert.NotNull(wrapperWithModifiedIdentifier.SyntaxNode);
            Assert.Single(wrapperWithModifiedIdentifier.Identifier.LeadingTrivia);
            Assert.Equal(" ", wrapperWithModifiedIdentifier.Identifier.LeadingTrivia.ToString());
        }

        [Fact]
        public void TestIsInstance()
        {
            Assert.False(TupleElementSyntaxWrapper.IsInstance(null));
            Assert.False(TupleElementSyntaxWrapper.IsInstance(SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression)));

            var syntaxNode = this.CreateTupleElement();
            Assert.True(TupleElementSyntaxWrapper.IsInstance(syntaxNode));
        }

        [Fact]
        public void TestConversionsNull()
        {
            var syntaxNode = default(SyntaxNode);
            var wrapper = (TupleElementSyntaxWrapper)syntaxNode;

            CSharpSyntaxNode syntax = wrapper;
            Assert.Null(syntax);
        }

        [Fact]
        public void TestConversions()
        {
            var syntaxNode = this.CreateTupleElement();
            var wrapper = (TupleElementSyntaxWrapper)syntaxNode;

            CSharpSyntaxNode syntax = wrapper;
            Assert.Same(syntaxNode, syntax);
        }

        [Fact]
        public void TestInvalidConversion()
        {
            var syntaxNode = SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression);
            Assert.Throws<InvalidCastException>(() => (TupleElementSyntaxWrapper)syntaxNode);
        }

        private TupleElementSyntax CreateTupleElement()
        {
            return SyntaxFactory.TupleElement(
                SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword)),
                SyntaxFactory.Identifier("id"));
        }
    }
}
