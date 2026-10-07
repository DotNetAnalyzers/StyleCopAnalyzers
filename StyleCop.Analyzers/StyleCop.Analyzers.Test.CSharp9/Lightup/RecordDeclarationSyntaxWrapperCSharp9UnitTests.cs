// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp9.Lightup
{
    using System;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using StyleCop.Analyzers.Lightup;
    using Xunit;

    /// <summary>
    /// Tests for <see cref="RecordDeclarationSyntaxWrapper"/>.
    /// </summary>
    /// <remarks>
    /// <para>Most properties of this wrapper override a property of <see cref="TypeDeclarationSyntax"/> or one of its
    /// base types. These tests read every one of them, which would overflow the stack if the generated wrapper
    /// delegated back to a light-up extension method that in turn calls the wrapper (see
    /// <see cref="TypeDeclarationSyntaxExtensions.ParameterList(TypeDeclarationSyntax)"/>).</para>
    /// </remarks>
    public partial class RecordDeclarationSyntaxWrapperCSharp9UnitTests
    {
        [Fact]
        public void TestNull()
        {
            var syntaxNode = default(SyntaxNode);
            var wrapper = (RecordDeclarationSyntaxWrapper)syntaxNode;
            Assert.Null(wrapper.SyntaxNode);
            Assert.Throws<NullReferenceException>(() => wrapper.AttributeLists);
            Assert.Throws<NullReferenceException>(() => wrapper.Modifiers);
            Assert.Throws<NullReferenceException>(() => wrapper.Keyword);
            Assert.Throws<NullReferenceException>(() => wrapper.ClassOrStructKeyword);
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
        public void TestPropertiesOfRecordWithBody()
        {
            var syntaxNode = ParseRecordDeclaration(@"
[System.Obsolete]
public sealed record R<T>(int X, int Y) : System.Object where T : class
{
    private readonly int z;
};
");

            Assert.True(syntaxNode.IsKind(SyntaxKindEx.RecordDeclaration));
            VerifyWrapperProperties(syntaxNode);

            var wrapper = (RecordDeclarationSyntaxWrapper)syntaxNode;

            Assert.Single(wrapper.AttributeLists);
            Assert.Equal(2, wrapper.Modifiers.Count);
            Assert.True(wrapper.Keyword.IsKind(SyntaxKind.RecordKeyword));
            Assert.True(wrapper.ClassOrStructKeyword.IsKind(SyntaxKind.None));
            Assert.Equal("R", wrapper.Identifier.ValueText);
            Assert.Single(wrapper.TypeParameterList.Parameters);
            Assert.Equal(2, wrapper.ParameterList.Parameters.Count);
            Assert.Single(wrapper.BaseList.Types);
            Assert.Single(wrapper.ConstraintClauses);
            Assert.True(wrapper.OpenBraceToken.IsKind(SyntaxKind.OpenBraceToken));
            Assert.Single(wrapper.Members);
            Assert.True(wrapper.CloseBraceToken.IsKind(SyntaxKind.CloseBraceToken));
            Assert.True(wrapper.SemicolonToken.IsKind(SyntaxKind.SemicolonToken));
        }

        [Fact]
        public void TestPropertiesOfRecordWithoutBody()
        {
            var syntaxNode = ParseRecordDeclaration("internal record P(string Name);");

            Assert.True(syntaxNode.IsKind(SyntaxKindEx.RecordDeclaration));
            VerifyWrapperProperties(syntaxNode);

            var wrapper = (RecordDeclarationSyntaxWrapper)syntaxNode;

            Assert.Empty(wrapper.AttributeLists);
            Assert.Single(wrapper.Modifiers);
            Assert.Null(wrapper.TypeParameterList);
            Assert.Single(wrapper.ParameterList.Parameters);
            Assert.Null(wrapper.BaseList);
            Assert.Empty(wrapper.ConstraintClauses);
            Assert.True(wrapper.OpenBraceToken.IsKind(SyntaxKind.None));
            Assert.Empty(wrapper.Members);
            Assert.True(wrapper.CloseBraceToken.IsKind(SyntaxKind.None));
            Assert.True(wrapper.SemicolonToken.IsKind(SyntaxKind.SemicolonToken));
        }

        protected static TypeDeclarationSyntax ParseRecordDeclaration(string source)
        {
            var syntaxTree = SyntaxFactory.ParseSyntaxTree(source, new CSharpParseOptions(LanguageVersion.Latest));
            Assert.Empty(syntaxTree.GetDiagnostics());

            var syntaxNode = syntaxTree.GetRoot().DescendantNodes().OfType<TypeDeclarationSyntax>().Single();
            Assert.True(RecordDeclarationSyntaxWrapper.IsInstance(syntaxNode));
            return syntaxNode;
        }

        /// <summary>
        /// Verifies that every property of the wrapper matches the corresponding property of the wrapped node, and
        /// that the light-up extension for <c>ParameterList</c> on the base type agrees with the wrapper.
        /// </summary>
        /// <param name="syntaxNode">The record declaration.</param>
        protected static void VerifyWrapperProperties(TypeDeclarationSyntax syntaxNode)
        {
            var recordDeclaration = (RecordDeclarationSyntax)syntaxNode;
            var wrapper = (RecordDeclarationSyntaxWrapper)syntaxNode;
            Assert.Same(syntaxNode, wrapper.SyntaxNode);
            Assert.Equal(recordDeclaration.AttributeLists, wrapper.AttributeLists); // This is a struct, so we can't use Same()
            Assert.Equal(recordDeclaration.Modifiers, wrapper.Modifiers);
            Assert.Equal(recordDeclaration.Keyword, wrapper.Keyword);
            Assert.Equal(recordDeclaration.Identifier, wrapper.Identifier);
            Assert.Same(recordDeclaration.TypeParameterList, wrapper.TypeParameterList);
            Assert.Same(recordDeclaration.ParameterList, wrapper.ParameterList);
            Assert.Same(recordDeclaration.BaseList, wrapper.BaseList);
            Assert.Equal(recordDeclaration.ConstraintClauses, wrapper.ConstraintClauses);
            Assert.Equal(recordDeclaration.OpenBraceToken, wrapper.OpenBraceToken);
            Assert.Equal(recordDeclaration.Members, wrapper.Members);
            Assert.Equal(recordDeclaration.CloseBraceToken, wrapper.CloseBraceToken);
            Assert.Equal(recordDeclaration.SemicolonToken, wrapper.SemicolonToken);

            // Prior to C# 12, this extension method calls RecordDeclarationSyntaxWrapper.ParameterList
            Assert.Same(recordDeclaration.ParameterList, TypeDeclarationSyntaxExtensions.ParameterList(syntaxNode));
        }
    }
}
