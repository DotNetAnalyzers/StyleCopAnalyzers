// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.CSharp7.Lightup
{
    using System;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using StyleCop.Analyzers.Helpers;
    using StyleCop.Analyzers.Lightup;
    using Xunit;

    public class DiscardDesignationSyntaxWrapperCSharp7UnitTests
    {
        [Fact]
        public void TestNull()
        {
            var syntaxNode = default(SyntaxNode);
            var discardDesignationSyntax = (DiscardDesignationSyntaxWrapper)syntaxNode;
            Assert.Null(discardDesignationSyntax.SyntaxNode);
            Assert.Throws<NullReferenceException>(() => discardDesignationSyntax.UnderscoreToken);
            Assert.Throws<NullReferenceException>(() => discardDesignationSyntax.WithUnderscoreToken(SyntaxFactory.Token(SyntaxKind.UnderscoreToken)));
        }

        [Fact]
        public void TestUnderscoreToken()
        {
            var syntaxNode = SyntaxFactory.DiscardDesignation();
            Assert.True(syntaxNode.IsKind(SyntaxKind.DiscardDesignation));
            Assert.True(syntaxNode.IsKind(SyntaxKindEx.DiscardDesignation));

            var discardDesignationSyntax = (DiscardDesignationSyntaxWrapper)syntaxNode;
            Assert.Same(syntaxNode, discardDesignationSyntax.SyntaxNode);
            Assert.Equal(syntaxNode.UnderscoreToken, discardDesignationSyntax.UnderscoreToken);

            discardDesignationSyntax = discardDesignationSyntax.WithUnderscoreToken(SpacingExtensions.WithoutTrivia(SyntaxFactory.Token(SyntaxKind.UnderscoreToken)));
            Assert.NotNull(discardDesignationSyntax.SyntaxNode);
            Assert.NotSame(syntaxNode, discardDesignationSyntax.SyntaxNode);
            Assert.False(syntaxNode.UnderscoreToken.IsEquivalentTo(discardDesignationSyntax.UnderscoreToken));
        }

        [Fact]
        public void TestIsInstance()
        {
            Assert.False(DiscardDesignationSyntaxWrapper.IsInstance(null));
            Assert.False(DiscardDesignationSyntaxWrapper.IsInstance(SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression)));

            var syntaxNode = SyntaxFactory.DiscardDesignation();
            Assert.True(DiscardDesignationSyntaxWrapper.IsInstance(syntaxNode));
        }

        [Fact]
        public void TestConversionsNull()
        {
            var syntaxNode = default(SyntaxNode);
            var discardDesignationSyntax = (DiscardDesignationSyntaxWrapper)syntaxNode;

            VariableDesignationSyntaxWrapper variableDesignationSyntax = discardDesignationSyntax;
            Assert.Null(variableDesignationSyntax.SyntaxNode);

            discardDesignationSyntax = (DiscardDesignationSyntaxWrapper)variableDesignationSyntax;
            Assert.Null(discardDesignationSyntax.SyntaxNode);

            SyntaxNode syntax = discardDesignationSyntax;
            Assert.Null(syntax);
        }

        [Fact]
        public void TestConversions()
        {
            var syntaxNode = SyntaxFactory.DiscardDesignation();
            var discardDesignationSyntax = (DiscardDesignationSyntaxWrapper)syntaxNode;

            VariableDesignationSyntaxWrapper variableDesignationSyntax = discardDesignationSyntax;
            Assert.Same(syntaxNode, variableDesignationSyntax.SyntaxNode);

            discardDesignationSyntax = (DiscardDesignationSyntaxWrapper)variableDesignationSyntax;
            Assert.Same(syntaxNode, discardDesignationSyntax.SyntaxNode);

            SyntaxNode syntax = discardDesignationSyntax;
            Assert.Same(syntaxNode, syntax);
        }

        [Fact]
        public void TestInvalidConversion()
        {
            var syntaxNode = SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression);
            Assert.Throws<InvalidCastException>(() => (DiscardDesignationSyntaxWrapper)syntaxNode);
        }
    }
}
