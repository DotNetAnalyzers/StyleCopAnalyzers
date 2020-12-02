// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal readonly partial struct FunctionPointerUnmanagedCallingConventionListSyntaxWrapper : ISyntaxWrapper<CSharpSyntaxNode>
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.CSharp.Syntax.FunctionPointerUnmanagedCallingConventionListSyntax";
        private static readonly Type WrappedType;

        private static readonly Func<CSharpSyntaxNode, SyntaxToken> OpenBracketTokenAccessor;
        private static readonly Func<CSharpSyntaxNode, SeparatedSyntaxListWrapper<FunctionPointerUnmanagedCallingConventionSyntaxWrapper>> CallingConventionsAccessor;
        private static readonly Func<CSharpSyntaxNode, SyntaxToken> CloseBracketTokenAccessor;
        private static readonly Func<CSharpSyntaxNode, SyntaxToken, CSharpSyntaxNode> WithOpenBracketTokenAccessor;
        private static readonly Func<CSharpSyntaxNode, SeparatedSyntaxListWrapper<FunctionPointerUnmanagedCallingConventionSyntaxWrapper>, CSharpSyntaxNode> WithCallingConventionsAccessor;
        private static readonly Func<CSharpSyntaxNode, SyntaxToken, CSharpSyntaxNode> WithCloseBracketTokenAccessor;

        private readonly CSharpSyntaxNode node;

        static FunctionPointerUnmanagedCallingConventionListSyntaxWrapper()
        {
            WrappedType = SyntaxWrapperHelper.GetWrappedType(typeof(FunctionPointerUnmanagedCallingConventionListSyntaxWrapper));
            OpenBracketTokenAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<CSharpSyntaxNode, SyntaxToken>(WrappedType, nameof(OpenBracketToken));
            CallingConventionsAccessor = LightupHelpers.CreateSeparatedSyntaxListPropertyAccessor<CSharpSyntaxNode, FunctionPointerUnmanagedCallingConventionSyntaxWrapper>(WrappedType, nameof(CallingConventions));
            CloseBracketTokenAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<CSharpSyntaxNode, SyntaxToken>(WrappedType, nameof(CloseBracketToken));
            WithOpenBracketTokenAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<CSharpSyntaxNode, SyntaxToken>(WrappedType, nameof(OpenBracketToken));
            WithCallingConventionsAccessor = LightupHelpers.CreateSeparatedSyntaxListWithPropertyAccessor<CSharpSyntaxNode, FunctionPointerUnmanagedCallingConventionSyntaxWrapper>(WrappedType, nameof(CallingConventions));
            WithCloseBracketTokenAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<CSharpSyntaxNode, SyntaxToken>(WrappedType, nameof(CloseBracketToken));
        }

        private FunctionPointerUnmanagedCallingConventionListSyntaxWrapper(CSharpSyntaxNode node)
        {
            this.node = node;
        }

        public CSharpSyntaxNode SyntaxNode => this.node;

        public SyntaxToken OpenBracketToken
        {
            get
            {
                return OpenBracketTokenAccessor(this.SyntaxNode);
            }
        }

        public SeparatedSyntaxListWrapper<FunctionPointerUnmanagedCallingConventionSyntaxWrapper> CallingConventions
        {
            get
            {
                return CallingConventionsAccessor(this.SyntaxNode);
            }
        }

        public SyntaxToken CloseBracketToken
        {
            get
            {
                return CloseBracketTokenAccessor(this.SyntaxNode);
            }
        }

        public static explicit operator FunctionPointerUnmanagedCallingConventionListSyntaxWrapper(SyntaxNode node)
        {
            if (node == null)
            {
                return default;
            }

            if (!IsInstance(node))
            {
                throw new InvalidCastException($"Cannot cast '{node.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new FunctionPointerUnmanagedCallingConventionListSyntaxWrapper((CSharpSyntaxNode)node);
        }

        public static implicit operator CSharpSyntaxNode(FunctionPointerUnmanagedCallingConventionListSyntaxWrapper wrapper)
        {
            return wrapper.node;
        }

        public static bool IsInstance(SyntaxNode node)
        {
            return node != null && LightupHelpers.CanWrapNode(node, WrappedType);
        }

        public FunctionPointerUnmanagedCallingConventionListSyntaxWrapper WithOpenBracketToken(SyntaxToken openBracketToken)
        {
            return new FunctionPointerUnmanagedCallingConventionListSyntaxWrapper(WithOpenBracketTokenAccessor(this.SyntaxNode, openBracketToken));
        }

        public FunctionPointerUnmanagedCallingConventionListSyntaxWrapper WithCallingConventions(SeparatedSyntaxListWrapper<FunctionPointerUnmanagedCallingConventionSyntaxWrapper> callingConventions)
        {
            return new FunctionPointerUnmanagedCallingConventionListSyntaxWrapper(WithCallingConventionsAccessor(this.SyntaxNode, callingConventions));
        }

        public FunctionPointerUnmanagedCallingConventionListSyntaxWrapper WithCloseBracketToken(SyntaxToken closeBracketToken)
        {
            return new FunctionPointerUnmanagedCallingConventionListSyntaxWrapper(WithCloseBracketTokenAccessor(this.SyntaxNode, closeBracketToken));
        }
    }
}
