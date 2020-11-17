// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal readonly partial struct WithExpressionSyntaxWrapper : ISyntaxWrapper<ExpressionSyntax>
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.CSharp.Syntax.WithExpressionSyntax";
        private static readonly Type WrappedType;
        private readonly ExpressionSyntax node;
        private static readonly Func<ExpressionSyntax, ExpressionSyntax> ExpressionAccessor;
        private static readonly Func<ExpressionSyntax, SyntaxToken> WithKeywordAccessor;
        private static readonly Func<ExpressionSyntax, InitializerExpressionSyntax> InitializerAccessor;
        private static readonly Func<ExpressionSyntax, ExpressionSyntax, ExpressionSyntax> WithExpressionAccessor;
        private static readonly Func<ExpressionSyntax, SyntaxToken, ExpressionSyntax> WithWithKeywordAccessor;
        private static readonly Func<ExpressionSyntax, InitializerExpressionSyntax, ExpressionSyntax> WithInitializerAccessor;
        private WithExpressionSyntaxWrapper(ExpressionSyntax node)
        {
            this.node = node;
        }

        public ExpressionSyntax SyntaxNode => this.node;
        public static explicit operator WithExpressionSyntaxWrapper(SyntaxNode node)
        {
            if (node == null)
            {
                return default;
            }

            if (!IsInstance(node))
            {
                throw new InvalidCastException($"Cannot cast '{node.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new WithExpressionSyntaxWrapper((ExpressionSyntax)node);
        }

        public static implicit operator ExpressionSyntax(WithExpressionSyntaxWrapper wrapper)
        {
            return wrapper.node;
        }

        public static bool IsInstance(SyntaxNode node)
        {
            return node != null && LightupHelpers.CanWrapNode(node, WrappedType);
        }
    }
}
