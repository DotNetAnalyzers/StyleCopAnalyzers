// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal readonly partial struct IgnoredDirectiveTriviaSyntaxWrapper : ISyntaxWrapper<DirectiveTriviaSyntax>
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.CSharp.Syntax.IgnoredDirectiveTriviaSyntax";
        private static readonly Type WrappedType;

        private static readonly Func<DirectiveTriviaSyntax, SyntaxToken> ColonTokenAccessor;
        private static readonly Func<DirectiveTriviaSyntax, SyntaxToken> ContentAccessor;
        private static readonly Func<DirectiveTriviaSyntax, SyntaxToken, DirectiveTriviaSyntax> WithHashTokenAccessor;
        private static readonly Func<DirectiveTriviaSyntax, SyntaxToken, DirectiveTriviaSyntax> WithColonTokenAccessor;
        private static readonly Func<DirectiveTriviaSyntax, SyntaxToken, DirectiveTriviaSyntax> WithContentAccessor;
        private static readonly Func<DirectiveTriviaSyntax, SyntaxToken, DirectiveTriviaSyntax> WithEndOfDirectiveTokenAccessor;
        private static readonly Func<DirectiveTriviaSyntax, bool, DirectiveTriviaSyntax> WithIsActiveAccessor;

        private readonly DirectiveTriviaSyntax node;

        static IgnoredDirectiveTriviaSyntaxWrapper()
        {
            WrappedType = SyntaxWrapperHelper.GetWrappedType(typeof(IgnoredDirectiveTriviaSyntaxWrapper));
            ColonTokenAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<DirectiveTriviaSyntax, SyntaxToken>(WrappedType, nameof(ColonToken));
            ContentAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<DirectiveTriviaSyntax, SyntaxToken>(WrappedType, nameof(Content));
            WithHashTokenAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<DirectiveTriviaSyntax, SyntaxToken>(WrappedType, nameof(HashToken));
            WithColonTokenAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<DirectiveTriviaSyntax, SyntaxToken>(WrappedType, nameof(ColonToken));
            WithContentAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<DirectiveTriviaSyntax, SyntaxToken>(WrappedType, nameof(Content));
            WithEndOfDirectiveTokenAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<DirectiveTriviaSyntax, SyntaxToken>(WrappedType, nameof(EndOfDirectiveToken));
            WithIsActiveAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<DirectiveTriviaSyntax, bool>(WrappedType, nameof(IsActive));
        }

        private IgnoredDirectiveTriviaSyntaxWrapper(DirectiveTriviaSyntax node)
        {
            this.node = node;
        }

        public DirectiveTriviaSyntax SyntaxNode => this.node;

        public SyntaxToken HashToken
        {
            get
            {
                return this.SyntaxNode.HashToken;
            }
        }

        public SyntaxToken ColonToken
        {
            get
            {
                return ColonTokenAccessor(this.SyntaxNode);
            }
        }

        public SyntaxToken Content
        {
            get
            {
                return ContentAccessor(this.SyntaxNode);
            }
        }

        public SyntaxToken EndOfDirectiveToken
        {
            get
            {
                return this.SyntaxNode.EndOfDirectiveToken;
            }
        }

        public bool IsActive
        {
            get
            {
                return this.SyntaxNode.IsActive;
            }
        }

        public static explicit operator IgnoredDirectiveTriviaSyntaxWrapper(SyntaxNode node)
        {
            if (node == null)
            {
                return default;
            }

            if (!IsInstance(node))
            {
                throw new InvalidCastException($"Cannot cast '{node.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new IgnoredDirectiveTriviaSyntaxWrapper((DirectiveTriviaSyntax)node);
        }

        public static implicit operator DirectiveTriviaSyntax(IgnoredDirectiveTriviaSyntaxWrapper wrapper)
        {
            return wrapper.node;
        }

        public static bool IsInstance(SyntaxNode node)
        {
            return node != null && LightupHelpers.CanWrapNode(node, WrappedType);
        }

        public IgnoredDirectiveTriviaSyntaxWrapper WithHashToken(SyntaxToken hashToken)
        {
            return new IgnoredDirectiveTriviaSyntaxWrapper(WithHashTokenAccessor(this.SyntaxNode, hashToken));
        }

        public IgnoredDirectiveTriviaSyntaxWrapper WithColonToken(SyntaxToken colonToken)
        {
            return new IgnoredDirectiveTriviaSyntaxWrapper(WithColonTokenAccessor(this.SyntaxNode, colonToken));
        }

        public IgnoredDirectiveTriviaSyntaxWrapper WithContent(SyntaxToken content)
        {
            return new IgnoredDirectiveTriviaSyntaxWrapper(WithContentAccessor(this.SyntaxNode, content));
        }

        public IgnoredDirectiveTriviaSyntaxWrapper WithEndOfDirectiveToken(SyntaxToken endOfDirectiveToken)
        {
            return new IgnoredDirectiveTriviaSyntaxWrapper(WithEndOfDirectiveTokenAccessor(this.SyntaxNode, endOfDirectiveToken));
        }

        public IgnoredDirectiveTriviaSyntaxWrapper WithIsActive(bool isActive)
        {
            return new IgnoredDirectiveTriviaSyntaxWrapper(WithIsActiveAccessor(this.SyntaxNode, isActive));
        }
    }
}
