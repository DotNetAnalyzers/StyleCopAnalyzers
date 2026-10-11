// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp8.Lightup
{
    using System;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using StyleCop.Analyzers.Lightup;
    using Xunit;

    public partial class NullableDirectiveTriviaSyntaxWrapperCSharp8UnitTests
    {
        [Fact]
        public void TestNull()
        {
            var syntaxNode = default(SyntaxNode);
            var wrapper = (NullableDirectiveTriviaSyntaxWrapper)syntaxNode;
            Assert.Null(wrapper.SyntaxNode);
            Assert.Throws<NullReferenceException>(() => wrapper.HashToken);
            Assert.Throws<NullReferenceException>(() => wrapper.EndOfDirectiveToken);
            Assert.Throws<NullReferenceException>(() => wrapper.IsActive);
        }

        /// <summary>
        /// Verifies the properties of the wrapper which override a property of <see cref="DirectiveTriviaSyntax"/>.
        /// </summary>
        [Fact]
        public void TestOverriddenProperties()
        {
            var syntaxTree = SyntaxFactory.ParseSyntaxTree(
                "#nullable enable\r\nclass C { }\r\n",
                new CSharpParseOptions(LanguageVersion.CSharp8));
            Assert.Empty(syntaxTree.GetDiagnostics());

            var syntaxNode = syntaxTree.GetRoot().DescendantNodes(descendIntoTrivia: true).OfType<NullableDirectiveTriviaSyntax>().Single();
            Assert.True(NullableDirectiveTriviaSyntaxWrapper.IsInstance(syntaxNode));

            var wrapper = (NullableDirectiveTriviaSyntaxWrapper)syntaxNode;
            Assert.Same(syntaxNode, wrapper.SyntaxNode);
            Assert.Equal(syntaxNode.HashToken, wrapper.HashToken);
            Assert.True(wrapper.HashToken.IsKind(SyntaxKind.HashToken));
            Assert.Equal(syntaxNode.NullableKeyword, wrapper.NullableKeyword);
            Assert.Equal(syntaxNode.SettingToken, wrapper.SettingToken);
            Assert.True(wrapper.SettingToken.IsKind(SyntaxKind.EnableKeyword));
            Assert.Equal(syntaxNode.EndOfDirectiveToken, wrapper.EndOfDirectiveToken);
            Assert.True(wrapper.EndOfDirectiveToken.IsKind(SyntaxKind.EndOfDirectiveToken));
            Assert.Equal(syntaxNode.IsActive, wrapper.IsActive);
            Assert.True(wrapper.IsActive);
        }
    }
}
