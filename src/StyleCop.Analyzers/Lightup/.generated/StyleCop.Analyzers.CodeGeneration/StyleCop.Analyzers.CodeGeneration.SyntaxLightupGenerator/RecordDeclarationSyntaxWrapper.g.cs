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

    internal readonly partial struct RecordDeclarationSyntaxWrapper : ISyntaxWrapper<TypeDeclarationSyntax>
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.CSharp.Syntax.RecordDeclarationSyntax";
        private static readonly Type? WrappedType;

        private static readonly Func<TypeDeclarationSyntax, SyntaxToken> ClassOrStructKeywordAccessor;
        private static readonly Func<TypeDeclarationSyntax, ParameterListSyntax?> ParameterListAccessor;
        private static readonly Func<TypeDeclarationSyntax, SyntaxList<AttributeListSyntax>, TypeDeclarationSyntax> WithAttributeListsAccessor;
        private static readonly Func<TypeDeclarationSyntax, SyntaxTokenList, TypeDeclarationSyntax> WithModifiersAccessor;
        private static readonly Func<TypeDeclarationSyntax, SyntaxToken, TypeDeclarationSyntax> WithKeywordAccessor;
        private static readonly Func<TypeDeclarationSyntax, SyntaxToken, TypeDeclarationSyntax> WithClassOrStructKeywordAccessor;
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

        static RecordDeclarationSyntaxWrapper()
        {
            WrappedType = SyntaxWrapperHelper.GetWrappedType(typeof(RecordDeclarationSyntaxWrapper));
            ClassOrStructKeywordAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<TypeDeclarationSyntax, SyntaxToken>(WrappedType, nameof(ClassOrStructKeyword));
            ParameterListAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<TypeDeclarationSyntax, ParameterListSyntax?>(WrappedType, nameof(ParameterList));
            WithAttributeListsAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<TypeDeclarationSyntax, SyntaxList<AttributeListSyntax>>(WrappedType, nameof(AttributeLists));
            WithModifiersAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<TypeDeclarationSyntax, SyntaxTokenList>(WrappedType, nameof(Modifiers));
            WithKeywordAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<TypeDeclarationSyntax, SyntaxToken>(WrappedType, nameof(Keyword));
            WithClassOrStructKeywordAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<TypeDeclarationSyntax, SyntaxToken>(WrappedType, nameof(ClassOrStructKeyword));
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

        private RecordDeclarationSyntaxWrapper(TypeDeclarationSyntax? node)
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

        public SyntaxToken ClassOrStructKeyword
        {
            get
            {
                return ClassOrStructKeywordAccessor(this.node!);
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

        public static explicit operator RecordDeclarationSyntaxWrapper(SyntaxNode? node)
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

        public static implicit operator TypeDeclarationSyntax? (RecordDeclarationSyntaxWrapper wrapper)
        {
            return wrapper.node;
        }

        public static bool IsInstance([NotNullWhen(true)] SyntaxNode? node)
        {
            return node != null && LightupHelpers.CanWrapNode(node, WrappedType);
        }

        public RecordDeclarationSyntaxWrapper WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists)
        {
            return new RecordDeclarationSyntaxWrapper(WithAttributeListsAccessor(this.node!, attributeLists));
        }

        public RecordDeclarationSyntaxWrapper WithModifiers(SyntaxTokenList modifiers)
        {
            return new RecordDeclarationSyntaxWrapper(WithModifiersAccessor(this.node!, modifiers));
        }

        public RecordDeclarationSyntaxWrapper WithKeyword(SyntaxToken keyword)
        {
            return new RecordDeclarationSyntaxWrapper(WithKeywordAccessor(this.node!, keyword));
        }

        public RecordDeclarationSyntaxWrapper WithClassOrStructKeyword(SyntaxToken classOrStructKeyword)
        {
            return new RecordDeclarationSyntaxWrapper(WithClassOrStructKeywordAccessor(this.node!, classOrStructKeyword));
        }

        public RecordDeclarationSyntaxWrapper WithIdentifier(SyntaxToken identifier)
        {
            return new RecordDeclarationSyntaxWrapper(WithIdentifierAccessor(this.node!, identifier));
        }

        public RecordDeclarationSyntaxWrapper WithTypeParameterList(TypeParameterListSyntax? typeParameterList)
        {
            return new RecordDeclarationSyntaxWrapper(WithTypeParameterListAccessor(this.node!, typeParameterList));
        }

        public RecordDeclarationSyntaxWrapper WithParameterList(ParameterListSyntax? parameterList)
        {
            return new RecordDeclarationSyntaxWrapper(WithParameterListAccessor(this.node!, parameterList));
        }

        public RecordDeclarationSyntaxWrapper WithBaseList(BaseListSyntax? baseList)
        {
            return new RecordDeclarationSyntaxWrapper(WithBaseListAccessor(this.node!, baseList));
        }

        public RecordDeclarationSyntaxWrapper WithConstraintClauses(SyntaxList<TypeParameterConstraintClauseSyntax> constraintClauses)
        {
            return new RecordDeclarationSyntaxWrapper(WithConstraintClausesAccessor(this.node!, constraintClauses));
        }

        public RecordDeclarationSyntaxWrapper WithOpenBraceToken(SyntaxToken openBraceToken)
        {
            return new RecordDeclarationSyntaxWrapper(WithOpenBraceTokenAccessor(this.node!, openBraceToken));
        }

        public RecordDeclarationSyntaxWrapper WithMembers(SyntaxList<MemberDeclarationSyntax> members)
        {
            return new RecordDeclarationSyntaxWrapper(WithMembersAccessor(this.node!, members));
        }

        public RecordDeclarationSyntaxWrapper WithCloseBraceToken(SyntaxToken closeBraceToken)
        {
            return new RecordDeclarationSyntaxWrapper(WithCloseBraceTokenAccessor(this.node!, closeBraceToken));
        }

        public RecordDeclarationSyntaxWrapper WithSemicolonToken(SyntaxToken semicolonToken)
        {
            return new RecordDeclarationSyntaxWrapper(WithSemicolonTokenAccessor(this.node!, semicolonToken));
        }
    }
}
