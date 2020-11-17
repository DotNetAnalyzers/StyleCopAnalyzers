// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;

    internal partial struct PositionalPatternClauseSyntaxWrapper : ISyntaxWrapper<CSharpSyntaxNode>
    {
        public SyntaxToken OpenParenToken
        {
            get
            {
                return OpenParenTokenAccessor(this.SyntaxNode);
            }
        }

        public SeparatedSyntaxListWrapper<SubpatternSyntaxWrapper> Subpatterns
        {
            get
            {
                return SubpatternsAccessor(this.SyntaxNode);
            }
        }

        public SyntaxToken CloseParenToken
        {
            get
            {
                return CloseParenTokenAccessor(this.SyntaxNode);
            }
        }

        public PositionalPatternClauseSyntaxWrapper AddSubpatterns(params SubpatternSyntaxWrapper[] items)
        {
            return new PositionalPatternClauseSyntaxWrapper(this.WithSubpatterns(this.Subpatterns.AddRange(items)));
        }

        public PositionalPatternClauseSyntaxWrapper WithOpenParenToken(SyntaxToken openParenToken)
        {
            return new PositionalPatternClauseSyntaxWrapper(WithOpenParenTokenAccessor(this.SyntaxNode, openParenToken));
        }

        public PositionalPatternClauseSyntaxWrapper WithSubpatterns(SeparatedSyntaxListWrapper<SubpatternSyntaxWrapper> subpatterns)
        {
            return new PositionalPatternClauseSyntaxWrapper(WithSubpatternsAccessor(this.SyntaxNode, subpatterns));
        }

        public PositionalPatternClauseSyntaxWrapper WithCloseParenToken(SyntaxToken closeParenToken)
        {
            return new PositionalPatternClauseSyntaxWrapper(WithCloseParenTokenAccessor(this.SyntaxNode, closeParenToken));
        }
    }
}
