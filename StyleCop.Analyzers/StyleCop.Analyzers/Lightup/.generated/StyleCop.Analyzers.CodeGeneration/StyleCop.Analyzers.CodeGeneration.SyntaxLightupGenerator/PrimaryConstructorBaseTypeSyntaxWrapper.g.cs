// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal readonly partial struct PrimaryConstructorBaseTypeSyntaxWrapper : ISyntaxWrapper<BaseTypeSyntax>
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.CSharp.Syntax.PrimaryConstructorBaseTypeSyntax";
        private static readonly Type WrappedType;
        private static readonly Func<BaseTypeSyntax, ArgumentListSyntax> ArgumentListAccessor;
        private static readonly Func<BaseTypeSyntax, TypeSyntax, BaseTypeSyntax> WithTypeAccessor;
        private static readonly Func<BaseTypeSyntax, ArgumentListSyntax, BaseTypeSyntax> WithArgumentListAccessor;
        private readonly BaseTypeSyntax node;
        static PrimaryConstructorBaseTypeSyntaxWrapper()
        {
            WrappedType = SyntaxWrapperHelper.GetWrappedType(typeof(PrimaryConstructorBaseTypeSyntaxWrapper));
            ArgumentListAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<BaseTypeSyntax, ArgumentListSyntax>(WrappedType, nameof(ArgumentList));
            WithTypeAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<BaseTypeSyntax, TypeSyntax>(WrappedType, nameof(Type));
            WithArgumentListAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<BaseTypeSyntax, ArgumentListSyntax>(WrappedType, nameof(ArgumentList));
        }

        private PrimaryConstructorBaseTypeSyntaxWrapper(BaseTypeSyntax node)
        {
            this.node = node;
        }

        public BaseTypeSyntax SyntaxNode => this.node;
        public TypeSyntax Type
        {
            get
            {
                return this.SyntaxNode.Type;
            }
        }

        public ArgumentListSyntax ArgumentList
        {
            get
            {
                return ArgumentListAccessor(this.SyntaxNode);
            }
        }

        public static explicit operator PrimaryConstructorBaseTypeSyntaxWrapper(SyntaxNode node)
        {
            if (node == null)
            {
                return default;
            }

            if (!IsInstance(node))
            {
                throw new InvalidCastException($"Cannot cast '{node.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new PrimaryConstructorBaseTypeSyntaxWrapper((BaseTypeSyntax)node);
        }

        public static implicit operator BaseTypeSyntax(PrimaryConstructorBaseTypeSyntaxWrapper wrapper)
        {
            return wrapper.node;
        }

        public static bool IsInstance(SyntaxNode node)
        {
            return node != null && LightupHelpers.CanWrapNode(node, WrappedType);
        }
    }
}
