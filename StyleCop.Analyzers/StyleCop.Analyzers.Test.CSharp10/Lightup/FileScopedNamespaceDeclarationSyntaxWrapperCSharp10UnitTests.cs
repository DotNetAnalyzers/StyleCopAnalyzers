// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp10.Lightup
{
    using System;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using StyleCop.Analyzers.Lightup;
    using Xunit;

    public partial class FileScopedNamespaceDeclarationSyntaxWrapperCSharp10UnitTests
    {
        [Fact]
        public void TestNull()
        {
            var syntaxNode = default(SyntaxNode);
            var wrapper = (FileScopedNamespaceDeclarationSyntaxWrapper)syntaxNode;
            Assert.Null(wrapper.SyntaxNode);
            Assert.Throws<NullReferenceException>(() => wrapper.AttributeLists);
            Assert.Throws<NullReferenceException>(() => wrapper.Modifiers);
            Assert.Throws<NullReferenceException>(() => wrapper.NamespaceKeyword);
            Assert.Throws<NullReferenceException>(() => wrapper.Name);
            Assert.Throws<NullReferenceException>(() => wrapper.SemicolonToken);
            Assert.Throws<NullReferenceException>(() => wrapper.Externs);
            Assert.Throws<NullReferenceException>(() => wrapper.Usings);
            Assert.Throws<NullReferenceException>(() => wrapper.Members);
        }

        /// <summary>
        /// Verifies the properties of the wrapper, most of which override a property of
        /// <see cref="BaseNamespaceDeclarationSyntaxWrapper"/> or <see cref="MemberDeclarationSyntax"/>.
        /// </summary>
        [Fact]
        public void TestProperties()
        {
            var syntaxTree = SyntaxFactory.ParseSyntaxTree(
                "extern alias A;\r\nnamespace N.M;\r\nusing System;\r\nclass C { }\r\nclass D { }\r\n",
                new CSharpParseOptions(LanguageVersion.CSharp10));
            Assert.Empty(syntaxTree.GetDiagnostics());

            var syntaxNode = syntaxTree.GetRoot().DescendantNodes().OfType<FileScopedNamespaceDeclarationSyntax>().Single();
            Assert.True(FileScopedNamespaceDeclarationSyntaxWrapper.IsInstance(syntaxNode));

            var wrapper = (FileScopedNamespaceDeclarationSyntaxWrapper)syntaxNode;
            Assert.Same(syntaxNode, wrapper.SyntaxNode);
            Assert.Equal(syntaxNode.AttributeLists, wrapper.AttributeLists); // This is a struct, so we can't use Same()
            Assert.Equal(syntaxNode.Modifiers, wrapper.Modifiers);
            Assert.Equal(syntaxNode.NamespaceKeyword, wrapper.NamespaceKeyword);
            Assert.Same(syntaxNode.Name, wrapper.Name);
            Assert.Equal("N.M", wrapper.Name.ToString());
            Assert.Equal(syntaxNode.SemicolonToken, wrapper.SemicolonToken);
            Assert.Equal(syntaxNode.Externs, wrapper.Externs);
            Assert.Empty(wrapper.Externs);
            Assert.Equal(syntaxNode.Usings, wrapper.Usings);
            Assert.Single(wrapper.Usings);
            Assert.Equal(syntaxNode.Members, wrapper.Members);
            Assert.Equal(2, wrapper.Members.Count);

            // The same values are available through the base wrapper type
            var baseWrapper = (BaseNamespaceDeclarationSyntaxWrapper)wrapper;
            Assert.Same(wrapper.Name, baseWrapper.Name);
            Assert.Equal(wrapper.Usings, baseWrapper.Usings);
            Assert.Equal(wrapper.Members, baseWrapper.Members);
        }
    }
}
