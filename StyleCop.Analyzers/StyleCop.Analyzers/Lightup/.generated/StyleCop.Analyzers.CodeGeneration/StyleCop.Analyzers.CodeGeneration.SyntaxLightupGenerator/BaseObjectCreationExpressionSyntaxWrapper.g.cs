// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal readonly partial struct BaseObjectCreationExpressionSyntaxWrapper : ISyntaxWrapper<ExpressionSyntax>
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.CSharp.Syntax.BaseObjectCreationExpressionSyntax";
        private static readonly Type WrappedType;
        private readonly ExpressionSyntax node;
        private static readonly Func<ExpressionSyntax, SyntaxToken> NewKeywordAccessor;
        private static readonly Func<ExpressionSyntax, ArgumentListSyntax> ArgumentListAccessor;
        private static readonly Func<ExpressionSyntax, InitializerExpressionSyntax> InitializerAccessor;
        private static readonly Func<ExpressionSyntax, SyntaxToken, ExpressionSyntax> WithNewKeywordAccessor;
        private static readonly Func<ExpressionSyntax, ArgumentListSyntax, ExpressionSyntax> WithArgumentListAccessor;
        private static readonly Func<ExpressionSyntax, InitializerExpressionSyntax, ExpressionSyntax> WithInitializerAccessor;
        private BaseObjectCreationExpressionSyntaxWrapper(ExpressionSyntax node)
        {
            this.node = node;
        }

        public ExpressionSyntax SyntaxNode => this.node;
        public static explicit operator BaseObjectCreationExpressionSyntaxWrapper(SyntaxNode node)
        {
            if (node == null)
            {
                return default;
            }

            if (!IsInstance(node))
            {
                throw new InvalidCastException($"Cannot cast '{node.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new BaseObjectCreationExpressionSyntaxWrapper((ExpressionSyntax)node);
        }

        public static implicit operator ExpressionSyntax(BaseObjectCreationExpressionSyntaxWrapper wrapper)
        {
            return wrapper.node;
        }

        public static bool IsInstance(SyntaxNode node)
        {
            return node != null && LightupHelpers.CanWrapNode(node, WrappedType);
        }

        internal static BaseObjectCreationExpressionSyntaxWrapper FromUpcast(ExpressionSyntax node)
        {
            return new BaseObjectCreationExpressionSyntaxWrapper(node);
        }
    }
}
