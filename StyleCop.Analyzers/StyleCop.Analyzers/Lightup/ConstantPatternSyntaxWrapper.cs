// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal partial struct ConstantPatternSyntaxWrapper : ISyntaxWrapper<CSharpSyntaxNode>
    {
        public ExpressionSyntax Expression
        {
            get
            {
                return ExpressionAccessor(this.SyntaxNode);
            }
        }

        public static explicit operator ConstantPatternSyntaxWrapper(PatternSyntaxWrapper node)
        {
            return (ConstantPatternSyntaxWrapper)node.SyntaxNode;
        }

        public static implicit operator PatternSyntaxWrapper(ConstantPatternSyntaxWrapper wrapper)
        {
            return PatternSyntaxWrapper.FromUpcast(wrapper.node);
        }

        public ConstantPatternSyntaxWrapper WithExpression(ExpressionSyntax expression)
        {
            return new ConstantPatternSyntaxWrapper(WithExpressionAccessor(this.SyntaxNode, expression));
        }
    }
}
