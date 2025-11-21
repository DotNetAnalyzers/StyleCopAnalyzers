// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp8.Lightup
{
    using System;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using StyleCop.Analyzers.Helpers;
    using StyleCop.Analyzers.Lightup;
    using Xunit;

    public class ForEachVariableStatementSyntaxWrapperCSharp8UnitTests
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
        public void TestWrapperIdentity()
        {
            var syntaxNode = this.CreateForEachVariableStatement();
            Assert.True(syntaxNode.IsKind(SyntaxKind.ForEachVariableStatement));

            var wrapper = (ForEachVariableStatementSyntaxWrapper)syntaxNode;
            Assert.Same(syntaxNode, wrapper.SyntaxNode);
        }

        [Fact]
        public void TestAttributeList()
        {
            var syntaxNode = this.CreateForEachVariableStatement();
            var wrapper = (ForEachVariableStatementSyntaxWrapper)syntaxNode;
            Assert.Equal(syntaxNode.AttributeLists, wrapper.AttributeLists);

            var newPropertyValue = new SyntaxList<AttributeListSyntax>(
                SyntaxFactory.AttributeList(
                    SyntaxFactory.SeparatedList(
                        new[]
                        {
                            SyntaxFactory.Attribute(
                                SyntaxFactory.IdentifierName("y"),
                                null),
                        })));
            wrapper = wrapper.WithAttributeLists(newPropertyValue);
            Assert.NotNull(wrapper.SyntaxNode);
            Assert.NotSame(syntaxNode, wrapper.SyntaxNode);
            //// NOTE: IsEquivalentTo() is normally called here, but SyntaxList doesn't have that method
            Assert.NotEqual(syntaxNode.AttributeLists, wrapper.AttributeLists);
        }

        [Fact]
        public void TestAwaitKeyword()
        {
            var syntaxNode = this.CreateForEachVariableStatement();
            var wrapper = (ForEachVariableStatementSyntaxWrapper)syntaxNode;
            Assert.Equal(syntaxNode.AwaitKeyword, wrapper.AwaitKeyword);

            wrapper = wrapper.WithAwaitKeyword(SpacingExtensions.WithoutTrivia(SyntaxFactory.Token(SyntaxKind.AwaitKeyword)));
            Assert.NotNull(wrapper.SyntaxNode);
            Assert.NotSame(syntaxNode, wrapper.SyntaxNode);
            Assert.False(syntaxNode.AwaitKeyword.IsEquivalentTo(wrapper.AwaitKeyword));
        }

        [Fact]
        public void TestForEachKeyword()
        {
            var syntaxNode = this.CreateForEachVariableStatement();
            var wrapper = (ForEachVariableStatementSyntaxWrapper)syntaxNode;
            Assert.Equal(syntaxNode.ForEachKeyword, wrapper.ForEachKeyword);

            wrapper = wrapper.WithForEachKeyword(SpacingExtensions.WithoutTrivia(SyntaxFactory.Token(SyntaxKind.ForEachKeyword)));
            Assert.NotNull(wrapper.SyntaxNode);
            Assert.NotSame(syntaxNode, wrapper.SyntaxNode);
            Assert.False(syntaxNode.ForEachKeyword.IsEquivalentTo(wrapper.ForEachKeyword));
        }

        [Fact]
        public void TestOpenParenToken()
        {
            var syntaxNode = this.CreateForEachVariableStatement();
            var wrapper = (ForEachVariableStatementSyntaxWrapper)syntaxNode;
            Assert.Equal(syntaxNode.OpenParenToken, wrapper.OpenParenToken);

            wrapper = wrapper.WithOpenParenToken(SpacingExtensions.WithoutTrivia(SyntaxFactory.Token(SyntaxKind.OpenParenToken)));
            Assert.NotNull(wrapper.SyntaxNode);
            Assert.NotSame(syntaxNode, wrapper.SyntaxNode);
            Assert.False(syntaxNode.OpenParenToken.IsEquivalentTo(wrapper.OpenParenToken));
        }

        [Fact]
        public void TestVariable()
        {
            var syntaxNode = this.CreateForEachVariableStatement();
            var wrapper = (ForEachVariableStatementSyntaxWrapper)syntaxNode;
            Assert.Same(syntaxNode.Variable, wrapper.Variable);

            wrapper = wrapper.WithVariable(SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(0)));
            Assert.NotNull(wrapper.SyntaxNode);
            Assert.NotSame(syntaxNode, wrapper.SyntaxNode);
            Assert.NotSame(syntaxNode.Variable, wrapper.Variable);
            Assert.False(syntaxNode.Variable.IsEquivalentTo(wrapper.Variable));
        }

        [Fact]
        public void TestInKeyword()
        {
            var syntaxNode = this.CreateForEachVariableStatement();
            var wrapper = (ForEachVariableStatementSyntaxWrapper)syntaxNode;
            Assert.Equal(syntaxNode.InKeyword, wrapper.InKeyword);

            wrapper = wrapper.WithInKeyword(SpacingExtensions.WithoutTrivia(SyntaxFactory.Token(SyntaxKind.InKeyword)));
            Assert.NotNull(wrapper.SyntaxNode);
            Assert.NotSame(syntaxNode, wrapper.SyntaxNode);
            Assert.False(syntaxNode.InKeyword.IsEquivalentTo(wrapper.InKeyword));
        }

        [Fact]
        public void TestExpression()
        {
            var syntaxNode = this.CreateForEachVariableStatement();
            var wrapper = (ForEachVariableStatementSyntaxWrapper)syntaxNode;
            Assert.Same(syntaxNode.Expression, wrapper.Expression);

            wrapper = wrapper.WithExpression(SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(0)));
            Assert.NotNull(wrapper.SyntaxNode);
            Assert.NotSame(syntaxNode, wrapper.SyntaxNode);
            Assert.NotSame(syntaxNode.Expression, wrapper.Expression);
            Assert.False(syntaxNode.Expression.IsEquivalentTo(wrapper.Expression));
        }

        [Fact]
        public void TestCloseParenToken()
        {
            var syntaxNode = this.CreateForEachVariableStatement();
            var wrapper = (ForEachVariableStatementSyntaxWrapper)syntaxNode;
            Assert.Equal(syntaxNode.CloseParenToken, wrapper.CloseParenToken);

            wrapper = wrapper.WithCloseParenToken(SpacingExtensions.WithoutTrivia(SyntaxFactory.Token(SyntaxKind.CloseParenToken)));
            Assert.NotNull(wrapper.SyntaxNode);
            Assert.NotSame(syntaxNode, wrapper.SyntaxNode);
            Assert.False(syntaxNode.CloseParenToken.IsEquivalentTo(wrapper.CloseParenToken));
        }

        [Fact]
        public void TestStatement()
        {
            var syntaxNode = this.CreateForEachVariableStatement();
            var wrapper = (ForEachVariableStatementSyntaxWrapper)syntaxNode;
            Assert.Same(syntaxNode.Statement, wrapper.Statement);

            wrapper = wrapper.WithStatement(SyntaxFactory.Block());
            Assert.NotNull(wrapper.SyntaxNode);
            Assert.NotSame(syntaxNode, wrapper.SyntaxNode);
            Assert.NotSame(syntaxNode.Statement, wrapper.Statement);
            Assert.False(syntaxNode.Statement.IsEquivalentTo(wrapper.Statement));
        }

        [Fact]
        public void TestIsInstance()
        {
            Assert.False(ForEachVariableStatementSyntaxWrapper.IsInstance(null));
            Assert.False(ForEachVariableStatementSyntaxWrapper.IsInstance(SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression)));

            var forEachStatement = SyntaxFactory.ForEachStatement(
                SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword)),
                "item",
                SyntaxFactory.IdentifierName("collection"),
                SyntaxFactory.EmptyStatement());
            Assert.False(ForEachVariableStatementSyntaxWrapper.IsInstance(forEachStatement));

            var syntaxNode = this.CreateForEachVariableStatement();
            Assert.True(ForEachVariableStatementSyntaxWrapper.IsInstance(syntaxNode));
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
        public void TestConversions()
        {
            var syntaxNode = this.CreateForEachVariableStatement();
            var wrapper = (ForEachVariableStatementSyntaxWrapper)syntaxNode;

            StatementSyntax syntax = wrapper;
            Assert.Same(syntaxNode, syntax);

            wrapper = (ForEachVariableStatementSyntaxWrapper)(CommonForEachStatementSyntaxWrapper)syntaxNode;

            syntax = wrapper;
            Assert.Same(syntaxNode, syntax);
        }

        [Fact]
        public void TestInvalidConversion()
        {
            var syntaxNode = SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression);
            Assert.Throws<InvalidCastException>(() => (ForEachVariableStatementSyntaxWrapper)syntaxNode);

            var forEachStatement = SyntaxFactory.ForEachStatement(
                SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword)),
                "item",
                SyntaxFactory.IdentifierName("collection"),
                SyntaxFactory.EmptyStatement());
            Assert.Throws<InvalidCastException>(() => (ForEachVariableStatementSyntaxWrapper)forEachStatement);

            var commonWrapper = (CommonForEachStatementSyntaxWrapper)forEachStatement;
            Assert.Throws<InvalidCastException>(() => (ForEachVariableStatementSyntaxWrapper)commonWrapper);
        }

        private ForEachVariableStatementSyntax CreateForEachVariableStatement()
        {
            return SyntaxFactory.ForEachVariableStatement(
                new SyntaxList<AttributeListSyntax>(
                    SyntaxFactory.AttributeList(
                        SyntaxFactory.SeparatedList(
                            new[]
                            {
                                SyntaxFactory.Attribute(
                                    SyntaxFactory.IdentifierName("x"),
                                    null),
                            }))),
                SyntaxFactory.Token(SyntaxKind.AwaitKeyword),
                SyntaxFactory.Token(SyntaxKind.ForEachKeyword),
                SyntaxFactory.Token(SyntaxKind.OpenParenToken),
                SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression),
                SyntaxFactory.Token(SyntaxKind.InKeyword),
                SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression),
                SyntaxFactory.Token(SyntaxKind.CloseParenToken),
                SyntaxFactory.EmptyStatement());
        }
    }
}
