// Copyright (c) Contributors to the New StyleCop Analyzers project.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal readonly partial struct AllowsConstraintClauseSyntaxWrapper : ISyntaxWrapper<TypeParameterConstraintSyntax>
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.CSharp.Syntax.AllowsConstraintClauseSyntax";
        private static readonly Type WrappedType;

        private static readonly Func<TypeParameterConstraintSyntax, SyntaxToken> AllowsKeywordAccessor;
        private static readonly Func<TypeParameterConstraintSyntax, SeparatedSyntaxListWrapper<AllowsConstraintSyntaxWrapper>> ConstraintsAccessor;
        private static readonly Func<TypeParameterConstraintSyntax, SyntaxToken, TypeParameterConstraintSyntax> WithAllowsKeywordAccessor;
        private static readonly Func<TypeParameterConstraintSyntax, SeparatedSyntaxListWrapper<AllowsConstraintSyntaxWrapper>, TypeParameterConstraintSyntax> WithConstraintsAccessor;

        private readonly TypeParameterConstraintSyntax node;

        static AllowsConstraintClauseSyntaxWrapper()
        {
            WrappedType = SyntaxWrapperHelper.GetWrappedType(typeof(AllowsConstraintClauseSyntaxWrapper));
            AllowsKeywordAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<TypeParameterConstraintSyntax, SyntaxToken>(WrappedType, nameof(AllowsKeyword));
            ConstraintsAccessor = LightupHelpers.CreateSeparatedSyntaxListPropertyAccessor<TypeParameterConstraintSyntax, AllowsConstraintSyntaxWrapper>(WrappedType, nameof(Constraints));
            WithAllowsKeywordAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<TypeParameterConstraintSyntax, SyntaxToken>(WrappedType, nameof(AllowsKeyword));
            WithConstraintsAccessor = LightupHelpers.CreateSeparatedSyntaxListWithPropertyAccessor<TypeParameterConstraintSyntax, AllowsConstraintSyntaxWrapper>(WrappedType, nameof(Constraints));
        }

        private AllowsConstraintClauseSyntaxWrapper(TypeParameterConstraintSyntax node)
        {
            this.node = node;
        }

        public TypeParameterConstraintSyntax SyntaxNode => this.node;

        public SyntaxToken AllowsKeyword
        {
            get
            {
                return AllowsKeywordAccessor(this.SyntaxNode);
            }
        }

        public SeparatedSyntaxListWrapper<AllowsConstraintSyntaxWrapper> Constraints
        {
            get
            {
                return ConstraintsAccessor(this.SyntaxNode);
            }
        }

        public static explicit operator AllowsConstraintClauseSyntaxWrapper(SyntaxNode node)
        {
            if (node == null)
            {
                return default;
            }

            if (!IsInstance(node))
            {
                throw new InvalidCastException($"Cannot cast '{node.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new AllowsConstraintClauseSyntaxWrapper((TypeParameterConstraintSyntax)node);
        }

        public static implicit operator TypeParameterConstraintSyntax(AllowsConstraintClauseSyntaxWrapper wrapper)
        {
            return wrapper.node;
        }

        public static bool IsInstance(SyntaxNode node)
        {
            return node != null && LightupHelpers.CanWrapNode(node, WrappedType);
        }

        public AllowsConstraintClauseSyntaxWrapper WithAllowsKeyword(SyntaxToken allowsKeyword)
        {
            return new AllowsConstraintClauseSyntaxWrapper(WithAllowsKeywordAccessor(this.SyntaxNode, allowsKeyword));
        }

        public AllowsConstraintClauseSyntaxWrapper WithConstraints(SeparatedSyntaxListWrapper<AllowsConstraintSyntaxWrapper> constraints)
        {
            return new AllowsConstraintClauseSyntaxWrapper(WithConstraintsAccessor(this.SyntaxNode, constraints));
        }
    }
}
