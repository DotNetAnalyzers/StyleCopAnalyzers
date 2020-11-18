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
        private static readonly Func<ExpressionSyntax, SyntaxToken> NewKeywordAccessor;
        private static readonly Func<ExpressionSyntax, ArgumentListSyntax> ArgumentListAccessor;
        private static readonly Func<ExpressionSyntax, InitializerExpressionSyntax> InitializerAccessor;
        private static readonly Func<ExpressionSyntax, SyntaxToken, ExpressionSyntax> WithNewKeywordAccessor;
        private static readonly Func<ExpressionSyntax, ArgumentListSyntax, ExpressionSyntax> WithArgumentListAccessor;
        private static readonly Func<ExpressionSyntax, InitializerExpressionSyntax, ExpressionSyntax> WithInitializerAccessor;
        private readonly ExpressionSyntax node;
        static BaseObjectCreationExpressionSyntaxWrapper()
        {
            WrappedType = SyntaxWrapperHelper.GetWrappedType(typeof(BaseObjectCreationExpressionSyntaxWrapper));
            NewKeywordAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<ExpressionSyntax, SyntaxToken>(WrappedType, nameof(NewKeyword));
            ArgumentListAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<ExpressionSyntax, ArgumentListSyntax>(WrappedType, nameof(ArgumentList));
            InitializerAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<ExpressionSyntax, InitializerExpressionSyntax>(WrappedType, nameof(Initializer));
            WithNewKeywordAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<ExpressionSyntax, SyntaxToken>(WrappedType, nameof(NewKeyword));
            WithArgumentListAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<ExpressionSyntax, ArgumentListSyntax>(WrappedType, nameof(ArgumentList));
            WithInitializerAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<ExpressionSyntax, InitializerExpressionSyntax>(WrappedType, nameof(Initializer));
        }

        private BaseObjectCreationExpressionSyntaxWrapper(ExpressionSyntax node)
        {
            this.node = node;
        }

        public ExpressionSyntax SyntaxNode => this.node;
        public SyntaxToken NewKeyword
        {
            get
            {
                return NewKeywordAccessor(this.SyntaxNode);
            }
        }

        public ArgumentListSyntax ArgumentList
        {
            get
            {
                return ArgumentListAccessor(this.SyntaxNode);
            }
        }

        public InitializerExpressionSyntax Initializer
        {
            get
            {
                return InitializerAccessor(this.SyntaxNode);
            }
        }

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

        public BaseObjectCreationExpressionSyntaxWrapper WithNewKeyword(SyntaxToken newKeyword)
        {
            return new BaseObjectCreationExpressionSyntaxWrapper(WithNewKeywordAccessor(this.SyntaxNode, newKeyword));
        }

        public BaseObjectCreationExpressionSyntaxWrapper WithArgumentList(ArgumentListSyntax argumentList)
        {
            return new BaseObjectCreationExpressionSyntaxWrapper(WithArgumentListAccessor(this.SyntaxNode, argumentList));
        }

        public BaseObjectCreationExpressionSyntaxWrapper WithInitializer(InitializerExpressionSyntax initializer)
        {
            return new BaseObjectCreationExpressionSyntaxWrapper(WithInitializerAccessor(this.SyntaxNode, initializer));
        }

        internal static BaseObjectCreationExpressionSyntaxWrapper FromUpcast(ExpressionSyntax node)
        {
            return new BaseObjectCreationExpressionSyntaxWrapper(node);
        }
    }
}
