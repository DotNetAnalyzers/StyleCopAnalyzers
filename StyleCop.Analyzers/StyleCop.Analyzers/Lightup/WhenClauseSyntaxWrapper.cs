// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal partial struct WhenClauseSyntaxWrapper : ISyntaxWrapper<CSharpSyntaxNode>
    {
        private static readonly Func<CSharpSyntaxNode, SyntaxToken> WhenKeywordAccessor;
        private static readonly Func<CSharpSyntaxNode, ExpressionSyntax> ConditionAccessor;
        private static readonly Func<CSharpSyntaxNode, SyntaxToken, CSharpSyntaxNode> WithWhenKeywordAccessor;
        private static readonly Func<CSharpSyntaxNode, ExpressionSyntax, CSharpSyntaxNode> WithConditionAccessor;

        static WhenClauseSyntaxWrapper()
        {
            WrappedType = SyntaxWrapperHelper.GetWrappedType(typeof(WhenClauseSyntaxWrapper));
            WhenKeywordAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<CSharpSyntaxNode, SyntaxToken>(WrappedType, nameof(WhenKeyword));
            ConditionAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<CSharpSyntaxNode, ExpressionSyntax>(WrappedType, nameof(Condition));
            WithWhenKeywordAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<CSharpSyntaxNode, SyntaxToken>(WrappedType, nameof(WhenKeyword));
            WithConditionAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<CSharpSyntaxNode, ExpressionSyntax>(WrappedType, nameof(Condition));
        }

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
