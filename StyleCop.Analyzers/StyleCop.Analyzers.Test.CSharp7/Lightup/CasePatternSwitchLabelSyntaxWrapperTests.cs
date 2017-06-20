// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp7.Lightup
{
    using System;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using StyleCop.Analyzers.Lightup;
    using Xunit;

    public class CasePatternSwitchLabelSyntaxWrapperTests
    {
        [Fact]
        public void TestNull()
        {
            var syntaxNode = default(SyntaxNode);
            var wrapper = (CasePatternSwitchLabelSyntaxWrapper)syntaxNode;
            Assert.Null(wrapper.SyntaxNode);
            Assert.Throws<NullReferenceException>(() => wrapper.Pattern);
            Assert.Throws<NullReferenceException>(() => wrapper.WhenClause);
            Assert.Throws<NullReferenceException>(() => wrapper.WithKeyword(SyntaxFactory.Token(SyntaxKind.CaseKeyword)));
            Assert.Throws<NullReferenceException>(() => wrapper.WithColonToken(SyntaxFactory.Token(SyntaxKind.ColonToken)));
            Assert.Throws<NullReferenceException>(() => wrapper.WithPattern((PatternSyntaxWrapper)null));
            Assert.Throws<NullReferenceException>(() => wrapper.WithWhenClause((WhenClauseSyntaxWrapper)null));
        }

        [Fact]
        public void TestProperties()
        {
            var syntaxNode = this.CreateCasePatternSwitchLabel();
            Assert.True(syntaxNode.IsKind(SyntaxKind.CasePatternSwitchLabel));
            Assert.True(syntaxNode.IsKind(SyntaxKindEx.CasePatternSwitchLabel));

            var wrapper = (CasePatternSwitchLabelSyntaxWrapper)syntaxNode;
            Assert.Same(syntaxNode, wrapper.SyntaxNode);
            Assert.Same(syntaxNode.Pattern, wrapper.Pattern.SyntaxNode);
            Assert.Same(syntaxNode.WhenClause, wrapper.WhenClause.SyntaxNode);
            ////There are no properties for Keyword and ColonToken, so we can't test them..

            var newKeyword = SyntaxFactory.Token(SyntaxKind.CaseKeyword).WithLeadingTrivia(SyntaxFactory.Space);
            var wrapperWithModifiedKeyword = wrapper.WithKeyword(newKeyword);
            Assert.NotNull(wrapperWithModifiedKeyword.SyntaxNode);
            ////There is no property for Keyword, so we can't test it...
            ////Assert.Equal(1, wrapperWithModifiedKeyword.Keyword.LeadingTrivia.Count);
            ////Assert.Equal(" ", wrapperWithModifiedKeyword.Keyword.LeadingTrivia.ToString());

            var newColonToken = SyntaxFactory.Token(SyntaxKind.ColonToken).WithLeadingTrivia(SyntaxFactory.Space);
            var wrapperWithModifiedColonToken = wrapper.WithColonToken(newColonToken);
            Assert.NotNull(wrapperWithModifiedColonToken.SyntaxNode);
            ////There is no property for ColonToken, so we can't test it...
            ////Assert.Equal(1, wrapperWithModifiedColonToken.ColonToken.LeadingTrivia.Count);
            ////Assert.Equal(" ", wrapperWithModifiedColonToken.ColonToken.LeadingTrivia.ToString());

            var newPattern = SyntaxFactory.ConstantPattern(SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression));
            var wrapperWithModifiedPattern = wrapper.WithPattern((PatternSyntaxWrapper)newPattern);
            Assert.NotNull(wrapperWithModifiedPattern.SyntaxNode);
            Assert.NotSame(syntaxNode.Pattern, wrapperWithModifiedPattern.Pattern.SyntaxNode);
            Assert.Equal(SyntaxKind.ConstantPattern, wrapperWithModifiedPattern.Pattern.SyntaxNode.Kind());
            Assert.Equal(SyntaxKind.NullLiteralExpression, ((ConstantPatternSyntax)wrapperWithModifiedPattern.Pattern).Expression.Kind());

            var newWhenClause = SyntaxFactory.WhenClause(SyntaxFactory.LiteralExpression(SyntaxKind.TrueLiteralExpression));
            var wrapperWithModifiedWhenClause = wrapper.WithWhenClause((WhenClauseSyntaxWrapper)newWhenClause);
            Assert.NotNull(wrapperWithModifiedWhenClause.SyntaxNode);
            Assert.NotSame(syntaxNode.WhenClause, wrapperWithModifiedWhenClause.WhenClause.SyntaxNode);
            Assert.Equal(SyntaxKind.WhenClause, wrapperWithModifiedWhenClause.WhenClause.SyntaxNode.Kind());
            Assert.Equal(SyntaxKind.TrueLiteralExpression, wrapperWithModifiedWhenClause.WhenClause.Condition.Kind());
        }

        [Fact]
        public void TestIsInstance()
        {
            Assert.False(CasePatternSwitchLabelSyntaxWrapper.IsInstance(null));
            Assert.False(CasePatternSwitchLabelSyntaxWrapper.IsInstance(SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression)));

            var syntaxNode = this.CreateCasePatternSwitchLabel();
            Assert.True(CasePatternSwitchLabelSyntaxWrapper.IsInstance(syntaxNode));
        }

        [Fact]
        public void TestConversionsNull()
        {
            var syntaxNode = default(SyntaxNode);
            var wrapper = (CasePatternSwitchLabelSyntaxWrapper)syntaxNode;

            SwitchLabelSyntax syntax = wrapper;
            Assert.Null(syntax);
        }

        [Fact]
        public void TestConversions()
        {
            var syntaxNode = this.CreateCasePatternSwitchLabel();
            var wrapper = (CasePatternSwitchLabelSyntaxWrapper)syntaxNode;

            SwitchLabelSyntax syntax = wrapper;
            Assert.Same(syntaxNode, syntax);
        }

        [Fact]
        public void TestInvalidConversion()
        {
            var syntaxNode = SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression);
            Assert.Throws<InvalidCastException>(() => (CasePatternSwitchLabelSyntaxWrapper)syntaxNode);
        }

        private CasePatternSwitchLabelSyntax CreateCasePatternSwitchLabel()
        {
            return SyntaxFactory.CasePatternSwitchLabel(
                SyntaxFactory.Token(SyntaxKind.CaseKeyword),
                SyntaxFactory.ConstantPattern(SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression)),
                SyntaxFactory.WhenClause(SyntaxFactory.LiteralExpression(SyntaxKind.FalseLiteralExpression)),
                SyntaxFactory.Token(SyntaxKind.ColonToken));
        }
    }
}
