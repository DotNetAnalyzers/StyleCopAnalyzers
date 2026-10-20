// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp9.Lightup;

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using StyleCop.Analyzers.Lightup;
using Xunit;

public partial class SyntaxWrapperCSharp9UnitTests
{
    [Fact]
    [WorkItem(3258, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3258")]
    public void TestBaseParameterWithFunctionPointerParameter()
    {
        var type = Assert.IsType<FunctionPointerTypeSyntax>(SyntaxFactory.ParseTypeName("delegate*<int, void>"));
        var parameter = type.ParameterList.Parameters[0];
        Assert.True(BaseParameterSyntaxWrapper.IsInstance(parameter));
        var wrapper = (BaseParameterSyntaxWrapper)parameter;
        Assert.Same(parameter, wrapper.SyntaxNode);
        Assert.Same(parameter.Type, wrapper.Type);
        Assert.Empty(wrapper.AttributeLists);
        Assert.Empty(wrapper.Modifiers);
        Assert.Equal("string", wrapper.WithType(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.StringKeyword))).Type!.ToString());
        Assert.Equal("ref", wrapper.WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.RefKeyword))).Modifiers.ToString());
        BaseParameterSyntaxWrapper upcast = (FunctionPointerParameterSyntaxWrapper)parameter;
        Assert.Same(parameter, upcast.SyntaxNode);
        Assert.Same(parameter, ((FunctionPointerParameterSyntaxWrapper)upcast).SyntaxNode);
    }
}
