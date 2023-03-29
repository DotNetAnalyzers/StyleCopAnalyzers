// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal readonly partial struct BaseExpressionColonSyntaxWrapper : ISyntaxWrapper<CSharpSyntaxNode>
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.CSharp.Syntax.BaseExpressionColonSyntax";
        private static readonly Type WrappedType;

        private static readonly Func<CSharpSyntaxNode, ExpressionSyntax> ExpressionAccessor;
        private static readonly Func<CSharpSyntaxNode, SyntaxToken> ColonTokenAccessor;
        private static readonly Func<CSharpSyntaxNode, ExpressionSyntax, CSharpSyntaxNode> WithExpressionAccessor;
        private static readonly Func<CSharpSyntaxNode, SyntaxToken, CSharpSyntaxNode> WithColonTokenAccessor;

        private readonly CSharpSyntaxNode node;

        static BaseExpressionColonSyntaxWrapper()
        {
            WrappedType = SyntaxWrapperHelper.GetWrappedType(typeof(BaseExpressionColonSyntaxWrapper));
            ExpressionAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<CSharpSyntaxNode, ExpressionSyntax>(WrappedType, nameof(Expression));
            ColonTokenAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<CSharpSyntaxNode, SyntaxToken>(WrappedType, nameof(ColonToken));
            WithExpressionAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<CSharpSyntaxNode, ExpressionSyntax>(WrappedType, nameof(Expression));
            WithColonTokenAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<CSharpSyntaxNode, SyntaxToken>(WrappedType, nameof(ColonToken));
        }

        private BaseExpressionColonSyntaxWrapper(CSharpSyntaxNode node)
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

        public SyntaxToken ColonToken
        {
            get
            {
                return ColonTokenAccessor(this.SyntaxNode);
            }
        }

        public static explicit operator BaseExpressionColonSyntaxWrapper(SyntaxNode node)
        {
            if (node == null)
            {
                return default;
            }

            if (!IsInstance(node))
            {
                throw new InvalidCastException($"Cannot cast '{node.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new BaseExpressionColonSyntaxWrapper((CSharpSyntaxNode)node);
        }

        public static implicit operator CSharpSyntaxNode(BaseExpressionColonSyntaxWrapper wrapper)
        {
            return wrapper.node;
        }

        public static bool IsInstance(SyntaxNode node)
        {
            return node != null && LightupHelpers.CanWrapNode(node, WrappedType);
        }

        public BaseExpressionColonSyntaxWrapper WithExpression(ExpressionSyntax expression)
        {
            return new BaseExpressionColonSyntaxWrapper(WithExpressionAccessor(this.SyntaxNode, expression));
        }

        public BaseExpressionColonSyntaxWrapper WithColonToken(SyntaxToken colonToken)
        {
            return new BaseExpressionColonSyntaxWrapper(WithColonTokenAccessor(this.SyntaxNode, colonToken));
        }

        internal static BaseExpressionColonSyntaxWrapper FromUpcast(CSharpSyntaxNode node)
        {
            return new BaseExpressionColonSyntaxWrapper(node);
        }
    }
}
