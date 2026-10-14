// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal readonly partial struct CommonForEachStatementSyntaxWrapper : ISyntaxWrapper<StatementSyntax>
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.CSharp.Syntax.CommonForEachStatementSyntax";
        private static readonly Type? WrappedType;

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

        private readonly StatementSyntax? node;

        static CommonForEachStatementSyntaxWrapper()
        {
            WrappedType = SyntaxWrapperHelper.GetWrappedType(typeof(CommonForEachStatementSyntaxWrapper));
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

        private CommonForEachStatementSyntaxWrapper(StatementSyntax? node)
        {
            this.node = node;
        }

        public StatementSyntax? SyntaxNode => this.node;


        public SyntaxToken AwaitKeyword
        {
            get
            {
                return AwaitKeywordAccessor(this.node!);
            }
        }

        public SyntaxToken ForEachKeyword
        {
            get
            {
                return ForEachKeywordAccessor(this.node!);
            }
        }

        public SyntaxToken OpenParenToken
        {
            get
            {
                return OpenParenTokenAccessor(this.node!);
            }
        }

        public SyntaxToken InKeyword
        {
            get
            {
                return InKeywordAccessor(this.node!);
            }
        }

        public ExpressionSyntax Expression
        {
            get
            {
                return ExpressionAccessor(this.node!);
            }
        }

        public SyntaxToken CloseParenToken
        {
            get
            {
                return CloseParenTokenAccessor(this.node!);
            }
        }

        public StatementSyntax Statement
        {
            get
            {
                return StatementAccessor(this.node!);
            }
        }

        public static explicit operator CommonForEachStatementSyntaxWrapper(SyntaxNode? node)
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

        public static implicit operator StatementSyntax? (CommonForEachStatementSyntaxWrapper wrapper)
        {
            return wrapper.node;
        }

        public static bool IsInstance([NotNullWhen(true)] SyntaxNode? node)
        {
            return node != null && LightupHelpers.CanWrapNode(node, WrappedType);
        }

        public CommonForEachStatementSyntaxWrapper WithAwaitKeyword(SyntaxToken awaitKeyword)
        {
            return new CommonForEachStatementSyntaxWrapper(WithAwaitKeywordAccessor(this.node!, awaitKeyword));
        }

        public CommonForEachStatementSyntaxWrapper WithForEachKeyword(SyntaxToken forEachKeyword)
        {
            return new CommonForEachStatementSyntaxWrapper(WithForEachKeywordAccessor(this.node!, forEachKeyword));
        }

        public CommonForEachStatementSyntaxWrapper WithOpenParenToken(SyntaxToken openParenToken)
        {
            return new CommonForEachStatementSyntaxWrapper(WithOpenParenTokenAccessor(this.node!, openParenToken));
        }

        public CommonForEachStatementSyntaxWrapper WithInKeyword(SyntaxToken inKeyword)
        {
            return new CommonForEachStatementSyntaxWrapper(WithInKeywordAccessor(this.node!, inKeyword));
        }

        public CommonForEachStatementSyntaxWrapper WithExpression(ExpressionSyntax expression)
        {
            return new CommonForEachStatementSyntaxWrapper(WithExpressionAccessor(this.node!, expression));
        }

        public CommonForEachStatementSyntaxWrapper WithCloseParenToken(SyntaxToken closeParenToken)
        {
            return new CommonForEachStatementSyntaxWrapper(WithCloseParenTokenAccessor(this.node!, closeParenToken));
        }

        public CommonForEachStatementSyntaxWrapper WithStatement(StatementSyntax statement)
        {
            return new CommonForEachStatementSyntaxWrapper(WithStatementAccessor(this.node!, statement));
        }

        internal static CommonForEachStatementSyntaxWrapper FromUpcast(StatementSyntax? node)
        {
            return new CommonForEachStatementSyntaxWrapper(node);
        }
    }
}
