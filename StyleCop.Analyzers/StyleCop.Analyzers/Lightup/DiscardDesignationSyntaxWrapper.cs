﻿// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;

    internal struct DiscardDesignationSyntaxWrapper : ISyntaxWrapper<CSharpSyntaxNode>
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.CSharp.Syntax.DiscardDesignationSyntax";
        private static readonly Type WrappedType;

        private static readonly Func<CSharpSyntaxNode, SyntaxToken> UnderscoreTokenAccessor;
        private static readonly Func<CSharpSyntaxNode, SyntaxToken, CSharpSyntaxNode> WithUnderscoreTokenAccessor;

        private readonly CSharpSyntaxNode node;

        static DiscardDesignationSyntaxWrapper()
        {
            WrappedType = WrapperHelper.GetWrappedType(typeof(DiscardDesignationSyntaxWrapper));
            UnderscoreTokenAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<CSharpSyntaxNode, SyntaxToken>(WrappedType, nameof(UnderscoreToken));
            WithUnderscoreTokenAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<CSharpSyntaxNode, SyntaxToken>(WrappedType, nameof(UnderscoreToken));
        }

        private DiscardDesignationSyntaxWrapper(CSharpSyntaxNode node)
        {
            this.node = node;
        }

        public CSharpSyntaxNode SyntaxNode => this.node;

        public SyntaxToken UnderscoreToken
        {
            get
            {
                return UnderscoreTokenAccessor(this.SyntaxNode);
            }
        }

        public static explicit operator DiscardDesignationSyntaxWrapper(VariableDesignationSyntaxWrapper node)
        {
            return (DiscardDesignationSyntaxWrapper)node.SyntaxNode;
        }

        public static explicit operator DiscardDesignationSyntaxWrapper(SyntaxNode node)
        {
            if (node == null)
            {
                return default;
            }

            if (!IsInstance(node))
            {
                throw new InvalidCastException($"Cannot cast '{node.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new DiscardDesignationSyntaxWrapper((CSharpSyntaxNode)node);
        }

        public static implicit operator VariableDesignationSyntaxWrapper(DiscardDesignationSyntaxWrapper wrapper)
        {
            return VariableDesignationSyntaxWrapper.FromUpcast(wrapper.node);
        }

        public static implicit operator CSharpSyntaxNode(DiscardDesignationSyntaxWrapper wrapper)
        {
            return wrapper.node;
        }

        public static bool IsInstance(SyntaxNode node)
        {
            return node != null && LightupHelpers.CanWrapNode(node, WrappedType);
        }

        public DiscardDesignationSyntaxWrapper WithUnderscoreToken(SyntaxToken identifier)
        {
            return new DiscardDesignationSyntaxWrapper(WithUnderscoreTokenAccessor(this.SyntaxNode, identifier));
        }
    }
}
