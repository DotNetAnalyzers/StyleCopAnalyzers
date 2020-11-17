// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal partial struct ForEachVariableStatementSyntaxWrapper : ISyntaxWrapper<StatementSyntax>
    {
        public ForEachVariableStatementSyntaxWrapper WithAwaitKeyword(SyntaxToken awaitKeyword)
        {
            return new ForEachVariableStatementSyntaxWrapper(WithAwaitKeywordAccessor(this.SyntaxNode, awaitKeyword));
        }

        public ForEachVariableStatementSyntaxWrapper WithForEachKeyword(SyntaxToken forEachKeyword)
        {
            return new ForEachVariableStatementSyntaxWrapper(WithForEachKeywordAccessor(this.SyntaxNode, forEachKeyword));
        }

        public ForEachVariableStatementSyntaxWrapper WithOpenParenToken(SyntaxToken openParenToken)
        {
            return new ForEachVariableStatementSyntaxWrapper(WithOpenParenTokenAccessor(this.SyntaxNode, openParenToken));
        }

        public ForEachVariableStatementSyntaxWrapper WithVariable(ExpressionSyntax variable)
        {
            return new ForEachVariableStatementSyntaxWrapper(WithVariableAccessor(this.SyntaxNode, variable));
        }

        public ForEachVariableStatementSyntaxWrapper WithInKeyword(SyntaxToken inKeyword)
        {
            return new ForEachVariableStatementSyntaxWrapper(WithInKeywordAccessor(this.SyntaxNode, inKeyword));
        }

        public ForEachVariableStatementSyntaxWrapper WithExpression(ExpressionSyntax expression)
        {
            return new ForEachVariableStatementSyntaxWrapper(WithExpressionAccessor(this.SyntaxNode, expression));
        }

        public ForEachVariableStatementSyntaxWrapper WithCloseParenToken(SyntaxToken closeParenToken)
        {
            return new ForEachVariableStatementSyntaxWrapper(WithCloseParenTokenAccessor(this.SyntaxNode, closeParenToken));
        }

        public ForEachVariableStatementSyntaxWrapper WithStatement(StatementSyntax statement)
        {
            return new ForEachVariableStatementSyntaxWrapper(WithStatementAccessor(this.SyntaxNode, statement));
        }
    }
}
