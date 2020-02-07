// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp7.Lightup
{
    using System;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using StyleCop.Analyzers.Lightup;
    using StyleCop.Analyzers.Test.Lightup;
    using Xunit;

    public class SwitchExpressionArmSyntaxWrapperTestsCSharp7 : SwitchExpressionArmSyntaxWrapperTests
    {
        [Fact]
        public void TestWithPatternOnNullNode()
        {
            var switchExpressionSyntax = (SwitchExpressionArmSyntaxWrapper)default(SyntaxNode);

            var patternSyntax = SyntaxFactory.ConstantPattern(SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression));
            Assert.Throws<NullReferenceException>(() => switchExpressionSyntax.WithPattern((PatternSyntaxWrapper)patternSyntax));
        }

        [Fact]
        public void TestWithWhenClauseOnNullNode()
        {
            var switchExpressionSyntax = (SwitchExpressionArmSyntaxWrapper)default(SyntaxNode);

            var whenClause = SyntaxFactory.WhenClause(SyntaxFactory.LiteralExpression(SyntaxKind.TrueLiteralExpression));
            Assert.Throws<NullReferenceException>(() => switchExpressionSyntax.WithWhenClause((WhenClauseSyntaxWrapper)whenClause));
        }
    }
}
