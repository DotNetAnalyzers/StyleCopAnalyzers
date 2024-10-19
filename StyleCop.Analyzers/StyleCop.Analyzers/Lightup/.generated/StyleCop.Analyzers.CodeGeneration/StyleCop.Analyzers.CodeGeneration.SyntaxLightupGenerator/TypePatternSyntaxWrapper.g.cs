﻿// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal readonly partial struct TypePatternSyntaxWrapper : ISyntaxWrapper<CSharpSyntaxNode>
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.CSharp.Syntax.TypePatternSyntax";
        private static readonly Type WrappedType;

        private static readonly Func<CSharpSyntaxNode, TypeSyntax> TypeAccessor;
        private static readonly Func<CSharpSyntaxNode, TypeSyntax, CSharpSyntaxNode> WithTypeAccessor;

        private readonly CSharpSyntaxNode node;

        static TypePatternSyntaxWrapper()
        {
            WrappedType = SyntaxWrapperHelper.GetWrappedType(typeof(TypePatternSyntaxWrapper));
            TypeAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<CSharpSyntaxNode, TypeSyntax>(WrappedType, nameof(Type));
            WithTypeAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<CSharpSyntaxNode, TypeSyntax>(WrappedType, nameof(Type));
        }

        private TypePatternSyntaxWrapper(CSharpSyntaxNode node)
        {
            this.node = node;
        }

        public CSharpSyntaxNode SyntaxNode => this.node;

        public TypeSyntax Type
        {
            get
            {
                return TypeAccessor(this.SyntaxNode);
            }
        }

        public static explicit operator TypePatternSyntaxWrapper(PatternSyntaxWrapper node)
        {
            return (TypePatternSyntaxWrapper)node.SyntaxNode;
        }

        public static explicit operator TypePatternSyntaxWrapper(ExpressionOrPatternSyntaxWrapper node)
        {
            return (TypePatternSyntaxWrapper)node.SyntaxNode;
        }

        public static explicit operator TypePatternSyntaxWrapper(SyntaxNode node)
        {
            if (node == null)
            {
                return default;
            }

            if (!IsInstance(node))
            {
                throw new InvalidCastException($"Cannot cast '{node.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new TypePatternSyntaxWrapper((CSharpSyntaxNode)node);
        }

        public static implicit operator PatternSyntaxWrapper(TypePatternSyntaxWrapper wrapper)
        {
            return PatternSyntaxWrapper.FromUpcast(wrapper.node);
        }

        public static implicit operator ExpressionOrPatternSyntaxWrapper(TypePatternSyntaxWrapper wrapper)
        {
            return ExpressionOrPatternSyntaxWrapper.FromUpcast(wrapper.node);
        }

        public static implicit operator CSharpSyntaxNode(TypePatternSyntaxWrapper wrapper)
        {
            return wrapper.node;
        }

        public static bool IsInstance(SyntaxNode node)
        {
            return node != null && LightupHelpers.CanWrapNode(node, WrappedType);
        }

        public TypePatternSyntaxWrapper WithType(TypeSyntax type)
        {
            return new TypePatternSyntaxWrapper(WithTypeAccessor(this.SyntaxNode, type));
        }
    }
}
