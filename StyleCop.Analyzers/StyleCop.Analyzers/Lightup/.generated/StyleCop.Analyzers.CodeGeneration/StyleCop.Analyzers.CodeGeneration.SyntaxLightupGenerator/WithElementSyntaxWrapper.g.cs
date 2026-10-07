// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal readonly partial struct WithElementSyntaxWrapper : ISyntaxWrapper<CSharpSyntaxNode>
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.CSharp.Syntax.WithElementSyntax";
        private static readonly Type WrappedType;

        private static readonly Func<CSharpSyntaxNode, SyntaxToken> WithKeywordAccessor;
        private static readonly Func<CSharpSyntaxNode, ArgumentListSyntax> ArgumentListAccessor;
        private static readonly Func<CSharpSyntaxNode, SyntaxToken, CSharpSyntaxNode> WithWithKeywordAccessor;
        private static readonly Func<CSharpSyntaxNode, ArgumentListSyntax, CSharpSyntaxNode> WithArgumentListAccessor;

        private readonly CSharpSyntaxNode node;

        static WithElementSyntaxWrapper()
        {
            WrappedType = SyntaxWrapperHelper.GetWrappedType(typeof(WithElementSyntaxWrapper));
            WithKeywordAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<CSharpSyntaxNode, SyntaxToken>(WrappedType, nameof(WithKeyword));
            ArgumentListAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<CSharpSyntaxNode, ArgumentListSyntax>(WrappedType, nameof(ArgumentList));
            WithWithKeywordAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<CSharpSyntaxNode, SyntaxToken>(WrappedType, nameof(WithKeyword));
            WithArgumentListAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<CSharpSyntaxNode, ArgumentListSyntax>(WrappedType, nameof(ArgumentList));
        }

        private WithElementSyntaxWrapper(CSharpSyntaxNode node)
        {
            this.node = node;
        }

        public CSharpSyntaxNode SyntaxNode => this.node;

        public SyntaxToken WithKeyword
        {
            get
            {
                return WithKeywordAccessor(this.SyntaxNode);
            }
        }

        public ArgumentListSyntax ArgumentList
        {
            get
            {
                return ArgumentListAccessor(this.SyntaxNode);
            }
        }

        public static explicit operator WithElementSyntaxWrapper(CollectionElementSyntaxWrapper node)
        {
            return (WithElementSyntaxWrapper)node.SyntaxNode;
        }

        public static explicit operator WithElementSyntaxWrapper(SyntaxNode node)
        {
            if (node == null)
            {
                return default;
            }

            if (!IsInstance(node))
            {
                throw new InvalidCastException($"Cannot cast '{node.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new WithElementSyntaxWrapper((CSharpSyntaxNode)node);
        }

        public static implicit operator CollectionElementSyntaxWrapper(WithElementSyntaxWrapper wrapper)
        {
            return CollectionElementSyntaxWrapper.FromUpcast(wrapper.node);
        }

        public static implicit operator CSharpSyntaxNode(WithElementSyntaxWrapper wrapper)
        {
            return wrapper.node;
        }

        public static bool IsInstance(SyntaxNode node)
        {
            return node != null && LightupHelpers.CanWrapNode(node, WrappedType);
        }

        public WithElementSyntaxWrapper WithWithKeyword(SyntaxToken withKeyword)
        {
            return new WithElementSyntaxWrapper(WithWithKeywordAccessor(this.SyntaxNode, withKeyword));
        }

        public WithElementSyntaxWrapper WithArgumentList(ArgumentListSyntax argumentList)
        {
            return new WithElementSyntaxWrapper(WithArgumentListAccessor(this.SyntaxNode, argumentList));
        }
    }
}
