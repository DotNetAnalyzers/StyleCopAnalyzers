// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal readonly partial struct BaseParameterSyntaxWrapper : ISyntaxWrapper<CSharpSyntaxNode>
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.CSharp.Syntax.BaseParameterSyntax";
        private static readonly Type WrappedType;

        private static readonly Func<CSharpSyntaxNode, SyntaxList<AttributeListSyntax>> AttributeListsAccessor;
        private static readonly Func<CSharpSyntaxNode, SyntaxTokenList> ModifiersAccessor;
        private static readonly Func<CSharpSyntaxNode, TypeSyntax> TypeAccessor;
        private static readonly Func<CSharpSyntaxNode, SyntaxList<AttributeListSyntax>, CSharpSyntaxNode> WithAttributeListsAccessor;
        private static readonly Func<CSharpSyntaxNode, SyntaxTokenList, CSharpSyntaxNode> WithModifiersAccessor;
        private static readonly Func<CSharpSyntaxNode, TypeSyntax, CSharpSyntaxNode> WithTypeAccessor;

        private readonly CSharpSyntaxNode node;

        static BaseParameterSyntaxWrapper()
        {
            WrappedType = SyntaxWrapperHelper.GetWrappedType(typeof(BaseParameterSyntaxWrapper));
            AttributeListsAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<CSharpSyntaxNode, SyntaxList<AttributeListSyntax>>(WrappedType, nameof(AttributeLists));
            ModifiersAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<CSharpSyntaxNode, SyntaxTokenList>(WrappedType, nameof(Modifiers));
            TypeAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<CSharpSyntaxNode, TypeSyntax>(WrappedType, nameof(Type));
            WithAttributeListsAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<CSharpSyntaxNode, SyntaxList<AttributeListSyntax>>(WrappedType, nameof(AttributeLists));
            WithModifiersAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<CSharpSyntaxNode, SyntaxTokenList>(WrappedType, nameof(Modifiers));
            WithTypeAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<CSharpSyntaxNode, TypeSyntax>(WrappedType, nameof(Type));
        }

        private BaseParameterSyntaxWrapper(CSharpSyntaxNode node)
        {
            this.node = node;
        }

        public CSharpSyntaxNode SyntaxNode => this.node;

        public SyntaxList<AttributeListSyntax> AttributeLists
        {
            get
            {
                return AttributeListsAccessor(this.SyntaxNode);
            }
        }

        public SyntaxTokenList Modifiers
        {
            get
            {
                return ModifiersAccessor(this.SyntaxNode);
            }
        }

        public TypeSyntax Type
        {
            get
            {
                return TypeAccessor(this.SyntaxNode);
            }
        }

        public static explicit operator BaseParameterSyntaxWrapper(SyntaxNode node)
        {
            if (node == null)
            {
                return default;
            }

            if (!IsInstance(node))
            {
                throw new InvalidCastException($"Cannot cast '{node.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new BaseParameterSyntaxWrapper((CSharpSyntaxNode)node);
        }

        public static implicit operator CSharpSyntaxNode(BaseParameterSyntaxWrapper wrapper)
        {
            return wrapper.node;
        }

        public static bool IsInstance(SyntaxNode node)
        {
            return node != null && LightupHelpers.CanWrapNode(node, WrappedType);
        }

        public BaseParameterSyntaxWrapper WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists)
        {
            return new BaseParameterSyntaxWrapper(WithAttributeListsAccessor(this.SyntaxNode, attributeLists));
        }

        public BaseParameterSyntaxWrapper WithModifiers(SyntaxTokenList modifiers)
        {
            return new BaseParameterSyntaxWrapper(WithModifiersAccessor(this.SyntaxNode, modifiers));
        }

        public BaseParameterSyntaxWrapper WithType(TypeSyntax type)
        {
            return new BaseParameterSyntaxWrapper(WithTypeAccessor(this.SyntaxNode, type));
        }

        internal static BaseParameterSyntaxWrapper FromUpcast(CSharpSyntaxNode node)
        {
            return new BaseParameterSyntaxWrapper(node);
        }
    }
}
