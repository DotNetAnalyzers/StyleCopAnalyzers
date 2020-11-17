// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal partial struct SwitchExpressionSyntaxWrapper : ISyntaxWrapper<ExpressionSyntax>
    {
        public ExpressionSyntax GoverningExpression
        {
            get
            {
                return GoverningExpressionAccessor(this.SyntaxNode);
            }
        }

        public SeparatedSyntaxListWrapper<SwitchExpressionArmSyntaxWrapper> Arms
        {
            get
            {
                return ArmsAccessor(this.SyntaxNode);
            }
        }

        public SyntaxToken OpenBraceToken
        {
            get
            {
                return OpenBraceTokenAccessor(this.SyntaxNode);
            }
        }

        public SyntaxToken SwitchKeyword
        {
            get
            {
                return SwitchKeywordAccessor(this.SyntaxNode);
            }
        }

        public SyntaxToken CloseBraceToken
        {
            get
            {
                return CloseBraceTokenAccessor(this.SyntaxNode);
            }
        }

        public SwitchExpressionSyntaxWrapper AddArms(params SwitchExpressionArmSyntaxWrapper[] arms)
        {
            return new SwitchExpressionSyntaxWrapper(this.WithArms(this.Arms.AddRange(arms)));
        }

        public SwitchExpressionSyntaxWrapper WithArms(SeparatedSyntaxListWrapper<SwitchExpressionArmSyntaxWrapper> arms)
        {
            return new SwitchExpressionSyntaxWrapper(WithArmsAccessor(this.SyntaxNode, arms));
        }

        public SwitchExpressionSyntaxWrapper WithCloseBraceToken(SyntaxToken closeBraceToken)
        {
            return new SwitchExpressionSyntaxWrapper(WithCloseBraceTokenAccessor(this.SyntaxNode, closeBraceToken));
        }

        public SwitchExpressionSyntaxWrapper WithGoverningExpression(ExpressionSyntax expression)
        {
            return new SwitchExpressionSyntaxWrapper(WithGoverningExpressionAccessor(this.SyntaxNode, expression));
        }

        public SwitchExpressionSyntaxWrapper WithOpenBraceToken(SyntaxToken openBraceToken)
        {
            return new SwitchExpressionSyntaxWrapper(WithOpenBraceTokenAccessor(this.SyntaxNode, openBraceToken));
        }

        public SwitchExpressionSyntaxWrapper WithSwitchKeyword(SyntaxToken switchKeyword)
        {
            return new SwitchExpressionSyntaxWrapper(WithSwitchKeywordAccessor(this.SyntaxNode, switchKeyword));
        }
    }
}
