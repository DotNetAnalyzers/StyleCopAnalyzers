// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp10.Lightup
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using StyleCop.Analyzers.Lightup;
    using StyleCop.Analyzers.Test.CSharp9.Lightup;
    using Xunit;

    public partial class RecordDeclarationSyntaxWrapperCSharp10UnitTests : RecordDeclarationSyntaxWrapperCSharp9UnitTests
    {
        [Fact]
        public void TestPropertiesOfRecordStruct()
        {
            var syntaxNode = ParseRecordDeclaration("public readonly record struct S<T>(T Value) : System.IEquatable<S<T>> where T : struct { }");

            Assert.True(syntaxNode.IsKind(SyntaxKindEx.RecordStructDeclaration));
            VerifyWrapperProperties(syntaxNode);

            var wrapper = (RecordDeclarationSyntaxWrapper)syntaxNode;

            Assert.True(wrapper.Keyword.IsKind(SyntaxKind.RecordKeyword));
            Assert.True(wrapper.ClassOrStructKeyword.IsKind(SyntaxKind.StructKeyword));
            Assert.Equal("S", wrapper.Identifier.ValueText);
            Assert.Single(wrapper.TypeParameterList.Parameters);
            Assert.Single(wrapper.ParameterList.Parameters);
            Assert.Single(wrapper.BaseList.Types);
            Assert.Single(wrapper.ConstraintClauses);
            Assert.True(wrapper.SemicolonToken.IsKind(SyntaxKind.None));
        }
    }
}
