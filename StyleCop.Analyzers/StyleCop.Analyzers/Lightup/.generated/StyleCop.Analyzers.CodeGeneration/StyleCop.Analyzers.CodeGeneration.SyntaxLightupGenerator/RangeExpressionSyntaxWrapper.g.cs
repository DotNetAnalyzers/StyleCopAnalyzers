// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal readonly partial struct RangeExpressionSyntaxWrapper : ISyntaxWrapper<ExpressionSyntax>
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.CSharp.Syntax.RangeExpressionSyntax";
        private static readonly Type WrappedType;

        private static readonly Func<ExpressionSyntax, ExpressionSyntax> LeftOperandAccessor;
        private static readonly Func<ExpressionSyntax, SyntaxToken> OperatorTokenAccessor;
        private static readonly Func<ExpressionSyntax, ExpressionSyntax> RightOperandAccessor;
        private static readonly Func<ExpressionSyntax, ExpressionSyntax, ExpressionSyntax> WithLeftOperandAccessor;
        private static readonly Func<ExpressionSyntax, SyntaxToken, ExpressionSyntax> WithOperatorTokenAccessor;
        private static readonly Func<ExpressionSyntax, ExpressionSyntax, ExpressionSyntax> WithRightOperandAccessor;

        private readonly ExpressionSyntax node;

        static RangeExpressionSyntaxWrapper()
        {
            WrappedType = SyntaxWrapperHelper.GetWrappedType(typeof(RangeExpressionSyntaxWrapper));
            LeftOperandAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<ExpressionSyntax, ExpressionSyntax>(WrappedType, nameof(LeftOperand));
            OperatorTokenAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<ExpressionSyntax, SyntaxToken>(WrappedType, nameof(OperatorToken));
            RightOperandAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<ExpressionSyntax, ExpressionSyntax>(WrappedType, nameof(RightOperand));
            WithLeftOperandAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<ExpressionSyntax, ExpressionSyntax>(WrappedType, nameof(LeftOperand));
            WithOperatorTokenAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<ExpressionSyntax, SyntaxToken>(WrappedType, nameof(OperatorToken));
            WithRightOperandAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<ExpressionSyntax, ExpressionSyntax>(WrappedType, nameof(RightOperand));
        }

        private RangeExpressionSyntaxWrapper(ExpressionSyntax node)
        {
            this.node = node;
        }

        public ExpressionSyntax SyntaxNode => this.node;

        public ExpressionSyntax LeftOperand
        {
            get
            {
                return LeftOperandAccessor(this.SyntaxNode);
            }
        }

        public SyntaxToken OperatorToken
        {
            get
            {
                return OperatorTokenAccessor(this.SyntaxNode);
            }
        }

        public ExpressionSyntax RightOperand
        {
            get
            {
                return RightOperandAccessor(this.SyntaxNode);
            }
        }

        public static explicit operator RangeExpressionSyntaxWrapper(SyntaxNode node)
        {
            if (node == null)
            {
                return default;
            }

            if (!IsInstance(node))
            {
                throw new InvalidCastException($"Cannot cast '{node.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new RangeExpressionSyntaxWrapper((ExpressionSyntax)node);
        }

        public static implicit operator ExpressionSyntax(RangeExpressionSyntaxWrapper wrapper)
        {
            return wrapper.node;
        }

        public static bool IsInstance(SyntaxNode node)
        {
            return node != null && LightupHelpers.CanWrapNode(node, WrappedType);
        }

        public RangeExpressionSyntaxWrapper WithLeftOperand(ExpressionSyntax leftOperand)
        {
            return new RangeExpressionSyntaxWrapper(WithLeftOperandAccessor(this.SyntaxNode, leftOperand));
        }

        public RangeExpressionSyntaxWrapper WithOperatorToken(SyntaxToken operatorToken)
        {
            return new RangeExpressionSyntaxWrapper(WithOperatorTokenAccessor(this.SyntaxNode, operatorToken));
        }

        public RangeExpressionSyntaxWrapper WithRightOperand(ExpressionSyntax rightOperand)
        {
            return new RangeExpressionSyntaxWrapper(WithRightOperandAccessor(this.SyntaxNode, rightOperand));
        }
    }
}
