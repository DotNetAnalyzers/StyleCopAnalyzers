// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp8.Lightup;

using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using StyleCop.Analyzers.Lightup;
using Xunit;
using NullabilityInfo = StyleCop.Analyzers.Lightup.NullabilityInfo;
using NullableAnnotation = StyleCop.Analyzers.Lightup.NullableAnnotation;
using NullableFlowState = StyleCop.Analyzers.Lightup.NullableFlowState;

public class TypeInfoExtensionsCSharp8UnitTests
{
    [Fact]
    public void NullabilityPropertiesExposeNullableAnnotationsAndFlowState()
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(
            "#nullable enable\nclass C { void M() { object? value = null; value.ToString(); } }",
            new CSharpParseOptions(LanguageVersion.CSharp8));
        var compilation = CSharpCompilation.Create(
            "Test",
            new[] { syntaxTree },
            new[] { MetadataReference.CreateFromFile(typeof(object).Assembly.Location) },
            new CSharpCompilationOptions(
                OutputKind.DynamicallyLinkedLibrary,
                nullableContextOptions: NullableContextOptions.Enable));
        var expression = syntaxTree.GetRoot().DescendantNodes().OfType<IdentifierNameSyntax>()
            .Single(node => node.Identifier.ValueText == "value");
        var typeInfo = compilation.GetSemanticModel(syntaxTree).GetTypeInfo(expression);

        Assert.Equal(NullableAnnotation.Annotated, typeInfo.Nullability().Annotation);
        Assert.Equal(NullableFlowState.MaybeNull, typeInfo.Nullability().FlowState);
        Assert.Equal(NullableAnnotation.Annotated, typeInfo.ConvertedNullability().Annotation);
        Assert.Equal(NullableFlowState.MaybeNull, typeInfo.ConvertedNullability().FlowState);
        Assert.Equal(
            new NullabilityInfo(NullableAnnotation.Annotated, NullableFlowState.MaybeNull),
            typeInfo.Nullability());
        Assert.Equal(
            new NullabilityInfo(NullableAnnotation.Annotated, NullableFlowState.MaybeNull),
            typeInfo.ConvertedNullability());
    }

    [Fact]
    public void ConvertedNullabilityCanDifferFromNullability()
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(
            "#nullable enable\nclass B { } class C { void M() { C value = new C(); M(value); } void M(B? value) { } public static implicit operator B?(C value) => null; }",
            new CSharpParseOptions(LanguageVersion.CSharp8));
        var compilation = CSharpCompilation.Create(
            "Test",
            new[] { syntaxTree },
            new[] { MetadataReference.CreateFromFile(typeof(object).Assembly.Location) },
            new CSharpCompilationOptions(
                OutputKind.DynamicallyLinkedLibrary,
                nullableContextOptions: NullableContextOptions.Enable));
        var expression = syntaxTree.GetRoot().DescendantNodes().OfType<IdentifierNameSyntax>()
            .Single(node => node.Identifier.ValueText == "value" && node.Parent is ArgumentSyntax);
        var typeInfo = compilation.GetSemanticModel(syntaxTree).GetTypeInfo(expression);

        Assert.Equal(NullableAnnotation.NotAnnotated, typeInfo.Nullability().Annotation);
        Assert.Equal(NullableFlowState.NotNull, typeInfo.Nullability().FlowState);
        Assert.Equal(NullableAnnotation.Annotated, typeInfo.ConvertedNullability().Annotation);
        Assert.Equal(NullableFlowState.MaybeNull, typeInfo.ConvertedNullability().FlowState);
        Assert.Equal(
            new NullabilityInfo(NullableAnnotation.NotAnnotated, NullableFlowState.NotNull),
            typeInfo.Nullability());
        Assert.Equal(
            new NullabilityInfo(NullableAnnotation.Annotated, NullableFlowState.MaybeNull),
            typeInfo.ConvertedNullability());
    }

    [Fact]
    public void NullabilityPropertiesDefaultWhenNullableContextIsDisabled()
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(
            "class C { object M(object value) => value; }",
            new CSharpParseOptions(LanguageVersion.CSharp8));
        var compilation = CSharpCompilation.Create(
            "Test",
            new[] { syntaxTree },
            new[] { MetadataReference.CreateFromFile(typeof(object).Assembly.Location) });
        var expression = syntaxTree.GetRoot().DescendantNodes().OfType<IdentifierNameSyntax>().Single();
        var typeInfo = compilation.GetSemanticModel(syntaxTree).GetTypeInfo(expression);

        Assert.Equal(NullableAnnotation.None, typeInfo.Nullability().Annotation);
        Assert.Equal(NullableFlowState.None, typeInfo.Nullability().FlowState);
        Assert.Equal(NullableAnnotation.None, typeInfo.ConvertedNullability().Annotation);
        Assert.Equal(NullableFlowState.None, typeInfo.ConvertedNullability().FlowState);
    }
}
