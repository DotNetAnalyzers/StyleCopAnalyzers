// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal partial struct CasePatternSwitchLabelSyntaxWrapper : ISyntaxWrapper<SwitchLabelSyntax>
    {
        static CasePatternSwitchLabelSyntaxWrapper()
        {
            WrappedType = SyntaxWrapperHelper.GetWrappedType(typeof(CasePatternSwitchLabelSyntaxWrapper));
            PatternAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<SwitchLabelSyntax, CSharpSyntaxNode>(WrappedType, nameof(Pattern));
            WhenClauseAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<SwitchLabelSyntax, CSharpSyntaxNode>(WrappedType, nameof(WhenClause));
            WithKeywordAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<SwitchLabelSyntax, SyntaxToken>(WrappedType, nameof(SwitchLabelSyntax.Keyword));
            WithColonTokenAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<SwitchLabelSyntax, SyntaxToken>(WrappedType, nameof(SwitchLabelSyntax.ColonToken));
            WithPatternAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<SwitchLabelSyntax, CSharpSyntaxNode>(WrappedType, nameof(Pattern));
            WithWhenClauseAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<SwitchLabelSyntax, CSharpSyntaxNode>(WrappedType, nameof(WhenClause));
        }

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
