// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal readonly partial struct RefTypeSyntaxWrapper : ISyntaxWrapper<TypeSyntax>
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.CSharp.Syntax.RefTypeSyntax";
        private static readonly Type WrappedType;
        private readonly TypeSyntax node;
        private static readonly Func<TypeSyntax, SyntaxToken> RefKeywordAccessor;
        private static readonly Func<TypeSyntax, SyntaxToken> ReadOnlyKeywordAccessor;
        private static readonly Func<TypeSyntax, TypeSyntax> TypeAccessor;
        private static readonly Func<TypeSyntax, SyntaxToken, TypeSyntax> WithRefKeywordAccessor;
        private static readonly Func<TypeSyntax, SyntaxToken, TypeSyntax> WithReadOnlyKeywordAccessor;
        private static readonly Func<TypeSyntax, TypeSyntax, TypeSyntax> WithTypeAccessor;
        private RefTypeSyntaxWrapper(TypeSyntax node)
        {
            this.node = node;
        }

        public TypeSyntax SyntaxNode => this.node;
        public static explicit operator RefTypeSyntaxWrapper(SyntaxNode node)
        {
            if (node == null)
            {
                return default;
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
    }
}
