// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal readonly partial struct ExtensionMemberCrefSyntaxWrapper : ISyntaxWrapper<MemberCrefSyntax>
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.CSharp.Syntax.ExtensionMemberCrefSyntax";
        private static readonly Type WrappedType;

        private static readonly Func<MemberCrefSyntax, SyntaxToken> ExtensionKeywordAccessor;
        private static readonly Func<MemberCrefSyntax, TypeArgumentListSyntax> TypeArgumentListAccessor;
        private static readonly Func<MemberCrefSyntax, CrefParameterListSyntax> ParametersAccessor;
        private static readonly Func<MemberCrefSyntax, SyntaxToken> DotTokenAccessor;
        private static readonly Func<MemberCrefSyntax, MemberCrefSyntax> MemberAccessor;
        private static readonly Func<MemberCrefSyntax, SyntaxToken, MemberCrefSyntax> WithExtensionKeywordAccessor;
        private static readonly Func<MemberCrefSyntax, TypeArgumentListSyntax, MemberCrefSyntax> WithTypeArgumentListAccessor;
        private static readonly Func<MemberCrefSyntax, CrefParameterListSyntax, MemberCrefSyntax> WithParametersAccessor;
        private static readonly Func<MemberCrefSyntax, SyntaxToken, MemberCrefSyntax> WithDotTokenAccessor;
        private static readonly Func<MemberCrefSyntax, MemberCrefSyntax, MemberCrefSyntax> WithMemberAccessor;

        private readonly MemberCrefSyntax node;

        static ExtensionMemberCrefSyntaxWrapper()
        {
            WrappedType = SyntaxWrapperHelper.GetWrappedType(typeof(ExtensionMemberCrefSyntaxWrapper));
            ExtensionKeywordAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<MemberCrefSyntax, SyntaxToken>(WrappedType, nameof(ExtensionKeyword));
            TypeArgumentListAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<MemberCrefSyntax, TypeArgumentListSyntax>(WrappedType, nameof(TypeArgumentList));
            ParametersAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<MemberCrefSyntax, CrefParameterListSyntax>(WrappedType, nameof(Parameters));
            DotTokenAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<MemberCrefSyntax, SyntaxToken>(WrappedType, nameof(DotToken));
            MemberAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<MemberCrefSyntax, MemberCrefSyntax>(WrappedType, nameof(Member));
            WithExtensionKeywordAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<MemberCrefSyntax, SyntaxToken>(WrappedType, nameof(ExtensionKeyword));
            WithTypeArgumentListAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<MemberCrefSyntax, TypeArgumentListSyntax>(WrappedType, nameof(TypeArgumentList));
            WithParametersAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<MemberCrefSyntax, CrefParameterListSyntax>(WrappedType, nameof(Parameters));
            WithDotTokenAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<MemberCrefSyntax, SyntaxToken>(WrappedType, nameof(DotToken));
            WithMemberAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<MemberCrefSyntax, MemberCrefSyntax>(WrappedType, nameof(Member));
        }

        private ExtensionMemberCrefSyntaxWrapper(MemberCrefSyntax node)
        {
            this.node = node;
        }

        public MemberCrefSyntax SyntaxNode => this.node;

        public SyntaxToken ExtensionKeyword
        {
            get
            {
                return ExtensionKeywordAccessor(this.SyntaxNode);
            }
        }

        public TypeArgumentListSyntax TypeArgumentList
        {
            get
            {
                return TypeArgumentListAccessor(this.SyntaxNode);
            }
        }

        public CrefParameterListSyntax Parameters
        {
            get
            {
                return ParametersAccessor(this.SyntaxNode);
            }
        }

        public SyntaxToken DotToken
        {
            get
            {
                return DotTokenAccessor(this.SyntaxNode);
            }
        }

        public MemberCrefSyntax Member
        {
            get
            {
                return MemberAccessor(this.SyntaxNode);
            }
        }

        public static explicit operator ExtensionMemberCrefSyntaxWrapper(SyntaxNode node)
        {
            if (node == null)
            {
                return default;
            }

            if (!IsInstance(node))
            {
                throw new InvalidCastException($"Cannot cast '{node.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new ExtensionMemberCrefSyntaxWrapper((MemberCrefSyntax)node);
        }

        public static implicit operator MemberCrefSyntax(ExtensionMemberCrefSyntaxWrapper wrapper)
        {
            return wrapper.node;
        }

        public static bool IsInstance(SyntaxNode node)
        {
            return node != null && LightupHelpers.CanWrapNode(node, WrappedType);
        }

        public ExtensionMemberCrefSyntaxWrapper WithExtensionKeyword(SyntaxToken extensionKeyword)
        {
            return new ExtensionMemberCrefSyntaxWrapper(WithExtensionKeywordAccessor(this.SyntaxNode, extensionKeyword));
        }

        public ExtensionMemberCrefSyntaxWrapper WithTypeArgumentList(TypeArgumentListSyntax typeArgumentList)
        {
            return new ExtensionMemberCrefSyntaxWrapper(WithTypeArgumentListAccessor(this.SyntaxNode, typeArgumentList));
        }

        public ExtensionMemberCrefSyntaxWrapper WithParameters(CrefParameterListSyntax parameters)
        {
            return new ExtensionMemberCrefSyntaxWrapper(WithParametersAccessor(this.SyntaxNode, parameters));
        }

        public ExtensionMemberCrefSyntaxWrapper WithDotToken(SyntaxToken dotToken)
        {
            return new ExtensionMemberCrefSyntaxWrapper(WithDotTokenAccessor(this.SyntaxNode, dotToken));
        }

        public ExtensionMemberCrefSyntaxWrapper WithMember(MemberCrefSyntax member)
        {
            return new ExtensionMemberCrefSyntaxWrapper(WithMemberAccessor(this.SyntaxNode, member));
        }
    }
}
