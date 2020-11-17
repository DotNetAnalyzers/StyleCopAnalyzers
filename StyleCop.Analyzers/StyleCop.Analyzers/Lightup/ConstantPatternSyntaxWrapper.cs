// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal partial struct ConstantPatternSyntaxWrapper : ISyntaxWrapper<CSharpSyntaxNode>
    {
        public ConstantPatternSyntaxWrapper WithExpression(ExpressionSyntax expression)
        {
            return new ConstantPatternSyntaxWrapper(WithExpressionAccessor(this.SyntaxNode, expression));
        }
    }
}
