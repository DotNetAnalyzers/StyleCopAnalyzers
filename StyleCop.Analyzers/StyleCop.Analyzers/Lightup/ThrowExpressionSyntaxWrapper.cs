// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Reflection;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal struct ThrowExpressionSyntaxWrapper : ISyntaxWrapper<ExpressionSyntax>
    {
        private const string ThrowExpressionSyntaxTypeName = "Microsoft.CodeAnalysis.CSharp.Syntax.ThrowExpressionSyntax";
        private static readonly Type ThrowExpressionSyntaxType;

        private static readonly Func<ExpressionSyntax, SyntaxToken> ThrowKeywordAccessor;
        private static readonly Func<ExpressionSyntax, ExpressionSyntax> ExpressionAccessor;
        private static readonly Func<ExpressionSyntax, SyntaxToken, ExpressionSyntax> WithThrowKeywordAccessor;
        private static readonly Func<ExpressionSyntax, ExpressionSyntax, ExpressionSyntax> WithExpressionAccessor;

        private readonly ExpressionSyntax node;

        static ThrowExpressionSyntaxWrapper()
        {
            ThrowExpressionSyntaxType = typeof(CSharpSyntaxNode).GetTypeInfo().Assembly.GetType(ThrowExpressionSyntaxTypeName);
            ThrowKeywordAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<ExpressionSyntax, SyntaxToken>(ThrowExpressionSyntaxType, nameof(ThrowKeyword));
            ExpressionAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<ExpressionSyntax, ExpressionSyntax>(ThrowExpressionSyntaxType, nameof(Expression));
            WithThrowKeywordAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<ExpressionSyntax, SyntaxToken>(ThrowExpressionSyntaxType, nameof(ThrowKeyword));
            WithExpressionAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<ExpressionSyntax, ExpressionSyntax>(ThrowExpressionSyntaxType, nameof(Expression));
        }

        private ThrowExpressionSyntaxWrapper(ExpressionSyntax node)
        {
            this.node = node;
        }

        public ExpressionSyntax SyntaxNode => this.node;

        public SyntaxToken ThrowKeyword
        {
            get
            {
                return ThrowKeywordAccessor(this.SyntaxNode);
            }
        }

        public ExpressionSyntax Expression
        {
            get
            {
                return ExpressionAccessor(this.SyntaxNode);
            }
        }

        public static explicit operator ThrowExpressionSyntaxWrapper(SyntaxNode node)
        {
            if (node == null)
            {
                return default(ThrowExpressionSyntaxWrapper);
            }

            if (!IsInstance(node))
            {
                throw new InvalidCastException($"Cannot cast '{node.GetType().FullName}' to '{ThrowExpressionSyntaxTypeName}'");
            }

            return new ThrowExpressionSyntaxWrapper((ExpressionSyntax)node);
        }

        public static implicit operator ExpressionSyntax(ThrowExpressionSyntaxWrapper wrapper)
        {
            return wrapper.node;
        }

        public static bool IsInstance(SyntaxNode node)
        {
            return node != null && LightupHelpers.CanWrapNode(node, ThrowExpressionSyntaxType);
        }

        public ThrowExpressionSyntaxWrapper WithThrowKeyword(SyntaxToken throwKeyword)
        {
            return new ThrowExpressionSyntaxWrapper(WithThrowKeywordAccessor(this.SyntaxNode, throwKeyword));
        }

        public ThrowExpressionSyntaxWrapper WithExpression(ExpressionSyntax expression)
        {
            return new ThrowExpressionSyntaxWrapper(WithExpressionAccessor(this.SyntaxNode, expression));
        }
    }
}
