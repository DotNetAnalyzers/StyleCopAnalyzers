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

    public class LocalFunctionStatementSyntaxWrapperTests
    {
        [Fact]
        public void TestNull()
        {
            var syntaxNode = default(SyntaxNode);
            var wrapper = (LocalFunctionStatementSyntaxWrapper)syntaxNode;
            Assert.Null(wrapper.SyntaxNode);
            Assert.Throws<NullReferenceException>(() => wrapper.Modifiers);
            Assert.Throws<NullReferenceException>(() => wrapper.ReturnType);
            Assert.Throws<NullReferenceException>(() => wrapper.Identifier);
            Assert.Throws<NullReferenceException>(() => wrapper.TypeParameterList);
            Assert.Throws<NullReferenceException>(() => wrapper.ParameterList);
            Assert.Throws<NullReferenceException>(() => wrapper.ConstraintClauses);
            Assert.Throws<NullReferenceException>(() => wrapper.Body);
            Assert.Throws<NullReferenceException>(() => wrapper.ExpressionBody);
            Assert.Throws<NullReferenceException>(() => wrapper.SemicolonToken);
            Assert.Throws<NullReferenceException>(() => wrapper.WithModifiers(SyntaxFactory.TokenList()));
            Assert.Throws<NullReferenceException>(() => wrapper.WithReturnType(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword))));
            Assert.Throws<NullReferenceException>(() => wrapper.WithIdentifier(SyntaxFactory.Identifier("Identifier")));
            Assert.Throws<NullReferenceException>(() => wrapper.WithTypeParameterList(SyntaxFactory.TypeParameterList()));
            Assert.Throws<NullReferenceException>(() => wrapper.WithParameterList(SyntaxFactory.ParameterList()));
            Assert.Throws<NullReferenceException>(() => wrapper.WithConstraintClauses(SyntaxFactory.List<TypeParameterConstraintClauseSyntax>()));
            Assert.Throws<NullReferenceException>(() => wrapper.WithBody(SyntaxFactory.Block()));
            Assert.Throws<NullReferenceException>(() => wrapper.WithExpressionBody(SyntaxFactory.ArrowExpressionClause(SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(0)))));
            Assert.Throws<NullReferenceException>(() => wrapper.WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)));
            Assert.Throws<NullReferenceException>(() => wrapper.AddModifiers());
            Assert.Throws<NullReferenceException>(() => wrapper.AddTypeParameterListParameters());
            Assert.Throws<NullReferenceException>(() => wrapper.AddParameterListParameters());
            Assert.Throws<NullReferenceException>(() => wrapper.AddConstraintClauses());
            Assert.Throws<NullReferenceException>(() => wrapper.AddBodyStatements());
        }

        [Fact]
        public void TestIsInstance()
        {
            Assert.False(LocalFunctionStatementSyntaxWrapper.IsInstance(null));
            Assert.False(LocalFunctionStatementSyntaxWrapper.IsInstance(SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression)));
        }

        [Fact]
        public void TestConversionsNull()
        {
            var syntaxNode = default(SyntaxNode);
            var wrapper = (LocalFunctionStatementSyntaxWrapper)syntaxNode;

            StatementSyntax syntax = wrapper;
            Assert.Null(syntax);
        }

        [Fact]
        public void TestInvalidConversion()
        {
            var syntaxNode = SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression);
            Assert.Throws<InvalidCastException>(() => (LocalFunctionStatementSyntaxWrapper)syntaxNode);
        }
    }
}
