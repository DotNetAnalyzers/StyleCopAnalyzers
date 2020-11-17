// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal readonly partial struct DefaultConstraintSyntaxWrapper : ISyntaxWrapper<TypeParameterConstraintSyntax>
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.CSharp.Syntax.DefaultConstraintSyntax";
        private static readonly Type WrappedType;
        private readonly TypeParameterConstraintSyntax node;
        private static readonly Func<TypeParameterConstraintSyntax, SyntaxToken> DefaultKeywordAccessor;
        private static readonly Func<TypeParameterConstraintSyntax, SyntaxToken, TypeParameterConstraintSyntax> WithDefaultKeywordAccessor;
        private DefaultConstraintSyntaxWrapper(TypeParameterConstraintSyntax node)
        {
            this.node = node;
        }

        public TypeParameterConstraintSyntax SyntaxNode => this.node;
        public static explicit operator DefaultConstraintSyntaxWrapper(SyntaxNode node)
        {
            if (node == null)
            {
                return default;
            }

            if (!IsInstance(node))
            {
                throw new InvalidCastException($"Cannot cast '{node.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new DefaultConstraintSyntaxWrapper((TypeParameterConstraintSyntax)node);
        }

        public static implicit operator TypeParameterConstraintSyntax(DefaultConstraintSyntaxWrapper wrapper)
        {
            return wrapper.node;
        }

        public static bool IsInstance(SyntaxNode node)
        {
            return node != null && LightupHelpers.CanWrapNode(node, WrappedType);
        }
    }
}
