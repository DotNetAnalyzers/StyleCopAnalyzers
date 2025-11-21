// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal readonly partial struct BinaryPatternSyntaxWrapper : ISyntaxWrapper<CSharpSyntaxNode>
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.CSharp.Syntax.BinaryPatternSyntax";
        private static readonly Type WrappedType;

        private static readonly Func<CSharpSyntaxNode, CSharpSyntaxNode> LeftAccessor;
        private static readonly Func<CSharpSyntaxNode, SyntaxToken> OperatorTokenAccessor;
        private static readonly Func<CSharpSyntaxNode, CSharpSyntaxNode> RightAccessor;
        private static readonly Func<CSharpSyntaxNode, CSharpSyntaxNode, CSharpSyntaxNode> WithLeftAccessor;
        private static readonly Func<CSharpSyntaxNode, SyntaxToken, CSharpSyntaxNode> WithOperatorTokenAccessor;
        private static readonly Func<CSharpSyntaxNode, CSharpSyntaxNode, CSharpSyntaxNode> WithRightAccessor;

        private readonly CSharpSyntaxNode node;

        static BinaryPatternSyntaxWrapper()
        {
            WrappedType = SyntaxWrapperHelper.GetWrappedType(typeof(BinaryPatternSyntaxWrapper));
            LeftAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<CSharpSyntaxNode, CSharpSyntaxNode>(WrappedType, nameof(Left));
            OperatorTokenAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<CSharpSyntaxNode, SyntaxToken>(WrappedType, nameof(OperatorToken));
            RightAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<CSharpSyntaxNode, CSharpSyntaxNode>(WrappedType, nameof(Right));
            WithLeftAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<CSharpSyntaxNode, CSharpSyntaxNode>(WrappedType, nameof(Left));
            WithOperatorTokenAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<CSharpSyntaxNode, SyntaxToken>(WrappedType, nameof(OperatorToken));
            WithRightAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<CSharpSyntaxNode, CSharpSyntaxNode>(WrappedType, nameof(Right));
        }

        private BinaryPatternSyntaxWrapper(CSharpSyntaxNode node)
        {
            this.node = node;
        }

        public CSharpSyntaxNode SyntaxNode => this.node;

        public PatternSyntaxWrapper Left
        {
            get
            {
                return (PatternSyntaxWrapper)LeftAccessor(this.SyntaxNode);
            }
        }

        public SyntaxToken OperatorToken
        {
            get
            {
                return OperatorTokenAccessor(this.SyntaxNode);
            }
        }

        public PatternSyntaxWrapper Right
        {
            get
            {
                return (PatternSyntaxWrapper)RightAccessor(this.SyntaxNode);
            }
        }

        public static explicit operator BinaryPatternSyntaxWrapper(PatternSyntaxWrapper node)
        {
            return (BinaryPatternSyntaxWrapper)node.SyntaxNode;
        }

        public static explicit operator BinaryPatternSyntaxWrapper(ExpressionOrPatternSyntaxWrapper node)
        {
            return (BinaryPatternSyntaxWrapper)node.SyntaxNode;
        }

        public static explicit operator BinaryPatternSyntaxWrapper(SyntaxNode node)
        {
            if (node == null)
            {
                return default;
            }

            if (!IsInstance(node))
            {
                throw new InvalidCastException($"Cannot cast '{node.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new BinaryPatternSyntaxWrapper((CSharpSyntaxNode)node);
        }

        public static implicit operator PatternSyntaxWrapper(BinaryPatternSyntaxWrapper wrapper)
        {
            return PatternSyntaxWrapper.FromUpcast(wrapper.node);
        }

        public static implicit operator ExpressionOrPatternSyntaxWrapper(BinaryPatternSyntaxWrapper wrapper)
        {
            return ExpressionOrPatternSyntaxWrapper.FromUpcast(wrapper.node);
        }

        public static implicit operator CSharpSyntaxNode(BinaryPatternSyntaxWrapper wrapper)
        {
            return wrapper.node;
        }

        public static bool IsInstance(SyntaxNode node)
        {
            return node != null && LightupHelpers.CanWrapNode(node, WrappedType);
        }

        public BinaryPatternSyntaxWrapper WithLeft(PatternSyntaxWrapper left)
        {
            return new BinaryPatternSyntaxWrapper(WithLeftAccessor(this.SyntaxNode, left));
        }

        public BinaryPatternSyntaxWrapper WithOperatorToken(SyntaxToken operatorToken)
        {
            return new BinaryPatternSyntaxWrapper(WithOperatorTokenAccessor(this.SyntaxNode, operatorToken));
        }

        public BinaryPatternSyntaxWrapper WithRight(PatternSyntaxWrapper right)
        {
            return new BinaryPatternSyntaxWrapper(WithRightAccessor(this.SyntaxNode, right));
        }
    }
}
