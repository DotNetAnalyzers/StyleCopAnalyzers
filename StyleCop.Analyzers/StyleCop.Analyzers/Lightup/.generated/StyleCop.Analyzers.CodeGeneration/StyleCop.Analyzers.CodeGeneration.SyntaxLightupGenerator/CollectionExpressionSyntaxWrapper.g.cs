// Copyright (c) Contributors to the New StyleCop Analyzers project.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal readonly partial struct CollectionExpressionSyntaxWrapper : ISyntaxWrapper<ExpressionSyntax>
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.CSharp.Syntax.CollectionExpressionSyntax";
        private static readonly Type WrappedType;

        private static readonly Func<ExpressionSyntax, SyntaxToken> OpenBracketTokenAccessor;
        private static readonly Func<ExpressionSyntax, SeparatedSyntaxListWrapper<CollectionElementSyntaxWrapper>> ElementsAccessor;
        private static readonly Func<ExpressionSyntax, SyntaxToken> CloseBracketTokenAccessor;
        private static readonly Func<ExpressionSyntax, SyntaxToken, ExpressionSyntax> WithOpenBracketTokenAccessor;
        private static readonly Func<ExpressionSyntax, SeparatedSyntaxListWrapper<CollectionElementSyntaxWrapper>, ExpressionSyntax> WithElementsAccessor;
        private static readonly Func<ExpressionSyntax, SyntaxToken, ExpressionSyntax> WithCloseBracketTokenAccessor;

        private readonly ExpressionSyntax node;

        static CollectionExpressionSyntaxWrapper()
        {
            WrappedType = SyntaxWrapperHelper.GetWrappedType(typeof(CollectionExpressionSyntaxWrapper));
            OpenBracketTokenAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<ExpressionSyntax, SyntaxToken>(WrappedType, nameof(OpenBracketToken));
            ElementsAccessor = LightupHelpers.CreateSeparatedSyntaxListPropertyAccessor<ExpressionSyntax, CollectionElementSyntaxWrapper>(WrappedType, nameof(Elements));
            CloseBracketTokenAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<ExpressionSyntax, SyntaxToken>(WrappedType, nameof(CloseBracketToken));
            WithOpenBracketTokenAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<ExpressionSyntax, SyntaxToken>(WrappedType, nameof(OpenBracketToken));
            WithElementsAccessor = LightupHelpers.CreateSeparatedSyntaxListWithPropertyAccessor<ExpressionSyntax, CollectionElementSyntaxWrapper>(WrappedType, nameof(Elements));
            WithCloseBracketTokenAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<ExpressionSyntax, SyntaxToken>(WrappedType, nameof(CloseBracketToken));
        }

        private CollectionExpressionSyntaxWrapper(ExpressionSyntax node)
        {
            this.node = node;
        }

        public ExpressionSyntax SyntaxNode => this.node;

        public SyntaxToken OpenBracketToken
        {
            get
            {
                return OpenBracketTokenAccessor(this.SyntaxNode);
            }
        }

        public SeparatedSyntaxListWrapper<CollectionElementSyntaxWrapper> Elements
        {
            get
            {
                return ElementsAccessor(this.SyntaxNode);
            }
        }

        public SyntaxToken CloseBracketToken
        {
            get
            {
                return CloseBracketTokenAccessor(this.SyntaxNode);
            }
        }

        public static explicit operator CollectionExpressionSyntaxWrapper(SyntaxNode node)
        {
            if (node == null)
            {
                return default;
            }

            if (!IsInstance(node))
            {
                throw new InvalidCastException($"Cannot cast '{node.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new CollectionExpressionSyntaxWrapper((ExpressionSyntax)node);
        }

        public static implicit operator ExpressionSyntax(CollectionExpressionSyntaxWrapper wrapper)
        {
            return wrapper.node;
        }

        public static bool IsInstance(SyntaxNode node)
        {
            return node != null && LightupHelpers.CanWrapNode(node, WrappedType);
        }

        public CollectionExpressionSyntaxWrapper WithOpenBracketToken(SyntaxToken openBracketToken)
        {
            return new CollectionExpressionSyntaxWrapper(WithOpenBracketTokenAccessor(this.SyntaxNode, openBracketToken));
        }

        public CollectionExpressionSyntaxWrapper WithElements(SeparatedSyntaxListWrapper<CollectionElementSyntaxWrapper> elements)
        {
            return new CollectionExpressionSyntaxWrapper(WithElementsAccessor(this.SyntaxNode, elements));
        }

        public CollectionExpressionSyntaxWrapper WithCloseBracketToken(SyntaxToken closeBracketToken)
        {
            return new CollectionExpressionSyntaxWrapper(WithCloseBracketTokenAccessor(this.SyntaxNode, closeBracketToken));
        }
    }
}
