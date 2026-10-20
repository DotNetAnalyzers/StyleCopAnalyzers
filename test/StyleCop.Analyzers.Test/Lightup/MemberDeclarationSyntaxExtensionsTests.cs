// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.Lightup;

using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using StyleCop.Analyzers.Lightup;
using Xunit;

public class MemberDeclarationSyntaxExtensionsTests
{
    [Theory]
    [InlineData("class C { }")]
    [InlineData("struct S { }")]
    [InlineData("interface I { }")]
    [InlineData("enum E { }")]
    [InlineData("delegate void D();")]
    [InlineData("class C { public int }")]
    [InlineData("class C { void M() { } }")]
    [InlineData("class C { C() { } }")]
    [InlineData("class C { ~C() { } }")]
    [InlineData("class C { int P { get; set; } }")]
    [InlineData("class C { int this[int i] { get { return 0; } } }")]
    [InlineData("class C { event System.Action E { add { } remove { } } }")]
    [InlineData("class C { event System.Action E; }")]
    [InlineData("class C { int f; }")]
    [InlineData("class C { public static C operator +(C x, C y) { return x; } }")]
    [InlineData("class C { public static explicit operator int(C x) { return 0; } }")]
    [WorkItem(3258, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3258")]
    public void TestDerivedMemberFallback(string source)
    {
        var member = SyntaxFactory.ParseCompilationUnit(source).Members[0];
        if (member is ClassDeclarationSyntax { Members.Count: > 0 } type)
        {
            member = type.Members[0];
        }

        var attributes = SyntaxFactory.SingletonList(SyntaxFactory.AttributeList(
            SyntaxFactory.SingletonSeparatedList(SyntaxFactory.Attribute(SyntaxFactory.IdentifierName("A")))));
        var modifiers = SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword));
        member = MemberDeclarationSyntaxExtensions.WithAttributeLists(member, attributes);
        member = MemberDeclarationSyntaxExtensions.WithModifiers(member, modifiers);
        Assert.Equal(attributes.ToFullString(), MemberDeclarationSyntaxExtensions.AttributeLists(member).ToFullString());
        Assert.Equal(modifiers.ToFullString(), MemberDeclarationSyntaxExtensions.Modifiers(member).ToFullString());
        Assert.Equal(2, MemberDeclarationSyntaxExtensions.AttributeLists(MemberDeclarationSyntaxExtensions.AddAttributeLists(member, attributes[0])).Count);
        Assert.Equal(2, MemberDeclarationSyntaxExtensions.Modifiers(MemberDeclarationSyntaxExtensions.AddModifiers(member, SyntaxFactory.Token(SyntaxKind.StaticKeyword))).Count);
        Assert.Empty(MemberDeclarationSyntaxExtensions.AttributeLists(MemberDeclarationSyntaxExtensions.WithAttributeLists(member, default)));
        Assert.Empty(MemberDeclarationSyntaxExtensions.Modifiers(MemberDeclarationSyntaxExtensions.WithModifiers(member, default)));
    }

    [Fact]
    public void TestEnumMemberAttributes()
    {
        var member = SyntaxFactory.EnumMemberDeclaration("Value");
        var attribute = SyntaxFactory.AttributeList(SyntaxFactory.SingletonSeparatedList(SyntaxFactory.Attribute(SyntaxFactory.IdentifierName("A"))));
        var updated = MemberDeclarationSyntaxExtensions.AddAttributeLists(member, attribute);
        Assert.Equal("[A]", MemberDeclarationSyntaxExtensions.AttributeLists(updated).ToFullString());
        Assert.Empty(MemberDeclarationSyntaxExtensions.AttributeLists(MemberDeclarationSyntaxExtensions.WithAttributeLists(updated, default)));
        Assert.Empty(MemberDeclarationSyntaxExtensions.Modifiers(member));
    }

    [Fact]
    public void TestUnsupportedMemberFallback()
    {
        var member = SyntaxFactory.GlobalStatement(SyntaxFactory.EmptyStatement());
        Assert.Empty(MemberDeclarationSyntaxExtensions.AttributeLists(member));
        Assert.Empty(MemberDeclarationSyntaxExtensions.Modifiers(member));
        if (typeof(MemberDeclarationSyntax).GetProperty("AttributeLists") == null)
        {
            Assert.Same(member, MemberDeclarationSyntaxExtensions.WithAttributeLists(member, default));
            Assert.Same(member, MemberDeclarationSyntaxExtensions.WithModifiers(member, default));
            Assert.Throws<NotSupportedException>(() => MemberDeclarationSyntaxExtensions.AddAttributeLists(member, SyntaxFactory.AttributeList()));
            Assert.Throws<NotSupportedException>(() => MemberDeclarationSyntaxExtensions.AddModifiers(member, SyntaxFactory.Token(SyntaxKind.PublicKeyword)));
        }
    }

    [Fact]
    public void TestNullMember()
    {
        Assert.Throws<NullReferenceException>(() => MemberDeclarationSyntaxExtensions.AttributeLists(null!));
        Assert.Throws<NullReferenceException>(() => MemberDeclarationSyntaxExtensions.Modifiers(null!));
        Assert.Throws<NullReferenceException>(() => MemberDeclarationSyntaxExtensions.WithAttributeLists(null!, default));
        Assert.Throws<NullReferenceException>(() => MemberDeclarationSyntaxExtensions.WithModifiers(null!, default));
    }
}
