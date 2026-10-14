// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal readonly partial struct UnionDeclarationSyntaxWrapper : ISyntaxWrapper<TypeDeclarationSyntax>
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.CSharp.Syntax.UnionDeclarationSyntax";
        private static readonly Type? WrappedType;

        private static readonly Func<TypeDeclarationSyntax, ParameterListSyntax?> ParameterListAccessor;
        private static readonly Func<TypeDeclarationSyntax, SyntaxList<AttributeListSyntax>, TypeDeclarationSyntax> WithAttributeListsAccessor;
        private static readonly Func<TypeDeclarationSyntax, SyntaxTokenList, TypeDeclarationSyntax> WithModifiersAccessor;
        private static readonly Func<TypeDeclarationSyntax, SyntaxToken, TypeDeclarationSyntax> WithKeywordAccessor;
        private static readonly Func<TypeDeclarationSyntax, SyntaxToken, TypeDeclarationSyntax> WithIdentifierAccessor;
        private static readonly Func<TypeDeclarationSyntax, TypeParameterListSyntax?, TypeDeclarationSyntax> WithTypeParameterListAccessor;
        private static readonly Func<TypeDeclarationSyntax, ParameterListSyntax?, TypeDeclarationSyntax> WithParameterListAccessor;
        private static readonly Func<TypeDeclarationSyntax, BaseListSyntax?, TypeDeclarationSyntax> WithBaseListAccessor;
        private static readonly Func<TypeDeclarationSyntax, SyntaxList<TypeParameterConstraintClauseSyntax>, TypeDeclarationSyntax> WithConstraintClausesAccessor;
        private static readonly Func<TypeDeclarationSyntax, SyntaxToken, TypeDeclarationSyntax> WithOpenBraceTokenAccessor;
        private static readonly Func<TypeDeclarationSyntax, SyntaxList<MemberDeclarationSyntax>, TypeDeclarationSyntax> WithMembersAccessor;
        private static readonly Func<TypeDeclarationSyntax, SyntaxToken, TypeDeclarationSyntax> WithCloseBraceTokenAccessor;
        private static readonly Func<TypeDeclarationSyntax, SyntaxToken, TypeDeclarationSyntax> WithSemicolonTokenAccessor;

        private readonly TypeDeclarationSyntax? node;

        static UnionDeclarationSyntaxWrapper()
        {
            WrappedType = SyntaxWrapperHelper.GetWrappedType(typeof(UnionDeclarationSyntaxWrapper));
            ParameterListAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<TypeDeclarationSyntax, ParameterListSyntax?>(WrappedType, nameof(ParameterList));
            WithAttributeListsAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<TypeDeclarationSyntax, SyntaxList<AttributeListSyntax>>(WrappedType, nameof(AttributeLists));
            WithModifiersAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<TypeDeclarationSyntax, SyntaxTokenList>(WrappedType, nameof(Modifiers));
            WithKeywordAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<TypeDeclarationSyntax, SyntaxToken>(WrappedType, nameof(Keyword));
            WithIdentifierAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<TypeDeclarationSyntax, SyntaxToken>(WrappedType, nameof(Identifier));
            WithTypeParameterListAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<TypeDeclarationSyntax, TypeParameterListSyntax?>(WrappedType, nameof(TypeParameterList));
            WithParameterListAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<TypeDeclarationSyntax, ParameterListSyntax?>(WrappedType, nameof(ParameterList));
            WithBaseListAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<TypeDeclarationSyntax, BaseListSyntax?>(WrappedType, nameof(BaseList));
            WithConstraintClausesAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<TypeDeclarationSyntax, SyntaxList<TypeParameterConstraintClauseSyntax>>(WrappedType, nameof(ConstraintClauses));
            WithOpenBraceTokenAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<TypeDeclarationSyntax, SyntaxToken>(WrappedType, nameof(OpenBraceToken));
            WithMembersAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<TypeDeclarationSyntax, SyntaxList<MemberDeclarationSyntax>>(WrappedType, nameof(Members));
            WithCloseBraceTokenAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<TypeDeclarationSyntax, SyntaxToken>(WrappedType, nameof(CloseBraceToken));
            WithSemicolonTokenAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<TypeDeclarationSyntax, SyntaxToken>(WrappedType, nameof(SemicolonToken));
        }

        private UnionDeclarationSyntaxWrapper(TypeDeclarationSyntax? node)
        {
            this.node = node;
        }

        public TypeDeclarationSyntax? SyntaxNode => this.node;


        public SyntaxList<AttributeListSyntax> AttributeLists
        {
            get
            {
                return this.node!.AttributeLists;
            }
        }

        public SyntaxTokenList Modifiers
        {
            get
            {
                return this.node!.Modifiers;
            }
        }

        public SyntaxToken Keyword
        {
            get
            {
                return this.node!.Keyword;
            }
        }

        public SyntaxToken Identifier
        {
            get
            {
                return this.node!.Identifier;
            }
        }

        public TypeParameterListSyntax? TypeParameterList
        {
            get
            {
                return this.node!.TypeParameterList;
            }
        }

        public ParameterListSyntax? ParameterList
        {
            get
            {
                return ParameterListAccessor(this.node!);
            }
        }

        public BaseListSyntax? BaseList
        {
            get
            {
                return this.node!.BaseList;
            }
        }

        public SyntaxList<TypeParameterConstraintClauseSyntax> ConstraintClauses
        {
            get
            {
                return this.node!.ConstraintClauses;
            }
        }

        public SyntaxToken OpenBraceToken
        {
            get
            {
                return this.node!.OpenBraceToken;
            }
        }

        public SyntaxList<MemberDeclarationSyntax> Members
        {
            get
            {
                return this.node!.Members;
            }
        }

        public SyntaxToken CloseBraceToken
        {
            get
            {
                return this.node!.CloseBraceToken;
            }
        }

        public SyntaxToken SemicolonToken
        {
            get
            {
                return this.node!.SemicolonToken;
            }
        }

        public static explicit operator UnionDeclarationSyntaxWrapper(SyntaxNode? node)
        {
            if (node == null)
            {
                return default;
            }

            if (!IsInstance(node))
            {
                throw new InvalidCastException($"Cannot cast '{node.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new UnionDeclarationSyntaxWrapper((TypeDeclarationSyntax)node);
        }

        public static implicit operator TypeDeclarationSyntax? (UnionDeclarationSyntaxWrapper wrapper)
        {
            return wrapper.node;
        }

        public static bool IsInstance([NotNullWhen(true)] SyntaxNode? node)
        {
            return node != null && LightupHelpers.CanWrapNode(node, WrappedType);
        }

        public UnionDeclarationSyntaxWrapper WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists)
        {
            return new UnionDeclarationSyntaxWrapper(WithAttributeListsAccessor(this.node!, attributeLists));
        }

        public UnionDeclarationSyntaxWrapper WithModifiers(SyntaxTokenList modifiers)
        {
            return new UnionDeclarationSyntaxWrapper(WithModifiersAccessor(this.node!, modifiers));
        }

        public UnionDeclarationSyntaxWrapper WithKeyword(SyntaxToken keyword)
        {
            return new UnionDeclarationSyntaxWrapper(WithKeywordAccessor(this.node!, keyword));
        }

        public UnionDeclarationSyntaxWrapper WithIdentifier(SyntaxToken identifier)
        {
            return new UnionDeclarationSyntaxWrapper(WithIdentifierAccessor(this.node!, identifier));
        }

        public UnionDeclarationSyntaxWrapper WithTypeParameterList(TypeParameterListSyntax? typeParameterList)
        {
            return new UnionDeclarationSyntaxWrapper(WithTypeParameterListAccessor(this.node!, typeParameterList));
        }

        public UnionDeclarationSyntaxWrapper WithParameterList(ParameterListSyntax? parameterList)
        {
            return new UnionDeclarationSyntaxWrapper(WithParameterListAccessor(this.node!, parameterList));
        }

        public UnionDeclarationSyntaxWrapper WithBaseList(BaseListSyntax? baseList)
        {
            return new UnionDeclarationSyntaxWrapper(WithBaseListAccessor(this.node!, baseList));
        }

        public UnionDeclarationSyntaxWrapper WithConstraintClauses(SyntaxList<TypeParameterConstraintClauseSyntax> constraintClauses)
        {
            return new UnionDeclarationSyntaxWrapper(WithConstraintClausesAccessor(this.node!, constraintClauses));
        }

        public UnionDeclarationSyntaxWrapper WithOpenBraceToken(SyntaxToken openBraceToken)
        {
            return new UnionDeclarationSyntaxWrapper(WithOpenBraceTokenAccessor(this.node!, openBraceToken));
        }

        public UnionDeclarationSyntaxWrapper WithMembers(SyntaxList<MemberDeclarationSyntax> members)
        {
            return new UnionDeclarationSyntaxWrapper(WithMembersAccessor(this.node!, members));
        }

        public UnionDeclarationSyntaxWrapper WithCloseBraceToken(SyntaxToken closeBraceToken)
        {
            return new UnionDeclarationSyntaxWrapper(WithCloseBraceTokenAccessor(this.node!, closeBraceToken));
        }

        public UnionDeclarationSyntaxWrapper WithSemicolonToken(SyntaxToken semicolonToken)
        {
            return new UnionDeclarationSyntaxWrapper(WithSemicolonTokenAccessor(this.node!, semicolonToken));
        }
    }
}
