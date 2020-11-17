// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal readonly partial struct SwitchExpressionArmSyntaxWrapper : ISyntaxWrapper<CSharpSyntaxNode>
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.CSharp.Syntax.SwitchExpressionArmSyntax";
        private static readonly Type WrappedType;
        private readonly CSharpSyntaxNode node;
        private static readonly Func<CSharpSyntaxNode, CSharpSyntaxNode> PatternAccessor;
        private static readonly Func<CSharpSyntaxNode, CSharpSyntaxNode> WhenClauseAccessor;
        private static readonly Func<CSharpSyntaxNode, SyntaxToken> EqualsGreaterThanTokenAccessor;
        private static readonly Func<CSharpSyntaxNode, ExpressionSyntax> ExpressionAccessor;
        private static readonly Func<CSharpSyntaxNode, CSharpSyntaxNode, CSharpSyntaxNode> WithPatternAccessor;
        private static readonly Func<CSharpSyntaxNode, CSharpSyntaxNode, CSharpSyntaxNode> WithWhenClauseAccessor;
        private static readonly Func<CSharpSyntaxNode, SyntaxToken, CSharpSyntaxNode> WithEqualsGreaterThanTokenAccessor;
        private static readonly Func<CSharpSyntaxNode, ExpressionSyntax, CSharpSyntaxNode> WithExpressionAccessor;
        private SwitchExpressionArmSyntaxWrapper(CSharpSyntaxNode node)
        {
            this.node = node;
        }

        public CSharpSyntaxNode SyntaxNode => this.node;
        public static explicit operator SwitchExpressionArmSyntaxWrapper(SyntaxNode node)
        {
            if (node == null)
            {
                return default;
            }

            if (!IsInstance(node))
            {
                throw new InvalidCastException($"Cannot cast '{node.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new SwitchExpressionArmSyntaxWrapper((CSharpSyntaxNode)node);
        }

        public static implicit operator CSharpSyntaxNode(SwitchExpressionArmSyntaxWrapper wrapper)
        {
            return wrapper.node;
        }

        public static bool IsInstance(SyntaxNode node)
        {
            return node != null && LightupHelpers.CanWrapNode(node, WrappedType);
        }
    }
}
