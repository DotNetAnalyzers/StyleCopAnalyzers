// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal readonly partial struct FunctionPointerParameterListSyntaxWrapper : ISyntaxWrapper<CSharpSyntaxNode>
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.CSharp.Syntax.FunctionPointerParameterListSyntax";
        private static readonly Type WrappedType;
        private static readonly Func<CSharpSyntaxNode, SyntaxToken> LessThanTokenAccessor;
        private static readonly Func<CSharpSyntaxNode, SeparatedSyntaxListWrapper<FunctionPointerParameterSyntaxWrapper>> ParametersAccessor;
        private static readonly Func<CSharpSyntaxNode, SyntaxToken> GreaterThanTokenAccessor;
        private static readonly Func<CSharpSyntaxNode, SyntaxToken, CSharpSyntaxNode> WithLessThanTokenAccessor;
        private static readonly Func<CSharpSyntaxNode, SeparatedSyntaxListWrapper<FunctionPointerParameterSyntaxWrapper>, CSharpSyntaxNode> WithParametersAccessor;
        private static readonly Func<CSharpSyntaxNode, SyntaxToken, CSharpSyntaxNode> WithGreaterThanTokenAccessor;
        private readonly CSharpSyntaxNode node;
        static FunctionPointerParameterListSyntaxWrapper()
        {
            WrappedType = SyntaxWrapperHelper.GetWrappedType(typeof(FunctionPointerParameterListSyntaxWrapper));
            LessThanTokenAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<CSharpSyntaxNode, SyntaxToken>(WrappedType, nameof(LessThanToken));
            ParametersAccessor = LightupHelpers.CreateSeparatedSyntaxListPropertyAccessor<CSharpSyntaxNode, FunctionPointerParameterSyntaxWrapper>(WrappedType, nameof(Parameters));
            GreaterThanTokenAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<CSharpSyntaxNode, SyntaxToken>(WrappedType, nameof(GreaterThanToken));
            WithLessThanTokenAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<CSharpSyntaxNode, SyntaxToken>(WrappedType, nameof(LessThanToken));
            WithParametersAccessor = LightupHelpers.CreateSeparatedSyntaxListWithPropertyAccessor<CSharpSyntaxNode, FunctionPointerParameterSyntaxWrapper>(WrappedType, nameof(Parameters));
            WithGreaterThanTokenAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<CSharpSyntaxNode, SyntaxToken>(WrappedType, nameof(GreaterThanToken));
        }

        private FunctionPointerParameterListSyntaxWrapper(CSharpSyntaxNode node)
        {
            this.node = node;
        }

        public CSharpSyntaxNode SyntaxNode => this.node;
        public static explicit operator FunctionPointerParameterListSyntaxWrapper(SyntaxNode node)
        {
            if (node == null)
            {
                return default;
            }

            if (!IsInstance(node))
            {
                throw new InvalidCastException($"Cannot cast '{node.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new FunctionPointerParameterListSyntaxWrapper((CSharpSyntaxNode)node);
        }

        public static implicit operator CSharpSyntaxNode(FunctionPointerParameterListSyntaxWrapper wrapper)
        {
            return wrapper.node;
        }

        public static bool IsInstance(SyntaxNode node)
        {
            return node != null && LightupHelpers.CanWrapNode(node, WrappedType);
        }
    }
}
