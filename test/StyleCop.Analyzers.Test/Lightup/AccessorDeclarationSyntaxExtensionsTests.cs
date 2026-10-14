// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.Lightup
{
    using System;
    using Microsoft.CodeAnalysis.CSharp;
    using StyleCop.Analyzers.Lightup;
    using Xunit;

    public class AccessorDeclarationSyntaxExtensionsTests
    {
        [Fact]
        public void TestExpressionBody()
        {
            var accessorDeclarationSyntax = SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration);
            Assert.Null(AccessorDeclarationSyntaxExtensions.ExpressionBody(accessorDeclarationSyntax));
        }

        [Fact]
        public void TestWithExpressionBody()
        {
            var accessorDeclarationSyntax = SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration);

            // With default value is allowed
            var accessorWithDefaultBody = AccessorDeclarationSyntaxExtensions.WithExpressionBody(accessorDeclarationSyntax, null);
            Assert.Null(AccessorDeclarationSyntaxExtensions.ExpressionBody(accessorWithDefaultBody));

            // Non-default throws an exception
            var expressionBody = SyntaxFactory.ArrowExpressionClause(SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression));
            Assert.Throws<NotSupportedException>(() => AccessorDeclarationSyntaxExtensions.WithExpressionBody(accessorDeclarationSyntax, expressionBody));
        }
    }
}
