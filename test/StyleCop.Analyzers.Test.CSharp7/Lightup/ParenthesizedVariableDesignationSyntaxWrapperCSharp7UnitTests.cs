// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.CSharp7.Lightup
{
    using System;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using StyleCop.Analyzers.Helpers;
    using StyleCop.Analyzers.Lightup;
    using Xunit;

    public class ParenthesizedVariableDesignationSyntaxWrapperCSharp7UnitTests
    {
        [Fact]
        public void TestNull()
        {
            var syntaxNode = default(SyntaxNode);
            var parenthesizedVariableDesignationSyntax = (ParenthesizedVariableDesignationSyntaxWrapper)syntaxNode;
            Assert.Null(parenthesizedVariableDesignationSyntax.SyntaxNode);
            Assert.Throws<NullReferenceException>(() => parenthesizedVariableDesignationSyntax.OpenParenToken);
            Assert.Throws<NullReferenceException>(() => parenthesizedVariableDesignationSyntax.Variables);
            Assert.Throws<NullReferenceException>(() => parenthesizedVariableDesignationSyntax.CloseParenToken);
            Assert.Throws<NullReferenceException>(() => parenthesizedVariableDesignationSyntax.AddVariables((SingleVariableDesignationSyntaxWrapper)SyntaxFactory.SingleVariableDesignation(SyntaxFactory.Identifier("Anything"))));
            Assert.Throws<NullReferenceException>(() => parenthesizedVariableDesignationSyntax.WithOpenParenToken(SyntaxFactory.Token(SyntaxKind.OpenParenToken)));
            Assert.Throws<NullReferenceException>(() => parenthesizedVariableDesignationSyntax.WithVariables(new SeparatedSyntaxListWrapper<VariableDesignationSyntaxWrapper>.AutoWrapSeparatedSyntaxList<VariableDesignationSyntax>(SyntaxFactory.SeparatedList<VariableDesignationSyntax>())));
            Assert.Throws<NullReferenceException>(() => parenthesizedVariableDesignationSyntax.WithCloseParenToken(SyntaxFactory.Token(SyntaxKind.CloseParenToken)));
        }

        [Fact]
        public void TestProperties()
        {
            var syntaxNode = SyntaxFactory.ParenthesizedVariableDesignation();
            Assert.True(syntaxNode.IsKind(SyntaxKind.ParenthesizedVariableDesignation));
            Assert.True(syntaxNode.IsKind(SyntaxKindEx.ParenthesizedVariableDesignation));

            var parenthesizedVariableDesignationSyntax = (ParenthesizedVariableDesignationSyntaxWrapper)syntaxNode;
            Assert.Same(syntaxNode, parenthesizedVariableDesignationSyntax.SyntaxNode);
            Assert.True(syntaxNode.OpenParenToken.IsEquivalentTo(parenthesizedVariableDesignationSyntax.OpenParenToken));

            var variableSyntaxNode = SyntaxFactory.SingleVariableDesignation(SyntaxFactory.Identifier("Anything"));
            parenthesizedVariableDesignationSyntax = parenthesizedVariableDesignationSyntax.AddVariables((SingleVariableDesignationSyntaxWrapper)variableSyntaxNode);
            Assert.Same(
                ((ParenthesizedVariableDesignationSyntax)parenthesizedVariableDesignationSyntax.SyntaxNode).Variables[0],
                parenthesizedVariableDesignationSyntax.Variables[0].SyntaxNode);

            parenthesizedVariableDesignationSyntax = parenthesizedVariableDesignationSyntax.WithOpenParenToken(SpacingExtensions.WithoutTrivia(SyntaxFactory.Token(SyntaxKind.OpenParenToken)));
            Assert.NotNull(parenthesizedVariableDesignationSyntax.SyntaxNode);
            Assert.NotSame(syntaxNode, parenthesizedVariableDesignationSyntax.SyntaxNode);
            Assert.False(syntaxNode.OpenParenToken.IsEquivalentTo(parenthesizedVariableDesignationSyntax.OpenParenToken));
            Assert.True(syntaxNode.CloseParenToken.IsEquivalentTo(parenthesizedVariableDesignationSyntax.CloseParenToken));

            var variables = SyntaxFactory.SingletonSeparatedList<VariableDesignationSyntax>(SyntaxFactory.DiscardDesignation());
            parenthesizedVariableDesignationSyntax = parenthesizedVariableDesignationSyntax.WithVariables(new SeparatedSyntaxListWrapper<VariableDesignationSyntaxWrapper>.AutoWrapSeparatedSyntaxList<VariableDesignationSyntax>(variables));
            Assert.Same(
                ((ParenthesizedVariableDesignationSyntax)parenthesizedVariableDesignationSyntax.SyntaxNode).Variables[0],
                parenthesizedVariableDesignationSyntax.Variables[0].SyntaxNode);

            parenthesizedVariableDesignationSyntax = parenthesizedVariableDesignationSyntax.WithCloseParenToken(SpacingExtensions.WithoutTrivia(SyntaxFactory.Token(SyntaxKind.CloseParenToken)));
            Assert.NotNull(parenthesizedVariableDesignationSyntax.SyntaxNode);
            Assert.False(syntaxNode.OpenParenToken.IsEquivalentTo(parenthesizedVariableDesignationSyntax.OpenParenToken));
            Assert.False(syntaxNode.CloseParenToken.IsEquivalentTo(parenthesizedVariableDesignationSyntax.CloseParenToken));
        }

        [Fact]
        public void TestIsInstance()
        {
            Assert.False(ParenthesizedVariableDesignationSyntaxWrapper.IsInstance(null));
            Assert.False(ParenthesizedVariableDesignationSyntaxWrapper.IsInstance(SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression)));

            var syntaxNode = SyntaxFactory.ParenthesizedVariableDesignation();
            Assert.True(ParenthesizedVariableDesignationSyntaxWrapper.IsInstance(syntaxNode));
        }

        [Fact]
        public void TestConversionsNull()
        {
            var syntaxNode = default(SyntaxNode);
            var parenthesizedVariableDesignationSyntax = (ParenthesizedVariableDesignationSyntaxWrapper)syntaxNode;

            VariableDesignationSyntaxWrapper variableDesignationSyntax = parenthesizedVariableDesignationSyntax;
            Assert.Null(variableDesignationSyntax.SyntaxNode);

            parenthesizedVariableDesignationSyntax = (ParenthesizedVariableDesignationSyntaxWrapper)variableDesignationSyntax;
            Assert.Null(parenthesizedVariableDesignationSyntax.SyntaxNode);

            SyntaxNode syntax = parenthesizedVariableDesignationSyntax;
            Assert.Null(syntax);
        }

        [Fact]
        public void TestConversions()
        {
            var syntaxNode = SyntaxFactory.ParenthesizedVariableDesignation();
            var parenthesizedVariableDesignationSyntax = (ParenthesizedVariableDesignationSyntaxWrapper)syntaxNode;

            VariableDesignationSyntaxWrapper variableDesignationSyntax = parenthesizedVariableDesignationSyntax;
            Assert.Same(syntaxNode, variableDesignationSyntax.SyntaxNode);

            parenthesizedVariableDesignationSyntax = (ParenthesizedVariableDesignationSyntaxWrapper)variableDesignationSyntax;
            Assert.Same(syntaxNode, parenthesizedVariableDesignationSyntax.SyntaxNode);

            SyntaxNode syntax = parenthesizedVariableDesignationSyntax;
            Assert.Same(syntaxNode, syntax);
        }

        [Fact]
        public void TestInvalidConversion()
        {
            var syntaxNode = SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression);
            Assert.Throws<InvalidCastException>(() => (ParenthesizedVariableDesignationSyntaxWrapper)syntaxNode);
        }
    }
}
