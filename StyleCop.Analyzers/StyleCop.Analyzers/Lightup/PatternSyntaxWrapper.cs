// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;

    internal struct PatternSyntaxWrapper : ISyntaxWrapper<CSharpSyntaxNode>
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.CSharp.Syntax.PatternSyntax";
        private static readonly Type WrappedType;

        private readonly CSharpSyntaxNode node;

        static PatternSyntaxWrapper()
        {
            WrappedType = WrapperHelper.GetWrappedType(typeof(PatternSyntaxWrapper));
        }

        private PatternSyntaxWrapper(CSharpSyntaxNode node)
        {
            this.node = node;
        }

        public CSharpSyntaxNode SyntaxNode => this.node;

        public static explicit operator PatternSyntaxWrapper(SyntaxNode node)
        {
            if (node == null)
            {
                return default(PatternSyntaxWrapper);
            }

            if (!IsInstance(node))
            {
                throw new InvalidCastException($"Cannot cast '{node.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new PatternSyntaxWrapper((CSharpSyntaxNode)node);
        }

        public static implicit operator CSharpSyntaxNode(PatternSyntaxWrapper wrapper)
        {
            return wrapper.node;
        }

        public static bool IsInstance(SyntaxNode node)
        {
            return node != null && LightupHelpers.CanWrapNode(node, WrappedType);
        }

        internal static PatternSyntaxWrapper FromUpcast(CSharpSyntaxNode node)
        {
            return new PatternSyntaxWrapper(node);
        }
    }
}
