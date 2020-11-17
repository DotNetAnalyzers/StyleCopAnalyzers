// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal partial struct CommonForEachStatementSyntaxWrapper : ISyntaxWrapper<StatementSyntax>
    {
        internal const string FallbackWrappedTypeName = "Microsoft.CodeAnalysis.CSharp.Syntax.ForEachStatementSyntax";

        public SyntaxToken AwaitKeyword
        {
            get
            {
                return AwaitKeywordAccessor(this.SyntaxNode);
            }
        }

        public SyntaxToken ForEachKeyword
        {
            get
            {
                return ForEachKeywordAccessor(this.SyntaxNode);
            }
        }

        public SyntaxToken OpenParenToken
        {
            get
            {
                return OpenParenTokenAccessor(this.SyntaxNode);
            }
        }

        public SyntaxToken InKeyword
        {
            get
            {
                return InKeywordAccessor(this.SyntaxNode);
            }
        }

        public ExpressionSyntax Expression
        {
            get
            {
                return ExpressionAccessor(this.SyntaxNode);
            }
        }

        public SyntaxToken CloseParenToken
        {
            get
            {
                return CloseParenTokenAccessor(this.SyntaxNode);
            }
        }

        public StatementSyntax Statement
        {
            get
            {
                return StatementAccessor(this.SyntaxNode);
            }
        }

        public static implicit operator CommonForEachStatementSyntaxWrapper(ForEachStatementSyntax node)
        {
            return new CommonForEachStatementSyntaxWrapper(node);
        }

        public CommonForEachStatementSyntaxWrapper WithAwaitKeyword(SyntaxToken awaitKeyword)
        {
            return new CommonForEachStatementSyntaxWrapper(WithAwaitKeywordAccessor(this.SyntaxNode, awaitKeyword));
        }

        public CommonForEachStatementSyntaxWrapper WithForEachKeyword(SyntaxToken forEachKeyword)
        {
            return new CommonForEachStatementSyntaxWrapper(WithForEachKeywordAccessor(this.SyntaxNode, forEachKeyword));
        }

        public CommonForEachStatementSyntaxWrapper WithOpenParenToken(SyntaxToken openParenToken)
        {
            return new CommonForEachStatementSyntaxWrapper(WithOpenParenTokenAccessor(this.SyntaxNode, openParenToken));
        }

        public CommonForEachStatementSyntaxWrapper WithInKeyword(SyntaxToken inKeyword)
        {
            return new CommonForEachStatementSyntaxWrapper(WithInKeywordAccessor(this.SyntaxNode, inKeyword));
        }

        public CommonForEachStatementSyntaxWrapper WithExpression(ExpressionSyntax expression)
        {
            return new CommonForEachStatementSyntaxWrapper(WithExpressionAccessor(this.SyntaxNode, expression));
        }

        public CommonForEachStatementSyntaxWrapper WithCloseParenToken(SyntaxToken closeParenToken)
        {
            return new CommonForEachStatementSyntaxWrapper(WithCloseParenTokenAccessor(this.SyntaxNode, closeParenToken));
        }

        public CommonForEachStatementSyntaxWrapper WithStatement(StatementSyntax statement)
        {
            return new CommonForEachStatementSyntaxWrapper(WithStatementAccessor(this.SyntaxNode, statement));
        }
    }
}
