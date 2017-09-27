// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Reflection;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;

    internal struct PatternSyntaxWrapper : ISyntaxWrapper<CSharpSyntaxNode>
    {
        private const string PatternSyntaxTypeName = "Microsoft.CodeAnalysis.CSharp.Syntax.PatternSyntax";

        private readonly CSharpSyntaxNode node;

        static PatternSyntaxWrapper()
        {
            WrappedType = typeof(CSharpSyntaxNode).GetTypeInfo().Assembly.GetType(PatternSyntaxTypeName);
        }

        private PatternSyntaxWrapper(CSharpSyntaxNode node)
        {
            this.node = node;
        }

        public static Type WrappedType { get; private set; }

        public CSharpSyntaxNode SyntaxNode => this.node;

        public static explicit operator PatternSyntaxWrapper(SyntaxNode node)
        {
            if (node == null)
            {
                return default(PatternSyntaxWrapper);
            }

            if (!IsInstance(node))
            {
                throw new InvalidCastException($"Cannot cast '{node.GetType().FullName}' to '{PatternSyntaxTypeName}'");
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
