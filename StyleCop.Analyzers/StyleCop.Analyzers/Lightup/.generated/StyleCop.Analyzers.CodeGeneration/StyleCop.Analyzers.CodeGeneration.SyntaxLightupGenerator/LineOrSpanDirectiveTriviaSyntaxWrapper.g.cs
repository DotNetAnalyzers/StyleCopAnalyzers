// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal readonly partial struct LineOrSpanDirectiveTriviaSyntaxWrapper : ISyntaxWrapper<DirectiveTriviaSyntax>
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.CSharp.Syntax.LineOrSpanDirectiveTriviaSyntax";
        private static readonly Type WrappedType;

        private static readonly Func<DirectiveTriviaSyntax, SyntaxToken> LineKeywordAccessor;
        private static readonly Func<DirectiveTriviaSyntax, SyntaxToken> FileAccessor;
        private static readonly Func<DirectiveTriviaSyntax, SyntaxToken, DirectiveTriviaSyntax> WithLineKeywordAccessor;
        private static readonly Func<DirectiveTriviaSyntax, SyntaxToken, DirectiveTriviaSyntax> WithFileAccessor;

        private readonly DirectiveTriviaSyntax node;

        static LineOrSpanDirectiveTriviaSyntaxWrapper()
        {
            WrappedType = SyntaxWrapperHelper.GetWrappedType(typeof(LineOrSpanDirectiveTriviaSyntaxWrapper));
            LineKeywordAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<DirectiveTriviaSyntax, SyntaxToken>(WrappedType, nameof(LineKeyword));
            FileAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<DirectiveTriviaSyntax, SyntaxToken>(WrappedType, nameof(File));
            WithLineKeywordAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<DirectiveTriviaSyntax, SyntaxToken>(WrappedType, nameof(LineKeyword));
            WithFileAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<DirectiveTriviaSyntax, SyntaxToken>(WrappedType, nameof(File));
        }

        private LineOrSpanDirectiveTriviaSyntaxWrapper(DirectiveTriviaSyntax node)
        {
            this.node = node;
        }

        public DirectiveTriviaSyntax SyntaxNode => this.node;

        public SyntaxToken LineKeyword
        {
            get
            {
                return LineKeywordAccessor(this.SyntaxNode);
            }
        }

        public SyntaxToken File
        {
            get
            {
                return FileAccessor(this.SyntaxNode);
            }
        }

        public static explicit operator LineOrSpanDirectiveTriviaSyntaxWrapper(SyntaxNode node)
        {
            if (node == null)
            {
                return default;
            }

            if (!IsInstance(node))
            {
                throw new InvalidCastException($"Cannot cast '{node.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new LineOrSpanDirectiveTriviaSyntaxWrapper((DirectiveTriviaSyntax)node);
        }

        public static implicit operator DirectiveTriviaSyntax(LineOrSpanDirectiveTriviaSyntaxWrapper wrapper)
        {
            return wrapper.node;
        }

        public static bool IsInstance(SyntaxNode node)
        {
            return node != null && LightupHelpers.CanWrapNode(node, WrappedType);
        }

        public LineOrSpanDirectiveTriviaSyntaxWrapper WithLineKeyword(SyntaxToken lineKeyword)
        {
            return new LineOrSpanDirectiveTriviaSyntaxWrapper(WithLineKeywordAccessor(this.SyntaxNode, lineKeyword));
        }

        public LineOrSpanDirectiveTriviaSyntaxWrapper WithFile(SyntaxToken file)
        {
            return new LineOrSpanDirectiveTriviaSyntaxWrapper(WithFileAccessor(this.SyntaxNode, file));
        }

        internal static LineOrSpanDirectiveTriviaSyntaxWrapper FromUpcast(DirectiveTriviaSyntax node)
        {
            return new LineOrSpanDirectiveTriviaSyntaxWrapper(node);
        }
    }
}
