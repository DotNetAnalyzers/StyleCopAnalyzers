// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp10.SpacingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using StyleCop.Analyzers.Test.CSharp9.SpacingRules;
    using Xunit;
    using static StyleCop.Analyzers.SpacingRules.SA1008OpeningParenthesisMustBeSpacedCorrectly;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.SpacingRules.SA1008OpeningParenthesisMustBeSpacedCorrectly,
        StyleCop.Analyzers.SpacingRules.TokenSpacingCodeFixProvider>;

    public partial class SA1008CSharp10UnitTests : SA1008CSharp9UnitTests
    {
        [Fact]
        [WorkItem(3985, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3985")]
        public async Task TestLambdaAttributeSpacingAsync()
        {
            var testCode = @"
using System;

class TestClass
{
    void M()
    {
        var f = [My]{|#0:(|}) => 0;
        var g = [My][Other]{|#1:(|}) => 1;
        var h = [My] () => 2;
    }
}

class MyAttribute : Attribute
{
}

class OtherAttribute : Attribute
{
}
";

            var fixedCode = @"
using System;

class TestClass
{
    void M()
    {
        var f = [My] () => 0;
        var g = [My][Other] () => 1;
        var h = [My] () => 2;
    }
}

class MyAttribute : Attribute
{
}

class OtherAttribute : Attribute
{
}
";

            await new CSharpTest
            {
                TestCode = testCode,
                FixedCode = fixedCode,
                ExpectedDiagnostics =
                {
                    Diagnostic(DescriptorPreceded).WithLocation(0),
                    Diagnostic(DescriptorPreceded).WithLocation(1),
                },
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3985, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3985")]
        public async Task TestLambdaReturnTypeSpacingAsync()
        {
            var testCode = @"
class TestClass
{
    void M()
    {
        var projector = (int, int){|#0:(|}int value) => (value, value);
    }
}
";

            var fixedCode = @"
class TestClass
{
    void M()
    {
        var projector = (int, int) (int value) => (value, value);
    }
}
";

            await new CSharpTest
            {
                TestCode = testCode,
                FixedCode = fixedCode,
                ExpectedDiagnostics =
                {
                    Diagnostic(DescriptorPreceded).WithLocation(0),
                },
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }
    }
}
