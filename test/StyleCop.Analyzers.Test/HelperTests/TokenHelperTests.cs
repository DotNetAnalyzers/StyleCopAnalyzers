// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.HelperTests
{
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using StyleCop.Analyzers.Helpers;
    using Xunit;

    public class TokenHelperTests
    {
        [Fact]
        public void TestIsFirstInLineWithoutSyntaxTree()
        {
            // A token that is not attached to a tree has a null SyntaxTree. Incomplete editor buffers produce these
            // tokens, and the helper must not throw.
            var token = default(SyntaxToken);

            Assert.Null(token.SyntaxTree);
            Assert.True(token.IsFirstInLine());
            Assert.True(token.IsFirstInLine(allowNonWhitespaceTrivia: false));
        }

        [Fact]
        public void TestIsLastInLineWithoutSyntaxTree()
        {
            var token = default(SyntaxToken);

            Assert.Null(token.SyntaxTree);
            Assert.True(token.IsLastInLine());
        }

        [Fact]
        public void TestIsFirstInLineSpecialCase()
        {
            string testCode = @"class MyClass {
} /// <summary>
/// Text
/// </summary>
struct MyStruct { }";
            var syntaxTree = CSharpSyntaxTree.ParseText(testCode);
            var root = syntaxTree.GetRoot();

            Assert.True(root.DescendantTokens().Single(i => i.IsKind(SyntaxKind.StructKeyword)).IsFirstInLine());
        }

        [Fact]
        public void TestIsLastInLineSpecialCase()
        {
            string testCode = @"class MyClass {
} /// <summary>
/// Text
/// </summary>
struct /**
Foo
**/ MyStruct { }";
            var syntaxTree = CSharpSyntaxTree.ParseText(testCode);
            var root = syntaxTree.GetRoot();

            Assert.True(root.DescendantTokens().Single(i => i.IsKind(SyntaxKind.StructKeyword)).IsLastInLine());
        }
    }
}
