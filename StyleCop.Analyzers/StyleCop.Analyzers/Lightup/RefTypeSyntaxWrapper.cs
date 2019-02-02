// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal struct RefTypeSyntaxWrapper : ISyntaxWrapper<TypeSyntax>
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.CSharp.Syntax.RefTypeSyntax";
        private static readonly Type WrappedType;

        private static readonly Func<TypeSyntax, SyntaxToken> RefKeywordAccessor;
        private static readonly Func<TypeSyntax, SyntaxToken> ReadOnlyKeywordAccessor;
        private static readonly Func<TypeSyntax, TypeSyntax> TypeAccessor;
        private static readonly Func<TypeSyntax, SyntaxToken, TypeSyntax> WithRefKeywordAccessor;
        private static readonly Func<TypeSyntax, SyntaxToken, TypeSyntax> WithReadOnlyKeywordAccessor;
        private static readonly Func<TypeSyntax, TypeSyntax, TypeSyntax> WithTypeAccessor;

        private readonly TypeSyntax node;

        static RefTypeSyntaxWrapper()
        {
            WrappedType = WrapperHelper.GetWrappedType(typeof(RefTypeSyntaxWrapper));
            RefKeywordAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<TypeSyntax, SyntaxToken>(WrappedType, nameof(RefKeyword));
            ReadOnlyKeywordAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<TypeSyntax, SyntaxToken>(WrappedType, nameof(ReadOnlyKeyword));
            TypeAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<TypeSyntax, TypeSyntax>(WrappedType, nameof(Type));
            WithRefKeywordAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<TypeSyntax, SyntaxToken>(WrappedType, nameof(RefKeyword));
            WithReadOnlyKeywordAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<TypeSyntax, SyntaxToken>(WrappedType, nameof(ReadOnlyKeyword));
            WithTypeAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<TypeSyntax, TypeSyntax>(WrappedType, nameof(Type));
        }

        private RefTypeSyntaxWrapper(TypeSyntax node)
        {
            this.node = node;
        }

        public TypeSyntax SyntaxNode => this.node;

        public SyntaxToken RefKeyword
        {
            get
            {
                return RefKeywordAccessor(this.SyntaxNode);
            }
        }

        public SyntaxToken ReadOnlyKeyword
        {
            get
            {
                return ReadOnlyKeywordAccessor(this.SyntaxNode);
            }
        }

        public TypeSyntax Type
        {
            get
            {
                return TypeAccessor(this.SyntaxNode);
            }
        }

        public static explicit operator RefTypeSyntaxWrapper(SyntaxNode node)
        {
            if (node == null)
            {
                return default(RefTypeSyntaxWrapper);
            }

            if (!IsInstance(node))
            {
                throw new InvalidCastException($"Cannot cast '{node.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new RefTypeSyntaxWrapper((TypeSyntax)node);
        }

        public static implicit operator TypeSyntax(RefTypeSyntaxWrapper wrapper)
        {
            return wrapper.node;
        }

        public static bool IsInstance(SyntaxNode node)
        {
            return node != null && LightupHelpers.CanWrapNode(node, WrappedType);
        }

        public RefTypeSyntaxWrapper WithRefKeyword(SyntaxToken refKeyword)
        {
            return new RefTypeSyntaxWrapper(WithRefKeywordAccessor(this.SyntaxNode, refKeyword));
        }

        public RefTypeSyntaxWrapper WithReadOnlyKeyword(SyntaxToken readOnlyKeyword)
        {
            return new RefTypeSyntaxWrapper(WithReadOnlyKeywordAccessor(this.SyntaxNode, readOnlyKeyword));
        }

        public RefTypeSyntaxWrapper WithType(TypeSyntax type)
        {
            return new RefTypeSyntaxWrapper(WithTypeAccessor(this.SyntaxNode, type));
        }
    }
}
