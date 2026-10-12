// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp15.Lightup
{
    using System;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using StyleCop.Analyzers.Lightup;
    using Xunit;

    /// <summary>
    /// Tests for <see cref="UnionDeclarationSyntaxWrapper"/>.
    /// </summary>
    /// <remarks>
    /// <para>Most properties of this wrapper override a property of <see cref="TypeDeclarationSyntax"/> or one of its
    /// base types. These tests read every one of them, including <c>ParameterList</c> through
    /// <see cref="TypeDeclarationSyntaxExtensions.ParameterList(TypeDeclarationSyntax)"/>.</para>
    /// </remarks>
    public partial class UnionDeclarationSyntaxWrapperCSharp15UnitTests
    {
        [Fact]
        public void TestNull()
        {
            var syntaxNode = default(SyntaxNode);
            var wrapper = (UnionDeclarationSyntaxWrapper)syntaxNode;
            Assert.Null(wrapper.SyntaxNode);
            Assert.Throws<NullReferenceException>(() => wrapper.AttributeLists);
            Assert.Throws<NullReferenceException>(() => wrapper.Modifiers);
            Assert.Throws<NullReferenceException>(() => wrapper.Keyword);
            Assert.Throws<NullReferenceException>(() => wrapper.Identifier);
            Assert.Throws<NullReferenceException>(() => wrapper.TypeParameterList);
            Assert.Throws<NullReferenceException>(() => wrapper.ParameterList);
            Assert.Throws<NullReferenceException>(() => wrapper.BaseList);
            Assert.Throws<NullReferenceException>(() => wrapper.ConstraintClauses);
            Assert.Throws<NullReferenceException>(() => wrapper.OpenBraceToken);
            Assert.Throws<NullReferenceException>(() => wrapper.Members);
            Assert.Throws<NullReferenceException>(() => wrapper.CloseBraceToken);
            Assert.Throws<NullReferenceException>(() => wrapper.SemicolonToken);
        }

        [Fact]
        public void TestPropertiesOfUnionWithBody()
        {
            var syntaxNode = ParseUnionDeclaration(@"
[System.Obsolete]
public union U<T>(int, T) : System.IDisposable where T : class
{
    public void Dispose() { }
}
");

            VerifyWrapperProperties(syntaxNode);

            var wrapper = (UnionDeclarationSyntaxWrapper)syntaxNode;
            Assert.Single(wrapper.AttributeLists);
            Assert.Single(wrapper.Modifiers);
            Assert.True(wrapper.Keyword.IsKind(SyntaxKind.UnionKeyword));
            Assert.Equal("U", wrapper.Identifier.ValueText);
            Assert.Single(wrapper.TypeParameterList.Parameters);
            Assert.Equal(2, wrapper.ParameterList.Parameters.Count);
            Assert.Single(wrapper.BaseList.Types);
            Assert.Single(wrapper.ConstraintClauses);
            Assert.True(wrapper.OpenBraceToken.IsKind(SyntaxKind.OpenBraceToken));
            Assert.Single(wrapper.Members);
            Assert.True(wrapper.CloseBraceToken.IsKind(SyntaxKind.CloseBraceToken));
            Assert.True(wrapper.SemicolonToken.IsKind(SyntaxKind.None));
        }

        [Fact]
        public void TestPropertiesOfUnionWithoutBody()
        {
            var syntaxNode = ParseUnionDeclaration("internal union Pet(string, int);");

            VerifyWrapperProperties(syntaxNode);

            var wrapper = (UnionDeclarationSyntaxWrapper)syntaxNode;
            Assert.Empty(wrapper.AttributeLists);
            Assert.Single(wrapper.Modifiers);
            Assert.Equal("Pet", wrapper.Identifier.ValueText);
            Assert.Null(wrapper.TypeParameterList);
            Assert.Equal(2, wrapper.ParameterList.Parameters.Count);
            Assert.Null(wrapper.BaseList);
            Assert.Empty(wrapper.ConstraintClauses);
            Assert.True(wrapper.OpenBraceToken.IsKind(SyntaxKind.None));
            Assert.Empty(wrapper.Members);
            Assert.True(wrapper.CloseBraceToken.IsKind(SyntaxKind.None));
            Assert.True(wrapper.SemicolonToken.IsKind(SyntaxKind.SemicolonToken));
        }

        private static TypeDeclarationSyntax ParseUnionDeclaration(string source)
        {
            var syntaxTree = SyntaxFactory.ParseSyntaxTree(source, new CSharpParseOptions(LanguageVersion.Preview));
            Assert.Empty(syntaxTree.GetDiagnostics());

            var syntaxNode = syntaxTree.GetRoot().DescendantNodes().OfType<TypeDeclarationSyntax>().Single();
            Assert.True(syntaxNode.IsKind(SyntaxKind.UnionDeclaration));
            Assert.True(UnionDeclarationSyntaxWrapper.IsInstance(syntaxNode));
            return syntaxNode;
        }

        private static void VerifyWrapperProperties(TypeDeclarationSyntax syntaxNode)
        {
            var unionDeclaration = (UnionDeclarationSyntax)syntaxNode;
            var wrapper = (UnionDeclarationSyntaxWrapper)syntaxNode;
            Assert.Same(syntaxNode, wrapper.SyntaxNode);
            Assert.Equal(unionDeclaration.AttributeLists, wrapper.AttributeLists); // This is a struct, so we can't use Same()
            Assert.Equal(unionDeclaration.Modifiers, wrapper.Modifiers);
            Assert.Equal(unionDeclaration.Keyword, wrapper.Keyword);
            Assert.Equal(unionDeclaration.Identifier, wrapper.Identifier);
            Assert.Same(unionDeclaration.TypeParameterList, wrapper.TypeParameterList);
            Assert.Same(unionDeclaration.ParameterList, wrapper.ParameterList);
            Assert.Same(unionDeclaration.BaseList, wrapper.BaseList);
            Assert.Equal(unionDeclaration.ConstraintClauses, wrapper.ConstraintClauses);
            Assert.Equal(unionDeclaration.OpenBraceToken, wrapper.OpenBraceToken);
            Assert.Equal(unionDeclaration.Members, wrapper.Members);
            Assert.Equal(unionDeclaration.CloseBraceToken, wrapper.CloseBraceToken);
            Assert.Equal(unionDeclaration.SemicolonToken, wrapper.SemicolonToken);

            // The light-up extension on the base type must agree with the wrapper
            Assert.Same(unionDeclaration.ParameterList, TypeDeclarationSyntaxExtensions.ParameterList(syntaxNode));
        }
    }
}
