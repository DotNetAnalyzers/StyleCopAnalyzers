// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.Lightup
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using StyleCop.Analyzers.Lightup;
    using Xunit;

    public class SyntaxWrapperTests
    {
        [Fact]
        public void TestWrapSyntaxNode()
        {
            SyntaxNode syntaxNode = SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression);

            Assert.Same(syntaxNode, SyntaxWrapper<LiteralExpressionSyntax>.Default.Wrap(syntaxNode));
            Assert.Same(syntaxNode, SyntaxWrapper<LiteralExpressionSyntax>.Default.Unwrap((LiteralExpressionSyntax)syntaxNode));
        }

        [Fact]
        public void TestWrapNullSyntaxNode()
        {
            Assert.Null(SyntaxWrapper<LiteralExpressionSyntax>.Default.Wrap(null));
            Assert.Null(SyntaxWrapper<LiteralExpressionSyntax>.Default.Unwrap(null));
        }

        [Fact]
        public void TestHandwrittenNullConversions()
        {
            BaseNamespaceDeclarationSyntaxWrapper namespaceWrapper = (NamespaceDeclarationSyntax)null;
            BaseObjectCreationExpressionSyntaxWrapper creationWrapper = (ObjectCreationExpressionSyntax)null;
            CommonForEachStatementSyntaxWrapper loopWrapper = (ForEachStatementSyntax)null;

            Assert.Null(namespaceWrapper.SyntaxNode);
            Assert.Null(creationWrapper.SyntaxNode);
            Assert.Null(loopWrapper.SyntaxNode);
        }
    }
}
