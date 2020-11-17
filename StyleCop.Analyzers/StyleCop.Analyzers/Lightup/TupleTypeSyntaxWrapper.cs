// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal partial struct TupleTypeSyntaxWrapper : ISyntaxWrapper<TypeSyntax>
    {
        public SyntaxToken OpenParenToken
        {
            get
            {
                return OpenParenTokenAccessor(this.SyntaxNode);
            }
        }

        public SeparatedSyntaxListWrapper<TupleElementSyntaxWrapper> Elements
        {
            get
            {
                return ElementsAccessor(this.SyntaxNode);
            }
        }

        public SyntaxToken CloseParenToken
        {
            get
            {
                return CloseParenTokenAccessor(this.SyntaxNode);
            }
        }

        public TupleTypeSyntaxWrapper AddElements(params TupleElementSyntaxWrapper[] items)
        {
            return new TupleTypeSyntaxWrapper(this.WithElements(this.Elements.AddRange(items)));
        }

        public TupleTypeSyntaxWrapper WithOpenParenToken(SyntaxToken openParenToken)
        {
            return new TupleTypeSyntaxWrapper(WithOpenParenTokenAccessor(this.SyntaxNode, openParenToken));
        }

        public TupleTypeSyntaxWrapper WithElements(SeparatedSyntaxListWrapper<TupleElementSyntaxWrapper> elements)
        {
            return new TupleTypeSyntaxWrapper(WithElementsAccessor(this.SyntaxNode, elements));
        }

        public TupleTypeSyntaxWrapper WithCloseParenToken(SyntaxToken closeParenToken)
        {
            return new TupleTypeSyntaxWrapper(WithCloseParenTokenAccessor(this.SyntaxNode, closeParenToken));
        }
    }
}
