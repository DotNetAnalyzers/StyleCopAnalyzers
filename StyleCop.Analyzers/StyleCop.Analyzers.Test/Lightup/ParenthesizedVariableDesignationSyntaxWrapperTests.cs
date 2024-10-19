// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.Lightup
{
    using System;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using StyleCop.Analyzers.Lightup;
    using Xunit;

    public class ParenthesizedVariableDesignationSyntaxWrapperTests
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
            Assert.Throws<NullReferenceException>(() => parenthesizedVariableDesignationSyntax.AddVariables((VariableDesignationSyntaxWrapper)null));
            Assert.Throws<NullReferenceException>(() => parenthesizedVariableDesignationSyntax.WithOpenParenToken(SyntaxFactory.Token(SyntaxKind.OpenParenToken)));
            Assert.Throws<NullReferenceException>(() => parenthesizedVariableDesignationSyntax.WithVariables(SeparatedSyntaxListWrapper<VariableDesignationSyntaxWrapper>.UnsupportedEmpty));
            Assert.Throws<NullReferenceException>(() => parenthesizedVariableDesignationSyntax.WithCloseParenToken(SyntaxFactory.Token(SyntaxKind.CloseParenToken)));
        }

        [Fact]
        public void TestIsInstance()
        {
            Assert.False(ParenthesizedVariableDesignationSyntaxWrapper.IsInstance(null));
            Assert.False(ParenthesizedVariableDesignationSyntaxWrapper.IsInstance(SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression)));
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
        public void TestInvalidConversion()
        {
            var syntaxNode = SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression);
            Assert.Throws<InvalidCastException>(() => (ParenthesizedVariableDesignationSyntaxWrapper)syntaxNode);
        }
    }
}
