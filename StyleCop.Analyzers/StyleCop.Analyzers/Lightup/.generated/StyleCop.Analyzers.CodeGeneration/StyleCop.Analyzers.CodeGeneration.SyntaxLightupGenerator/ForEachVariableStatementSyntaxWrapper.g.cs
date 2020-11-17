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
        private readonly StatementSyntax node;
        private static readonly Func<StatementSyntax, ExpressionSyntax> VariableAccessor;
        private static readonly Func<StatementSyntax, SyntaxList<AttributeListSyntax>, StatementSyntax> WithAttributeListsAccessor;
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
            WithAttributeListsAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<StatementSyntax, SyntaxList<AttributeListSyntax>>(WrappedType, nameof(StatementSyntax.AttributeLists));
            WithAwaitKeywordAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<StatementSyntax, SyntaxToken>(WrappedType, nameof(CommonForEachStatementSyntaxWrapper.AwaitKeyword));
            WithForEachKeywordAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<StatementSyntax, SyntaxToken>(WrappedType, nameof(CommonForEachStatementSyntaxWrapper.ForEachKeyword));
            WithOpenParenTokenAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<StatementSyntax, SyntaxToken>(WrappedType, nameof(CommonForEachStatementSyntaxWrapper.OpenParenToken));
            WithVariableAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<StatementSyntax, ExpressionSyntax>(WrappedType, nameof(Variable));
            WithInKeywordAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<StatementSyntax, SyntaxToken>(WrappedType, nameof(CommonForEachStatementSyntaxWrapper.InKeyword));
            WithExpressionAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<StatementSyntax, ExpressionSyntax>(WrappedType, nameof(CommonForEachStatementSyntaxWrapper.Expression));
            WithCloseParenTokenAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<StatementSyntax, SyntaxToken>(WrappedType, nameof(CommonForEachStatementSyntaxWrapper.CloseParenToken));
            WithStatementAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<StatementSyntax, StatementSyntax>(WrappedType, nameof(CommonForEachStatementSyntaxWrapper.Statement));
        }

        private ForEachVariableStatementSyntaxWrapper(StatementSyntax node)
        {
            this.node = node;
        }

        public StatementSyntax SyntaxNode => this.node;
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

        public static implicit operator StatementSyntax(ForEachVariableStatementSyntaxWrapper wrapper)
        {
            return wrapper.node;
        }

        public static bool IsInstance(SyntaxNode node)
        {
            return node != null && LightupHelpers.CanWrapNode(node, WrappedType);
        }
    }
}
