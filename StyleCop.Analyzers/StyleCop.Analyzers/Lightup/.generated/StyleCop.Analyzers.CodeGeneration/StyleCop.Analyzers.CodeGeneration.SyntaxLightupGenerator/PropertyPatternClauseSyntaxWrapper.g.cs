// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal readonly partial struct PropertyPatternClauseSyntaxWrapper : ISyntaxWrapper<CSharpSyntaxNode>
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.CSharp.Syntax.PropertyPatternClauseSyntax";
        private static readonly Type WrappedType;
        private static readonly Func<CSharpSyntaxNode, SyntaxToken> OpenBraceTokenAccessor;
        private static readonly Func<CSharpSyntaxNode, SeparatedSyntaxListWrapper<SubpatternSyntaxWrapper>> SubpatternsAccessor;
        private static readonly Func<CSharpSyntaxNode, SyntaxToken> CloseBraceTokenAccessor;
        private static readonly Func<CSharpSyntaxNode, SyntaxToken, CSharpSyntaxNode> WithOpenBraceTokenAccessor;
        private static readonly Func<CSharpSyntaxNode, SeparatedSyntaxListWrapper<SubpatternSyntaxWrapper>, CSharpSyntaxNode> WithSubpatternsAccessor;
        private static readonly Func<CSharpSyntaxNode, SyntaxToken, CSharpSyntaxNode> WithCloseBraceTokenAccessor;
        private readonly CSharpSyntaxNode node;
        static PropertyPatternClauseSyntaxWrapper()
        {
            WrappedType = SyntaxWrapperHelper.GetWrappedType(typeof(PropertyPatternClauseSyntaxWrapper));
            OpenBraceTokenAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<CSharpSyntaxNode, SyntaxToken>(WrappedType, nameof(OpenBraceToken));
            SubpatternsAccessor = LightupHelpers.CreateSeparatedSyntaxListPropertyAccessor<CSharpSyntaxNode, SubpatternSyntaxWrapper>(WrappedType, nameof(Subpatterns));
            CloseBraceTokenAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<CSharpSyntaxNode, SyntaxToken>(WrappedType, nameof(CloseBraceToken));
            WithOpenBraceTokenAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<CSharpSyntaxNode, SyntaxToken>(WrappedType, nameof(OpenBraceToken));
            WithSubpatternsAccessor = LightupHelpers.CreateSeparatedSyntaxListWithPropertyAccessor<CSharpSyntaxNode, SubpatternSyntaxWrapper>(WrappedType, nameof(Subpatterns));
            WithCloseBraceTokenAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<CSharpSyntaxNode, SyntaxToken>(WrappedType, nameof(CloseBraceToken));
        }

        private PropertyPatternClauseSyntaxWrapper(CSharpSyntaxNode node)
        {
            this.node = node;
        }

        public CSharpSyntaxNode SyntaxNode => this.node;
        public static explicit operator PropertyPatternClauseSyntaxWrapper(SyntaxNode node)
        {
            if (node == null)
            {
                return default;
            }

            if (!IsInstance(node))
            {
                throw new InvalidCastException($"Cannot cast '{node.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new PropertyPatternClauseSyntaxWrapper((CSharpSyntaxNode)node);
        }

        public static implicit operator CSharpSyntaxNode(PropertyPatternClauseSyntaxWrapper wrapper)
        {
            return wrapper.node;
        }

        public static bool IsInstance(SyntaxNode node)
        {
            return node != null && LightupHelpers.CanWrapNode(node, WrappedType);
        }
    }
}
