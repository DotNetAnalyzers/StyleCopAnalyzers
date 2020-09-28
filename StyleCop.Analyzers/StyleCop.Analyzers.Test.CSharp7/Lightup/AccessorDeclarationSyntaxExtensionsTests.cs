// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp7.Lightup
{
    using Microsoft.CodeAnalysis.CSharp;
    using StyleCop.Analyzers.Lightup;
    using Xunit;

    public class AccessorDeclarationSyntaxExtensionsTests
    {
        [Fact]
        public void TestExpressionBody()
        {
            var accessorDeclarationSyntax = SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                .WithExpressionBody(SyntaxFactory.ArrowExpressionClause(SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression)));
            Assert.Same(accessorDeclarationSyntax.ExpressionBody, AccessorDeclarationSyntaxExtensions.ExpressionBody(accessorDeclarationSyntax));
        }

        [Fact]
        public void TestWithExpressionBody()
        {
            var accessorDeclarationSyntax = SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration);
            var expressionBody = SyntaxFactory.ArrowExpressionClause(SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression));
            var accessorWithBody = AccessorDeclarationSyntaxExtensions.WithExpressionBody(accessorDeclarationSyntax, expressionBody);
            Assert.Null(accessorDeclarationSyntax.ExpressionBody);
            Assert.NotNull(accessorWithBody.ExpressionBody);
            Assert.Equal(SyntaxKind.NullLiteralExpression, accessorWithBody.ExpressionBody.Expression.Kind());
        }
    }
}
