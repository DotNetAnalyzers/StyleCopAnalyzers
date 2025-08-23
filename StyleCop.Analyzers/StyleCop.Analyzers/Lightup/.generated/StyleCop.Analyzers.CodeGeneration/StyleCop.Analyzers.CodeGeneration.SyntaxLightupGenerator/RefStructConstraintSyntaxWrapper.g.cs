// Copyright (c) Contributors to the New StyleCop Analyzers project.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal readonly partial struct RefStructConstraintSyntaxWrapper : ISyntaxWrapper<CSharpSyntaxNode>
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.CSharp.Syntax.RefStructConstraintSyntax";
        private static readonly Type WrappedType;

        private static readonly Func<CSharpSyntaxNode, SyntaxToken> RefKeywordAccessor;
        private static readonly Func<CSharpSyntaxNode, SyntaxToken> StructKeywordAccessor;
        private static readonly Func<CSharpSyntaxNode, SyntaxToken, CSharpSyntaxNode> WithRefKeywordAccessor;
        private static readonly Func<CSharpSyntaxNode, SyntaxToken, CSharpSyntaxNode> WithStructKeywordAccessor;

        private readonly CSharpSyntaxNode node;

        static RefStructConstraintSyntaxWrapper()
        {
            WrappedType = SyntaxWrapperHelper.GetWrappedType(typeof(RefStructConstraintSyntaxWrapper));
            RefKeywordAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<CSharpSyntaxNode, SyntaxToken>(WrappedType, nameof(RefKeyword));
            StructKeywordAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<CSharpSyntaxNode, SyntaxToken>(WrappedType, nameof(StructKeyword));
            WithRefKeywordAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<CSharpSyntaxNode, SyntaxToken>(WrappedType, nameof(RefKeyword));
            WithStructKeywordAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<CSharpSyntaxNode, SyntaxToken>(WrappedType, nameof(StructKeyword));
        }

        private RefStructConstraintSyntaxWrapper(CSharpSyntaxNode node)
        {
            this.node = node;
        }

        public CSharpSyntaxNode SyntaxNode => this.node;

        public SyntaxToken RefKeyword
        {
            get
            {
                return RefKeywordAccessor(this.SyntaxNode);
            }
        }

        public SyntaxToken StructKeyword
        {
            get
            {
                return StructKeywordAccessor(this.SyntaxNode);
            }
        }

        public static explicit operator RefStructConstraintSyntaxWrapper(AllowsConstraintSyntaxWrapper node)
        {
            return (RefStructConstraintSyntaxWrapper)node.SyntaxNode;
        }

        public static explicit operator RefStructConstraintSyntaxWrapper(SyntaxNode node)
        {
            if (node == null)
            {
                return default;
            }

            if (!IsInstance(node))
            {
                throw new InvalidCastException($"Cannot cast '{node.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new RefStructConstraintSyntaxWrapper((CSharpSyntaxNode)node);
        }

        public static implicit operator AllowsConstraintSyntaxWrapper(RefStructConstraintSyntaxWrapper wrapper)
        {
            return AllowsConstraintSyntaxWrapper.FromUpcast(wrapper.node);
        }

        public static implicit operator CSharpSyntaxNode(RefStructConstraintSyntaxWrapper wrapper)
        {
            return wrapper.node;
        }

        public static bool IsInstance(SyntaxNode node)
        {
            return node != null && LightupHelpers.CanWrapNode(node, WrappedType);
        }

        public RefStructConstraintSyntaxWrapper WithRefKeyword(SyntaxToken refKeyword)
        {
            return new RefStructConstraintSyntaxWrapper(WithRefKeywordAccessor(this.SyntaxNode, refKeyword));
        }

        public RefStructConstraintSyntaxWrapper WithStructKeyword(SyntaxToken structKeyword)
        {
            return new RefStructConstraintSyntaxWrapper(WithStructKeywordAccessor(this.SyntaxNode, structKeyword));
        }
    }
}
