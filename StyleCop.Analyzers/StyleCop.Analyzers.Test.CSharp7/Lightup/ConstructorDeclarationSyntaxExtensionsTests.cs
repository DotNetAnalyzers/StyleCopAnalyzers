// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.CSharp7.Lightup
{
    using Microsoft.CodeAnalysis.CSharp;
    using StyleCop.Analyzers.Lightup;
    using Xunit;

    public class ConstructorDeclarationSyntaxExtensionsTests
    {
        [Fact]
        public void TestWithExpressionBody()
        {
            var syntax = SyntaxFactory.ConstructorDeclaration(SyntaxFactory.Identifier("Anything"));
            var expressionBody = SyntaxFactory.ArrowExpressionClause(SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression));
            var syntaxWithBody = ConstructorDeclarationSyntaxExtensions.WithExpressionBody(syntax, expressionBody);
            Assert.Null(syntax.ExpressionBody);
            Assert.NotNull(syntaxWithBody.ExpressionBody);
            Assert.Equal(SyntaxKind.NullLiteralExpression, syntaxWithBody.ExpressionBody.Expression.Kind());
        }
    }
}
