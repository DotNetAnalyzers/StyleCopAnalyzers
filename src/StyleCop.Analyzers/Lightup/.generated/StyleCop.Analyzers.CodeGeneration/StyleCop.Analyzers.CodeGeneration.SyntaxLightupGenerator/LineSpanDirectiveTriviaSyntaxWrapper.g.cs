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

    internal readonly partial struct LineSpanDirectiveTriviaSyntaxWrapper : ISyntaxWrapper<DirectiveTriviaSyntax>
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.CSharp.Syntax.LineSpanDirectiveTriviaSyntax";
        private static readonly Type? WrappedType;

        private static readonly Func<DirectiveTriviaSyntax, SyntaxToken> LineKeywordAccessor;
        private static readonly Func<DirectiveTriviaSyntax, CSharpSyntaxNode> StartAccessor;
        private static readonly Func<DirectiveTriviaSyntax, SyntaxToken> MinusTokenAccessor;
        private static readonly Func<DirectiveTriviaSyntax, CSharpSyntaxNode> EndAccessor;
        private static readonly Func<DirectiveTriviaSyntax, SyntaxToken> CharacterOffsetAccessor;
        private static readonly Func<DirectiveTriviaSyntax, SyntaxToken> FileAccessor;
        private static readonly Func<DirectiveTriviaSyntax, SyntaxToken, DirectiveTriviaSyntax> WithHashTokenAccessor;
        private static readonly Func<DirectiveTriviaSyntax, SyntaxToken, DirectiveTriviaSyntax> WithLineKeywordAccessor;
        private static readonly Func<DirectiveTriviaSyntax, CSharpSyntaxNode, DirectiveTriviaSyntax> WithStartAccessor;
        private static readonly Func<DirectiveTriviaSyntax, SyntaxToken, DirectiveTriviaSyntax> WithMinusTokenAccessor;
        private static readonly Func<DirectiveTriviaSyntax, CSharpSyntaxNode, DirectiveTriviaSyntax> WithEndAccessor;
        private static readonly Func<DirectiveTriviaSyntax, SyntaxToken, DirectiveTriviaSyntax> WithCharacterOffsetAccessor;
        private static readonly Func<DirectiveTriviaSyntax, SyntaxToken, DirectiveTriviaSyntax> WithFileAccessor;
        private static readonly Func<DirectiveTriviaSyntax, SyntaxToken, DirectiveTriviaSyntax> WithEndOfDirectiveTokenAccessor;
        private static readonly Func<DirectiveTriviaSyntax, bool, DirectiveTriviaSyntax> WithIsActiveAccessor;

        private readonly DirectiveTriviaSyntax? node;

        static LineSpanDirectiveTriviaSyntaxWrapper()
        {
            WrappedType = SyntaxWrapperHelper.GetWrappedType(typeof(LineSpanDirectiveTriviaSyntaxWrapper));
            LineKeywordAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<DirectiveTriviaSyntax, SyntaxToken>(WrappedType, nameof(LineKeyword));
            StartAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<DirectiveTriviaSyntax, CSharpSyntaxNode>(WrappedType, nameof(Start));
            MinusTokenAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<DirectiveTriviaSyntax, SyntaxToken>(WrappedType, nameof(MinusToken));
            EndAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<DirectiveTriviaSyntax, CSharpSyntaxNode>(WrappedType, nameof(End));
            CharacterOffsetAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<DirectiveTriviaSyntax, SyntaxToken>(WrappedType, nameof(CharacterOffset));
            FileAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<DirectiveTriviaSyntax, SyntaxToken>(WrappedType, nameof(File));
            WithHashTokenAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<DirectiveTriviaSyntax, SyntaxToken>(WrappedType, nameof(HashToken));
            WithLineKeywordAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<DirectiveTriviaSyntax, SyntaxToken>(WrappedType, nameof(LineKeyword));
            WithStartAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<DirectiveTriviaSyntax, CSharpSyntaxNode>(WrappedType, nameof(Start));
            WithMinusTokenAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<DirectiveTriviaSyntax, SyntaxToken>(WrappedType, nameof(MinusToken));
            WithEndAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<DirectiveTriviaSyntax, CSharpSyntaxNode>(WrappedType, nameof(End));
            WithCharacterOffsetAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<DirectiveTriviaSyntax, SyntaxToken>(WrappedType, nameof(CharacterOffset));
            WithFileAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<DirectiveTriviaSyntax, SyntaxToken>(WrappedType, nameof(File));
            WithEndOfDirectiveTokenAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<DirectiveTriviaSyntax, SyntaxToken>(WrappedType, nameof(EndOfDirectiveToken));
            WithIsActiveAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<DirectiveTriviaSyntax, bool>(WrappedType, nameof(IsActive));
        }

        private LineSpanDirectiveTriviaSyntaxWrapper(DirectiveTriviaSyntax? node)
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

        public SyntaxToken LineKeyword
        {
            get
            {
                return LineKeywordAccessor(this.node!);
            }
        }

        public LineDirectivePositionSyntaxWrapper Start
        {
            get
            {
                return (LineDirectivePositionSyntaxWrapper)StartAccessor(this.node!);
            }
        }

        public SyntaxToken MinusToken
        {
            get
            {
                return MinusTokenAccessor(this.node!);
            }
        }

        public LineDirectivePositionSyntaxWrapper End
        {
            get
            {
                return (LineDirectivePositionSyntaxWrapper)EndAccessor(this.node!);
            }
        }

        public SyntaxToken CharacterOffset
        {
            get
            {
                return CharacterOffsetAccessor(this.node!);
            }
        }

        public SyntaxToken File
        {
            get
            {
                return FileAccessor(this.node!);
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

        public static explicit operator LineSpanDirectiveTriviaSyntaxWrapper(LineOrSpanDirectiveTriviaSyntaxWrapper node)
        {
            return (LineSpanDirectiveTriviaSyntaxWrapper)node.SyntaxNode;
        }

        public static explicit operator LineSpanDirectiveTriviaSyntaxWrapper(SyntaxNode? node)
        {
            if (node == null)
            {
                return default;
            }

            if (!IsInstance(node))
            {
                throw new InvalidCastException($"Cannot cast '{node.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new LineSpanDirectiveTriviaSyntaxWrapper((DirectiveTriviaSyntax)node);
        }

        public static implicit operator LineOrSpanDirectiveTriviaSyntaxWrapper(LineSpanDirectiveTriviaSyntaxWrapper wrapper)
        {
            return LineOrSpanDirectiveTriviaSyntaxWrapper.FromUpcast(wrapper.node);
        }

        public static implicit operator DirectiveTriviaSyntax? (LineSpanDirectiveTriviaSyntaxWrapper wrapper)
        {
            return wrapper.node;
        }

        public static bool IsInstance([NotNullWhen(true)] SyntaxNode? node)
        {
            return node != null && LightupHelpers.CanWrapNode(node, WrappedType);
        }

        public LineSpanDirectiveTriviaSyntaxWrapper WithHashToken(SyntaxToken hashToken)
        {
            return new LineSpanDirectiveTriviaSyntaxWrapper(WithHashTokenAccessor(this.node!, hashToken));
        }

        public LineSpanDirectiveTriviaSyntaxWrapper WithLineKeyword(SyntaxToken lineKeyword)
        {
            return new LineSpanDirectiveTriviaSyntaxWrapper(WithLineKeywordAccessor(this.node!, lineKeyword));
        }

        public LineSpanDirectiveTriviaSyntaxWrapper WithStart(LineDirectivePositionSyntaxWrapper start)
        {
            return new LineSpanDirectiveTriviaSyntaxWrapper(WithStartAccessor(this.node!, start.SyntaxNode!));
        }

        public LineSpanDirectiveTriviaSyntaxWrapper WithMinusToken(SyntaxToken minusToken)
        {
            return new LineSpanDirectiveTriviaSyntaxWrapper(WithMinusTokenAccessor(this.node!, minusToken));
        }

        public LineSpanDirectiveTriviaSyntaxWrapper WithEnd(LineDirectivePositionSyntaxWrapper end)
        {
            return new LineSpanDirectiveTriviaSyntaxWrapper(WithEndAccessor(this.node!, end.SyntaxNode!));
        }

        public LineSpanDirectiveTriviaSyntaxWrapper WithCharacterOffset(SyntaxToken characterOffset)
        {
            return new LineSpanDirectiveTriviaSyntaxWrapper(WithCharacterOffsetAccessor(this.node!, characterOffset));
        }

        public LineSpanDirectiveTriviaSyntaxWrapper WithFile(SyntaxToken file)
        {
            return new LineSpanDirectiveTriviaSyntaxWrapper(WithFileAccessor(this.node!, file));
        }

        public LineSpanDirectiveTriviaSyntaxWrapper WithEndOfDirectiveToken(SyntaxToken endOfDirectiveToken)
        {
            return new LineSpanDirectiveTriviaSyntaxWrapper(WithEndOfDirectiveTokenAccessor(this.node!, endOfDirectiveToken));
        }

        public LineSpanDirectiveTriviaSyntaxWrapper WithIsActive(bool isActive)
        {
            return new LineSpanDirectiveTriviaSyntaxWrapper(WithIsActiveAccessor(this.node!, isActive));
        }
    }
}
