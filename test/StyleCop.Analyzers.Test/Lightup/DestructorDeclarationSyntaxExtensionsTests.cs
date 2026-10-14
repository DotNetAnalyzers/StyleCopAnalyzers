// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.Lightup
{
    using System;
    using Microsoft.CodeAnalysis.CSharp;
    using StyleCop.Analyzers.Lightup;
    using Xunit;

    public class DestructorDeclarationSyntaxExtensionsTests
    {
        [Fact]
        public void TestWithExpressionBody()
        {
            var syntax = SyntaxFactory.DestructorDeclaration(SyntaxFactory.Identifier("Anything"));

            // With default value is allowed
            var syntaxWithDefaultBody = DestructorDeclarationSyntaxExtensions.WithExpressionBody(syntax, null);
            Assert.Null(BaseMethodDeclarationSyntaxExtensions.ExpressionBody(syntaxWithDefaultBody));

            // Non-default throws an exception
            var expressionBody = SyntaxFactory.ArrowExpressionClause(SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression));
            Assert.Throws<NotSupportedException>(() => DestructorDeclarationSyntaxExtensions.WithExpressionBody(syntax, expressionBody));
        }
    }
}
