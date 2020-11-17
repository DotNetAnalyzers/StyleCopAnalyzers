// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal readonly partial struct ImplicitObjectCreationExpressionSyntaxWrapper : ISyntaxWrapper<ExpressionSyntax>
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.CSharp.Syntax.ImplicitObjectCreationExpressionSyntax";
        private static readonly Type WrappedType;
        private static readonly Func<ExpressionSyntax, SyntaxToken, ExpressionSyntax> WithNewKeywordAccessor;
        private static readonly Func<ExpressionSyntax, ArgumentListSyntax, ExpressionSyntax> WithArgumentListAccessor;
        private static readonly Func<ExpressionSyntax, InitializerExpressionSyntax, ExpressionSyntax> WithInitializerAccessor;
        private readonly ExpressionSyntax node;
        static ImplicitObjectCreationExpressionSyntaxWrapper()
        {
            WrappedType = SyntaxWrapperHelper.GetWrappedType(typeof(ImplicitObjectCreationExpressionSyntaxWrapper));
            WithNewKeywordAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<ExpressionSyntax, SyntaxToken>(WrappedType, nameof(BaseObjectCreationExpressionSyntaxWrapper.NewKeyword));
            WithArgumentListAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<ExpressionSyntax, ArgumentListSyntax>(WrappedType, nameof(BaseObjectCreationExpressionSyntaxWrapper.ArgumentList));
            WithInitializerAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<ExpressionSyntax, InitializerExpressionSyntax>(WrappedType, nameof(BaseObjectCreationExpressionSyntaxWrapper.Initializer));
        }

        private ImplicitObjectCreationExpressionSyntaxWrapper(ExpressionSyntax node)
        {
            this.node = node;
        }

        public ExpressionSyntax SyntaxNode => this.node;
        public SyntaxToken NewKeyword
        {
            get
            {
                return ((BaseObjectCreationExpressionSyntaxWrapper)this).NewKeyword;
            }
        }

        public ArgumentListSyntax ArgumentList
        {
            get
            {
                return ((BaseObjectCreationExpressionSyntaxWrapper)this).ArgumentList;
            }
        }

        public InitializerExpressionSyntax Initializer
        {
            get
            {
                return ((BaseObjectCreationExpressionSyntaxWrapper)this).Initializer;
            }
        }

        public static explicit operator ImplicitObjectCreationExpressionSyntaxWrapper(SyntaxNode node)
        {
            if (node == null)
            {
                return default;
            }

            if (!IsInstance(node))
            {
                throw new InvalidCastException($"Cannot cast '{node.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new ImplicitObjectCreationExpressionSyntaxWrapper((ExpressionSyntax)node);
        }

        public static implicit operator ExpressionSyntax(ImplicitObjectCreationExpressionSyntaxWrapper wrapper)
        {
            return wrapper.node;
        }

        public static bool IsInstance(SyntaxNode node)
        {
            return node != null && LightupHelpers.CanWrapNode(node, WrappedType);
        }
    }
}
