// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal struct CommonForEachStatementSyntaxWrapper : ISyntaxWrapper<StatementSyntax>
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.CSharp.Syntax.CommonForEachStatementSyntax";
        internal const string FallbackWrappedTypeName = "Microsoft.CodeAnalysis.CSharp.Syntax.ForEachStatementSyntax";
        private static readonly Type WrappedType;

        private static readonly Func<StatementSyntax, SyntaxToken> AwaitKeywordAccessor;
        private static readonly Func<StatementSyntax, SyntaxToken> ForEachKeywordAccessor;
        private static readonly Func<StatementSyntax, SyntaxToken> OpenParenTokenAccessor;
        private static readonly Func<StatementSyntax, SyntaxToken> InKeywordAccessor;
        private static readonly Func<StatementSyntax, ExpressionSyntax> ExpressionAccessor;
        private static readonly Func<StatementSyntax, SyntaxToken> CloseParenTokenAccessor;
        private static readonly Func<StatementSyntax, StatementSyntax> StatementAccessor;

        private static readonly Func<StatementSyntax, SyntaxToken, StatementSyntax> WithAwaitKeywordAccessor;
        private static readonly Func<StatementSyntax, SyntaxToken, StatementSyntax> WithForEachKeywordAccessor;
        private static readonly Func<StatementSyntax, SyntaxToken, StatementSyntax> WithOpenParenTokenAccessor;
        private static readonly Func<StatementSyntax, SyntaxToken, StatementSyntax> WithInKeywordAccessor;
        private static readonly Func<StatementSyntax, ExpressionSyntax, StatementSyntax> WithExpressionAccessor;
        private static readonly Func<StatementSyntax, SyntaxToken, StatementSyntax> WithCloseParenTokenAccessor;
        private static readonly Func<StatementSyntax, StatementSyntax, StatementSyntax> WithStatementAccessor;

        private readonly StatementSyntax node;

        static CommonForEachStatementSyntaxWrapper()
        {
            WrappedType = WrapperHelper.GetWrappedType(typeof(CommonForEachStatementSyntaxWrapper));
            AwaitKeywordAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<StatementSyntax, SyntaxToken>(WrappedType, nameof(AwaitKeyword));
            ForEachKeywordAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<StatementSyntax, SyntaxToken>(WrappedType, nameof(ForEachKeyword));
            OpenParenTokenAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<StatementSyntax, SyntaxToken>(WrappedType, nameof(OpenParenToken));
            InKeywordAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<StatementSyntax, SyntaxToken>(WrappedType, nameof(InKeyword));
            ExpressionAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<StatementSyntax, ExpressionSyntax>(WrappedType, nameof(Expression));
            CloseParenTokenAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<StatementSyntax, SyntaxToken>(WrappedType, nameof(CloseParenToken));
            StatementAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<StatementSyntax, StatementSyntax>(WrappedType, nameof(Statement));

            WithAwaitKeywordAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<StatementSyntax, SyntaxToken>(WrappedType, nameof(AwaitKeyword));
            WithForEachKeywordAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<StatementSyntax, SyntaxToken>(WrappedType, nameof(ForEachKeyword));
            WithOpenParenTokenAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<StatementSyntax, SyntaxToken>(WrappedType, nameof(OpenParenToken));
            WithInKeywordAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<StatementSyntax, SyntaxToken>(WrappedType, nameof(InKeyword));
            WithExpressionAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<StatementSyntax, ExpressionSyntax>(WrappedType, nameof(Expression));
            WithCloseParenTokenAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<StatementSyntax, SyntaxToken>(WrappedType, nameof(CloseParenToken));
            WithStatementAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<StatementSyntax, StatementSyntax>(WrappedType, nameof(Statement));
        }

        private CommonForEachStatementSyntaxWrapper(StatementSyntax node)
        {
            this.node = node;
        }

        public StatementSyntax SyntaxNode => this.node;

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

        public static explicit operator CommonForEachStatementSyntaxWrapper(SyntaxNode node)
        {
            if (node == null)
            {
                return default;
            }

            if (!IsInstance(node))
            {
                throw new InvalidCastException($"Cannot cast '{node.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new CommonForEachStatementSyntaxWrapper((StatementSyntax)node);
        }

        public static implicit operator StatementSyntax(CommonForEachStatementSyntaxWrapper wrapper)
        {
            return wrapper.node;
        }

        public static bool IsInstance(SyntaxNode node)
        {
            return node != null && LightupHelpers.CanWrapNode(node, WrappedType);
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

        internal static CommonForEachStatementSyntaxWrapper FromUpcast(StatementSyntax node)
        {
            return new CommonForEachStatementSyntaxWrapper(node);
        }
    }
}
