// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal partial struct SwitchExpressionArmSyntaxWrapper : ISyntaxWrapper<CSharpSyntaxNode>
    {
        static SwitchExpressionArmSyntaxWrapper()
        {
            WrappedType = SyntaxWrapperHelper.GetWrappedType(typeof(SwitchExpressionArmSyntaxWrapper));
            PatternAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<CSharpSyntaxNode, CSharpSyntaxNode>(WrappedType, nameof(Pattern));
            WhenClauseAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<CSharpSyntaxNode, CSharpSyntaxNode>(WrappedType, nameof(WhenClause));
            ExpressionAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<CSharpSyntaxNode, ExpressionSyntax>(WrappedType, nameof(Expression));
            EqualsGreaterThanTokenAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<CSharpSyntaxNode, SyntaxToken>(WrappedType, nameof(EqualsGreaterThanToken));

            WithPatternAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<CSharpSyntaxNode, CSharpSyntaxNode>(WrappedType, nameof(Pattern));
            WithWhenClauseAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<CSharpSyntaxNode, CSharpSyntaxNode>(WrappedType, nameof(WhenClause));
            WithExpressionAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<CSharpSyntaxNode, ExpressionSyntax>(WrappedType, nameof(Expression));
            WithEqualsGreaterThanTokenAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<CSharpSyntaxNode, SyntaxToken>(WrappedType, nameof(EqualsGreaterThanToken));
        }

        public PatternSyntaxWrapper Pattern
        {
            get
            {
                return (PatternSyntaxWrapper)PatternAccessor(this.SyntaxNode);
            }
        }

        public WhenClauseSyntaxWrapper WhenClause
        {
            get
            {
                return (WhenClauseSyntaxWrapper)WhenClauseAccessor(this.SyntaxNode);
            }
        }

        public ExpressionSyntax Expression
        {
            get
            {
                return ExpressionAccessor(this.SyntaxNode);
            }
        }

        public SyntaxToken EqualsGreaterThanToken
        {
            get
            {
                return EqualsGreaterThanTokenAccessor(this.SyntaxNode);
            }
        }

        public SwitchExpressionArmSyntaxWrapper WithPattern(PatternSyntaxWrapper pattern)
        {
            return new SwitchExpressionArmSyntaxWrapper(WithPatternAccessor(this.SyntaxNode, pattern.SyntaxNode));
        }

        public SwitchExpressionArmSyntaxWrapper WithWhenClause(WhenClauseSyntaxWrapper whenClause)
        {
            return new SwitchExpressionArmSyntaxWrapper(WithWhenClauseAccessor(this.SyntaxNode, whenClause));
        }

        public SwitchExpressionArmSyntaxWrapper WithExpression(ExpressionSyntax expression)
        {
            return new SwitchExpressionArmSyntaxWrapper(WithExpressionAccessor(this.SyntaxNode, expression));
        }

        public SwitchExpressionArmSyntaxWrapper WithEqualsGreaterThanToken(SyntaxToken equalsGreaterThanToken)
        {
            return new SwitchExpressionArmSyntaxWrapper(WithEqualsGreaterThanTokenAccessor(this.SyntaxNode, equalsGreaterThanToken));
        }
    }
}
