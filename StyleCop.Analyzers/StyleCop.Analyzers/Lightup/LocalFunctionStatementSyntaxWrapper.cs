// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Lightup
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal partial struct LocalFunctionStatementSyntaxWrapper : ISyntaxWrapper<StatementSyntax>
    {
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
