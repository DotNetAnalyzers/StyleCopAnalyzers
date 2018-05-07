// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.Lightup
{
    using System;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using StyleCop.Analyzers.Lightup;
    using Xunit;

    public class DiscardDesignationSyntaxWrapperTests
    {
        [Fact]
        public void TestNull()
        {
            var syntaxNode = default(SyntaxNode);
            var discardDesignationSyntax = (DiscardDesignationSyntaxWrapper)syntaxNode;
            Assert.Null(discardDesignationSyntax.SyntaxNode);
            Assert.Throws<NullReferenceException>(() => discardDesignationSyntax.UnderscoreToken);
            Assert.Throws<NullReferenceException>(() => discardDesignationSyntax.WithUnderscoreToken(SyntaxFactory.Token(SyntaxKindEx.UnderscoreToken)));
        }

        [Fact]
        public void TestIsInstance()
        {
            Assert.False(DiscardDesignationSyntaxWrapper.IsInstance(null));
            Assert.False(DiscardDesignationSyntaxWrapper.IsInstance(SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression)));
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
        public void TestInvalidConversion()
        {
            var syntaxNode = SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression);
            Assert.Throws<InvalidCastException>(() => (DiscardDesignationSyntaxWrapper)syntaxNode);
        }
    }
}
