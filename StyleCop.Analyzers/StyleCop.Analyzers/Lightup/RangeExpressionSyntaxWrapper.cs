// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal partial struct RangeExpressionSyntaxWrapper : ISyntaxWrapper<ExpressionSyntax>
    {
        public RangeExpressionSyntaxWrapper WithLeftOperand(ExpressionSyntax leftOperand)
        {
            return new RangeExpressionSyntaxWrapper(WithLeftOperandAccessor(this.SyntaxNode, leftOperand));
        }

        public RangeExpressionSyntaxWrapper WithOperatorToken(SyntaxToken operatorToken)
        {
            return new RangeExpressionSyntaxWrapper(WithOperatorTokenAccessor(this.SyntaxNode, operatorToken));
        }

        public RangeExpressionSyntaxWrapper WithRightOperand(ExpressionSyntax rightOperand)
        {
            return new RangeExpressionSyntaxWrapper(WithRightOperandAccessor(this.SyntaxNode, rightOperand));
        }
    }
}
