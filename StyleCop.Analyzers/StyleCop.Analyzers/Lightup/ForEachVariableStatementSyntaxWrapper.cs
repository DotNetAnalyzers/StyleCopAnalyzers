// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal partial struct ForEachVariableStatementSyntaxWrapper : ISyntaxWrapper<StatementSyntax>
    {
        private static readonly Func<StatementSyntax, ExpressionSyntax> VariableAccessor;

        private static readonly Func<StatementSyntax, SyntaxToken, StatementSyntax> WithAwaitKeywordAccessor;
        private static readonly Func<StatementSyntax, SyntaxToken, StatementSyntax> WithForEachKeywordAccessor;
        private static readonly Func<StatementSyntax, SyntaxToken, StatementSyntax> WithOpenParenTokenAccessor;
        private static readonly Func<StatementSyntax, ExpressionSyntax, StatementSyntax> WithVariableAccessor;
        private static readonly Func<StatementSyntax, SyntaxToken, StatementSyntax> WithInKeywordAccessor;
        private static readonly Func<StatementSyntax, ExpressionSyntax, StatementSyntax> WithExpressionAccessor;
        private static readonly Func<StatementSyntax, SyntaxToken, StatementSyntax> WithCloseParenTokenAccessor;
        private static readonly Func<StatementSyntax, StatementSyntax, StatementSyntax> WithStatementAccessor;

        static ForEachVariableStatementSyntaxWrapper()
        {
            WrappedType = SyntaxWrapperHelper.GetWrappedType(typeof(ForEachVariableStatementSyntaxWrapper));
            VariableAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<StatementSyntax, ExpressionSyntax>(WrappedType, nameof(Variable));

            WithAwaitKeywordAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<StatementSyntax, SyntaxToken>(WrappedType, nameof(AwaitKeyword));
            WithForEachKeywordAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<StatementSyntax, SyntaxToken>(WrappedType, nameof(ForEachKeyword));
            WithOpenParenTokenAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<StatementSyntax, SyntaxToken>(WrappedType, nameof(OpenParenToken));
            WithVariableAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<StatementSyntax, ExpressionSyntax>(WrappedType, nameof(Variable));
            WithInKeywordAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<StatementSyntax, SyntaxToken>(WrappedType, nameof(InKeyword));
            WithExpressionAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<StatementSyntax, ExpressionSyntax>(WrappedType, nameof(Expression));
            WithCloseParenTokenAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<StatementSyntax, SyntaxToken>(WrappedType, nameof(CloseParenToken));
            WithStatementAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<StatementSyntax, StatementSyntax>(WrappedType, nameof(Statement));
        }

        public SyntaxToken AwaitKeyword
        {
            get
            {
                return ((CommonForEachStatementSyntaxWrapper)this).AwaitKeyword;
            }
        }

        public SyntaxToken ForEachKeyword
        {
            get
            {
                return ((CommonForEachStatementSyntaxWrapper)this).ForEachKeyword;
            }
        }

        public SyntaxToken OpenParenToken
        {
            get
            {
                return ((CommonForEachStatementSyntaxWrapper)this).OpenParenToken;
            }
        }

        public ExpressionSyntax Variable
        {
            get
            {
                return VariableAccessor(this.SyntaxNode);
            }
        }

        public SyntaxToken InKeyword
        {
            get
            {
                return ((CommonForEachStatementSyntaxWrapper)this).InKeyword;
            }
        }

        public ExpressionSyntax Expression
        {
            get
            {
                return ((CommonForEachStatementSyntaxWrapper)this).Expression;
            }
        }

        public SyntaxToken CloseParenToken
        {
            get
            {
                return ((CommonForEachStatementSyntaxWrapper)this).CloseParenToken;
            }
        }

        public StatementSyntax Statement
        {
            get
            {
                return ((CommonForEachStatementSyntaxWrapper)this).Statement;
            }
        }

        public static explicit operator ForEachVariableStatementSyntaxWrapper(CommonForEachStatementSyntaxWrapper node)
        {
            return (ForEachVariableStatementSyntaxWrapper)node.SyntaxNode;
        }

        public static implicit operator CommonForEachStatementSyntaxWrapper(ForEachVariableStatementSyntaxWrapper wrapper)
        {
            return CommonForEachStatementSyntaxWrapper.FromUpcast(wrapper.node);
        }

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
