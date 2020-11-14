// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal partial struct ImplicitStackAllocArrayCreationExpressionSyntaxWrapper : ISyntaxWrapper<ExpressionSyntax>
    {
        private static readonly Func<ExpressionSyntax, SyntaxToken> StackAllocKeywordAccessor;
        private static readonly Func<ExpressionSyntax, SyntaxToken> OpenBracketTokenAccessor;
        private static readonly Func<ExpressionSyntax, SyntaxToken> CloseBracketTokenAccessor;
        private static readonly Func<ExpressionSyntax, InitializerExpressionSyntax> InitializerAccessor;
        private static readonly Func<ExpressionSyntax, SyntaxToken, ExpressionSyntax> WithStackAllocKeywordAccessor;
        private static readonly Func<ExpressionSyntax, SyntaxToken, ExpressionSyntax> WithOpenBracketTokenAccessor;
        private static readonly Func<ExpressionSyntax, SyntaxToken, ExpressionSyntax> WithCloseBracketTokenAccessor;
        private static readonly Func<ExpressionSyntax, InitializerExpressionSyntax, ExpressionSyntax> WithInitializerAccessor;

        static ImplicitStackAllocArrayCreationExpressionSyntaxWrapper()
        {
            WrappedType = WrapperHelper.GetWrappedType(typeof(ImplicitStackAllocArrayCreationExpressionSyntaxWrapper));
            StackAllocKeywordAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<ExpressionSyntax, SyntaxToken>(WrappedType, nameof(StackAllocKeyword));
            OpenBracketTokenAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<ExpressionSyntax, SyntaxToken>(WrappedType, nameof(OpenBracketToken));
            CloseBracketTokenAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<ExpressionSyntax, SyntaxToken>(WrappedType, nameof(CloseBracketToken));
            InitializerAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<ExpressionSyntax, InitializerExpressionSyntax>(WrappedType, nameof(Initializer));
            WithStackAllocKeywordAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<ExpressionSyntax, SyntaxToken>(WrappedType, nameof(StackAllocKeyword));
            WithOpenBracketTokenAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<ExpressionSyntax, SyntaxToken>(WrappedType, nameof(OpenBracketToken));
            WithCloseBracketTokenAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<ExpressionSyntax, SyntaxToken>(WrappedType, nameof(CloseBracketToken));
            WithInitializerAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<ExpressionSyntax, InitializerExpressionSyntax>(WrappedType, nameof(Initializer));
        }

        public SyntaxToken StackAllocKeyword
        {
            get
            {
                return StackAllocKeywordAccessor(this.SyntaxNode);
            }
        }

        public SyntaxToken OpenBracketToken
        {
            get
            {
                return OpenBracketTokenAccessor(this.SyntaxNode);
            }
        }

        public SyntaxToken CloseBracketToken
        {
            get
            {
                return CloseBracketTokenAccessor(this.SyntaxNode);
            }
        }

        public InitializerExpressionSyntax Initializer
        {
            get
            {
                return InitializerAccessor(this.SyntaxNode);
            }
        }

        public static explicit operator ImplicitStackAllocArrayCreationExpressionSyntaxWrapper(SyntaxNode node)
        {
            if (node == null)
            {
                return default;
            }

            if (!IsInstance(node))
            {
                throw new InvalidCastException($"Cannot cast '{node.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new ImplicitStackAllocArrayCreationExpressionSyntaxWrapper((ExpressionSyntax)node);
        }

        public static implicit operator ExpressionSyntax(ImplicitStackAllocArrayCreationExpressionSyntaxWrapper wrapper)
        {
            return wrapper.node;
        }

        public static bool IsInstance(SyntaxNode node)
        {
            return node != null && LightupHelpers.CanWrapNode(node, WrappedType);
        }

        public ImplicitStackAllocArrayCreationExpressionSyntaxWrapper WithStackAllocKeyword(SyntaxToken stackAllocKeyword)
        {
            return new ImplicitStackAllocArrayCreationExpressionSyntaxWrapper(WithStackAllocKeywordAccessor(this.SyntaxNode, stackAllocKeyword));
        }

        public ImplicitStackAllocArrayCreationExpressionSyntaxWrapper WithOpenBracketToken(SyntaxToken openBracketToken)
        {
            return new ImplicitStackAllocArrayCreationExpressionSyntaxWrapper(WithOpenBracketTokenAccessor(this.SyntaxNode, openBracketToken));
        }

        public ImplicitStackAllocArrayCreationExpressionSyntaxWrapper WithCloseBracketToken(SyntaxToken closeBracketToken)
        {
            return new ImplicitStackAllocArrayCreationExpressionSyntaxWrapper(WithCloseBracketTokenAccessor(this.SyntaxNode, closeBracketToken));
        }

        public ImplicitStackAllocArrayCreationExpressionSyntaxWrapper WithInitializer(InitializerExpressionSyntax initializer)
        {
            return new ImplicitStackAllocArrayCreationExpressionSyntaxWrapper(WithInitializerAccessor(this.SyntaxNode, initializer));
        }

        public ImplicitStackAllocArrayCreationExpressionSyntaxWrapper AddInitializerExpressions(params ExpressionSyntax[] items)
        {
            return new ImplicitStackAllocArrayCreationExpressionSyntaxWrapper(this.WithInitializer(this.Initializer.AddExpressions(items)));
        }
    }
}
