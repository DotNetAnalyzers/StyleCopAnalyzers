// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal readonly partial struct ExpressionColonSyntaxWrapper : ISyntaxWrapper<CSharpSyntaxNode>
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionColonSyntax";
        private static readonly Type WrappedType;
        private static readonly Func<CSharpSyntaxNode, ExpressionSyntax, CSharpSyntaxNode> WithExpressionAccessor;
        private static readonly Func<CSharpSyntaxNode, SyntaxToken, CSharpSyntaxNode> WithColonTokenAccessor;

        private readonly CSharpSyntaxNode node;

        static ExpressionColonSyntaxWrapper()
        {
            WrappedType = SyntaxWrapperHelper.GetWrappedType(typeof(ExpressionColonSyntaxWrapper));
            WithExpressionAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<CSharpSyntaxNode, ExpressionSyntax>(WrappedType, nameof(Expression));
            WithColonTokenAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<CSharpSyntaxNode, SyntaxToken>(WrappedType, nameof(ColonToken));
        }

        private ExpressionColonSyntaxWrapper(CSharpSyntaxNode node)
        {
            this.node = node;
        }

        public CSharpSyntaxNode SyntaxNode => this.node;


        public ExpressionSyntax Expression
        {
            get
            {
                return ((BaseExpressionColonSyntaxWrapper)this).Expression;
            }
        }

        public SyntaxToken ColonToken
        {
            get
            {
                return ((BaseExpressionColonSyntaxWrapper)this).ColonToken;
            }
        }

        public static explicit operator ExpressionColonSyntaxWrapper(BaseExpressionColonSyntaxWrapper node)
        {
            return (ExpressionColonSyntaxWrapper)node.SyntaxNode;
        }

        public static explicit operator ExpressionColonSyntaxWrapper(SyntaxNode node)
        {
            if (node == null)
            {
                return default;
            }

            if (!IsInstance(node))
            {
                throw new InvalidCastException($"Cannot cast '{node.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new ExpressionColonSyntaxWrapper((CSharpSyntaxNode)node);
        }

        public static implicit operator BaseExpressionColonSyntaxWrapper(ExpressionColonSyntaxWrapper wrapper)
        {
            return BaseExpressionColonSyntaxWrapper.FromUpcast(wrapper.node);
        }

        public static implicit operator CSharpSyntaxNode(ExpressionColonSyntaxWrapper wrapper)
        {
            return wrapper.node;
        }

        public static bool IsInstance(SyntaxNode node)
        {
            return node != null && LightupHelpers.CanWrapNode(node, WrappedType);
        }

        public ExpressionColonSyntaxWrapper WithExpression(ExpressionSyntax expression)
        {
            return new ExpressionColonSyntaxWrapper(WithExpressionAccessor(this.SyntaxNode, expression));
        }

        public ExpressionColonSyntaxWrapper WithColonToken(SyntaxToken colonToken)
        {
            return new ExpressionColonSyntaxWrapper(WithColonTokenAccessor(this.SyntaxNode, colonToken));
        }
    }
}
