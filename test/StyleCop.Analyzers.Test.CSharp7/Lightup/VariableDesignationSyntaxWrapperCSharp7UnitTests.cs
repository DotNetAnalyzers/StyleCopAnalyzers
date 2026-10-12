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

    public class VariableDesignationSyntaxWrapperCSharp7UnitTests
    {
        [Fact]
        public void TestNull()
        {
            var syntaxNode = default(SyntaxNode);
            var patternSyntax = (VariableDesignationSyntaxWrapper)syntaxNode;
            Assert.Null(patternSyntax.SyntaxNode);
        }

        [Fact]
        public void TestNonNull()
        {
            var syntaxNode = SyntaxFactory.DiscardDesignation();
            var patternSyntax = (VariableDesignationSyntaxWrapper)syntaxNode;
            Assert.Same(syntaxNode, patternSyntax.SyntaxNode);
        }

        [Fact]
        public void TestIsInstance()
        {
            Assert.False(VariableDesignationSyntaxWrapper.IsInstance(null));
            Assert.False(VariableDesignationSyntaxWrapper.IsInstance(SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression)));

            var discardDesignationSyntax = SyntaxFactory.DiscardDesignation();
            Assert.True(VariableDesignationSyntaxWrapper.IsInstance(discardDesignationSyntax));
        }

        [Fact]
        public void TestConversionsNull()
        {
            var syntaxNode = default(SyntaxNode);
            var patternSyntax = (VariableDesignationSyntaxWrapper)syntaxNode;

            SyntaxNode syntax = patternSyntax;
            Assert.Null(syntax);
        }

        [Fact]
        public void TestConversions()
        {
            var syntaxNode = SyntaxFactory.DiscardDesignation();
            var patternSyntax = (VariableDesignationSyntaxWrapper)syntaxNode;

            SyntaxNode syntax = patternSyntax;
            Assert.Same(syntaxNode, syntax);
        }

        [Fact]
        public void TestInvalidConversion()
        {
            var syntaxNode = SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression);
            Assert.Throws<InvalidCastException>(() => (VariableDesignationSyntaxWrapper)syntaxNode);
        }
    }
}
