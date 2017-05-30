// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.Lightup
{
    using Microsoft.CodeAnalysis.CSharp;
    using StyleCop.Analyzers.Lightup;
    using Xunit;

    public class BaseMethodDeclarationSyntaxExtensionsTests
    {
        [Fact]
        public void TestExpressionBody_Constructor()
        {
            var syntax = SyntaxFactory.ConstructorDeclaration(SyntaxFactory.Identifier("Anything"));
            Assert.Null(BaseMethodDeclarationSyntaxExtensions.ExpressionBody(syntax));
        }

        [Fact]
        public void TestExpressionBody_Destructor()
        {
            var syntax = SyntaxFactory.DestructorDeclaration(SyntaxFactory.Identifier("Anything"));
            Assert.Null(BaseMethodDeclarationSyntaxExtensions.ExpressionBody(syntax));
        }

        [Fact]
        public void TestExpressionBody_Operator()
        {
            var syntax = SyntaxFactory.OperatorDeclaration(
                SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword)),
                SyntaxFactory.Token(SyntaxKind.PlusToken));
            Assert.Null(BaseMethodDeclarationSyntaxExtensions.ExpressionBody(syntax));

            var expressionBody = SyntaxFactory.ArrowExpressionClause(SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression));
            syntax = syntax.WithExpressionBody(expressionBody);
            Assert.NotNull(BaseMethodDeclarationSyntaxExtensions.ExpressionBody(syntax));
            Assert.Equal(SyntaxKind.NullLiteralExpression, BaseMethodDeclarationSyntaxExtensions.ExpressionBody(syntax).Expression.Kind());
        }

        [Fact]
        public void TestExpressionBody_ConversionOperator()
        {
            var syntax = SyntaxFactory.ConversionOperatorDeclaration(
                SyntaxFactory.Token(SyntaxKind.ExplicitKeyword),
                SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword)));
            Assert.Null(BaseMethodDeclarationSyntaxExtensions.ExpressionBody(syntax));

            var expressionBody = SyntaxFactory.ArrowExpressionClause(SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression));
            syntax = syntax.WithExpressionBody(expressionBody);
            Assert.NotNull(BaseMethodDeclarationSyntaxExtensions.ExpressionBody(syntax));
            Assert.Equal(SyntaxKind.NullLiteralExpression, BaseMethodDeclarationSyntaxExtensions.ExpressionBody(syntax).Expression.Kind());
        }

        [Fact]
        public void TestExpressionBody_Method()
        {
            var syntax = SyntaxFactory.MethodDeclaration(
                SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword)),
                SyntaxFactory.Identifier("Anything"));
            Assert.Null(BaseMethodDeclarationSyntaxExtensions.ExpressionBody(syntax));

            var expressionBody = SyntaxFactory.ArrowExpressionClause(SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression));
            syntax = syntax.WithExpressionBody(expressionBody);
            Assert.NotNull(BaseMethodDeclarationSyntaxExtensions.ExpressionBody(syntax));
            Assert.Equal(SyntaxKind.NullLiteralExpression, BaseMethodDeclarationSyntaxExtensions.ExpressionBody(syntax).Expression.Kind());
        }
    }
}
