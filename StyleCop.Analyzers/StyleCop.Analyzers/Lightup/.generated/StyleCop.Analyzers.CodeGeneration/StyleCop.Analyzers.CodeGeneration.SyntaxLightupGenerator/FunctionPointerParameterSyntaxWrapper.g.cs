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
        private static readonly Func<CSharpSyntaxNode, SyntaxList<AttributeListSyntax>, CSharpSyntaxNode> WithAttributeListsAccessor;
        private static readonly Func<CSharpSyntaxNode, SyntaxTokenList, CSharpSyntaxNode> WithModifiersAccessor;
        private static readonly Func<CSharpSyntaxNode, TypeSyntax, CSharpSyntaxNode> WithTypeAccessor;
        private readonly CSharpSyntaxNode node;
        static FunctionPointerParameterSyntaxWrapper()
        {
            WrappedType = SyntaxWrapperHelper.GetWrappedType(typeof(FunctionPointerParameterSyntaxWrapper));
            WithAttributeListsAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<CSharpSyntaxNode, SyntaxList<AttributeListSyntax>>(WrappedType, nameof(BaseParameterSyntaxWrapper.AttributeLists));
            WithModifiersAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<CSharpSyntaxNode, SyntaxTokenList>(WrappedType, nameof(BaseParameterSyntaxWrapper.Modifiers));
            WithTypeAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<CSharpSyntaxNode, TypeSyntax>(WrappedType, nameof(BaseParameterSyntaxWrapper.Type));
        }

        private FunctionPointerParameterSyntaxWrapper(CSharpSyntaxNode node)
        {
            this.node = node;
        }

        public CSharpSyntaxNode SyntaxNode => this.node;
        public SyntaxList<AttributeListSyntax> AttributeLists
        {
            get
            {
                return ((BaseParameterSyntaxWrapper)this).AttributeLists;
            }
        }

        public SyntaxTokenList Modifiers
        {
            get
            {
                return ((BaseParameterSyntaxWrapper)this).Modifiers;
            }
        }

        public TypeSyntax Type
        {
            get
            {
                return ((BaseParameterSyntaxWrapper)this).Type;
            }
        }

        public static explicit operator FunctionPointerParameterSyntaxWrapper(BaseParameterSyntaxWrapper node)
        {
            return (FunctionPointerParameterSyntaxWrapper)node.SyntaxNode;
        }

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

        public static implicit operator BaseParameterSyntaxWrapper(FunctionPointerParameterSyntaxWrapper wrapper)
        {
            return BaseParameterSyntaxWrapper.FromUpcast(wrapper.node);
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
