// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal readonly partial struct SwitchExpressionSyntaxWrapper : ISyntaxWrapper<ExpressionSyntax>
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.CSharp.Syntax.SwitchExpressionSyntax";
        private static readonly Type WrappedType;
        private readonly ExpressionSyntax node;
        private static readonly Func<ExpressionSyntax, ExpressionSyntax> GoverningExpressionAccessor;
        private static readonly Func<ExpressionSyntax, SyntaxToken> SwitchKeywordAccessor;
        private static readonly Func<ExpressionSyntax, SyntaxToken> OpenBraceTokenAccessor;
        private static readonly Func<ExpressionSyntax, SeparatedSyntaxListWrapper<SwitchExpressionArmSyntaxWrapper>> ArmsAccessor;
        private static readonly Func<ExpressionSyntax, SyntaxToken> CloseBraceTokenAccessor;
        private static readonly Func<ExpressionSyntax, ExpressionSyntax, ExpressionSyntax> WithGoverningExpressionAccessor;
        private static readonly Func<ExpressionSyntax, SyntaxToken, ExpressionSyntax> WithSwitchKeywordAccessor;
        private static readonly Func<ExpressionSyntax, SyntaxToken, ExpressionSyntax> WithOpenBraceTokenAccessor;
        private static readonly Func<ExpressionSyntax, SeparatedSyntaxListWrapper<SwitchExpressionArmSyntaxWrapper>, ExpressionSyntax> WithArmsAccessor;
        private static readonly Func<ExpressionSyntax, SyntaxToken, ExpressionSyntax> WithCloseBraceTokenAccessor;
        private SwitchExpressionSyntaxWrapper(ExpressionSyntax node)
        {
            this.node = node;
        }

        public ExpressionSyntax SyntaxNode => this.node;
        public static explicit operator SwitchExpressionSyntaxWrapper(SyntaxNode node)
        {
            if (node == null)
            {
                return default;
            }

            if (!IsInstance(node))
            {
                throw new InvalidCastException($"Cannot cast '{node.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new SwitchExpressionSyntaxWrapper((ExpressionSyntax)node);
        }

        public static implicit operator ExpressionSyntax(SwitchExpressionSyntaxWrapper wrapper)
        {
            return wrapper.node;
        }

        public static bool IsInstance(SyntaxNode node)
        {
            return node != null && LightupHelpers.CanWrapNode(node, WrappedType);
        }
    }
}
