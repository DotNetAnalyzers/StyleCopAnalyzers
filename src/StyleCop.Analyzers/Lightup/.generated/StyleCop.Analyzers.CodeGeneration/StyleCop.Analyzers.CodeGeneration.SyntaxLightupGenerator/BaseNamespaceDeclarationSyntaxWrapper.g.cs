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

    internal readonly partial struct BaseNamespaceDeclarationSyntaxWrapper : ISyntaxWrapper<MemberDeclarationSyntax>
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.CSharp.Syntax.BaseNamespaceDeclarationSyntax";
        private static readonly Type? WrappedType;

        private static readonly Func<MemberDeclarationSyntax, SyntaxToken> NamespaceKeywordAccessor;
        private static readonly Func<MemberDeclarationSyntax, NameSyntax> NameAccessor;
        private static readonly Func<MemberDeclarationSyntax, SyntaxList<ExternAliasDirectiveSyntax>> ExternsAccessor;
        private static readonly Func<MemberDeclarationSyntax, SyntaxList<UsingDirectiveSyntax>> UsingsAccessor;
        private static readonly Func<MemberDeclarationSyntax, SyntaxList<MemberDeclarationSyntax>> MembersAccessor;
        private static readonly Func<MemberDeclarationSyntax, SyntaxToken, MemberDeclarationSyntax> WithNamespaceKeywordAccessor;
        private static readonly Func<MemberDeclarationSyntax, NameSyntax, MemberDeclarationSyntax> WithNameAccessor;
        private static readonly Func<MemberDeclarationSyntax, SyntaxList<ExternAliasDirectiveSyntax>, MemberDeclarationSyntax> WithExternsAccessor;
        private static readonly Func<MemberDeclarationSyntax, SyntaxList<UsingDirectiveSyntax>, MemberDeclarationSyntax> WithUsingsAccessor;
        private static readonly Func<MemberDeclarationSyntax, SyntaxList<MemberDeclarationSyntax>, MemberDeclarationSyntax> WithMembersAccessor;

        private readonly MemberDeclarationSyntax? node;

        static BaseNamespaceDeclarationSyntaxWrapper()
        {
            WrappedType = SyntaxWrapperHelper.GetWrappedType(typeof(BaseNamespaceDeclarationSyntaxWrapper));
            NamespaceKeywordAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<MemberDeclarationSyntax, SyntaxToken>(WrappedType, nameof(NamespaceKeyword));
            NameAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<MemberDeclarationSyntax, NameSyntax>(WrappedType, nameof(Name));
            ExternsAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<MemberDeclarationSyntax, SyntaxList<ExternAliasDirectiveSyntax>>(WrappedType, nameof(Externs));
            UsingsAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<MemberDeclarationSyntax, SyntaxList<UsingDirectiveSyntax>>(WrappedType, nameof(Usings));
            MembersAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<MemberDeclarationSyntax, SyntaxList<MemberDeclarationSyntax>>(WrappedType, nameof(Members));
            WithNamespaceKeywordAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<MemberDeclarationSyntax, SyntaxToken>(WrappedType, nameof(NamespaceKeyword));
            WithNameAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<MemberDeclarationSyntax, NameSyntax>(WrappedType, nameof(Name));
            WithExternsAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<MemberDeclarationSyntax, SyntaxList<ExternAliasDirectiveSyntax>>(WrappedType, nameof(Externs));
            WithUsingsAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<MemberDeclarationSyntax, SyntaxList<UsingDirectiveSyntax>>(WrappedType, nameof(Usings));
            WithMembersAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<MemberDeclarationSyntax, SyntaxList<MemberDeclarationSyntax>>(WrappedType, nameof(Members));
        }

        private BaseNamespaceDeclarationSyntaxWrapper(MemberDeclarationSyntax? node)
        {
            this.node = node;
        }

        public MemberDeclarationSyntax? SyntaxNode => this.node;


        public SyntaxToken NamespaceKeyword
        {
            get
            {
                return NamespaceKeywordAccessor(this.node!);
            }
        }

        public NameSyntax Name
        {
            get
            {
                return NameAccessor(this.node!);
            }
        }

        public SyntaxList<ExternAliasDirectiveSyntax> Externs
        {
            get
            {
                return ExternsAccessor(this.node!);
            }
        }

        public SyntaxList<UsingDirectiveSyntax> Usings
        {
            get
            {
                return UsingsAccessor(this.node!);
            }
        }

        public SyntaxList<MemberDeclarationSyntax> Members
        {
            get
            {
                return MembersAccessor(this.node!);
            }
        }

        public static explicit operator BaseNamespaceDeclarationSyntaxWrapper(SyntaxNode? node)
        {
            if (node == null)
            {
                return default;
            }

            if (!IsInstance(node))
            {
                throw new InvalidCastException($"Cannot cast '{node.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new BaseNamespaceDeclarationSyntaxWrapper((MemberDeclarationSyntax)node);
        }

        public static implicit operator MemberDeclarationSyntax? (BaseNamespaceDeclarationSyntaxWrapper wrapper)
        {
            return wrapper.node;
        }

        public static bool IsInstance([NotNullWhen(true)] SyntaxNode? node)
        {
            return node != null && LightupHelpers.CanWrapNode(node, WrappedType);
        }

        public BaseNamespaceDeclarationSyntaxWrapper WithNamespaceKeyword(SyntaxToken namespaceKeyword)
        {
            return new BaseNamespaceDeclarationSyntaxWrapper(WithNamespaceKeywordAccessor(this.node!, namespaceKeyword));
        }

        public BaseNamespaceDeclarationSyntaxWrapper WithName(NameSyntax name)
        {
            return new BaseNamespaceDeclarationSyntaxWrapper(WithNameAccessor(this.node!, name));
        }

        public BaseNamespaceDeclarationSyntaxWrapper WithExterns(SyntaxList<ExternAliasDirectiveSyntax> externs)
        {
            return new BaseNamespaceDeclarationSyntaxWrapper(WithExternsAccessor(this.node!, externs));
        }

        public BaseNamespaceDeclarationSyntaxWrapper WithUsings(SyntaxList<UsingDirectiveSyntax> usings)
        {
            return new BaseNamespaceDeclarationSyntaxWrapper(WithUsingsAccessor(this.node!, usings));
        }

        public BaseNamespaceDeclarationSyntaxWrapper WithMembers(SyntaxList<MemberDeclarationSyntax> members)
        {
            return new BaseNamespaceDeclarationSyntaxWrapper(WithMembersAccessor(this.node!, members));
        }

        internal static BaseNamespaceDeclarationSyntaxWrapper FromUpcast(MemberDeclarationSyntax? node)
        {
            return new BaseNamespaceDeclarationSyntaxWrapper(node);
        }
    }
}
