// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal partial struct IsPatternExpressionSyntaxWrapper : ISyntaxWrapper<ExpressionSyntax>
    {
        public ExpressionSyntax Expression
        {
            get
            {
                return ExpressionAccessor(this.SyntaxNode);
            }
        }

        public SyntaxToken IsKeyword
        {
            get
            {
                return IsKeywordAccessor(this.SyntaxNode);
            }
        }

        public PatternSyntaxWrapper Pattern
        {
            get
            {
                return (PatternSyntaxWrapper)PatternAccessor(this.SyntaxNode);
            }
        }

        public IsPatternExpressionSyntaxWrapper WithExpression(ExpressionSyntax expression)
        {
            return new IsPatternExpressionSyntaxWrapper(WithExpressionAccessor(this.SyntaxNode, expression));
        }

        public IsPatternExpressionSyntaxWrapper WithIsKeyword(SyntaxToken isKeyword)
        {
            return new IsPatternExpressionSyntaxWrapper(WithIsKeywordAccessor(this.SyntaxNode, isKeyword));
        }

        public IsPatternExpressionSyntaxWrapper WithPattern(PatternSyntaxWrapper pattern)
        {
            return new IsPatternExpressionSyntaxWrapper(WithPatternAccessor(this.SyntaxNode, pattern.SyntaxNode));
        }
    }
}
