// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal partial struct TupleExpressionSyntaxWrapper : ISyntaxWrapper<ExpressionSyntax>
    {
        public TupleExpressionSyntaxWrapper AddArguments(params ArgumentSyntax[] items)
        {
            return new TupleExpressionSyntaxWrapper(this.WithArguments(this.Arguments.AddRange(items)));
        }

        public TupleExpressionSyntaxWrapper WithOpenParenToken(SyntaxToken openParenToken)
        {
            return new TupleExpressionSyntaxWrapper(WithOpenParenTokenAccessor(this.SyntaxNode, openParenToken));
        }

        public TupleExpressionSyntaxWrapper WithArguments(SeparatedSyntaxList<ArgumentSyntax> arguments)
        {
            return new TupleExpressionSyntaxWrapper(WithArgumentsAccessor(this.SyntaxNode, arguments));
        }

        public TupleExpressionSyntaxWrapper WithCloseParenToken(SyntaxToken closeParenToken)
        {
            return new TupleExpressionSyntaxWrapper(WithCloseParenTokenAccessor(this.SyntaxNode, closeParenToken));
        }
    }
}
