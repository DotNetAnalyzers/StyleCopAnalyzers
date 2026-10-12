// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.Lightup
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
            Assert.Throws<NullReferenceException>(() => declarationExpressionSyntax.WithDesignation((VariableDesignationSyntaxWrapper)null));
        }

        [Fact]
        public void TestIsInstance()
        {
            Assert.False(DeclarationExpressionSyntaxWrapper.IsInstance(null));
            Assert.False(DeclarationExpressionSyntaxWrapper.IsInstance(SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression)));
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
        public void TestInvalidConversion()
        {
            var syntaxNode = SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression);
            Assert.Throws<InvalidCastException>(() => (DeclarationExpressionSyntaxWrapper)syntaxNode);
        }
    }
}
