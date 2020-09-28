// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp7.Lightup
{
    using System;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using StyleCop.Analyzers.Lightup;
    using Xunit;

    public class DeclarationExpressionSyntaxWrapperTests
    {
        [Fact]
        public void TestNull()
        {
            var syntaxNode = default(SyntaxNode);
            var declarationExpressionSyntax = (DeclarationExpressionSyntaxWrapper)syntaxNode;
            Assert.Null(declarationExpressionSyntax.SyntaxNode);
            Assert.Throws<NullReferenceException>(() => declarationExpressionSyntax.Type);
            Assert.Throws<NullReferenceException>(() => declarationExpressionSyntax.Designation);
            Assert.Throws<NullReferenceException>(() => declarationExpressionSyntax.WithType(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword))));
            Assert.Throws<NullReferenceException>(() => declarationExpressionSyntax.WithDesignation((VariableDesignationSyntaxWrapper)SyntaxFactory.DiscardDesignation()));
        }

        [Fact]
        public void TestProperties()
        {
            var syntaxNode = SyntaxFactory.DeclarationExpression(
                SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword)),
                SyntaxFactory.DiscardDesignation());
            Assert.True(syntaxNode.IsKind(SyntaxKind.DeclarationExpression));
            Assert.True(syntaxNode.IsKind(SyntaxKindEx.DeclarationExpression));

            var declarationExpressionSyntax = (DeclarationExpressionSyntaxWrapper)syntaxNode;
            Assert.Same(syntaxNode, declarationExpressionSyntax.SyntaxNode);
            Assert.Same(syntaxNode.Type, declarationExpressionSyntax.Type);
            Assert.Same(syntaxNode.Designation, declarationExpressionSyntax.Designation.SyntaxNode);

            declarationExpressionSyntax = declarationExpressionSyntax.WithType(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword)));
            Assert.NotNull(declarationExpressionSyntax.SyntaxNode);
            Assert.NotSame(syntaxNode, declarationExpressionSyntax.SyntaxNode);
            Assert.Same(((DeclarationExpressionSyntax)declarationExpressionSyntax.SyntaxNode).Type, declarationExpressionSyntax.Type);

            declarationExpressionSyntax = declarationExpressionSyntax.WithDesignation((VariableDesignationSyntaxWrapper)SyntaxFactory.SingleVariableDesignation(SyntaxFactory.Identifier("Anything")));
            Assert.NotNull(declarationExpressionSyntax.SyntaxNode);
            Assert.NotSame(syntaxNode, declarationExpressionSyntax.SyntaxNode);
            Assert.Same(((DeclarationExpressionSyntax)declarationExpressionSyntax.SyntaxNode).Designation, declarationExpressionSyntax.Designation.SyntaxNode);
        }

        [Fact]
        public void TestIsInstance()
        {
            Assert.False(DeclarationExpressionSyntaxWrapper.IsInstance(null));
            Assert.False(DeclarationExpressionSyntaxWrapper.IsInstance(SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression)));

            var syntaxNode = SyntaxFactory.DeclarationExpression(
                SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword)),
                SyntaxFactory.DiscardDesignation());
            Assert.True(DeclarationExpressionSyntaxWrapper.IsInstance(syntaxNode));
        }

        [Fact]
        public void TestConversionsNull()
        {
            var syntaxNode = default(SyntaxNode);
            var declarationExpressionSyntax = (DeclarationExpressionSyntaxWrapper)syntaxNode;

            ExpressionSyntax expressionSyntax = declarationExpressionSyntax;
            Assert.Null(expressionSyntax);

            declarationExpressionSyntax = (DeclarationExpressionSyntaxWrapper)expressionSyntax;
            Assert.Null(declarationExpressionSyntax.SyntaxNode);

            SyntaxNode syntax = declarationExpressionSyntax;
            Assert.Null(syntax);
        }

        [Fact]
        public void TestConversions()
        {
            var syntaxNode = SyntaxFactory.DeclarationExpression(
                SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword)),
                SyntaxFactory.DiscardDesignation());
            var declarationExpressionSyntax = (DeclarationExpressionSyntaxWrapper)syntaxNode;

            ExpressionSyntax expressionSyntax = declarationExpressionSyntax;
            Assert.Same(syntaxNode, expressionSyntax);

            declarationExpressionSyntax = (DeclarationExpressionSyntaxWrapper)expressionSyntax;
            Assert.Same(syntaxNode, declarationExpressionSyntax.SyntaxNode);

            SyntaxNode syntax = declarationExpressionSyntax;
            Assert.Same(syntaxNode, syntax);
        }

        [Fact]
        public void TestInvalidConversion()
        {
            var syntaxNode = SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression);
            Assert.Throws<InvalidCastException>(() => (DeclarationExpressionSyntaxWrapper)syntaxNode);
        }
    }
}
