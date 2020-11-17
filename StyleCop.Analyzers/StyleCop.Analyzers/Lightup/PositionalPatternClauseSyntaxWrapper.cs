// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;

    internal partial struct PositionalPatternClauseSyntaxWrapper : ISyntaxWrapper<CSharpSyntaxNode>
    {
        private static readonly Func<CSharpSyntaxNode, SyntaxToken> OpenParenTokenAccessor;
        private static readonly Func<CSharpSyntaxNode, SeparatedSyntaxListWrapper<SubpatternSyntaxWrapper>> SubpatternsAccessor;
        private static readonly Func<CSharpSyntaxNode, SyntaxToken> CloseParenTokenAccessor;

        private static readonly Func<CSharpSyntaxNode, SyntaxToken, CSharpSyntaxNode> WithOpenParenTokenAccessor;
        private static readonly Func<CSharpSyntaxNode, SeparatedSyntaxListWrapper<SubpatternSyntaxWrapper>, CSharpSyntaxNode> WithSubpatternsAccessor;
        private static readonly Func<CSharpSyntaxNode, SyntaxToken, CSharpSyntaxNode> WithCloseParenTokenAccessor;

        static PositionalPatternClauseSyntaxWrapper()
        {
            WrappedType = SyntaxWrapperHelper.GetWrappedType(typeof(PositionalPatternClauseSyntaxWrapper));
            OpenParenTokenAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<CSharpSyntaxNode, SyntaxToken>(WrappedType, nameof(OpenParenToken));
            SubpatternsAccessor = LightupHelpers.CreateSeparatedSyntaxListPropertyAccessor<CSharpSyntaxNode, SubpatternSyntaxWrapper>(WrappedType, nameof(Subpatterns));
            CloseParenTokenAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<CSharpSyntaxNode, SyntaxToken>(WrappedType, nameof(CloseParenToken));

            WithOpenParenTokenAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<CSharpSyntaxNode, SyntaxToken>(WrappedType, nameof(OpenParenToken));
            WithSubpatternsAccessor = LightupHelpers.CreateSeparatedSyntaxListWithPropertyAccessor<CSharpSyntaxNode, SubpatternSyntaxWrapper>(WrappedType, nameof(Subpatterns));
            WithCloseParenTokenAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<CSharpSyntaxNode, SyntaxToken>(WrappedType, nameof(CloseParenToken));
        }

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
