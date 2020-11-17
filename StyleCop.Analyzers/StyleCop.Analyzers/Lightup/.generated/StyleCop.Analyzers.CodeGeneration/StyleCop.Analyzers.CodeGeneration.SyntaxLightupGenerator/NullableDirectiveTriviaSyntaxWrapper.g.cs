// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal readonly partial struct NullableDirectiveTriviaSyntaxWrapper : ISyntaxWrapper<DirectiveTriviaSyntax>
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.CSharp.Syntax.NullableDirectiveTriviaSyntax";
        private static readonly Type WrappedType;
        private readonly DirectiveTriviaSyntax node;
        private static readonly Func<DirectiveTriviaSyntax, SyntaxToken> NullableKeywordAccessor;
        private static readonly Func<DirectiveTriviaSyntax, SyntaxToken> SettingTokenAccessor;
        private static readonly Func<DirectiveTriviaSyntax, SyntaxToken> TargetTokenAccessor;
        private static readonly Func<DirectiveTriviaSyntax, SyntaxToken, DirectiveTriviaSyntax> WithHashTokenAccessor;
        private static readonly Func<DirectiveTriviaSyntax, SyntaxToken, DirectiveTriviaSyntax> WithNullableKeywordAccessor;
        private static readonly Func<DirectiveTriviaSyntax, SyntaxToken, DirectiveTriviaSyntax> WithSettingTokenAccessor;
        private static readonly Func<DirectiveTriviaSyntax, SyntaxToken, DirectiveTriviaSyntax> WithTargetTokenAccessor;
        private static readonly Func<DirectiveTriviaSyntax, SyntaxToken, DirectiveTriviaSyntax> WithEndOfDirectiveTokenAccessor;
        private static readonly Func<DirectiveTriviaSyntax, bool, DirectiveTriviaSyntax> WithIsActiveAccessor;
        static NullableDirectiveTriviaSyntaxWrapper()
        {
            WrappedType = SyntaxWrapperHelper.GetWrappedType(typeof(NullableDirectiveTriviaSyntaxWrapper));
            NullableKeywordAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<DirectiveTriviaSyntax, SyntaxToken>(WrappedType, nameof(NullableKeyword));
            SettingTokenAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<DirectiveTriviaSyntax, SyntaxToken>(WrappedType, nameof(SettingToken));
            TargetTokenAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<DirectiveTriviaSyntax, SyntaxToken>(WrappedType, nameof(TargetToken));
            WithHashTokenAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<DirectiveTriviaSyntax, SyntaxToken>(WrappedType, nameof(DirectiveTriviaSyntax.HashToken));
            WithNullableKeywordAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<DirectiveTriviaSyntax, SyntaxToken>(WrappedType, nameof(NullableKeyword));
            WithSettingTokenAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<DirectiveTriviaSyntax, SyntaxToken>(WrappedType, nameof(SettingToken));
            WithTargetTokenAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<DirectiveTriviaSyntax, SyntaxToken>(WrappedType, nameof(TargetToken));
            WithEndOfDirectiveTokenAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<DirectiveTriviaSyntax, SyntaxToken>(WrappedType, nameof(DirectiveTriviaSyntax.EndOfDirectiveToken));
            WithIsActiveAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<DirectiveTriviaSyntax, bool>(WrappedType, nameof(DirectiveTriviaSyntax.IsActive));
        }

        private NullableDirectiveTriviaSyntaxWrapper(DirectiveTriviaSyntax node)
        {
            this.node = node;
        }

        public DirectiveTriviaSyntax SyntaxNode => this.node;
        public static explicit operator NullableDirectiveTriviaSyntaxWrapper(SyntaxNode node)
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

        public static implicit operator DirectiveTriviaSyntax(NullableDirectiveTriviaSyntaxWrapper wrapper)
        {
            return wrapper.node;
        }

        public static bool IsInstance(SyntaxNode node)
        {
            return node != null && LightupHelpers.CanWrapNode(node, WrappedType);
        }
    }
}
