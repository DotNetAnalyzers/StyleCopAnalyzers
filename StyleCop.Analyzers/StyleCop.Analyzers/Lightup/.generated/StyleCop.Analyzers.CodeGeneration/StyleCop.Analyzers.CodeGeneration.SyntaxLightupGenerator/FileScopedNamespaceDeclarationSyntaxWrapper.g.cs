// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal readonly partial struct FileScopedNamespaceDeclarationSyntaxWrapper : ISyntaxWrapper<MemberDeclarationSyntax>
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.CSharp.Syntax.FileScopedNamespaceDeclarationSyntax";
        private static readonly Type WrappedType;

        private static readonly Func<MemberDeclarationSyntax, SyntaxToken> SemicolonTokenAccessor;
        private static readonly Func<MemberDeclarationSyntax, SyntaxList<AttributeListSyntax>, MemberDeclarationSyntax> WithAttributeListsAccessor;
        private static readonly Func<MemberDeclarationSyntax, SyntaxTokenList, MemberDeclarationSyntax> WithModifiersAccessor;
        private static readonly Func<MemberDeclarationSyntax, SyntaxToken, MemberDeclarationSyntax> WithNamespaceKeywordAccessor;
        private static readonly Func<MemberDeclarationSyntax, NameSyntax, MemberDeclarationSyntax> WithNameAccessor;
        private static readonly Func<MemberDeclarationSyntax, SyntaxToken, MemberDeclarationSyntax> WithSemicolonTokenAccessor;
        private static readonly Func<MemberDeclarationSyntax, SyntaxList<ExternAliasDirectiveSyntax>, MemberDeclarationSyntax> WithExternsAccessor;
        private static readonly Func<MemberDeclarationSyntax, SyntaxList<UsingDirectiveSyntax>, MemberDeclarationSyntax> WithUsingsAccessor;
        private static readonly Func<MemberDeclarationSyntax, SyntaxList<MemberDeclarationSyntax>, MemberDeclarationSyntax> WithMembersAccessor;

        private readonly MemberDeclarationSyntax node;

        static FileScopedNamespaceDeclarationSyntaxWrapper()
        {
            WrappedType = SyntaxWrapperHelper.GetWrappedType(typeof(FileScopedNamespaceDeclarationSyntaxWrapper));
            SemicolonTokenAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<MemberDeclarationSyntax, SyntaxToken>(WrappedType, nameof(SemicolonToken));
            WithAttributeListsAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<MemberDeclarationSyntax, SyntaxList<AttributeListSyntax>>(WrappedType, nameof(AttributeLists));
            WithModifiersAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<MemberDeclarationSyntax, SyntaxTokenList>(WrappedType, nameof(Modifiers));
            WithNamespaceKeywordAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<MemberDeclarationSyntax, SyntaxToken>(WrappedType, nameof(NamespaceKeyword));
            WithNameAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<MemberDeclarationSyntax, NameSyntax>(WrappedType, nameof(Name));
            WithSemicolonTokenAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<MemberDeclarationSyntax, SyntaxToken>(WrappedType, nameof(SemicolonToken));
            WithExternsAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<MemberDeclarationSyntax, SyntaxList<ExternAliasDirectiveSyntax>>(WrappedType, nameof(Externs));
            WithUsingsAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<MemberDeclarationSyntax, SyntaxList<UsingDirectiveSyntax>>(WrappedType, nameof(Usings));
            WithMembersAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<MemberDeclarationSyntax, SyntaxList<MemberDeclarationSyntax>>(WrappedType, nameof(Members));
        }

        private FileScopedNamespaceDeclarationSyntaxWrapper(MemberDeclarationSyntax node)
        {
            this.node = node;
        }

        public MemberDeclarationSyntax SyntaxNode => this.node;


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

        public SyntaxToken NamespaceKeyword
        {
            get
            {
                return ((BaseNamespaceDeclarationSyntaxWrapper)this).NamespaceKeyword;
            }
        }

        public NameSyntax Name
        {
            get
            {
                return ((BaseNamespaceDeclarationSyntaxWrapper)this).Name;
            }
        }

        public SyntaxToken SemicolonToken
        {
            get
            {
                return SemicolonTokenAccessor(this.SyntaxNode);
            }
        }

        public SyntaxList<ExternAliasDirectiveSyntax> Externs
        {
            get
            {
                return ((BaseNamespaceDeclarationSyntaxWrapper)this).Externs;
            }
        }

        public SyntaxList<UsingDirectiveSyntax> Usings
        {
            get
            {
                return ((BaseNamespaceDeclarationSyntaxWrapper)this).Usings;
            }
        }

        public SyntaxList<MemberDeclarationSyntax> Members
        {
            get
            {
                return ((BaseNamespaceDeclarationSyntaxWrapper)this).Members;
            }
        }

        public static explicit operator FileScopedNamespaceDeclarationSyntaxWrapper(BaseNamespaceDeclarationSyntaxWrapper node)
        {
            return (FileScopedNamespaceDeclarationSyntaxWrapper)node.SyntaxNode;
        }

        public static explicit operator FileScopedNamespaceDeclarationSyntaxWrapper(SyntaxNode node)
        {
            if (node == null)
            {
                return default;
            }

            if (!IsInstance(node))
            {
                throw new InvalidCastException($"Cannot cast '{node.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new FileScopedNamespaceDeclarationSyntaxWrapper((MemberDeclarationSyntax)node);
        }

        public static implicit operator BaseNamespaceDeclarationSyntaxWrapper(FileScopedNamespaceDeclarationSyntaxWrapper wrapper)
        {
            return BaseNamespaceDeclarationSyntaxWrapper.FromUpcast(wrapper.node);
        }

        public static implicit operator MemberDeclarationSyntax(FileScopedNamespaceDeclarationSyntaxWrapper wrapper)
        {
            return wrapper.node;
        }

        public static bool IsInstance(SyntaxNode node)
        {
            return node != null && LightupHelpers.CanWrapNode(node, WrappedType);
        }

        public FileScopedNamespaceDeclarationSyntaxWrapper WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists)
        {
            return new FileScopedNamespaceDeclarationSyntaxWrapper(WithAttributeListsAccessor(this.SyntaxNode, attributeLists));
        }

        public FileScopedNamespaceDeclarationSyntaxWrapper WithModifiers(SyntaxTokenList modifiers)
        {
            return new FileScopedNamespaceDeclarationSyntaxWrapper(WithModifiersAccessor(this.SyntaxNode, modifiers));
        }

        public FileScopedNamespaceDeclarationSyntaxWrapper WithNamespaceKeyword(SyntaxToken namespaceKeyword)
        {
            return new FileScopedNamespaceDeclarationSyntaxWrapper(WithNamespaceKeywordAccessor(this.SyntaxNode, namespaceKeyword));
        }

        public FileScopedNamespaceDeclarationSyntaxWrapper WithName(NameSyntax name)
        {
            return new FileScopedNamespaceDeclarationSyntaxWrapper(WithNameAccessor(this.SyntaxNode, name));
        }

        public FileScopedNamespaceDeclarationSyntaxWrapper WithSemicolonToken(SyntaxToken semicolonToken)
        {
            return new FileScopedNamespaceDeclarationSyntaxWrapper(WithSemicolonTokenAccessor(this.SyntaxNode, semicolonToken));
        }

        public FileScopedNamespaceDeclarationSyntaxWrapper WithExterns(SyntaxList<ExternAliasDirectiveSyntax> externs)
        {
            return new FileScopedNamespaceDeclarationSyntaxWrapper(WithExternsAccessor(this.SyntaxNode, externs));
        }

        public FileScopedNamespaceDeclarationSyntaxWrapper WithUsings(SyntaxList<UsingDirectiveSyntax> usings)
        {
            return new FileScopedNamespaceDeclarationSyntaxWrapper(WithUsingsAccessor(this.SyntaxNode, usings));
        }

        public FileScopedNamespaceDeclarationSyntaxWrapper WithMembers(SyntaxList<MemberDeclarationSyntax> members)
        {
            return new FileScopedNamespaceDeclarationSyntaxWrapper(WithMembersAccessor(this.SyntaxNode, members));
        }
    }
}
