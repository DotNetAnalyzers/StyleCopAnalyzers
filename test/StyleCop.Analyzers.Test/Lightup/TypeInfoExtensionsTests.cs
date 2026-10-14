// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.Lightup;

using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using StyleCop.Analyzers.Lightup;
using Xunit;

public class TypeInfoExtensionsTests
{
    [Fact]
    public void NullabilityPropertiesDefaultWhenNullableContextIsDisabled()
    {
        var syntaxTree = CSharpSyntaxTree.ParseText("class C { object M(object value) => value; }");
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
