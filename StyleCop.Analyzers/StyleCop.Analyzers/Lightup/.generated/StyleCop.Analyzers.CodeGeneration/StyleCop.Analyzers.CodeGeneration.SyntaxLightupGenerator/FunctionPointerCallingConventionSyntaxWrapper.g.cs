// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal readonly partial struct FunctionPointerCallingConventionSyntaxWrapper : ISyntaxWrapper<CSharpSyntaxNode>
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.CSharp.Syntax.FunctionPointerCallingConventionSyntax";
        private static readonly Type WrappedType;
        private static readonly Func<CSharpSyntaxNode, SyntaxToken> ManagedOrUnmanagedKeywordAccessor;
        private static readonly Func<CSharpSyntaxNode, CSharpSyntaxNode> UnmanagedCallingConventionListAccessor;
        private static readonly Func<CSharpSyntaxNode, SyntaxToken, CSharpSyntaxNode> WithManagedOrUnmanagedKeywordAccessor;
        private static readonly Func<CSharpSyntaxNode, CSharpSyntaxNode, CSharpSyntaxNode> WithUnmanagedCallingConventionListAccessor;
        private readonly CSharpSyntaxNode node;
        static FunctionPointerCallingConventionSyntaxWrapper()
        {
            WrappedType = SyntaxWrapperHelper.GetWrappedType(typeof(FunctionPointerCallingConventionSyntaxWrapper));
            ManagedOrUnmanagedKeywordAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<CSharpSyntaxNode, SyntaxToken>(WrappedType, nameof(ManagedOrUnmanagedKeyword));
            UnmanagedCallingConventionListAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<CSharpSyntaxNode, CSharpSyntaxNode>(WrappedType, nameof(UnmanagedCallingConventionList));
            WithManagedOrUnmanagedKeywordAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<CSharpSyntaxNode, SyntaxToken>(WrappedType, nameof(ManagedOrUnmanagedKeyword));
            WithUnmanagedCallingConventionListAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<CSharpSyntaxNode, CSharpSyntaxNode>(WrappedType, nameof(UnmanagedCallingConventionList));
        }

        private FunctionPointerCallingConventionSyntaxWrapper(CSharpSyntaxNode node)
        {
            this.node = node;
        }

        public CSharpSyntaxNode SyntaxNode => this.node;
        public SyntaxToken ManagedOrUnmanagedKeyword
        {
            get
            {
                return ManagedOrUnmanagedKeywordAccessor(this.SyntaxNode);
            }
        }

        public FunctionPointerUnmanagedCallingConventionListSyntaxWrapper UnmanagedCallingConventionList
        {
            get
            {
                return (FunctionPointerUnmanagedCallingConventionListSyntaxWrapper)UnmanagedCallingConventionListAccessor(this.SyntaxNode);
            }
        }

        public static explicit operator FunctionPointerCallingConventionSyntaxWrapper(SyntaxNode node)
        {
            if (node == null)
            {
                return default;
            }

            if (!IsInstance(node))
            {
                throw new InvalidCastException($"Cannot cast '{node.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new FunctionPointerCallingConventionSyntaxWrapper((CSharpSyntaxNode)node);
        }

        public static implicit operator CSharpSyntaxNode(FunctionPointerCallingConventionSyntaxWrapper wrapper)
        {
            return wrapper.node;
        }

        public static bool IsInstance(SyntaxNode node)
        {
            return node != null && LightupHelpers.CanWrapNode(node, WrappedType);
        }
    }
}
