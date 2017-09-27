// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Reflection;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal struct TupleExpressionSyntaxWrapper : ISyntaxWrapper<ExpressionSyntax>
    {
        private const string TupleExpressionSyntaxTypeName = "Microsoft.CodeAnalysis.CSharp.Syntax.TupleExpressionSyntax";

        private static readonly Func<ExpressionSyntax, SyntaxToken> OpenParenTokenAccessor;
        private static readonly Func<ExpressionSyntax, SeparatedSyntaxList<ArgumentSyntax>> ArgumentsAccessor;
        private static readonly Func<ExpressionSyntax, SyntaxToken> CloseParenTokenAccessor;
        private static readonly Func<ExpressionSyntax, SyntaxToken, ExpressionSyntax> WithOpenParenTokenAccessor;
        private static readonly Func<ExpressionSyntax, SeparatedSyntaxList<ArgumentSyntax>, ExpressionSyntax> WithArgumentsAccessor;
        private static readonly Func<ExpressionSyntax, SyntaxToken, ExpressionSyntax> WithCloseParenTokenAccessor;

        private readonly ExpressionSyntax node;

        static TupleExpressionSyntaxWrapper()
        {
            WrappedType = typeof(CSharpSyntaxNode).GetTypeInfo().Assembly.GetType(TupleExpressionSyntaxTypeName);
            OpenParenTokenAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<ExpressionSyntax, SyntaxToken>(WrappedType, nameof(OpenParenToken));
            ArgumentsAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<ExpressionSyntax, SeparatedSyntaxList<ArgumentSyntax>>(WrappedType, nameof(Arguments));
            CloseParenTokenAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<ExpressionSyntax, SyntaxToken>(WrappedType, nameof(CloseParenToken));
            WithOpenParenTokenAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<ExpressionSyntax, SyntaxToken>(WrappedType, nameof(OpenParenToken));
            WithArgumentsAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<ExpressionSyntax, SeparatedSyntaxList<ArgumentSyntax>>(WrappedType, nameof(Arguments));
            WithCloseParenTokenAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<ExpressionSyntax, SyntaxToken>(WrappedType, nameof(CloseParenToken));
        }

        private TupleExpressionSyntaxWrapper(ExpressionSyntax node)
        {
            this.node = node;
        }

        public static Type WrappedType { get; private set; }

        public ExpressionSyntax SyntaxNode => this.node;

        public SyntaxToken OpenParenToken
        {
            get
            {
                return OpenParenTokenAccessor(this.SyntaxNode);
            }
        }

        public SeparatedSyntaxList<ArgumentSyntax> Arguments
        {
            get
            {
                return ArgumentsAccessor(this.SyntaxNode);
            }
        }

        public SyntaxToken CloseParenToken
        {
            get
            {
                return CloseParenTokenAccessor(this.SyntaxNode);
            }
        }

        public static explicit operator TupleExpressionSyntaxWrapper(SyntaxNode node)
        {
            if (node == null)
            {
                return default(TupleExpressionSyntaxWrapper);
            }

            if (!IsInstance(node))
            {
                throw new InvalidCastException($"Cannot cast '{node.GetType().FullName}' to '{TupleExpressionSyntaxTypeName}'");
            }

            return new TupleExpressionSyntaxWrapper((ExpressionSyntax)node);
        }

        public static implicit operator ExpressionSyntax(TupleExpressionSyntaxWrapper wrapper)
        {
            return wrapper.node;
        }

        public static bool IsInstance(SyntaxNode node)
        {
            return node != null && LightupHelpers.CanWrapNode(node, WrappedType);
        }

        public TupleExpressionSyntaxWrapper AddArguments(params ArgumentSyntax[] items)
        {
            return new TupleExpressionSyntaxWrapper(this.WithArguments(this.Arguments.AddRange(items)));
        }

        public TupleExpressionSyntaxWrapper WithOpenParenToken(SyntaxToken openParenToken)
        {
            return new TupleExpressionSyntaxWrapper(WithOpenParenTokenAccessor(this.SyntaxNode, openParenToken));
        }

        public TupleExpressionSyntaxWrapper WithArguments(SeparatedSyntaxList<ArgumentSyntax> arguments)
        {
            return new TupleExpressionSyntaxWrapper(WithArgumentsAccessor(this.SyntaxNode, arguments));
        }

        public TupleExpressionSyntaxWrapper WithCloseParenToken(SyntaxToken closeParenToken)
        {
            return new TupleExpressionSyntaxWrapper(WithCloseParenTokenAccessor(this.SyntaxNode, closeParenToken));
        }
    }
}
