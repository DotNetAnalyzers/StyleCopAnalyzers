// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal partial struct RefExpressionSyntaxWrapper : ISyntaxWrapper<ExpressionSyntax>
    {
        public SyntaxToken RefKeyword
        {
            get
            {
                return RefKeywordAccessor(this.SyntaxNode);
            }
        }

        public ExpressionSyntax Expression
        {
            get
            {
                return ExpressionAccessor(this.SyntaxNode);
            }
        }

        public RefExpressionSyntaxWrapper WithRefKeyword(SyntaxToken refKeyword)
        {
            return new RefExpressionSyntaxWrapper(WithRefKeywordAccessor(this.SyntaxNode, refKeyword));
        }

        public RefExpressionSyntaxWrapper WithExpression(ExpressionSyntax expression)
        {
            return new RefExpressionSyntaxWrapper(WithExpressionAccessor(this.SyntaxNode, expression));
        }
    }
}
