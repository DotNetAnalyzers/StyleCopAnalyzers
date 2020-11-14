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
        private TupleTypeSyntaxWrapper(TypeSyntax node)
        {
            this.node = node;
        }

        public TypeSyntax SyntaxNode => this.node;
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
