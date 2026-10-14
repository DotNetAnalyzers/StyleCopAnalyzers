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

    internal readonly partial struct LineDirectivePositionSyntaxWrapper : ISyntaxWrapper<CSharpSyntaxNode>
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.CSharp.Syntax.LineDirectivePositionSyntax";
        private static readonly Type? WrappedType;

        private static readonly Func<CSharpSyntaxNode, SyntaxToken> OpenParenTokenAccessor;
        private static readonly Func<CSharpSyntaxNode, SyntaxToken> LineAccessor;
        private static readonly Func<CSharpSyntaxNode, SyntaxToken> CommaTokenAccessor;
        private static readonly Func<CSharpSyntaxNode, SyntaxToken> CharacterAccessor;
        private static readonly Func<CSharpSyntaxNode, SyntaxToken> CloseParenTokenAccessor;
        private static readonly Func<CSharpSyntaxNode, SyntaxToken, CSharpSyntaxNode> WithOpenParenTokenAccessor;
        private static readonly Func<CSharpSyntaxNode, SyntaxToken, CSharpSyntaxNode> WithLineAccessor;
        private static readonly Func<CSharpSyntaxNode, SyntaxToken, CSharpSyntaxNode> WithCommaTokenAccessor;
        private static readonly Func<CSharpSyntaxNode, SyntaxToken, CSharpSyntaxNode> WithCharacterAccessor;
        private static readonly Func<CSharpSyntaxNode, SyntaxToken, CSharpSyntaxNode> WithCloseParenTokenAccessor;

        private readonly CSharpSyntaxNode? node;

        static LineDirectivePositionSyntaxWrapper()
        {
            WrappedType = SyntaxWrapperHelper.GetWrappedType(typeof(LineDirectivePositionSyntaxWrapper));
            OpenParenTokenAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<CSharpSyntaxNode, SyntaxToken>(WrappedType, nameof(OpenParenToken));
            LineAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<CSharpSyntaxNode, SyntaxToken>(WrappedType, nameof(Line));
            CommaTokenAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<CSharpSyntaxNode, SyntaxToken>(WrappedType, nameof(CommaToken));
            CharacterAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<CSharpSyntaxNode, SyntaxToken>(WrappedType, nameof(Character));
            CloseParenTokenAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<CSharpSyntaxNode, SyntaxToken>(WrappedType, nameof(CloseParenToken));
            WithOpenParenTokenAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<CSharpSyntaxNode, SyntaxToken>(WrappedType, nameof(OpenParenToken));
            WithLineAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<CSharpSyntaxNode, SyntaxToken>(WrappedType, nameof(Line));
            WithCommaTokenAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<CSharpSyntaxNode, SyntaxToken>(WrappedType, nameof(CommaToken));
            WithCharacterAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<CSharpSyntaxNode, SyntaxToken>(WrappedType, nameof(Character));
            WithCloseParenTokenAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<CSharpSyntaxNode, SyntaxToken>(WrappedType, nameof(CloseParenToken));
        }

        private LineDirectivePositionSyntaxWrapper(CSharpSyntaxNode? node)
        {
            this.node = node;
        }

        public CSharpSyntaxNode? SyntaxNode => this.node;


        public SyntaxToken OpenParenToken
        {
            get
            {
                return OpenParenTokenAccessor(this.node!);
            }
        }

        public SyntaxToken Line
        {
            get
            {
                return LineAccessor(this.node!);
            }
        }

        public SyntaxToken CommaToken
        {
            get
            {
                return CommaTokenAccessor(this.node!);
            }
        }

        public SyntaxToken Character
        {
            get
            {
                return CharacterAccessor(this.node!);
            }
        }

        public SyntaxToken CloseParenToken
        {
            get
            {
                return CloseParenTokenAccessor(this.node!);
            }
        }

        public static explicit operator LineDirectivePositionSyntaxWrapper(SyntaxNode? node)
        {
            if (node == null)
            {
                return default;
            }

            if (!IsInstance(node))
            {
                throw new InvalidCastException($"Cannot cast '{node.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new LineDirectivePositionSyntaxWrapper((CSharpSyntaxNode)node);
        }

        public static implicit operator CSharpSyntaxNode? (LineDirectivePositionSyntaxWrapper wrapper)
        {
            return wrapper.node;
        }

        public static bool IsInstance([NotNullWhen(true)] SyntaxNode? node)
        {
            return node != null && LightupHelpers.CanWrapNode(node, WrappedType);
        }

        public LineDirectivePositionSyntaxWrapper WithOpenParenToken(SyntaxToken openParenToken)
        {
            return new LineDirectivePositionSyntaxWrapper(WithOpenParenTokenAccessor(this.node!, openParenToken));
        }

        public LineDirectivePositionSyntaxWrapper WithLine(SyntaxToken line)
        {
            return new LineDirectivePositionSyntaxWrapper(WithLineAccessor(this.node!, line));
        }

        public LineDirectivePositionSyntaxWrapper WithCommaToken(SyntaxToken commaToken)
        {
            return new LineDirectivePositionSyntaxWrapper(WithCommaTokenAccessor(this.node!, commaToken));
        }

        public LineDirectivePositionSyntaxWrapper WithCharacter(SyntaxToken character)
        {
            return new LineDirectivePositionSyntaxWrapper(WithCharacterAccessor(this.node!, character));
        }

        public LineDirectivePositionSyntaxWrapper WithCloseParenToken(SyntaxToken closeParenToken)
        {
            return new LineDirectivePositionSyntaxWrapper(WithCloseParenTokenAccessor(this.node!, closeParenToken));
        }
    }
}
