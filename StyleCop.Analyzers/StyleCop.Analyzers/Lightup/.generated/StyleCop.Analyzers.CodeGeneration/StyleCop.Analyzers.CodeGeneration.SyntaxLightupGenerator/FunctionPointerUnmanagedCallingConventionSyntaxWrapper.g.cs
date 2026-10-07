// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal readonly partial struct FunctionPointerUnmanagedCallingConventionSyntaxWrapper : ISyntaxWrapper<CSharpSyntaxNode>
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.CSharp.Syntax.FunctionPointerUnmanagedCallingConventionSyntax";
        private static readonly Type WrappedType;

        private static readonly Func<CSharpSyntaxNode, SyntaxToken> NameAccessor;
        private static readonly Func<CSharpSyntaxNode, SyntaxToken, CSharpSyntaxNode> WithNameAccessor;

        private readonly CSharpSyntaxNode node;

        static FunctionPointerUnmanagedCallingConventionSyntaxWrapper()
        {
            WrappedType = SyntaxWrapperHelper.GetWrappedType(typeof(FunctionPointerUnmanagedCallingConventionSyntaxWrapper));
            NameAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<CSharpSyntaxNode, SyntaxToken>(WrappedType, nameof(Name));
            WithNameAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<CSharpSyntaxNode, SyntaxToken>(WrappedType, nameof(Name));
        }

        private FunctionPointerUnmanagedCallingConventionSyntaxWrapper(CSharpSyntaxNode node)
        {
            this.node = node;
        }

        public CSharpSyntaxNode SyntaxNode => this.node;


        public SyntaxToken Name
        {
            get
            {
                return NameAccessor(this.SyntaxNode);
            }
        }

        public static explicit operator FunctionPointerUnmanagedCallingConventionSyntaxWrapper(SyntaxNode node)
        {
            if (node == null)
            {
                return default;
            }

            if (!IsInstance(node))
            {
                throw new InvalidCastException($"Cannot cast '{node.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new FunctionPointerUnmanagedCallingConventionSyntaxWrapper((CSharpSyntaxNode)node);
        }

        public static implicit operator CSharpSyntaxNode(FunctionPointerUnmanagedCallingConventionSyntaxWrapper wrapper)
        {
            return wrapper.node;
        }

        public static bool IsInstance(SyntaxNode node)
        {
            return node != null && LightupHelpers.CanWrapNode(node, WrappedType);
        }

        public FunctionPointerUnmanagedCallingConventionSyntaxWrapper WithName(SyntaxToken name)
        {
            return new FunctionPointerUnmanagedCallingConventionSyntaxWrapper(WithNameAccessor(this.SyntaxNode, name));
        }
    }
}
