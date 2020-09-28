// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Helpers
{
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal static class ExpressionSyntaxHelpers
    {
        public static ExpressionSyntax WalkDownParentheses(this ExpressionSyntax expression)
        {
            var result = expression;
            while (result is ParenthesizedExpressionSyntax parenthesizedExpression)
            {
                result = parenthesizedExpression.Expression;
            }

            return result;
        }
    }
}
