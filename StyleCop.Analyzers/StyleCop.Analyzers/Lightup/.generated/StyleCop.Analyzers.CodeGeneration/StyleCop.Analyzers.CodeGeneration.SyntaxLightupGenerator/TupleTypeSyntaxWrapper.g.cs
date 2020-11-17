// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal readonly partial struct TupleTypeSyntaxWrapper : ISyntaxWrapper<TypeSyntax>
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.CSharp.Syntax.TupleTypeSyntax";
        private static readonly Type WrappedType;
        private readonly TypeSyntax node;
        private static readonly Func<TypeSyntax, SyntaxToken> OpenParenTokenAccessor;
        private static readonly Func<TypeSyntax, SeparatedSyntaxListWrapper<TupleElementSyntaxWrapper>> ElementsAccessor;
        private static readonly Func<TypeSyntax, SyntaxToken> CloseParenTokenAccessor;
        private static readonly Func<TypeSyntax, SyntaxToken, TypeSyntax> WithOpenParenTokenAccessor;
        private static readonly Func<TypeSyntax, SeparatedSyntaxListWrapper<TupleElementSyntaxWrapper>, TypeSyntax> WithElementsAccessor;
        private static readonly Func<TypeSyntax, SyntaxToken, TypeSyntax> WithCloseParenTokenAccessor;
        private TupleTypeSyntaxWrapper(TypeSyntax node)
        {
            this.node = node;
        }

        public TypeSyntax SyntaxNode => this.node;
        public static explicit operator TupleTypeSyntaxWrapper(SyntaxNode node)
        {
            if (node == null)
            {
                return default;
            }

            if (!IsInstance(node))
            {
                throw new InvalidCastException($"Cannot cast '{node.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new TupleTypeSyntaxWrapper((TypeSyntax)node);
        }

        public static implicit operator TypeSyntax(TupleTypeSyntaxWrapper wrapper)
        {
            return wrapper.node;
        }

        public static bool IsInstance(SyntaxNode node)
        {
            return node != null && LightupHelpers.CanWrapNode(node, WrappedType);
        }
    }
}
