// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.CSharp7.Lightup
{
    using System;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using StyleCop.Analyzers.Lightup;
    using Xunit;

    public class SingleVariableDesignationSyntaxWrapperTests
    {
        [Fact]
        public void TestNull()
        {
            var syntaxNode = default(SyntaxNode);
            var singleVariableDesignationSyntax = (SingleVariableDesignationSyntaxWrapper)syntaxNode;
            Assert.Null(singleVariableDesignationSyntax.SyntaxNode);
            Assert.Throws<NullReferenceException>(() => singleVariableDesignationSyntax.Identifier);
            Assert.Throws<NullReferenceException>(() => singleVariableDesignationSyntax.WithIdentifier(SyntaxFactory.Identifier("Anything")));
        }

        [Fact]
        public void TestIdentifier()
        {
            var syntaxNode = SyntaxFactory.SingleVariableDesignation(SyntaxFactory.Identifier("Anything"));
            Assert.True(syntaxNode.IsKind(SyntaxKind.SingleVariableDesignation));
            Assert.True(syntaxNode.IsKind(SyntaxKindEx.SingleVariableDesignation));

            var singleVariableDesignationSyntax = (SingleVariableDesignationSyntaxWrapper)syntaxNode;
            Assert.Same(syntaxNode, singleVariableDesignationSyntax.SyntaxNode);
            Assert.Equal(syntaxNode.Identifier, singleVariableDesignationSyntax.Identifier);

            singleVariableDesignationSyntax = singleVariableDesignationSyntax.WithIdentifier(SyntaxFactory.Identifier("AnythingElse"));
            Assert.NotNull(singleVariableDesignationSyntax.SyntaxNode);
            Assert.NotSame(syntaxNode, singleVariableDesignationSyntax.SyntaxNode);
            Assert.False(syntaxNode.Identifier.IsEquivalentTo(singleVariableDesignationSyntax.Identifier));
            Assert.Equal("AnythingElse", singleVariableDesignationSyntax.Identifier.ValueText);
        }

        [Fact]
        public void TestIsInstance()
        {
            Assert.False(SingleVariableDesignationSyntaxWrapper.IsInstance(null));
            Assert.False(SingleVariableDesignationSyntaxWrapper.IsInstance(SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression)));

            var syntaxNode = SyntaxFactory.SingleVariableDesignation(SyntaxFactory.Identifier("Anything"));
            Assert.True(SingleVariableDesignationSyntaxWrapper.IsInstance(syntaxNode));
        }

        [Fact]
        public void TestConversionsNull()
        {
            var syntaxNode = default(SyntaxNode);
            var singleVariableDesignationSyntax = (SingleVariableDesignationSyntaxWrapper)syntaxNode;

            VariableDesignationSyntaxWrapper variableDesignationSyntax = singleVariableDesignationSyntax;
            Assert.Null(variableDesignationSyntax.SyntaxNode);

            singleVariableDesignationSyntax = (SingleVariableDesignationSyntaxWrapper)variableDesignationSyntax;
            Assert.Null(singleVariableDesignationSyntax.SyntaxNode);

            SyntaxNode syntax = singleVariableDesignationSyntax;
            Assert.Null(syntax);
        }

        [Fact]
        public void TestConversions()
        {
            var syntaxNode = SyntaxFactory.SingleVariableDesignation(SyntaxFactory.Identifier("Anything"));
            var singleVariableDesignationSyntax = (SingleVariableDesignationSyntaxWrapper)syntaxNode;

            VariableDesignationSyntaxWrapper variableDesignationSyntax = singleVariableDesignationSyntax;
            Assert.Same(syntaxNode, variableDesignationSyntax.SyntaxNode);

            singleVariableDesignationSyntax = (SingleVariableDesignationSyntaxWrapper)variableDesignationSyntax;
            Assert.Same(syntaxNode, singleVariableDesignationSyntax.SyntaxNode);

            SyntaxNode syntax = singleVariableDesignationSyntax;
            Assert.Same(syntaxNode, syntax);
        }

        [Fact]
        public void TestInvalidConversion()
        {
            var syntaxNode = SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression);
            Assert.Throws<InvalidCastException>(() => (SingleVariableDesignationSyntaxWrapper)syntaxNode);
        }
    }
}
