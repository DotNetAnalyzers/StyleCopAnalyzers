// Copyright (c) Contributors to the New StyleCop Analyzers project.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal readonly partial struct ExpressionElementSyntaxWrapper : ISyntaxWrapper<CSharpSyntaxNode>
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionElementSyntax";
        private static readonly Type WrappedType;

        private static readonly Func<CSharpSyntaxNode, ExpressionSyntax> ExpressionAccessor;
        private static readonly Func<CSharpSyntaxNode, ExpressionSyntax, CSharpSyntaxNode> WithExpressionAccessor;

        private readonly CSharpSyntaxNode node;

        static ExpressionElementSyntaxWrapper()
        {
            WrappedType = SyntaxWrapperHelper.GetWrappedType(typeof(ExpressionElementSyntaxWrapper));
            ExpressionAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<CSharpSyntaxNode, ExpressionSyntax>(WrappedType, nameof(Expression));
            WithExpressionAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<CSharpSyntaxNode, ExpressionSyntax>(WrappedType, nameof(Expression));
        }

        private ExpressionElementSyntaxWrapper(CSharpSyntaxNode node)
        {
            this.node = node;
        }

        public CSharpSyntaxNode SyntaxNode => this.node;

        public ExpressionSyntax Expression
        {
            get
            {
                return ExpressionAccessor(this.SyntaxNode);
            }
        }

        public static explicit operator ExpressionElementSyntaxWrapper(CollectionElementSyntaxWrapper node)
        {
            return (ExpressionElementSyntaxWrapper)node.SyntaxNode;
        }

        public static explicit operator ExpressionElementSyntaxWrapper(SyntaxNode node)
        {
            if (node == null)
            {
                return default;
            }

            if (!IsInstance(node))
            {
                throw new InvalidCastException($"Cannot cast '{node.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new ExpressionElementSyntaxWrapper((CSharpSyntaxNode)node);
        }

        public static implicit operator CollectionElementSyntaxWrapper(ExpressionElementSyntaxWrapper wrapper)
        {
            return CollectionElementSyntaxWrapper.FromUpcast(wrapper.node);
        }

        public static implicit operator CSharpSyntaxNode(ExpressionElementSyntaxWrapper wrapper)
        {
            return wrapper.node;
        }

        public static bool IsInstance(SyntaxNode node)
        {
            return node != null && LightupHelpers.CanWrapNode(node, WrappedType);
        }

        public ExpressionElementSyntaxWrapper WithExpression(ExpressionSyntax expression)
        {
            return new ExpressionElementSyntaxWrapper(WithExpressionAccessor(this.SyntaxNode, expression));
        }
    }
}
