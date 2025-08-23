// Copyright (c) Contributors to the New StyleCop Analyzers project.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal readonly partial struct ScopedTypeSyntaxWrapper : ISyntaxWrapper<TypeSyntax>
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.CSharp.Syntax.ScopedTypeSyntax";
        private static readonly Type WrappedType;

        private static readonly Func<TypeSyntax, SyntaxToken> ScopedKeywordAccessor;
        private static readonly Func<TypeSyntax, TypeSyntax> TypeAccessor;
        private static readonly Func<TypeSyntax, SyntaxToken, TypeSyntax> WithScopedKeywordAccessor;
        private static readonly Func<TypeSyntax, TypeSyntax, TypeSyntax> WithTypeAccessor;

        private readonly TypeSyntax node;

        static ScopedTypeSyntaxWrapper()
        {
            WrappedType = SyntaxWrapperHelper.GetWrappedType(typeof(ScopedTypeSyntaxWrapper));
            ScopedKeywordAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<TypeSyntax, SyntaxToken>(WrappedType, nameof(ScopedKeyword));
            TypeAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<TypeSyntax, TypeSyntax>(WrappedType, nameof(Type));
            WithScopedKeywordAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<TypeSyntax, SyntaxToken>(WrappedType, nameof(ScopedKeyword));
            WithTypeAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<TypeSyntax, TypeSyntax>(WrappedType, nameof(Type));
        }

        private ScopedTypeSyntaxWrapper(TypeSyntax node)
        {
            this.node = node;
        }

        public TypeSyntax SyntaxNode => this.node;

        public SyntaxToken ScopedKeyword
        {
            get
            {
                return ScopedKeywordAccessor(this.SyntaxNode);
            }
        }

        public TypeSyntax Type
        {
            get
            {
                return TypeAccessor(this.SyntaxNode);
            }
        }

        public static explicit operator ScopedTypeSyntaxWrapper(SyntaxNode node)
        {
            if (node == null)
            {
                return default;
            }

            if (!IsInstance(node))
            {
                throw new InvalidCastException($"Cannot cast '{node.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new ScopedTypeSyntaxWrapper((TypeSyntax)node);
        }

        public static implicit operator TypeSyntax(ScopedTypeSyntaxWrapper wrapper)
        {
            return wrapper.node;
        }

        public static bool IsInstance(SyntaxNode node)
        {
            return node != null && LightupHelpers.CanWrapNode(node, WrappedType);
        }

        public ScopedTypeSyntaxWrapper WithScopedKeyword(SyntaxToken scopedKeyword)
        {
            return new ScopedTypeSyntaxWrapper(WithScopedKeywordAccessor(this.SyntaxNode, scopedKeyword));
        }

        public ScopedTypeSyntaxWrapper WithType(TypeSyntax type)
        {
            return new ScopedTypeSyntaxWrapper(WithTypeAccessor(this.SyntaxNode, type));
        }
    }
}
