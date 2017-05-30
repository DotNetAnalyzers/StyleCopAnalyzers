// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.Lightup
{
    using System;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using StyleCop.Analyzers.Lightup;
    using Xunit;

    public class DeclarationPatternSyntaxWrapperTests
    {
        [Fact]
        public void TestNull()
        {
            var syntaxNode = default(SyntaxNode);
            var declarationPatternSyntax = (DeclarationPatternSyntaxWrapper)syntaxNode;
            Assert.Null(declarationPatternSyntax.SyntaxNode);
            Assert.Throws<NullReferenceException>(() => declarationPatternSyntax.Type);
            Assert.Throws<NullReferenceException>(() => declarationPatternSyntax.Designation);
            Assert.Throws<NullReferenceException>(() => declarationPatternSyntax.WithType(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword))));
            Assert.Throws<NullReferenceException>(() => declarationPatternSyntax.WithDesignation((VariableDesignationSyntaxWrapper)null));
        }

        [Fact]
        public void TestIsInstance()
        {
            Assert.False(DeclarationPatternSyntaxWrapper.IsInstance(null));
            Assert.False(DeclarationPatternSyntaxWrapper.IsInstance(SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression)));
        }

        [Fact]
        public void TestConversionsNull()
        {
            var syntaxNode = default(SyntaxNode);
            var declarationPatternSyntax = (DeclarationPatternSyntaxWrapper)syntaxNode;

            PatternSyntaxWrapper patternSyntax = declarationPatternSyntax;
            Assert.Null(patternSyntax.SyntaxNode);

            declarationPatternSyntax = (DeclarationPatternSyntaxWrapper)patternSyntax;
            Assert.Null(declarationPatternSyntax.SyntaxNode);

            SyntaxNode syntax = declarationPatternSyntax;
            Assert.Null(syntax);
        }

        [Fact]
        public void TestInvalidConversion()
        {
            var syntaxNode = SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression);
            Assert.Throws<InvalidCastException>(() => (DeclarationPatternSyntaxWrapper)syntaxNode);
        }
    }
}
