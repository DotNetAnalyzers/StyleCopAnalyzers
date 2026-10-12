// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp14.Lightup
{
    using System;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using StyleCop.Analyzers.Lightup;
    using Xunit;

    public partial class ExtensionBlockDeclarationSyntaxWrapperCSharp14UnitTests
    {
        [Fact]
        public void TestNull()
        {
            var syntaxNode = default(SyntaxNode);
            var wrapper = (ExtensionBlockDeclarationSyntaxWrapper)syntaxNode;
            Assert.Null(wrapper.SyntaxNode);
            Assert.Throws<NullReferenceException>(() => wrapper.AttributeLists);
            Assert.Throws<NullReferenceException>(() => wrapper.Modifiers);
            Assert.Throws<NullReferenceException>(() => wrapper.Keyword);
            Assert.Throws<NullReferenceException>(() => wrapper.TypeParameterList);
            Assert.Throws<NullReferenceException>(() => wrapper.ParameterList);
            Assert.Throws<NullReferenceException>(() => wrapper.ConstraintClauses);
            Assert.Throws<NullReferenceException>(() => wrapper.OpenBraceToken);
            Assert.Throws<NullReferenceException>(() => wrapper.Members);
            Assert.Throws<NullReferenceException>(() => wrapper.CloseBraceToken);
            Assert.Throws<NullReferenceException>(() => wrapper.SemicolonToken);
        }

        /// <summary>
        /// Verifies the properties of the wrapper, which all override a property of <see cref="TypeDeclarationSyntax"/>
        /// or one of its base types.
        /// </summary>
        [Fact]
        public void TestProperties()
        {
            var syntaxTree = SyntaxFactory.ParseSyntaxTree(
                "static class E\r\n{\r\n    extension<T>(T value) where T : class\r\n    {\r\n        public int M() => 0;\r\n        public int P => 0;\r\n    }\r\n}\r\n",
                new CSharpParseOptions(LanguageVersion.Preview));
            Assert.Empty(syntaxTree.GetDiagnostics());

            var syntaxNode = syntaxTree.GetRoot().DescendantNodes().OfType<ExtensionBlockDeclarationSyntax>().Single();
            Assert.True(syntaxNode.IsKind(SyntaxKindEx.ExtensionBlockDeclaration));
            Assert.True(ExtensionBlockDeclarationSyntaxWrapper.IsInstance(syntaxNode));

            var wrapper = (ExtensionBlockDeclarationSyntaxWrapper)syntaxNode;
            Assert.Same(syntaxNode, wrapper.SyntaxNode);
            Assert.Equal(syntaxNode.AttributeLists, wrapper.AttributeLists); // This is a struct, so we can't use Same()
            Assert.Equal(syntaxNode.Modifiers, wrapper.Modifiers);
            Assert.Equal(syntaxNode.Keyword, wrapper.Keyword);
            Assert.True(wrapper.Keyword.IsKind(SyntaxKind.ExtensionKeyword));
            Assert.Same(syntaxNode.TypeParameterList, wrapper.TypeParameterList);
            Assert.Single(wrapper.TypeParameterList.Parameters);
            Assert.Same(syntaxNode.ParameterList, wrapper.ParameterList);
            Assert.Single(wrapper.ParameterList.Parameters);
            Assert.Same(syntaxNode.ParameterList, TypeDeclarationSyntaxExtensions.ParameterList(syntaxNode));
            Assert.Equal(syntaxNode.ConstraintClauses, wrapper.ConstraintClauses);
            Assert.Single(wrapper.ConstraintClauses);
            Assert.Equal(syntaxNode.OpenBraceToken, wrapper.OpenBraceToken);
            Assert.Equal(syntaxNode.Members, wrapper.Members);
            Assert.Equal(2, wrapper.Members.Count);
            Assert.Equal(syntaxNode.CloseBraceToken, wrapper.CloseBraceToken);
            Assert.Equal(syntaxNode.SemicolonToken, wrapper.SemicolonToken);
        }
    }
}
