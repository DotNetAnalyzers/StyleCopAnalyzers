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

    public class VariableDesignationSyntaxWrapperTests
    {
        [Fact]
        public void TestNull()
        {
            var syntaxNode = default(SyntaxNode);
            var patternSyntax = (VariableDesignationSyntaxWrapper)syntaxNode;
            Assert.Null(patternSyntax.SyntaxNode);
        }

        [Fact]
        public void TestIsInstance()
        {
            Assert.False(VariableDesignationSyntaxWrapper.IsInstance(null));
            Assert.False(VariableDesignationSyntaxWrapper.IsInstance(SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression)));
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
        public void TestInvalidConversion()
        {
            var syntaxNode = SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression);
            Assert.Throws<InvalidCastException>(() => (VariableDesignationSyntaxWrapper)syntaxNode);
        }
    }
}
