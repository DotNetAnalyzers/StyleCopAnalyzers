// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal partial struct LocalFunctionStatementSyntaxWrapper : ISyntaxWrapper<StatementSyntax>
    {
        public LocalFunctionStatementSyntaxWrapper WithModifiers(SyntaxTokenList modifiers)
        {
            return new LocalFunctionStatementSyntaxWrapper(WithModifiersAccessor(this.SyntaxNode, modifiers));
        }

        public LocalFunctionStatementSyntaxWrapper WithReturnType(TypeSyntax returnType)
        {
            return new LocalFunctionStatementSyntaxWrapper(WithReturnTypeAccessor(this.SyntaxNode, returnType));
        }

        public LocalFunctionStatementSyntaxWrapper WithIdentifier(SyntaxToken identifier)
        {
            return new LocalFunctionStatementSyntaxWrapper(WithIdentifierAccessor(this.SyntaxNode, identifier));
        }

        public LocalFunctionStatementSyntaxWrapper WithTypeParameterList(TypeParameterListSyntax typeParameterList)
        {
            return new LocalFunctionStatementSyntaxWrapper(WithTypeParameterListAccessor(this.SyntaxNode, typeParameterList));
        }

        public LocalFunctionStatementSyntaxWrapper WithParameterList(ParameterListSyntax parameterList)
        {
            return new LocalFunctionStatementSyntaxWrapper(WithParameterListAccessor(this.SyntaxNode, parameterList));
        }

        public LocalFunctionStatementSyntaxWrapper WithConstraintClauses(SyntaxList<TypeParameterConstraintClauseSyntax> constraintClauses)
        {
            return new LocalFunctionStatementSyntaxWrapper(WithConstraintClausesAccessor(this.SyntaxNode, constraintClauses));
        }

        public LocalFunctionStatementSyntaxWrapper WithBody(BlockSyntax body)
        {
            return new LocalFunctionStatementSyntaxWrapper(WithBodyAccessor(this.SyntaxNode, body));
        }

        public LocalFunctionStatementSyntaxWrapper WithExpressionBody(ArrowExpressionClauseSyntax expressionBody)
        {
            return new LocalFunctionStatementSyntaxWrapper(WithExpressionBodyAccessor(this.SyntaxNode, expressionBody));
        }

        public LocalFunctionStatementSyntaxWrapper WithSemicolonToken(SyntaxToken semicolonToken)
        {
            return new LocalFunctionStatementSyntaxWrapper(WithSemicolonTokenAccessor(this.SyntaxNode, semicolonToken));
        }

        public LocalFunctionStatementSyntaxWrapper AddModifiers(params SyntaxToken[] items)
        {
            return this.WithModifiers(this.Modifiers.AddRange(items));
        }

        public LocalFunctionStatementSyntaxWrapper AddTypeParameterListParameters(params TypeParameterSyntax[] items)
        {
            var typeParameterList = this.TypeParameterList ?? SyntaxFactory.TypeParameterList();
            return this.WithTypeParameterList(typeParameterList.WithParameters(typeParameterList.Parameters.AddRange(items)));
        }

        public LocalFunctionStatementSyntaxWrapper AddParameterListParameters(params ParameterSyntax[] items)
        {
            return this.WithParameterList(this.ParameterList.WithParameters(this.ParameterList.Parameters.AddRange(items)));
        }

        public LocalFunctionStatementSyntaxWrapper AddConstraintClauses(params TypeParameterConstraintClauseSyntax[] items)
        {
            return this.WithConstraintClauses(this.ConstraintClauses.AddRange(items));
        }

        public LocalFunctionStatementSyntaxWrapper AddBodyStatements(params StatementSyntax[] items)
        {
            var body = this.Body ?? SyntaxFactory.Block();
            return this.WithBody(body.WithStatements(body.Statements.AddRange(items)));
        }
    }
}
