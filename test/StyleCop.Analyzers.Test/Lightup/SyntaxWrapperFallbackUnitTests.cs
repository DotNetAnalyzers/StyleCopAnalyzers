// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.Lightup;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using StyleCop.Analyzers.Lightup;
using Xunit;

public class SyntaxWrapperFallbackUnitTests
{
    [Fact]
    [WorkItem(3258, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3258")]
    public void TestBaseParameterFallback()
    {
        var parameter = SyntaxFactory.Parameter(SyntaxFactory.Identifier("value"))
            .WithType(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword)))
            .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.RefKeyword)));
        Assert.True(BaseParameterSyntaxWrapper.IsInstance(parameter));
        BaseParameterSyntaxWrapper implicitWrapper = parameter;
        Assert.Same(parameter, implicitWrapper.SyntaxNode);
        var wrapper = (BaseParameterSyntaxWrapper)parameter;
        Assert.Same(parameter, wrapper.SyntaxNode);
        Assert.Same(parameter.Type, wrapper.Type);
        Assert.Equal(parameter.Modifiers, wrapper.Modifiers);
        Assert.Equal(parameter.AttributeLists, wrapper.AttributeLists);
        Assert.Equal("string", wrapper.WithType(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.StringKeyword))).Type!.ToString());
        Assert.Empty(wrapper.WithModifiers(default).Modifiers);
        Assert.Null(wrapper.WithType(null).Type);
        var attributes = SyntaxFactory.SingletonList(SyntaxFactory.AttributeList());
        Assert.Equal(attributes.ToFullString(), wrapper.WithAttributeLists(attributes).AttributeLists.ToFullString());
        Assert.False(BaseParameterSyntaxWrapper.IsInstance(SyntaxFactory.IdentifierName("value")));
        Assert.False(BaseParameterSyntaxWrapper.IsInstance(null));
        Assert.Throws<System.InvalidCastException>(() => (BaseParameterSyntaxWrapper)SyntaxFactory.IdentifierName("value"));
        Assert.Null(((BaseParameterSyntaxWrapper)(SyntaxNode?)null).SyntaxNode);
        BaseParameterSyntaxWrapper nullWrapper = (ParameterSyntax?)null;
        Assert.Null(nullWrapper.SyntaxNode);
        Assert.Throws<System.NullReferenceException>(() => nullWrapper.Type);
        Assert.Throws<System.NullReferenceException>(() => nullWrapper.Modifiers);
        Assert.Throws<System.NullReferenceException>(() => nullWrapper.AttributeLists);
        Assert.Throws<System.NullReferenceException>(() => nullWrapper.WithType(null));
        Assert.Throws<System.NullReferenceException>(() => nullWrapper.WithModifiers(default));
        Assert.Throws<System.NullReferenceException>(() => nullWrapper.WithAttributeLists(default));
    }

    [Fact]
    [WorkItem(3258, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3258")]
    public void TestExpressionOrPatternFallback()
    {
        var expression = SyntaxFactory.IdentifierName("value");
        Assert.True(ExpressionOrPatternSyntaxWrapper.IsInstance(expression));
        Assert.True(LightupHelpers.CanWrapExpressionOrPattern(expression, typeof(ExpressionSyntax)));
        Assert.False(LightupHelpers.CanWrapExpressionOrPattern(SyntaxFactory.Parameter(SyntaxFactory.Identifier("value")), typeof(ExpressionSyntax)));
        Assert.Same(expression, ((ExpressionOrPatternSyntaxWrapper)expression).SyntaxNode);
        Assert.False(ExpressionOrPatternSyntaxWrapper.IsInstance(SyntaxFactory.Parameter(SyntaxFactory.Identifier("value"))));
        Assert.False(ExpressionOrPatternSyntaxWrapper.IsInstance(null));
        Assert.Throws<System.InvalidCastException>(() => (ExpressionOrPatternSyntaxWrapper)SyntaxFactory.Parameter(SyntaxFactory.Identifier("value")));
        Assert.Null(((ExpressionOrPatternSyntaxWrapper)(SyntaxNode?)null).SyntaxNode);
    }
}
