// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp7.Lightup
{
    using System;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using StyleCop.Analyzers.Helpers;
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
            Assert.Throws<NullReferenceException>(() => declarationPatternSyntax.WithDesignation((VariableDesignationSyntaxWrapper)SyntaxFactory.DiscardDesignation()));
        }

        [Fact]
        public void TestProperties()
        {
            var syntaxNode = SyntaxFactory.DeclarationPattern(
                SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword)),
                SyntaxFactory.DiscardDesignation());
            Assert.True(syntaxNode.IsKind(SyntaxKind.DeclarationPattern));
            Assert.True(syntaxNode.IsKind(SyntaxKindEx.DeclarationPattern));

            var declarationPatternSyntax = (DeclarationPatternSyntaxWrapper)syntaxNode;
            Assert.Same(syntaxNode, declarationPatternSyntax.SyntaxNode);
            Assert.Same(syntaxNode.Type, declarationPatternSyntax.Type);
            Assert.Same(syntaxNode.Designation, declarationPatternSyntax.Designation.SyntaxNode);

            declarationPatternSyntax = declarationPatternSyntax.WithType(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword)));
            Assert.NotNull(declarationPatternSyntax.SyntaxNode);
            Assert.NotSame(syntaxNode, declarationPatternSyntax.SyntaxNode);
            Assert.Same(((DeclarationPatternSyntax)declarationPatternSyntax.SyntaxNode).Type, declarationPatternSyntax.Type);

            declarationPatternSyntax = declarationPatternSyntax.WithDesignation((VariableDesignationSyntaxWrapper)SyntaxFactory.SingleVariableDesignation(SyntaxFactory.Identifier("Anything")));
            Assert.NotNull(declarationPatternSyntax.SyntaxNode);
            Assert.NotSame(syntaxNode, declarationPatternSyntax.SyntaxNode);
            Assert.Same(((DeclarationPatternSyntax)declarationPatternSyntax.SyntaxNode).Designation, declarationPatternSyntax.Designation.SyntaxNode);
        }

        [Fact]
        public void TestIsInstance()
        {
            Assert.False(DeclarationPatternSyntaxWrapper.IsInstance(null));
            Assert.False(DeclarationPatternSyntaxWrapper.IsInstance(SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression)));

            var syntaxNode = SyntaxFactory.DeclarationPattern(
                SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword)),
                SyntaxFactory.DiscardDesignation());
            Assert.True(DeclarationPatternSyntaxWrapper.IsInstance(syntaxNode));
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
        public void TestConversions()
        {
            var syntaxNode = SyntaxFactory.DeclarationPattern(
                SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword)),
                SyntaxFactory.DiscardDesignation());
            var declarationPatternSyntax = (DeclarationPatternSyntaxWrapper)syntaxNode;

            PatternSyntaxWrapper patternSyntax = declarationPatternSyntax;
            Assert.Same(syntaxNode, patternSyntax.SyntaxNode);

            declarationPatternSyntax = (DeclarationPatternSyntaxWrapper)patternSyntax;
            Assert.Same(syntaxNode, declarationPatternSyntax.SyntaxNode);

            SyntaxNode syntax = declarationPatternSyntax;
            Assert.Same(syntaxNode, syntax);
        }

        [Fact]
        public void TestInvalidConversion()
        {
            var syntaxNode = SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression);
            Assert.Throws<InvalidCastException>(() => (DeclarationPatternSyntaxWrapper)syntaxNode);
        }
    }
}
