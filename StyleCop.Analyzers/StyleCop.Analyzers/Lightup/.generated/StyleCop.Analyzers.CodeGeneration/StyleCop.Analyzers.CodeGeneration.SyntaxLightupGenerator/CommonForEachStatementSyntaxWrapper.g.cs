// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal readonly partial struct CommonForEachStatementSyntaxWrapper : ISyntaxWrapper<StatementSyntax>
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.CSharp.Syntax.CommonForEachStatementSyntax";
        private static readonly Type WrappedType;
        private readonly StatementSyntax node;
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
        private CommonForEachStatementSyntaxWrapper(StatementSyntax node)
        {
            this.node = node;
        }

        public StatementSyntax SyntaxNode => this.node;
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

        internal static CommonForEachStatementSyntaxWrapper FromUpcast(StatementSyntax node)
        {
            return new CommonForEachStatementSyntaxWrapper(node);
        }
    }
}
