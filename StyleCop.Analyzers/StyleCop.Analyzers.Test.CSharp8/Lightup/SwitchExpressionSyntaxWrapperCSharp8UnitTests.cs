// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.CSharp8.Lightup
{
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using StyleCop.Analyzers.Lightup;
    using StyleCop.Analyzers.Test.CSharp7.Lightup;
    using Xunit;

    public partial class SwitchExpressionSyntaxWrapperCSharp8UnitTests : SwitchExpressionSyntaxWrapperCSharp7UnitTests
    {
        [Fact]
        public void TestSyntaxNodeProperty()
        {
            var switchExpression = this.CreateSwitchExpression();

            Assert.True(switchExpression.IsKind(SyntaxKind.SwitchExpression));
            Assert.True(switchExpression.IsKind(SyntaxKindEx.SwitchExpression));

            var wrapper = (SwitchExpressionSyntaxWrapper)switchExpression;
            Assert.Same(switchExpression, wrapper.SyntaxNode);
        }

        [Fact]
        public void TestArmsProperty()
        {
            var switchExpression = this.CreateSwitchExpression();

            var wrapper = (SwitchExpressionSyntaxWrapper)switchExpression;
            Assert.Single(switchExpression.Arms);
            Assert.Single(wrapper.Arms);
            Assert.Same(switchExpression.Arms[0], wrapper.Arms[0].SyntaxNode);

            var arm = SyntaxFactory.SwitchExpressionArm(
                SyntaxFactory.ConstantPattern(SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression)),
                SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression));
            var arms = SyntaxFactory.SingletonSeparatedList(arm);

            var wrapperWithArms = wrapper.WithArms(new SeparatedSyntaxListWrapper<SwitchExpressionArmSyntaxWrapper>.AutoWrapSeparatedSyntaxList<SwitchExpressionArmSyntax>(arms));
            Assert.Single(wrapperWithArms.Arms);
            Assert.True(arm.IsEquivalentTo(wrapperWithArms.Arms.Single()));
        }

        [Fact]
        public void TestGoverningExpressionProperty()
        {
            var switchExpression = this.CreateSwitchExpression();

            var wrapper = (SwitchExpressionSyntaxWrapper)switchExpression;
            Assert.Same(switchExpression.GoverningExpression, wrapper.GoverningExpression);

            var newGoverningExpression = SyntaxFactory.IdentifierName("x");
            var wrapperWithGoverningExpression = wrapper.WithGoverningExpression(newGoverningExpression);

            Assert.True(newGoverningExpression.IsEquivalentTo(wrapperWithGoverningExpression.GoverningExpression));
        }

        [Fact]
        public void TestOpenBraceTokenProperty()
        {
            var switchExpression = this.CreateSwitchExpression();

            var wrapper = (SwitchExpressionSyntaxWrapper)switchExpression;
            Assert.Equal(switchExpression.OpenBraceToken, wrapper.OpenBraceToken);

            var newOpenBraceToken = SyntaxFactory.Token(
                SyntaxTriviaList.Create(SyntaxFactory.SyntaxTrivia(SyntaxKind.SingleLineCommentTrivia, "/* 1 */")),
                SyntaxKind.OpenBraceToken,
                SyntaxTriviaList.Empty);
            var wrapperWithOpenBrace = wrapper.WithOpenBraceToken(newOpenBraceToken);
            Assert.True(newOpenBraceToken.IsEquivalentTo(wrapperWithOpenBrace.OpenBraceToken));
        }

        [Fact]
        public void TestSwitchKeywordProperty()
        {
            var switchExpression = this.CreateSwitchExpression();

            var wrapper = (SwitchExpressionSyntaxWrapper)switchExpression;
            Assert.Equal(switchExpression.SwitchKeyword, wrapper.SwitchKeyword);

            var newSwitchKeyword = SyntaxFactory.Token(
                SyntaxTriviaList.Create(SyntaxFactory.SyntaxTrivia(SyntaxKind.SingleLineCommentTrivia, "/* 1 */")),
                SyntaxKind.SwitchKeyword,
                SyntaxTriviaList.Empty);
            var wrapperWithSwitchKeyword = wrapper.WithSwitchKeyword(newSwitchKeyword);
            Assert.True(newSwitchKeyword.IsEquivalentTo(wrapperWithSwitchKeyword.SwitchKeyword));
        }

        [Fact]
        public void TestCloseBraceTokenProperty()
        {
            var switchExpression = this.CreateSwitchExpression();

            var wrapper = (SwitchExpressionSyntaxWrapper)switchExpression;
            Assert.Equal(switchExpression.CloseBraceToken, wrapper.CloseBraceToken);

            var newCloseBraceToken = SyntaxFactory.Token(
                SyntaxTriviaList.Create(SyntaxFactory.SyntaxTrivia(SyntaxKind.SingleLineCommentTrivia, "/* 1 */")),
                SyntaxKind.CloseBraceToken,
                SyntaxTriviaList.Empty);
            var wrapperWithCloseBrace = wrapper.WithCloseBraceToken(newCloseBraceToken);
            Assert.True(newCloseBraceToken.IsEquivalentTo(wrapperWithCloseBrace.CloseBraceToken));
        }

        [Fact]
        public void TestIsInstanceTrue()
        {
            var switchExpression = this.CreateSwitchExpression();
            Assert.True(SwitchExpressionSyntaxWrapper.IsInstance(switchExpression));
        }

        [Fact]
        public void TestConversions()
        {
            var switchExpressionSyntax = this.CreateSwitchExpression();
            var wrapper = (SwitchExpressionSyntaxWrapper)switchExpressionSyntax;

            CSharpSyntaxNode syntax = wrapper;
            Assert.Same(switchExpressionSyntax, syntax);
        }

        private SwitchExpressionSyntax CreateSwitchExpression()
            => SyntaxFactory.SwitchExpression(
                SyntaxFactory.IdentifierName(SyntaxFactory.Identifier("type")),
                SyntaxFactory.SeparatedList(new[] { this.CreateSwitchExpressionArm() }));

        private SwitchExpressionArmSyntax CreateSwitchExpressionArm()
            => SyntaxFactory.SwitchExpressionArm(
                SyntaxFactory.DiscardPattern(),
                SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression));
    }
}
