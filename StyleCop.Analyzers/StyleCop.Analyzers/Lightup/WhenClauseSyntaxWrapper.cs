// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal partial struct WhenClauseSyntaxWrapper : ISyntaxWrapper<CSharpSyntaxNode>
    {
        public SyntaxToken WhenKeyword
        {
            get
            {
                return WhenKeywordAccessor(this.SyntaxNode);
            }
        }

        public ExpressionSyntax Condition
        {
            get
            {
                return ConditionAccessor(this.SyntaxNode);
            }
        }

        public WhenClauseSyntaxWrapper WithWhenKeyword(SyntaxToken whenKeyword)
        {
            return new WhenClauseSyntaxWrapper(WithWhenKeywordAccessor(this.SyntaxNode, whenKeyword));
        }

        public WhenClauseSyntaxWrapper WithCondition(ExpressionSyntax condition)
        {
            return new WhenClauseSyntaxWrapper(WithConditionAccessor(this.SyntaxNode, condition));
        }
    }
}
