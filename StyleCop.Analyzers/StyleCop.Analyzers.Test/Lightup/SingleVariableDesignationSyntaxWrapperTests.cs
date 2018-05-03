// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.Lightup
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
        public void TestIsInstance()
        {
            Assert.False(SingleVariableDesignationSyntaxWrapper.IsInstance(null));
            Assert.False(SingleVariableDesignationSyntaxWrapper.IsInstance(SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression)));
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
        public void TestInvalidConversion()
        {
            var syntaxNode = SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression);
            Assert.Throws<InvalidCastException>(() => (SingleVariableDesignationSyntaxWrapper)syntaxNode);
        }
    }
}
