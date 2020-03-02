// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp7.Lightup
{
    using Microsoft.CodeAnalysis.CSharp;
    using StyleCop.Analyzers.Lightup;
    using Xunit;

    public class BaseMethodDeclarationSyntaxExtensionsTests
    {
        [Fact]
        public void TestExpressionBody()
        {
            var syntax = SyntaxFactory.ConstructorDeclaration(SyntaxFactory.Identifier("Anything"))
                .WithExpressionBody(SyntaxFactory.ArrowExpressionClause(SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression)));
            Assert.Same(syntax.ExpressionBody, BaseMethodDeclarationSyntaxExtensions.ExpressionBody(syntax));
        }
    }
}
