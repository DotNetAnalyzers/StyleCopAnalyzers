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

    internal readonly partial struct CasePatternSwitchLabelSyntaxWrapper : ISyntaxWrapper<SwitchLabelSyntax>
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.CSharp.Syntax.CasePatternSwitchLabelSyntax";
        private static readonly Type? WrappedType;

        private static readonly Func<SwitchLabelSyntax, CSharpSyntaxNode> PatternAccessor;
        private static readonly Func<SwitchLabelSyntax, CSharpSyntaxNode?> WhenClauseAccessor;
        private static readonly Func<SwitchLabelSyntax, SyntaxToken, SwitchLabelSyntax> WithKeywordAccessor;
        private static readonly Func<SwitchLabelSyntax, CSharpSyntaxNode, SwitchLabelSyntax> WithPatternAccessor;
        private static readonly Func<SwitchLabelSyntax, CSharpSyntaxNode?, SwitchLabelSyntax> WithWhenClauseAccessor;
        private static readonly Func<SwitchLabelSyntax, SyntaxToken, SwitchLabelSyntax> WithColonTokenAccessor;

        private readonly SwitchLabelSyntax? node;

        static CasePatternSwitchLabelSyntaxWrapper()
        {
            WrappedType = SyntaxWrapperHelper.GetWrappedType(typeof(CasePatternSwitchLabelSyntaxWrapper));
            PatternAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<SwitchLabelSyntax, CSharpSyntaxNode>(WrappedType, nameof(Pattern));
            WhenClauseAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<SwitchLabelSyntax, CSharpSyntaxNode?>(WrappedType, nameof(WhenClause));
            WithKeywordAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<SwitchLabelSyntax, SyntaxToken>(WrappedType, nameof(Keyword));
            WithPatternAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<SwitchLabelSyntax, CSharpSyntaxNode>(WrappedType, nameof(Pattern));
            WithWhenClauseAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<SwitchLabelSyntax, CSharpSyntaxNode?>(WrappedType, nameof(WhenClause));
            WithColonTokenAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<SwitchLabelSyntax, SyntaxToken>(WrappedType, nameof(ColonToken));
        }

        private CasePatternSwitchLabelSyntaxWrapper(SwitchLabelSyntax? node)
        {
            this.node = node;
        }

        public SwitchLabelSyntax? SyntaxNode => this.node;


        public SyntaxToken Keyword
        {
            get
            {
                return this.node!.Keyword;
            }
        }

        public PatternSyntaxWrapper Pattern
        {
            get
            {
                return (PatternSyntaxWrapper)PatternAccessor(this.node!);
            }
        }

        public WhenClauseSyntaxWrapper WhenClause
        {
            get
            {
                return (WhenClauseSyntaxWrapper)WhenClauseAccessor(this.node!);
            }
        }

        public SyntaxToken ColonToken
        {
            get
            {
                return this.node!.ColonToken;
            }
        }

        public static explicit operator CasePatternSwitchLabelSyntaxWrapper(SyntaxNode? node)
        {
            if (node == null)
            {
                return default;
            }

            if (!IsInstance(node))
            {
                throw new InvalidCastException($"Cannot cast '{node.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new CasePatternSwitchLabelSyntaxWrapper((SwitchLabelSyntax)node);
        }

        public static implicit operator SwitchLabelSyntax? (CasePatternSwitchLabelSyntaxWrapper wrapper)
        {
            return wrapper.node;
        }

        public static bool IsInstance([NotNullWhen(true)] SyntaxNode? node)
        {
            return node != null && LightupHelpers.CanWrapNode(node, WrappedType);
        }

        public CasePatternSwitchLabelSyntaxWrapper WithKeyword(SyntaxToken keyword)
        {
            return new CasePatternSwitchLabelSyntaxWrapper(WithKeywordAccessor(this.node!, keyword));
        }

        public CasePatternSwitchLabelSyntaxWrapper WithPattern(PatternSyntaxWrapper pattern)
        {
            return new CasePatternSwitchLabelSyntaxWrapper(WithPatternAccessor(this.node!, pattern.SyntaxNode!));
        }

        public CasePatternSwitchLabelSyntaxWrapper WithWhenClause(WhenClauseSyntaxWrapper whenClause)
        {
            return new CasePatternSwitchLabelSyntaxWrapper(WithWhenClauseAccessor(this.node!, whenClause.SyntaxNode));
        }

        public CasePatternSwitchLabelSyntaxWrapper WithColonToken(SyntaxToken colonToken)
        {
            return new CasePatternSwitchLabelSyntaxWrapper(WithColonTokenAccessor(this.node!, colonToken));
        }
    }
}
