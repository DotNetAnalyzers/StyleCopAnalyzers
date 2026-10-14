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

    internal readonly partial struct NullableDirectiveTriviaSyntaxWrapper : ISyntaxWrapper<DirectiveTriviaSyntax>
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.CSharp.Syntax.NullableDirectiveTriviaSyntax";
        private static readonly Type? WrappedType;

        private static readonly Func<DirectiveTriviaSyntax, SyntaxToken> NullableKeywordAccessor;
        private static readonly Func<DirectiveTriviaSyntax, SyntaxToken> SettingTokenAccessor;
        private static readonly Func<DirectiveTriviaSyntax, SyntaxToken> TargetTokenAccessor;
        private static readonly Func<DirectiveTriviaSyntax, SyntaxToken, DirectiveTriviaSyntax> WithHashTokenAccessor;
        private static readonly Func<DirectiveTriviaSyntax, SyntaxToken, DirectiveTriviaSyntax> WithNullableKeywordAccessor;
        private static readonly Func<DirectiveTriviaSyntax, SyntaxToken, DirectiveTriviaSyntax> WithSettingTokenAccessor;
        private static readonly Func<DirectiveTriviaSyntax, SyntaxToken, DirectiveTriviaSyntax> WithTargetTokenAccessor;
        private static readonly Func<DirectiveTriviaSyntax, SyntaxToken, DirectiveTriviaSyntax> WithEndOfDirectiveTokenAccessor;
        private static readonly Func<DirectiveTriviaSyntax, bool, DirectiveTriviaSyntax> WithIsActiveAccessor;

        private readonly DirectiveTriviaSyntax? node;

        static NullableDirectiveTriviaSyntaxWrapper()
        {
            WrappedType = SyntaxWrapperHelper.GetWrappedType(typeof(NullableDirectiveTriviaSyntaxWrapper));
            NullableKeywordAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<DirectiveTriviaSyntax, SyntaxToken>(WrappedType, nameof(NullableKeyword));
            SettingTokenAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<DirectiveTriviaSyntax, SyntaxToken>(WrappedType, nameof(SettingToken));
            TargetTokenAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<DirectiveTriviaSyntax, SyntaxToken>(WrappedType, nameof(TargetToken));
            WithHashTokenAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<DirectiveTriviaSyntax, SyntaxToken>(WrappedType, nameof(HashToken));
            WithNullableKeywordAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<DirectiveTriviaSyntax, SyntaxToken>(WrappedType, nameof(NullableKeyword));
            WithSettingTokenAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<DirectiveTriviaSyntax, SyntaxToken>(WrappedType, nameof(SettingToken));
            WithTargetTokenAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<DirectiveTriviaSyntax, SyntaxToken>(WrappedType, nameof(TargetToken));
            WithEndOfDirectiveTokenAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<DirectiveTriviaSyntax, SyntaxToken>(WrappedType, nameof(EndOfDirectiveToken));
            WithIsActiveAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<DirectiveTriviaSyntax, bool>(WrappedType, nameof(IsActive));
        }

        private NullableDirectiveTriviaSyntaxWrapper(DirectiveTriviaSyntax? node)
        {
            this.node = node;
        }

        public DirectiveTriviaSyntax? SyntaxNode => this.node;


        public SyntaxToken HashToken
        {
            get
            {
                return this.node!.HashToken;
            }
        }

        public SyntaxToken NullableKeyword
        {
            get
            {
                return NullableKeywordAccessor(this.node!);
            }
        }

        public SyntaxToken SettingToken
        {
            get
            {
                return SettingTokenAccessor(this.node!);
            }
        }

        public SyntaxToken TargetToken
        {
            get
            {
                return TargetTokenAccessor(this.node!);
            }
        }

        public SyntaxToken EndOfDirectiveToken
        {
            get
            {
                return this.node!.EndOfDirectiveToken;
            }
        }

        public bool IsActive
        {
            get
            {
                return this.node!.IsActive;
            }
        }

        public static explicit operator NullableDirectiveTriviaSyntaxWrapper(SyntaxNode? node)
        {
            if (node == null)
            {
                return default;
            }

            if (!IsInstance(node))
            {
                throw new InvalidCastException($"Cannot cast '{node.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new NullableDirectiveTriviaSyntaxWrapper((DirectiveTriviaSyntax)node);
        }

        public static implicit operator DirectiveTriviaSyntax? (NullableDirectiveTriviaSyntaxWrapper wrapper)
        {
            return wrapper.node;
        }

        public static bool IsInstance([NotNullWhen(true)] SyntaxNode? node)
        {
            return node != null && LightupHelpers.CanWrapNode(node, WrappedType);
        }

        public NullableDirectiveTriviaSyntaxWrapper WithHashToken(SyntaxToken hashToken)
        {
            return new NullableDirectiveTriviaSyntaxWrapper(WithHashTokenAccessor(this.node!, hashToken));
        }

        public NullableDirectiveTriviaSyntaxWrapper WithNullableKeyword(SyntaxToken nullableKeyword)
        {
            return new NullableDirectiveTriviaSyntaxWrapper(WithNullableKeywordAccessor(this.node!, nullableKeyword));
        }

        public NullableDirectiveTriviaSyntaxWrapper WithSettingToken(SyntaxToken settingToken)
        {
            return new NullableDirectiveTriviaSyntaxWrapper(WithSettingTokenAccessor(this.node!, settingToken));
        }

        public NullableDirectiveTriviaSyntaxWrapper WithTargetToken(SyntaxToken targetToken)
        {
            return new NullableDirectiveTriviaSyntaxWrapper(WithTargetTokenAccessor(this.node!, targetToken));
        }

        public NullableDirectiveTriviaSyntaxWrapper WithEndOfDirectiveToken(SyntaxToken endOfDirectiveToken)
        {
            return new NullableDirectiveTriviaSyntaxWrapper(WithEndOfDirectiveTokenAccessor(this.node!, endOfDirectiveToken));
        }

        public NullableDirectiveTriviaSyntaxWrapper WithIsActive(bool isActive)
        {
            return new NullableDirectiveTriviaSyntaxWrapper(WithIsActiveAccessor(this.node!, isActive));
        }
    }
}
