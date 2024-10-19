﻿// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal readonly partial struct RecordDeclarationSyntaxWrapper : ISyntaxWrapper<TypeDeclarationSyntax>
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.CSharp.Syntax.RecordDeclarationSyntax";
        private static readonly Type WrappedType;

        private static readonly Func<TypeDeclarationSyntax, SyntaxToken> ClassOrStructKeywordAccessor;
        private static readonly Func<TypeDeclarationSyntax, ParameterListSyntax> ParameterListAccessor;
        private static readonly Func<TypeDeclarationSyntax, SyntaxList<AttributeListSyntax>, TypeDeclarationSyntax> WithAttributeListsAccessor;
        private static readonly Func<TypeDeclarationSyntax, SyntaxTokenList, TypeDeclarationSyntax> WithModifiersAccessor;
        private static readonly Func<TypeDeclarationSyntax, SyntaxToken, TypeDeclarationSyntax> WithKeywordAccessor;
        private static readonly Func<TypeDeclarationSyntax, SyntaxToken, TypeDeclarationSyntax> WithClassOrStructKeywordAccessor;
        private static readonly Func<TypeDeclarationSyntax, SyntaxToken, TypeDeclarationSyntax> WithIdentifierAccessor;
        private static readonly Func<TypeDeclarationSyntax, TypeParameterListSyntax, TypeDeclarationSyntax> WithTypeParameterListAccessor;
        private static readonly Func<TypeDeclarationSyntax, ParameterListSyntax, TypeDeclarationSyntax> WithParameterListAccessor;
        private static readonly Func<TypeDeclarationSyntax, BaseListSyntax, TypeDeclarationSyntax> WithBaseListAccessor;
        private static readonly Func<TypeDeclarationSyntax, SyntaxList<TypeParameterConstraintClauseSyntax>, TypeDeclarationSyntax> WithConstraintClausesAccessor;
        private static readonly Func<TypeDeclarationSyntax, SyntaxToken, TypeDeclarationSyntax> WithOpenBraceTokenAccessor;
        private static readonly Func<TypeDeclarationSyntax, SyntaxList<MemberDeclarationSyntax>, TypeDeclarationSyntax> WithMembersAccessor;
        private static readonly Func<TypeDeclarationSyntax, SyntaxToken, TypeDeclarationSyntax> WithCloseBraceTokenAccessor;
        private static readonly Func<TypeDeclarationSyntax, SyntaxToken, TypeDeclarationSyntax> WithSemicolonTokenAccessor;

        private readonly TypeDeclarationSyntax node;

        static RecordDeclarationSyntaxWrapper()
        {
            WrappedType = SyntaxWrapperHelper.GetWrappedType(typeof(RecordDeclarationSyntaxWrapper));
            ClassOrStructKeywordAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<TypeDeclarationSyntax, SyntaxToken>(WrappedType, nameof(ClassOrStructKeyword));
            ParameterListAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<TypeDeclarationSyntax, ParameterListSyntax>(WrappedType, nameof(ParameterList));
            WithAttributeListsAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<TypeDeclarationSyntax, SyntaxList<AttributeListSyntax>>(WrappedType, nameof(AttributeLists));
            WithModifiersAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<TypeDeclarationSyntax, SyntaxTokenList>(WrappedType, nameof(Modifiers));
            WithKeywordAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<TypeDeclarationSyntax, SyntaxToken>(WrappedType, nameof(Keyword));
            WithClassOrStructKeywordAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<TypeDeclarationSyntax, SyntaxToken>(WrappedType, nameof(ClassOrStructKeyword));
            WithIdentifierAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<TypeDeclarationSyntax, SyntaxToken>(WrappedType, nameof(Identifier));
            WithTypeParameterListAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<TypeDeclarationSyntax, TypeParameterListSyntax>(WrappedType, nameof(TypeParameterList));
            WithParameterListAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<TypeDeclarationSyntax, ParameterListSyntax>(WrappedType, nameof(ParameterList));
            WithBaseListAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<TypeDeclarationSyntax, BaseListSyntax>(WrappedType, nameof(BaseList));
            WithConstraintClausesAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<TypeDeclarationSyntax, SyntaxList<TypeParameterConstraintClauseSyntax>>(WrappedType, nameof(ConstraintClauses));
            WithOpenBraceTokenAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<TypeDeclarationSyntax, SyntaxToken>(WrappedType, nameof(OpenBraceToken));
            WithMembersAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<TypeDeclarationSyntax, SyntaxList<MemberDeclarationSyntax>>(WrappedType, nameof(Members));
            WithCloseBraceTokenAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<TypeDeclarationSyntax, SyntaxToken>(WrappedType, nameof(CloseBraceToken));
            WithSemicolonTokenAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<TypeDeclarationSyntax, SyntaxToken>(WrappedType, nameof(SemicolonToken));
        }

        private RecordDeclarationSyntaxWrapper(TypeDeclarationSyntax node)
        {
            this.node = node;
        }

        public TypeDeclarationSyntax SyntaxNode => this.node;

        public SyntaxList<AttributeListSyntax> AttributeLists
        {
            get
            {
                return this.SyntaxNode.AttributeLists();
            }
        }

        public SyntaxTokenList Modifiers
        {
            get
            {
                return this.SyntaxNode.Modifiers();
            }
        }

        public SyntaxToken Keyword
        {
            get
            {
                return this.SyntaxNode.Keyword;
            }
        }

        public SyntaxToken ClassOrStructKeyword
        {
            get
            {
                return ClassOrStructKeywordAccessor(this.SyntaxNode);
            }
        }

        public SyntaxToken Identifier
        {
            get
            {
                return this.SyntaxNode.Identifier;
            }
        }

        public TypeParameterListSyntax TypeParameterList
        {
            get
            {
                return this.SyntaxNode.TypeParameterList;
            }
        }

        public ParameterListSyntax ParameterList
        {
            get
            {
                return ParameterListAccessor(this.SyntaxNode);
            }
        }

        public BaseListSyntax BaseList
        {
            get
            {
                return this.SyntaxNode.BaseList;
            }
        }

        public SyntaxList<TypeParameterConstraintClauseSyntax> ConstraintClauses
        {
            get
            {
                return this.SyntaxNode.ConstraintClauses;
            }
        }

        public SyntaxToken OpenBraceToken
        {
            get
            {
                return this.SyntaxNode.OpenBraceToken;
            }
        }

        public SyntaxList<MemberDeclarationSyntax> Members
        {
            get
            {
                return this.SyntaxNode.Members;
            }
        }

        public SyntaxToken CloseBraceToken
        {
            get
            {
                return this.SyntaxNode.CloseBraceToken;
            }
        }

        public SyntaxToken SemicolonToken
        {
            get
            {
                return this.SyntaxNode.SemicolonToken;
            }
        }

        public static explicit operator RecordDeclarationSyntaxWrapper(SyntaxNode node)
        {
            if (node == null)
            {
                return default;
            }

            if (!IsInstance(node))
            {
                throw new InvalidCastException($"Cannot cast '{node.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new RecordDeclarationSyntaxWrapper((TypeDeclarationSyntax)node);
        }

        public static implicit operator TypeDeclarationSyntax(RecordDeclarationSyntaxWrapper wrapper)
        {
            return wrapper.node;
        }

        public static bool IsInstance(SyntaxNode node)
        {
            return node != null && LightupHelpers.CanWrapNode(node, WrappedType);
        }

        public RecordDeclarationSyntaxWrapper WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists)
        {
            return new RecordDeclarationSyntaxWrapper(WithAttributeListsAccessor(this.SyntaxNode, attributeLists));
        }

        public RecordDeclarationSyntaxWrapper WithModifiers(SyntaxTokenList modifiers)
        {
            return new RecordDeclarationSyntaxWrapper(WithModifiersAccessor(this.SyntaxNode, modifiers));
        }

        public RecordDeclarationSyntaxWrapper WithKeyword(SyntaxToken keyword)
        {
            return new RecordDeclarationSyntaxWrapper(WithKeywordAccessor(this.SyntaxNode, keyword));
        }

        public RecordDeclarationSyntaxWrapper WithClassOrStructKeyword(SyntaxToken classOrStructKeyword)
        {
            return new RecordDeclarationSyntaxWrapper(WithClassOrStructKeywordAccessor(this.SyntaxNode, classOrStructKeyword));
        }

        public RecordDeclarationSyntaxWrapper WithIdentifier(SyntaxToken identifier)
        {
            return new RecordDeclarationSyntaxWrapper(WithIdentifierAccessor(this.SyntaxNode, identifier));
        }

        public RecordDeclarationSyntaxWrapper WithTypeParameterList(TypeParameterListSyntax typeParameterList)
        {
            return new RecordDeclarationSyntaxWrapper(WithTypeParameterListAccessor(this.SyntaxNode, typeParameterList));
        }

        public RecordDeclarationSyntaxWrapper WithParameterList(ParameterListSyntax parameterList)
        {
            return new RecordDeclarationSyntaxWrapper(WithParameterListAccessor(this.SyntaxNode, parameterList));
        }

        public RecordDeclarationSyntaxWrapper WithBaseList(BaseListSyntax baseList)
        {
            return new RecordDeclarationSyntaxWrapper(WithBaseListAccessor(this.SyntaxNode, baseList));
        }

        public RecordDeclarationSyntaxWrapper WithConstraintClauses(SyntaxList<TypeParameterConstraintClauseSyntax> constraintClauses)
        {
            return new RecordDeclarationSyntaxWrapper(WithConstraintClausesAccessor(this.SyntaxNode, constraintClauses));
        }

        public RecordDeclarationSyntaxWrapper WithOpenBraceToken(SyntaxToken openBraceToken)
        {
            return new RecordDeclarationSyntaxWrapper(WithOpenBraceTokenAccessor(this.SyntaxNode, openBraceToken));
        }

        public RecordDeclarationSyntaxWrapper WithMembers(SyntaxList<MemberDeclarationSyntax> members)
        {
            return new RecordDeclarationSyntaxWrapper(WithMembersAccessor(this.SyntaxNode, members));
        }

        public RecordDeclarationSyntaxWrapper WithCloseBraceToken(SyntaxToken closeBraceToken)
        {
            return new RecordDeclarationSyntaxWrapper(WithCloseBraceTokenAccessor(this.SyntaxNode, closeBraceToken));
        }

        public RecordDeclarationSyntaxWrapper WithSemicolonToken(SyntaxToken semicolonToken)
        {
            return new RecordDeclarationSyntaxWrapper(WithSemicolonTokenAccessor(this.SyntaxNode, semicolonToken));
        }
    }
}
