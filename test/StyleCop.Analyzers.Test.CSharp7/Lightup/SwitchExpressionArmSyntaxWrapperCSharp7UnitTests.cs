// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.CSharp7.Lightup
{
    using System;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using StyleCop.Analyzers.Lightup;
    using StyleCop.Analyzers.Test.Lightup;
    using Xunit;

    public partial class SwitchExpressionArmSyntaxWrapperCSharp7UnitTests : SwitchExpressionArmSyntaxWrapperUnitTests
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
