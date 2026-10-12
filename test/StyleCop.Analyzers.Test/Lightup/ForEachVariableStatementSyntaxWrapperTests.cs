// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.Lightup
{
    using System;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using StyleCop.Analyzers.Lightup;
    using Xunit;

    public class ForEachVariableStatementSyntaxWrapperTests
    {
        [Fact]
        public void TestNull()
        {
            var syntaxNode = default(SyntaxNode);
            var wrapper = (ForEachVariableStatementSyntaxWrapper)syntaxNode;
            Assert.Null(wrapper.SyntaxNode);
            Assert.Throws<NullReferenceException>(() => wrapper.AttributeLists);
            Assert.Throws<NullReferenceException>(() => wrapper.AwaitKeyword);
            Assert.Throws<NullReferenceException>(() => wrapper.ForEachKeyword);
            Assert.Throws<NullReferenceException>(() => wrapper.OpenParenToken);
            Assert.Throws<NullReferenceException>(() => wrapper.Variable);
            Assert.Throws<NullReferenceException>(() => wrapper.InKeyword);
            Assert.Throws<NullReferenceException>(() => wrapper.Expression);
            Assert.Throws<NullReferenceException>(() => wrapper.CloseParenToken);
            Assert.Throws<NullReferenceException>(() => wrapper.Statement);
            Assert.Throws<NullReferenceException>(() => wrapper.WithAttributeLists(default(SyntaxList<AttributeListSyntax>)));
            Assert.Throws<NullReferenceException>(() => wrapper.WithAwaitKeyword(SyntaxFactory.Token(SyntaxKind.AwaitKeyword)));
            Assert.Throws<NullReferenceException>(() => wrapper.WithForEachKeyword(SyntaxFactory.Token(SyntaxKind.ForEachKeyword)));
            Assert.Throws<NullReferenceException>(() => wrapper.WithOpenParenToken(SyntaxFactory.Token(SyntaxKind.OpenParenToken)));
            Assert.Throws<NullReferenceException>(() => wrapper.WithVariable(SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression)));
            Assert.Throws<NullReferenceException>(() => wrapper.WithInKeyword(SyntaxFactory.Token(SyntaxKind.InKeyword)));
            Assert.Throws<NullReferenceException>(() => wrapper.WithExpression(SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression)));
            Assert.Throws<NullReferenceException>(() => wrapper.WithCloseParenToken(SyntaxFactory.Token(SyntaxKind.CloseParenToken)));
            Assert.Throws<NullReferenceException>(() => wrapper.WithStatement(SyntaxFactory.EmptyStatement()));
        }

        [Fact]
        public void TestIsInstance()
        {
            Assert.False(ForEachVariableStatementSyntaxWrapper.IsInstance(null));
            Assert.False(ForEachVariableStatementSyntaxWrapper.IsInstance(SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression)));
        }

        [Fact]
        public void TestConversionsNull()
        {
            var syntaxNode = default(SyntaxNode);
            var wrapper = (ForEachVariableStatementSyntaxWrapper)syntaxNode;

            StatementSyntax syntax = wrapper;
            Assert.Null(syntax);

            wrapper = (ForEachVariableStatementSyntaxWrapper)(CommonForEachStatementSyntaxWrapper)syntaxNode;

            syntax = wrapper;
            Assert.Null(syntax);
        }

        [Fact]
        public void TestInvalidConversion()
        {
            var syntaxNode = SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression);
            Assert.Throws<InvalidCastException>(() => (ForEachVariableStatementSyntaxWrapper)syntaxNode);
        }
    }
}
