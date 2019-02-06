// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.Lightup
{
    using System;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using StyleCop.Analyzers.Lightup;
    using Xunit;

    public class PatternSyntaxWrapperTests
    {
        [Fact]
        public void TestNull()
        {
            var syntaxNode = default(SyntaxNode);
            var patternSyntax = (PatternSyntaxWrapper)syntaxNode;
            Assert.Null(patternSyntax.SyntaxNode);
        }

        [Fact]
        public void TestIsInstance()
        {
            Assert.False(PatternSyntaxWrapper.IsInstance(null));
            Assert.False(PatternSyntaxWrapper.IsInstance(SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression)));
        }

        [Fact]
        public void TestConversionsNull()
        {
            var syntaxNode = default(SyntaxNode);
            var patternSyntax = (PatternSyntaxWrapper)syntaxNode;

            SyntaxNode syntax = patternSyntax;
            Assert.Null(syntax);
        }

        [Fact]
        public void TestInvalidConversion()
        {
            var syntaxNode = SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression);
            Assert.Throws<InvalidCastException>(() => (PatternSyntaxWrapper)syntaxNode);
        }
    }
}
