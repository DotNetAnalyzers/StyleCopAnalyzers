// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal readonly partial struct FunctionPointerTypeSyntaxWrapper : ISyntaxWrapper<TypeSyntax>
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.CSharp.Syntax.FunctionPointerTypeSyntax";
        private static readonly Type WrappedType;
        private static readonly Func<TypeSyntax, SyntaxToken> DelegateKeywordAccessor;
        private static readonly Func<TypeSyntax, SyntaxToken> AsteriskTokenAccessor;
        private static readonly Func<TypeSyntax, CSharpSyntaxNode> CallingConventionAccessor;
        private static readonly Func<TypeSyntax, CSharpSyntaxNode> ParameterListAccessor;
        private static readonly Func<TypeSyntax, SyntaxToken, TypeSyntax> WithDelegateKeywordAccessor;
        private static readonly Func<TypeSyntax, SyntaxToken, TypeSyntax> WithAsteriskTokenAccessor;
        private static readonly Func<TypeSyntax, CSharpSyntaxNode, TypeSyntax> WithCallingConventionAccessor;
        private static readonly Func<TypeSyntax, CSharpSyntaxNode, TypeSyntax> WithParameterListAccessor;
        private readonly TypeSyntax node;
        static FunctionPointerTypeSyntaxWrapper()
        {
            WrappedType = SyntaxWrapperHelper.GetWrappedType(typeof(FunctionPointerTypeSyntaxWrapper));
            DelegateKeywordAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<TypeSyntax, SyntaxToken>(WrappedType, nameof(DelegateKeyword));
            AsteriskTokenAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<TypeSyntax, SyntaxToken>(WrappedType, nameof(AsteriskToken));
            CallingConventionAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<TypeSyntax, CSharpSyntaxNode>(WrappedType, nameof(CallingConvention));
            ParameterListAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<TypeSyntax, CSharpSyntaxNode>(WrappedType, nameof(ParameterList));
            WithDelegateKeywordAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<TypeSyntax, SyntaxToken>(WrappedType, nameof(DelegateKeyword));
            WithAsteriskTokenAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<TypeSyntax, SyntaxToken>(WrappedType, nameof(AsteriskToken));
            WithCallingConventionAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<TypeSyntax, CSharpSyntaxNode>(WrappedType, nameof(CallingConvention));
            WithParameterListAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<TypeSyntax, CSharpSyntaxNode>(WrappedType, nameof(ParameterList));
        }

        private FunctionPointerTypeSyntaxWrapper(TypeSyntax node)
        {
            this.node = node;
        }

        public TypeSyntax SyntaxNode => this.node;
        public static explicit operator FunctionPointerTypeSyntaxWrapper(SyntaxNode node)
        {
            if (node == null)
            {
                return default;
            }

            if (!IsInstance(node))
            {
                throw new InvalidCastException($"Cannot cast '{node.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new FunctionPointerTypeSyntaxWrapper((TypeSyntax)node);
        }

        public static implicit operator TypeSyntax(FunctionPointerTypeSyntaxWrapper wrapper)
        {
            return wrapper.node;
        }

        public static bool IsInstance(SyntaxNode node)
        {
            return node != null && LightupHelpers.CanWrapNode(node, WrappedType);
        }
    }
}
