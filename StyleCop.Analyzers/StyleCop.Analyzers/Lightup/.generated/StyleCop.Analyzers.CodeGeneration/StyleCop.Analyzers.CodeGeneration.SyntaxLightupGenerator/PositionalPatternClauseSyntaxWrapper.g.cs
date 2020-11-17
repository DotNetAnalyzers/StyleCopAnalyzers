// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal readonly partial struct PositionalPatternClauseSyntaxWrapper : ISyntaxWrapper<CSharpSyntaxNode>
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.CSharp.Syntax.PositionalPatternClauseSyntax";
        private static readonly Type WrappedType;
        private readonly CSharpSyntaxNode node;
        private static readonly Func<CSharpSyntaxNode, SyntaxToken> OpenParenTokenAccessor;
        private static readonly Func<CSharpSyntaxNode, SeparatedSyntaxListWrapper<SubpatternSyntaxWrapper>> SubpatternsAccessor;
        private static readonly Func<CSharpSyntaxNode, SyntaxToken> CloseParenTokenAccessor;
        private static readonly Func<CSharpSyntaxNode, SyntaxToken, CSharpSyntaxNode> WithOpenParenTokenAccessor;
        private static readonly Func<CSharpSyntaxNode, SeparatedSyntaxListWrapper<SubpatternSyntaxWrapper>, CSharpSyntaxNode> WithSubpatternsAccessor;
        private static readonly Func<CSharpSyntaxNode, SyntaxToken, CSharpSyntaxNode> WithCloseParenTokenAccessor;
        private PositionalPatternClauseSyntaxWrapper(CSharpSyntaxNode node)
        {
            this.node = node;
        }

        public CSharpSyntaxNode SyntaxNode => this.node;
        public static explicit operator PositionalPatternClauseSyntaxWrapper(SyntaxNode node)
        {
            if (node == null)
            {
                return default;
            }

            if (!IsInstance(node))
            {
                throw new InvalidCastException($"Cannot cast '{node.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new PositionalPatternClauseSyntaxWrapper((CSharpSyntaxNode)node);
        }

        public static implicit operator CSharpSyntaxNode(PositionalPatternClauseSyntaxWrapper wrapper)
        {
            return wrapper.node;
        }

        public static bool IsInstance(SyntaxNode node)
        {
            return node != null && LightupHelpers.CanWrapNode(node, WrappedType);
        }
    }
}
