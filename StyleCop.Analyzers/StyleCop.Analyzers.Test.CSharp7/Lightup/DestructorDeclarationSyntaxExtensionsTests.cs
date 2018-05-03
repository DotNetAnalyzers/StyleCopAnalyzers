// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp7.Lightup
{
    using Microsoft.CodeAnalysis.CSharp;
    using StyleCop.Analyzers.Lightup;
    using Xunit;

    public class DestructorDeclarationSyntaxExtensionsTests
    {
        [Fact]
        public void TestWithExpressionBody()
        {
            var syntax = SyntaxFactory.DestructorDeclaration(SyntaxFactory.Identifier("Anything"));
            var expressionBody = SyntaxFactory.ArrowExpressionClause(SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression));
            var syntaxWithBody = DestructorDeclarationSyntaxExtensions.WithExpressionBody(syntax, expressionBody);
            Assert.Null(syntax.ExpressionBody);
            Assert.NotNull(syntaxWithBody.ExpressionBody);
            Assert.Equal(SyntaxKind.NullLiteralExpression, syntaxWithBody.ExpressionBody.Expression.Kind());
        }
    }
}
