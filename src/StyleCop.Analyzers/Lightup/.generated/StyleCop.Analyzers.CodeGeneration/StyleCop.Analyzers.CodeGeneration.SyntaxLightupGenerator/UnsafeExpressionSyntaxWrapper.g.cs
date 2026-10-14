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

    internal readonly partial struct UnsafeExpressionSyntaxWrapper : ISyntaxWrapper<ExpressionSyntax>
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.CSharp.Syntax.UnsafeExpressionSyntax";
        private static readonly Type? WrappedType;

        private static readonly Func<ExpressionSyntax, SyntaxToken> KeywordAccessor;
        private static readonly Func<ExpressionSyntax, SyntaxToken> OpenParenTokenAccessor;
        private static readonly Func<ExpressionSyntax, ExpressionSyntax> ExpressionAccessor;
        private static readonly Func<ExpressionSyntax, SyntaxToken> CloseParenTokenAccessor;
        private static readonly Func<ExpressionSyntax, SyntaxToken, ExpressionSyntax> WithKeywordAccessor;
        private static readonly Func<ExpressionSyntax, SyntaxToken, ExpressionSyntax> WithOpenParenTokenAccessor;
        private static readonly Func<ExpressionSyntax, ExpressionSyntax, ExpressionSyntax> WithExpressionAccessor;
        private static readonly Func<ExpressionSyntax, SyntaxToken, ExpressionSyntax> WithCloseParenTokenAccessor;

        private readonly ExpressionSyntax? node;

        static UnsafeExpressionSyntaxWrapper()
        {
            WrappedType = SyntaxWrapperHelper.GetWrappedType(typeof(UnsafeExpressionSyntaxWrapper));
            KeywordAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<ExpressionSyntax, SyntaxToken>(WrappedType, nameof(Keyword));
            OpenParenTokenAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<ExpressionSyntax, SyntaxToken>(WrappedType, nameof(OpenParenToken));
            ExpressionAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<ExpressionSyntax, ExpressionSyntax>(WrappedType, nameof(Expression));
            CloseParenTokenAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<ExpressionSyntax, SyntaxToken>(WrappedType, nameof(CloseParenToken));
            WithKeywordAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<ExpressionSyntax, SyntaxToken>(WrappedType, nameof(Keyword));
            WithOpenParenTokenAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<ExpressionSyntax, SyntaxToken>(WrappedType, nameof(OpenParenToken));
            WithExpressionAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<ExpressionSyntax, ExpressionSyntax>(WrappedType, nameof(Expression));
            WithCloseParenTokenAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<ExpressionSyntax, SyntaxToken>(WrappedType, nameof(CloseParenToken));
        }

        private UnsafeExpressionSyntaxWrapper(ExpressionSyntax? node)
        {
            this.node = node;
        }

        public ExpressionSyntax? SyntaxNode => this.node;


        public SyntaxToken Keyword
        {
            get
            {
                return KeywordAccessor(this.node!);
            }
        }

        public SyntaxToken OpenParenToken
        {
            get
            {
                return OpenParenTokenAccessor(this.node!);
            }
        }

        public ExpressionSyntax Expression
        {
            get
            {
                return ExpressionAccessor(this.node!);
            }
        }

        public SyntaxToken CloseParenToken
        {
            get
            {
                return CloseParenTokenAccessor(this.node!);
            }
        }

        public static explicit operator UnsafeExpressionSyntaxWrapper(SyntaxNode? node)
        {
            if (node == null)
            {
                return default;
            }

            if (!IsInstance(node))
            {
                throw new InvalidCastException($"Cannot cast '{node.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new UnsafeExpressionSyntaxWrapper((ExpressionSyntax)node);
        }

        public static implicit operator ExpressionSyntax? (UnsafeExpressionSyntaxWrapper wrapper)
        {
            return wrapper.node;
        }

        public static bool IsInstance([NotNullWhen(true)] SyntaxNode? node)
        {
            return node != null && LightupHelpers.CanWrapNode(node, WrappedType);
        }

        public UnsafeExpressionSyntaxWrapper WithKeyword(SyntaxToken keyword)
        {
            return new UnsafeExpressionSyntaxWrapper(WithKeywordAccessor(this.node!, keyword));
        }

        public UnsafeExpressionSyntaxWrapper WithOpenParenToken(SyntaxToken openParenToken)
        {
            return new UnsafeExpressionSyntaxWrapper(WithOpenParenTokenAccessor(this.node!, openParenToken));
        }

        public UnsafeExpressionSyntaxWrapper WithExpression(ExpressionSyntax expression)
        {
            return new UnsafeExpressionSyntaxWrapper(WithExpressionAccessor(this.node!, expression));
        }

        public UnsafeExpressionSyntaxWrapper WithCloseParenToken(SyntaxToken closeParenToken)
        {
            return new UnsafeExpressionSyntaxWrapper(WithCloseParenTokenAccessor(this.node!, closeParenToken));
        }
    }
}
