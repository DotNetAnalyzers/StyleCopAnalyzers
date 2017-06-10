// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Reflection;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal struct CasePatternSwitchLabelSyntaxWrapper : ISyntaxWrapper<SwitchLabelSyntax>
    {
        private const string CasePatternSwitchLabelSyntaxTypeName = "Microsoft.CodeAnalysis.CSharp.Syntax.CasePatternSwitchLabelSyntax";
        private static readonly Type CasePatternSwitchLabelSyntaxType;

        private static readonly Func<SwitchLabelSyntax, CSharpSyntaxNode> PatternAccessor;
        private static readonly Func<SwitchLabelSyntax, CSharpSyntaxNode> WhenClauseAccessor;
        private static readonly Func<SwitchLabelSyntax, SyntaxToken, SwitchLabelSyntax> WithKeywordAccessor;
        private static readonly Func<SwitchLabelSyntax, SyntaxToken, SwitchLabelSyntax> WithColonTokenAccessor;
        private static readonly Func<SwitchLabelSyntax, CSharpSyntaxNode, SwitchLabelSyntax> WithPatternAccessor;
        private static readonly Func<SwitchLabelSyntax, CSharpSyntaxNode, SwitchLabelSyntax> WithWhenClauseAccessor;

        private readonly SwitchLabelSyntax node;

        static CasePatternSwitchLabelSyntaxWrapper()
        {
            CasePatternSwitchLabelSyntaxType = typeof(CSharpSyntaxNode).GetTypeInfo().Assembly.GetType(CasePatternSwitchLabelSyntaxTypeName);
            PatternAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<SwitchLabelSyntax, CSharpSyntaxNode>(CasePatternSwitchLabelSyntaxType, nameof(Pattern));
            WhenClauseAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<SwitchLabelSyntax, CSharpSyntaxNode>(CasePatternSwitchLabelSyntaxType, nameof(WhenClause));
            WithKeywordAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<SwitchLabelSyntax, SyntaxToken>(CasePatternSwitchLabelSyntaxType, nameof(SwitchLabelSyntax.Keyword));
            WithColonTokenAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<SwitchLabelSyntax, SyntaxToken>(CasePatternSwitchLabelSyntaxType, nameof(SwitchLabelSyntax.ColonToken));
            WithPatternAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<SwitchLabelSyntax, CSharpSyntaxNode>(CasePatternSwitchLabelSyntaxType, nameof(Pattern));
            WithWhenClauseAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<SwitchLabelSyntax, CSharpSyntaxNode>(CasePatternSwitchLabelSyntaxType, nameof(WhenClause));
        }

        private CasePatternSwitchLabelSyntaxWrapper(SwitchLabelSyntax node)
        {
            this.node = node;
        }

        public SwitchLabelSyntax SyntaxNode => this.node;

        public PatternSyntaxWrapper Pattern
        {
            get
            {
                return (PatternSyntaxWrapper)PatternAccessor(this.SyntaxNode);
            }
        }

        public WhenClauseSyntaxWrapper WhenClause
        {
            get
            {
                return (WhenClauseSyntaxWrapper)WhenClauseAccessor(this.SyntaxNode);
            }
        }

        public static explicit operator CasePatternSwitchLabelSyntaxWrapper(SyntaxNode node)
        {
            if (node == null)
            {
                return default(CasePatternSwitchLabelSyntaxWrapper);
            }

            if (!IsInstance(node))
            {
                throw new InvalidCastException($"Cannot cast '{node.GetType().FullName}' to '{CasePatternSwitchLabelSyntaxTypeName}'");
            }

            return new CasePatternSwitchLabelSyntaxWrapper((SwitchLabelSyntax)node);
        }

        public static implicit operator SwitchLabelSyntax(CasePatternSwitchLabelSyntaxWrapper wrapper)
        {
            return wrapper.node;
        }

        public static bool IsInstance(SyntaxNode node)
        {
            return node != null && LightupHelpers.CanWrapNode(node, CasePatternSwitchLabelSyntaxType);
        }

        public CasePatternSwitchLabelSyntaxWrapper WithKeyword(SyntaxToken keyword)
        {
            return new CasePatternSwitchLabelSyntaxWrapper(WithKeywordAccessor(this.SyntaxNode, keyword));
        }

        public CasePatternSwitchLabelSyntaxWrapper WithColonToken(SyntaxToken colonToken)
        {
            return new CasePatternSwitchLabelSyntaxWrapper(WithColonTokenAccessor(this.SyntaxNode, colonToken));
        }

        public CasePatternSwitchLabelSyntaxWrapper WithPattern(PatternSyntaxWrapper pattern)
        {
            return new CasePatternSwitchLabelSyntaxWrapper(WithPatternAccessor(this.SyntaxNode, pattern));
        }

        public CasePatternSwitchLabelSyntaxWrapper WithWhenClause(WhenClauseSyntaxWrapper whenClause)
        {
            return new CasePatternSwitchLabelSyntaxWrapper(WithWhenClauseAccessor(this.SyntaxNode, whenClause));
        }
    }
}
