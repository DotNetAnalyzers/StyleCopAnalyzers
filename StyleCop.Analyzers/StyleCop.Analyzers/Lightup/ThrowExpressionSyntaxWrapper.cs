// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal partial struct ThrowExpressionSyntaxWrapper : ISyntaxWrapper<ExpressionSyntax>
    {
        static ThrowExpressionSyntaxWrapper()
        {
            WrappedType = SyntaxWrapperHelper.GetWrappedType(typeof(ThrowExpressionSyntaxWrapper));
            ThrowKeywordAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<ExpressionSyntax, SyntaxToken>(WrappedType, nameof(ThrowKeyword));
            ExpressionAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<ExpressionSyntax, ExpressionSyntax>(WrappedType, nameof(Expression));
            WithThrowKeywordAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<ExpressionSyntax, SyntaxToken>(WrappedType, nameof(ThrowKeyword));
            WithExpressionAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<ExpressionSyntax, ExpressionSyntax>(WrappedType, nameof(Expression));
        }

        public SyntaxToken ThrowKeyword
        {
            get
            {
                return ThrowKeywordAccessor(this.SyntaxNode);
            }
        }

        public ExpressionSyntax Expression
        {
            get
            {
                return ExpressionAccessor(this.SyntaxNode);
            }
        }

        public ThrowExpressionSyntaxWrapper WithThrowKeyword(SyntaxToken throwKeyword)
        {
            return new ThrowExpressionSyntaxWrapper(WithThrowKeywordAccessor(this.SyntaxNode, throwKeyword));
        }

        public ThrowExpressionSyntaxWrapper WithExpression(ExpressionSyntax expression)
        {
            return new ThrowExpressionSyntaxWrapper(WithExpressionAccessor(this.SyntaxNode, expression));
        }
    }
}
