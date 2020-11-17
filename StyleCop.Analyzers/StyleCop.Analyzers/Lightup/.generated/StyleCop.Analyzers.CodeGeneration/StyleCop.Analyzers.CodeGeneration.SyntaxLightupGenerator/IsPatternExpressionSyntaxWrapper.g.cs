// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal readonly partial struct IsPatternExpressionSyntaxWrapper : ISyntaxWrapper<ExpressionSyntax>
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.CSharp.Syntax.IsPatternExpressionSyntax";
        private static readonly Type WrappedType;
        private readonly ExpressionSyntax node;
        private static readonly Func<ExpressionSyntax, ExpressionSyntax> ExpressionAccessor;
        private static readonly Func<ExpressionSyntax, SyntaxToken> IsKeywordAccessor;
        private static readonly Func<ExpressionSyntax, CSharpSyntaxNode> PatternAccessor;
        private static readonly Func<ExpressionSyntax, ExpressionSyntax, ExpressionSyntax> WithExpressionAccessor;
        private static readonly Func<ExpressionSyntax, SyntaxToken, ExpressionSyntax> WithIsKeywordAccessor;
        private static readonly Func<ExpressionSyntax, CSharpSyntaxNode, ExpressionSyntax> WithPatternAccessor;
        static IsPatternExpressionSyntaxWrapper()
        {
            WrappedType = SyntaxWrapperHelper.GetWrappedType(typeof(IsPatternExpressionSyntaxWrapper));
            ExpressionAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<ExpressionSyntax, ExpressionSyntax>(WrappedType, nameof(Expression));
            IsKeywordAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<ExpressionSyntax, SyntaxToken>(WrappedType, nameof(IsKeyword));
            PatternAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<ExpressionSyntax, CSharpSyntaxNode>(WrappedType, nameof(Pattern));
            WithExpressionAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<ExpressionSyntax, ExpressionSyntax>(WrappedType, nameof(Expression));
            WithIsKeywordAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<ExpressionSyntax, SyntaxToken>(WrappedType, nameof(IsKeyword));
            WithPatternAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<ExpressionSyntax, CSharpSyntaxNode>(WrappedType, nameof(Pattern));
        }

        private IsPatternExpressionSyntaxWrapper(ExpressionSyntax node)
        {
            this.node = node;
        }

        public ExpressionSyntax SyntaxNode => this.node;
        public static explicit operator IsPatternExpressionSyntaxWrapper(SyntaxNode node)
        {
            if (node == null)
            {
                return default;
            }

            if (!IsInstance(node))
            {
                throw new InvalidCastException($"Cannot cast '{node.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new IsPatternExpressionSyntaxWrapper((ExpressionSyntax)node);
        }

        public static implicit operator ExpressionSyntax(IsPatternExpressionSyntaxWrapper wrapper)
        {
            return wrapper.node;
        }

        public static bool IsInstance(SyntaxNode node)
        {
            return node != null && LightupHelpers.CanWrapNode(node, WrappedType);
        }
    }
}
