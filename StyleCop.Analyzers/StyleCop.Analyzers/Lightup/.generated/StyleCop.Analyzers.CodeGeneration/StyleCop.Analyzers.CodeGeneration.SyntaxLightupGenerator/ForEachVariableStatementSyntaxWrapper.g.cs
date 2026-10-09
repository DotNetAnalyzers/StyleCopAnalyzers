// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal readonly partial struct ForEachVariableStatementSyntaxWrapper : ISyntaxWrapper<StatementSyntax>
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.CSharp.Syntax.ForEachVariableStatementSyntax";
        private static readonly Type WrappedType;

        private static readonly Func<StatementSyntax, SyntaxList<AttributeListSyntax>> AttributeListsAccessor;
        private static readonly Func<StatementSyntax, SyntaxToken> AwaitKeywordAccessor;
        private static readonly Func<StatementSyntax, SyntaxToken> ForEachKeywordAccessor;
        private static readonly Func<StatementSyntax, SyntaxToken> OpenParenTokenAccessor;
        private static readonly Func<StatementSyntax, ExpressionSyntax> VariableAccessor;
        private static readonly Func<StatementSyntax, SyntaxToken> InKeywordAccessor;
        private static readonly Func<StatementSyntax, ExpressionSyntax> ExpressionAccessor;
        private static readonly Func<StatementSyntax, SyntaxToken> CloseParenTokenAccessor;
        private static readonly Func<StatementSyntax, StatementSyntax> StatementAccessor;
        private static readonly Func<StatementSyntax, SyntaxList<AttributeListSyntax>, StatementSyntax> WithAttributeListsAccessor;
        private static readonly Func<StatementSyntax, SyntaxToken, StatementSyntax> WithAwaitKeywordAccessor;
        private static readonly Func<StatementSyntax, SyntaxToken, StatementSyntax> WithForEachKeywordAccessor;
        private static readonly Func<StatementSyntax, SyntaxToken, StatementSyntax> WithOpenParenTokenAccessor;
        private static readonly Func<StatementSyntax, ExpressionSyntax, StatementSyntax> WithVariableAccessor;
        private static readonly Func<StatementSyntax, SyntaxToken, StatementSyntax> WithInKeywordAccessor;
        private static readonly Func<StatementSyntax, ExpressionSyntax, StatementSyntax> WithExpressionAccessor;
        private static readonly Func<StatementSyntax, SyntaxToken, StatementSyntax> WithCloseParenTokenAccessor;
        private static readonly Func<StatementSyntax, StatementSyntax, StatementSyntax> WithStatementAccessor;

        private readonly StatementSyntax node;

        static ForEachVariableStatementSyntaxWrapper()
        {
            WrappedType = SyntaxWrapperHelper.GetWrappedType(typeof(ForEachVariableStatementSyntaxWrapper));
            AttributeListsAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<StatementSyntax, SyntaxList<AttributeListSyntax>>(WrappedType, nameof(AttributeLists));
            AwaitKeywordAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<StatementSyntax, SyntaxToken>(WrappedType, nameof(AwaitKeyword));
            ForEachKeywordAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<StatementSyntax, SyntaxToken>(WrappedType, nameof(ForEachKeyword));
            OpenParenTokenAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<StatementSyntax, SyntaxToken>(WrappedType, nameof(OpenParenToken));
            VariableAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<StatementSyntax, ExpressionSyntax>(WrappedType, nameof(Variable));
            InKeywordAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<StatementSyntax, SyntaxToken>(WrappedType, nameof(InKeyword));
            ExpressionAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<StatementSyntax, ExpressionSyntax>(WrappedType, nameof(Expression));
            CloseParenTokenAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<StatementSyntax, SyntaxToken>(WrappedType, nameof(CloseParenToken));
            StatementAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<StatementSyntax, StatementSyntax>(WrappedType, nameof(Statement));
            WithAttributeListsAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<StatementSyntax, SyntaxList<AttributeListSyntax>>(WrappedType, nameof(AttributeLists));
            WithAwaitKeywordAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<StatementSyntax, SyntaxToken>(WrappedType, nameof(AwaitKeyword));
            WithForEachKeywordAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<StatementSyntax, SyntaxToken>(WrappedType, nameof(ForEachKeyword));
            WithOpenParenTokenAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<StatementSyntax, SyntaxToken>(WrappedType, nameof(OpenParenToken));
            WithVariableAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<StatementSyntax, ExpressionSyntax>(WrappedType, nameof(Variable));
            WithInKeywordAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<StatementSyntax, SyntaxToken>(WrappedType, nameof(InKeyword));
            WithExpressionAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<StatementSyntax, ExpressionSyntax>(WrappedType, nameof(Expression));
            WithCloseParenTokenAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<StatementSyntax, SyntaxToken>(WrappedType, nameof(CloseParenToken));
            WithStatementAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<StatementSyntax, StatementSyntax>(WrappedType, nameof(Statement));
        }

        private ForEachVariableStatementSyntaxWrapper(StatementSyntax node)
        {
            this.node = node;
        }

        public StatementSyntax SyntaxNode => this.node;


        public SyntaxList<AttributeListSyntax> AttributeLists
        {
            get
            {
                return AttributeListsAccessor(this.SyntaxNode);
            }
        }

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

        public static explicit operator ForEachVariableStatementSyntaxWrapper(CommonForEachStatementSyntaxWrapper node)
        {
            return (ForEachVariableStatementSyntaxWrapper)node.SyntaxNode;
        }

        public static explicit operator ForEachVariableStatementSyntaxWrapper(SyntaxNode node)
        {
            if (node == null)
            {
                return default;
            }

            if (!IsInstance(node))
            {
                throw new InvalidCastException($"Cannot cast '{node.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new ForEachVariableStatementSyntaxWrapper((StatementSyntax)node);
        }

        public static implicit operator CommonForEachStatementSyntaxWrapper(ForEachVariableStatementSyntaxWrapper wrapper)
        {
            return CommonForEachStatementSyntaxWrapper.FromUpcast(wrapper.node);
        }

        public static implicit operator StatementSyntax(ForEachVariableStatementSyntaxWrapper wrapper)
        {
            return wrapper.node;
        }

        public static bool IsInstance(SyntaxNode node)
        {
            return node != null && LightupHelpers.CanWrapNode(node, WrappedType);
        }

        public ForEachVariableStatementSyntaxWrapper WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists)
        {
            return new ForEachVariableStatementSyntaxWrapper(WithAttributeListsAccessor(this.SyntaxNode, attributeLists));
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
