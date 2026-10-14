// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal readonly partial struct ImplicitStackAllocArrayCreationExpressionSyntaxWrapper : ISyntaxWrapper<ExpressionSyntax>
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.CSharp.Syntax.ImplicitStackAllocArrayCreationExpressionSyntax";
        private static readonly Type? WrappedType;

        private static readonly Func<ExpressionSyntax, SyntaxToken> StackAllocKeywordAccessor;
        private static readonly Func<ExpressionSyntax, SyntaxToken> OpenBracketTokenAccessor;
        private static readonly Func<ExpressionSyntax, SyntaxToken> CloseBracketTokenAccessor;
        private static readonly Func<ExpressionSyntax, InitializerExpressionSyntax> InitializerAccessor;
        private static readonly Func<ExpressionSyntax, SyntaxToken, ExpressionSyntax> WithStackAllocKeywordAccessor;
        private static readonly Func<ExpressionSyntax, SyntaxToken, ExpressionSyntax> WithOpenBracketTokenAccessor;
        private static readonly Func<ExpressionSyntax, SyntaxToken, ExpressionSyntax> WithCloseBracketTokenAccessor;
        private static readonly Func<ExpressionSyntax, InitializerExpressionSyntax, ExpressionSyntax> WithInitializerAccessor;

        private readonly ExpressionSyntax? node;

        static ImplicitStackAllocArrayCreationExpressionSyntaxWrapper()
        {
            WrappedType = SyntaxWrapperHelper.GetWrappedType(typeof(ImplicitStackAllocArrayCreationExpressionSyntaxWrapper));
            StackAllocKeywordAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<ExpressionSyntax, SyntaxToken>(WrappedType, nameof(StackAllocKeyword));
            OpenBracketTokenAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<ExpressionSyntax, SyntaxToken>(WrappedType, nameof(OpenBracketToken));
            CloseBracketTokenAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<ExpressionSyntax, SyntaxToken>(WrappedType, nameof(CloseBracketToken));
            InitializerAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<ExpressionSyntax, InitializerExpressionSyntax>(WrappedType, nameof(Initializer));
            WithStackAllocKeywordAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<ExpressionSyntax, SyntaxToken>(WrappedType, nameof(StackAllocKeyword));
            WithOpenBracketTokenAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<ExpressionSyntax, SyntaxToken>(WrappedType, nameof(OpenBracketToken));
            WithCloseBracketTokenAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<ExpressionSyntax, SyntaxToken>(WrappedType, nameof(CloseBracketToken));
            WithInitializerAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<ExpressionSyntax, InitializerExpressionSyntax>(WrappedType, nameof(Initializer));
        }

        private ImplicitStackAllocArrayCreationExpressionSyntaxWrapper(ExpressionSyntax? node)
        {
            this.node = node;
        }

        public ExpressionSyntax? SyntaxNode => this.node;


        public SyntaxToken StackAllocKeyword
        {
            get
            {
                return StackAllocKeywordAccessor(this.node!);
            }
        }

        public SyntaxToken OpenBracketToken
        {
            get
            {
                return OpenBracketTokenAccessor(this.node!);
            }
        }

        public SyntaxToken CloseBracketToken
        {
            get
            {
                return CloseBracketTokenAccessor(this.node!);
            }
        }

        public InitializerExpressionSyntax Initializer
        {
            get
            {
                return InitializerAccessor(this.node!);
            }
        }

        public static explicit operator ImplicitStackAllocArrayCreationExpressionSyntaxWrapper(SyntaxNode? node)
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

        public static implicit operator ExpressionSyntax? (ImplicitStackAllocArrayCreationExpressionSyntaxWrapper wrapper)
        {
            return wrapper.node;
        }

        public static bool IsInstance([NotNullWhen(true)] SyntaxNode? node)
        {
            return node != null && LightupHelpers.CanWrapNode(node, WrappedType);
        }

        public ImplicitStackAllocArrayCreationExpressionSyntaxWrapper WithStackAllocKeyword(SyntaxToken stackAllocKeyword)
        {
            return new ImplicitStackAllocArrayCreationExpressionSyntaxWrapper(WithStackAllocKeywordAccessor(this.node!, stackAllocKeyword));
        }

        public ImplicitStackAllocArrayCreationExpressionSyntaxWrapper WithOpenBracketToken(SyntaxToken openBracketToken)
        {
            return new ImplicitStackAllocArrayCreationExpressionSyntaxWrapper(WithOpenBracketTokenAccessor(this.node!, openBracketToken));
        }

        public ImplicitStackAllocArrayCreationExpressionSyntaxWrapper WithCloseBracketToken(SyntaxToken closeBracketToken)
        {
            return new ImplicitStackAllocArrayCreationExpressionSyntaxWrapper(WithCloseBracketTokenAccessor(this.node!, closeBracketToken));
        }

        public ImplicitStackAllocArrayCreationExpressionSyntaxWrapper WithInitializer(InitializerExpressionSyntax initializer)
        {
            return new ImplicitStackAllocArrayCreationExpressionSyntaxWrapper(WithInitializerAccessor(this.node!, initializer));
        }
    }
}
