// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.Lightup
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
        public void TestIsInstance()
        {
            Assert.False(CasePatternSwitchLabelSyntaxWrapper.IsInstance(null));
            Assert.False(CasePatternSwitchLabelSyntaxWrapper.IsInstance(SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression)));
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
        public void TestInvalidConversion()
        {
            var syntaxNode = SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression);
            Assert.Throws<InvalidCastException>(() => (CasePatternSwitchLabelSyntaxWrapper)syntaxNode);
        }
    }
}
