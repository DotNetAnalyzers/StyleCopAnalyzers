// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.HelperTests
{
    using System.Linq;
    using Analyzers.Helpers;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Xunit;

    public class TokenHelperTests
    {
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
