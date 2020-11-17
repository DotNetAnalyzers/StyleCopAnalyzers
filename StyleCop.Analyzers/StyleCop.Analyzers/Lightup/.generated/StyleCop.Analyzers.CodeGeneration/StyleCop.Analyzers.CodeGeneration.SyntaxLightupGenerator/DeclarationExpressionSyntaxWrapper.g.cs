// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal readonly partial struct DeclarationExpressionSyntaxWrapper : ISyntaxWrapper<ExpressionSyntax>
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.CSharp.Syntax.DeclarationExpressionSyntax";
        private static readonly Type WrappedType;
        private static readonly Func<ExpressionSyntax, TypeSyntax> TypeAccessor;
        private static readonly Func<ExpressionSyntax, CSharpSyntaxNode> DesignationAccessor;
        private static readonly Func<ExpressionSyntax, TypeSyntax, ExpressionSyntax> WithTypeAccessor;
        private static readonly Func<ExpressionSyntax, CSharpSyntaxNode, ExpressionSyntax> WithDesignationAccessor;
        private readonly ExpressionSyntax node;
        static DeclarationExpressionSyntaxWrapper()
        {
            WrappedType = SyntaxWrapperHelper.GetWrappedType(typeof(DeclarationExpressionSyntaxWrapper));
            TypeAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<ExpressionSyntax, TypeSyntax>(WrappedType, nameof(Type));
            DesignationAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<ExpressionSyntax, CSharpSyntaxNode>(WrappedType, nameof(Designation));
            WithTypeAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<ExpressionSyntax, TypeSyntax>(WrappedType, nameof(Type));
            WithDesignationAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<ExpressionSyntax, CSharpSyntaxNode>(WrappedType, nameof(Designation));
        }

        private DeclarationExpressionSyntaxWrapper(ExpressionSyntax node)
        {
            this.node = node;
        }

        public ExpressionSyntax SyntaxNode => this.node;
        public static explicit operator DeclarationExpressionSyntaxWrapper(SyntaxNode node)
        {
            if (node == null)
            {
                return default;
            }

            if (!IsInstance(node))
            {
                throw new InvalidCastException($"Cannot cast '{node.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new DeclarationExpressionSyntaxWrapper((ExpressionSyntax)node);
        }

        public static implicit operator ExpressionSyntax(DeclarationExpressionSyntaxWrapper wrapper)
        {
            return wrapper.node;
        }

        public static bool IsInstance(SyntaxNode node)
        {
            return node != null && LightupHelpers.CanWrapNode(node, WrappedType);
        }
    }
}
