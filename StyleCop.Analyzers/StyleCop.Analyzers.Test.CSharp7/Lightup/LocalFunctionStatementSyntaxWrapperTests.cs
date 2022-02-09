// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.CSharp7.Lightup
{
    using System;
    using System.Collections.Immutable;
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
        public void TestProperties()
        {
            var syntaxNode = this.CreateLocalFunctionStatement();
            Assert.True(syntaxNode.IsKind(SyntaxKind.LocalFunctionStatement));
            Assert.True(syntaxNode.IsKind(SyntaxKindEx.LocalFunctionStatement));

            var wrapper = (LocalFunctionStatementSyntaxWrapper)syntaxNode;
            Assert.Same(syntaxNode, wrapper.SyntaxNode);
            Assert.Equal(syntaxNode.Modifiers, wrapper.Modifiers); // This is a struct, so we can't use Same()
            Assert.Same(syntaxNode.ReturnType, wrapper.ReturnType);
            Assert.Equal(syntaxNode.Identifier, wrapper.Identifier); // This is a struct, so we can't use Same()
            Assert.Same(syntaxNode.TypeParameterList, wrapper.TypeParameterList);
            Assert.Same(syntaxNode.ParameterList, wrapper.ParameterList);
            Assert.Equal(syntaxNode.ConstraintClauses, wrapper.ConstraintClauses); // This is a struct, so we can't use Same()
            Assert.Same(syntaxNode.Body, wrapper.Body);
            Assert.Same(syntaxNode.ExpressionBody, wrapper.ExpressionBody);
            Assert.True(syntaxNode.SemicolonToken.IsEquivalentTo(wrapper.SemicolonToken));

            var newModifiers = SyntaxFactory.TokenList();
            var wrapperWithModifiedModifiers = wrapper.WithModifiers(newModifiers);
            Assert.NotNull(wrapperWithModifiedModifiers.SyntaxNode);
            Assert.NotEqual(syntaxNode.Modifiers, wrapperWithModifiedModifiers.Modifiers);
            Assert.Empty(wrapperWithModifiedModifiers.Modifiers);

            var newReturnType = SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.BoolKeyword));
            var wrapperWithModifiedReturnType = wrapper.WithReturnType(newReturnType);
            Assert.NotNull(wrapperWithModifiedReturnType.SyntaxNode);
            Assert.NotSame(syntaxNode.ReturnType, wrapperWithModifiedReturnType.ReturnType);
            Assert.Equal(SyntaxKind.PredefinedType, wrapperWithModifiedReturnType.ReturnType.Kind());

            var newIdentifier = SyntaxFactory.Identifier("NewIdentifier");
            var wrapperWithModifiedIdentifier = wrapper.WithIdentifier(newIdentifier);
            Assert.NotNull(wrapperWithModifiedIdentifier.SyntaxNode);
            Assert.NotEqual(syntaxNode.Identifier, wrapperWithModifiedIdentifier.Identifier);
            Assert.Equal(SyntaxKind.IdentifierToken, wrapperWithModifiedIdentifier.Identifier.Kind());
            Assert.Equal("NewIdentifier", wrapperWithModifiedIdentifier.Identifier.Text);

            var newTypeParameterList = SyntaxFactory.TypeParameterList();
            var wrapperWithModifiedTypeParameterList = wrapper.WithTypeParameterList(newTypeParameterList);
            Assert.NotNull(wrapperWithModifiedTypeParameterList.SyntaxNode);
            Assert.NotSame(syntaxNode.TypeParameterList, wrapperWithModifiedTypeParameterList.TypeParameterList);
            Assert.Empty(wrapperWithModifiedTypeParameterList.TypeParameterList.Parameters);

            var newParameterList = SyntaxFactory.ParameterList();
            var wrapperWithModifiedParameterList = wrapper.WithParameterList(newParameterList);
            Assert.NotNull(wrapperWithModifiedParameterList.SyntaxNode);
            Assert.NotSame(syntaxNode.ParameterList, wrapperWithModifiedParameterList.ParameterList);
            Assert.Empty(wrapperWithModifiedParameterList.ParameterList.Parameters);

            var newConstraintClauses = SyntaxFactory.List<TypeParameterConstraintClauseSyntax>();
            var wrapperWithModifiedConstraintClauses = wrapper.WithConstraintClauses(newConstraintClauses);
            Assert.NotNull(wrapperWithModifiedConstraintClauses.SyntaxNode);
            Assert.NotEqual(syntaxNode.ConstraintClauses, wrapperWithModifiedConstraintClauses.ConstraintClauses);
            Assert.Empty(wrapperWithModifiedConstraintClauses.ConstraintClauses);

            var newBody = SyntaxFactory.Block();
            var wrapperWithModifiedBody = wrapper.WithBody(newBody);
            Assert.NotNull(wrapperWithModifiedBody.SyntaxNode);
            Assert.Equal(SyntaxKind.Block, wrapperWithModifiedBody.Body.Kind());
            Assert.Empty(wrapperWithModifiedBody.Body.Statements);

            var newExpressionBody = SyntaxFactory.ArrowExpressionClause(SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression));
            var wrapperWithModifiedExpressionBody = wrapper.WithExpressionBody(newExpressionBody);
            Assert.NotNull(wrapperWithModifiedExpressionBody.SyntaxNode);
            Assert.Equal(SyntaxKind.ArrowExpressionClause, wrapperWithModifiedExpressionBody.ExpressionBody.Kind());
            Assert.Equal(SyntaxKind.NullLiteralExpression, wrapperWithModifiedExpressionBody.ExpressionBody.Expression.Kind());

            var newSemicolonToken = SyntaxFactory.Token(SyntaxKind.SemicolonToken).WithLeadingTrivia(SyntaxFactory.Space);
            var wrapperWithModifiedSemicolonToken = wrapper.WithSemicolonToken(newSemicolonToken);
            Assert.NotNull(wrapperWithModifiedSemicolonToken.SyntaxNode);
            Assert.Single(wrapperWithModifiedSemicolonToken.SemicolonToken.LeadingTrivia);
            Assert.Equal(" ", wrapperWithModifiedSemicolonToken.SemicolonToken.LeadingTrivia.ToString());

            var addedModifiers = new SyntaxToken[] { SyntaxFactory.Token(SyntaxKind.AsyncKeyword) };
            var wrapperWithAddedModifiers = wrapper.AddModifiers(addedModifiers);
            Assert.NotNull(wrapperWithAddedModifiers.SyntaxNode);
            Assert.NotEqual(syntaxNode.Modifiers, wrapperWithAddedModifiers.Modifiers);
            Assert.Equal(2, wrapperWithAddedModifiers.Modifiers.Count);
            Assert.Equal(SyntaxKind.PrivateKeyword, wrapperWithAddedModifiers.Modifiers[0].Kind());
            Assert.Equal(SyntaxKind.AsyncKeyword, wrapperWithAddedModifiers.Modifiers[1].Kind());

            var addedTypeParameterList = new TypeParameterSyntax[] { SyntaxFactory.TypeParameter("T2") };
            var wrapperWithAddedTypeParameterList = wrapper.AddTypeParameterListParameters(addedTypeParameterList);
            Assert.NotNull(wrapperWithAddedTypeParameterList.SyntaxNode);
            Assert.NotSame(syntaxNode.TypeParameterList, wrapperWithAddedTypeParameterList.TypeParameterList);
            Assert.Equal(2, wrapperWithAddedTypeParameterList.TypeParameterList.Parameters.Count);
            Assert.Equal("T1", wrapperWithAddedTypeParameterList.TypeParameterList.Parameters[0].Identifier.Text);
            Assert.Equal("T2", wrapperWithAddedTypeParameterList.TypeParameterList.Parameters[1].Identifier.Text);

            var addedParameterList = new ParameterSyntax[] { SyntaxFactory.Parameter(SyntaxFactory.Identifier("param2")) };
            var wrapperWithAddedParameterList = wrapper.AddParameterListParameters(addedParameterList);
            Assert.NotNull(wrapperWithAddedParameterList.SyntaxNode);
            Assert.NotSame(syntaxNode.ParameterList, wrapperWithAddedParameterList.ParameterList);
            Assert.Equal(2, wrapperWithAddedParameterList.ParameterList.Parameters.Count);
            Assert.Equal("param1", wrapperWithAddedParameterList.ParameterList.Parameters[0].Identifier.Text);
            Assert.Equal("param2", wrapperWithAddedParameterList.ParameterList.Parameters[1].Identifier.Text);

            var addedConstraintClauses = new TypeParameterConstraintClauseSyntax[] { SyntaxFactory.TypeParameterConstraintClause(SyntaxFactory.IdentifierName("constraint2")) };
            var wrapperWithAddedConstraintClauses = wrapper.AddConstraintClauses(addedConstraintClauses);
            Assert.NotNull(wrapperWithAddedConstraintClauses.SyntaxNode);
            Assert.NotEqual(syntaxNode.ConstraintClauses, wrapperWithAddedConstraintClauses.ConstraintClauses);
            Assert.Equal(2, wrapperWithAddedConstraintClauses.ConstraintClauses.Count);
            Assert.Equal("constraint1", wrapperWithAddedConstraintClauses.ConstraintClauses[0].Name.Identifier.Text);
            Assert.Equal("constraint2", wrapperWithAddedConstraintClauses.ConstraintClauses[1].Name.Identifier.Text);

            var addedBodyStatements = new StatementSyntax[] { SyntaxFactory.ReturnStatement() };
            var wrapperWithAddedBodyStatements = wrapper.AddBodyStatements(addedBodyStatements);
            Assert.NotNull(wrapperWithAddedBodyStatements.SyntaxNode);
            Assert.Equal(SyntaxKind.Block, wrapperWithAddedBodyStatements.Body.Kind());
            Assert.Equal(2, wrapperWithAddedBodyStatements.Body.Statements.Count);
            Assert.Equal(SyntaxKind.BreakStatement, wrapperWithAddedBodyStatements.Body.Statements[0].Kind());
            Assert.Equal(SyntaxKind.ReturnStatement, wrapperWithAddedBodyStatements.Body.Statements[1].Kind());
        }

        [Fact]
        public void TestIsInstance()
        {
            Assert.False(LocalFunctionStatementSyntaxWrapper.IsInstance(null));
            Assert.False(LocalFunctionStatementSyntaxWrapper.IsInstance(SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression)));

            var syntaxNode = this.CreateLocalFunctionStatement();
            Assert.True(LocalFunctionStatementSyntaxWrapper.IsInstance(syntaxNode));
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
        public void TestConversions()
        {
            var syntaxNode = this.CreateLocalFunctionStatement();
            var wrapper = (LocalFunctionStatementSyntaxWrapper)syntaxNode;

            StatementSyntax syntax = wrapper;
            Assert.Same(syntaxNode, syntax);
        }

        [Fact]
        public void TestInvalidConversion()
        {
            var syntaxNode = SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression);
            Assert.Throws<InvalidCastException>(() => (LocalFunctionStatementSyntaxWrapper)syntaxNode);
        }

        private LocalFunctionStatementSyntax CreateLocalFunctionStatement()
        {
            return SyntaxFactory.LocalFunctionStatement(
                modifiers: SyntaxFactory.TokenList(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PrivateKeyword))),
                returnType: SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword)),
                identifier: SyntaxFactory.Identifier("Identifier"),
                typeParameterList: SyntaxFactory.TypeParameterList(SyntaxFactory.SeparatedList(ImmutableArray.Create(SyntaxFactory.TypeParameter("T1")))),
                parameterList: SyntaxFactory.ParameterList(SyntaxFactory.SeparatedList(ImmutableArray.Create(SyntaxFactory.Parameter(SyntaxFactory.Identifier("param1"))))),
                constraintClauses: SyntaxFactory.List<TypeParameterConstraintClauseSyntax>(ImmutableArray.Create(SyntaxFactory.TypeParameterConstraintClause(SyntaxFactory.IdentifierName("constraint1")))),
                body: SyntaxFactory.Block(SyntaxFactory.BreakStatement()),
                expressionBody: SyntaxFactory.ArrowExpressionClause(SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(0))));
        }
    }
}
