// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Reflection;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal struct TupleElementSyntaxWrapper : ISyntaxWrapper<CSharpSyntaxNode>
    {
        private const string TupleElementSyntaxTypeName = "Microsoft.CodeAnalysis.CSharp.Syntax.TupleElementSyntax";
        private static readonly Type TupleElementSyntaxType;

        private static readonly Func<CSharpSyntaxNode, SyntaxToken> IdentifierAccessor;
        private static readonly Func<CSharpSyntaxNode, TypeSyntax> TypeAccessor;
        private static readonly Func<CSharpSyntaxNode, SyntaxToken, CSharpSyntaxNode> WithIdentifierAccessor;
        private static readonly Func<CSharpSyntaxNode, TypeSyntax, CSharpSyntaxNode> WithTypeAccessor;

        private readonly CSharpSyntaxNode node;

        static TupleElementSyntaxWrapper()
        {
            TupleElementSyntaxType = typeof(CSharpSyntaxNode).GetTypeInfo().Assembly.GetType(TupleElementSyntaxTypeName);
            IdentifierAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<CSharpSyntaxNode, SyntaxToken>(TupleElementSyntaxType, nameof(Identifier));
            TypeAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<CSharpSyntaxNode, TypeSyntax>(TupleElementSyntaxType, nameof(Type));
            WithIdentifierAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<CSharpSyntaxNode, SyntaxToken>(TupleElementSyntaxType, nameof(Identifier));
            WithTypeAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<CSharpSyntaxNode, TypeSyntax>(TupleElementSyntaxType, nameof(Type));
        }

        private TupleElementSyntaxWrapper(CSharpSyntaxNode node)
        {
            this.node = node;
        }

        public CSharpSyntaxNode SyntaxNode => this.node;

        public SyntaxToken Identifier
        {
            get
            {
                return IdentifierAccessor(this.SyntaxNode);
            }
        }

        public TypeSyntax Type
        {
            get
            {
                return TypeAccessor(this.SyntaxNode);
            }
        }

        public static explicit operator TupleElementSyntaxWrapper(SyntaxNode node)
        {
            if (node == null)
            {
                return default(TupleElementSyntaxWrapper);
            }

            if (!IsInstance(node))
            {
                throw new InvalidCastException($"Cannot cast '{node.GetType().FullName}' to '{TupleElementSyntaxTypeName}'");
            }

            return new TupleElementSyntaxWrapper((CSharpSyntaxNode)node);
        }

        public static implicit operator CSharpSyntaxNode(TupleElementSyntaxWrapper wrapper)
        {
            return wrapper.node;
        }

        public static bool IsInstance(SyntaxNode node)
        {
            return node != null && LightupHelpers.CanWrapNode(node, TupleElementSyntaxType);
        }

        public TupleElementSyntaxWrapper WithIdentifier(SyntaxToken identifier)
        {
            return new TupleElementSyntaxWrapper(WithIdentifierAccessor(this.SyntaxNode, identifier));
        }

        public TupleElementSyntaxWrapper WithType(TypeSyntax type)
        {
            return new TupleElementSyntaxWrapper(WithTypeAccessor(this.SyntaxNode, type));
        }
    }
}
