// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal readonly partial struct FunctionPointerParameterSyntaxWrapper : ISyntaxWrapper<CSharpSyntaxNode>
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.CSharp.Syntax.FunctionPointerParameterSyntax";
        private static readonly Type WrappedType;
        private readonly CSharpSyntaxNode node;
        private static readonly Func<CSharpSyntaxNode, SyntaxList<AttributeListSyntax>, CSharpSyntaxNode> WithAttributeListsAccessor;
        private static readonly Func<CSharpSyntaxNode, SyntaxTokenList, CSharpSyntaxNode> WithModifiersAccessor;
        private static readonly Func<CSharpSyntaxNode, TypeSyntax, CSharpSyntaxNode> WithTypeAccessor;
        private FunctionPointerParameterSyntaxWrapper(CSharpSyntaxNode node)
        {
            this.node = node;
        }

        public CSharpSyntaxNode SyntaxNode => this.node;
        public static explicit operator FunctionPointerParameterSyntaxWrapper(SyntaxNode node)
        {
            if (node == null)
            {
                return default;
            }

            if (!IsInstance(node))
            {
                throw new InvalidCastException($"Cannot cast '{node.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new FunctionPointerParameterSyntaxWrapper((CSharpSyntaxNode)node);
        }

        public static implicit operator CSharpSyntaxNode(FunctionPointerParameterSyntaxWrapper wrapper)
        {
            return wrapper.node;
        }

        public static bool IsInstance(SyntaxNode node)
        {
            return node != null && LightupHelpers.CanWrapNode(node, WrappedType);
        }
    }
}
