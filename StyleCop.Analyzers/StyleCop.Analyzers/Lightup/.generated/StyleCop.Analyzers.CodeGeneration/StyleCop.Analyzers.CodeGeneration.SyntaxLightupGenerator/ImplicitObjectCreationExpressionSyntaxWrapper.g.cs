﻿// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
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
            WithNewKeywordAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<ExpressionSyntax, SyntaxToken>(WrappedType, nameof(NewKeyword));
            WithArgumentListAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<ExpressionSyntax, ArgumentListSyntax>(WrappedType, nameof(ArgumentList));
            WithInitializerAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<ExpressionSyntax, InitializerExpressionSyntax>(WrappedType, nameof(Initializer));
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

        public static explicit operator ImplicitObjectCreationExpressionSyntaxWrapper(BaseObjectCreationExpressionSyntaxWrapper node)
        {
            return (ImplicitObjectCreationExpressionSyntaxWrapper)node.SyntaxNode;
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

        public static implicit operator BaseObjectCreationExpressionSyntaxWrapper(ImplicitObjectCreationExpressionSyntaxWrapper wrapper)
        {
            return BaseObjectCreationExpressionSyntaxWrapper.FromUpcast(wrapper.node);
        }

        public static implicit operator ExpressionSyntax(ImplicitObjectCreationExpressionSyntaxWrapper wrapper)
        {
            return wrapper.node;
        }

        public static bool IsInstance(SyntaxNode node)
        {
            return node != null && LightupHelpers.CanWrapNode(node, WrappedType);
        }

        public ImplicitObjectCreationExpressionSyntaxWrapper WithNewKeyword(SyntaxToken newKeyword)
        {
            return new ImplicitObjectCreationExpressionSyntaxWrapper(WithNewKeywordAccessor(this.SyntaxNode, newKeyword));
        }

        public ImplicitObjectCreationExpressionSyntaxWrapper WithArgumentList(ArgumentListSyntax argumentList)
        {
            return new ImplicitObjectCreationExpressionSyntaxWrapper(WithArgumentListAccessor(this.SyntaxNode, argumentList));
        }

        public ImplicitObjectCreationExpressionSyntaxWrapper WithInitializer(InitializerExpressionSyntax initializer)
        {
            return new ImplicitObjectCreationExpressionSyntaxWrapper(WithInitializerAccessor(this.SyntaxNode, initializer));
        }
    }
}
