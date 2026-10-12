// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp10.SpacingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp9.SpacingRules;
    using Xunit;
    using static StyleCop.Analyzers.SpacingRules.SA1024ColonsMustBeSpacedCorrectly;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.SpacingRules.SA1024ColonsMustBeSpacedCorrectly,
        StyleCop.Analyzers.SpacingRules.TokenSpacingCodeFixProvider>;

    public partial class SA1024CSharp10UnitTests : SA1024CSharp9UnitTests
    {
        [Fact]
        [WorkItem(3984, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3984")]
        public async Task TestExtendedPropertyPatternSpacingAsync()
        {
            var testCode = @"
public class TestClass
{
    public bool Test(SomeType value) => value is { Nested.Value: 1, Other.Value {|#0::|} 2, Third.Value{|#1::|}3 };
}

public class SomeType
{
    public SomeType Nested { get; set; }
    public SomeType Other { get; set; }
    public SomeType Third { get; set; }
    public SomeType Child { get; set; }
    public int Value { get; set; }
}";

            var fixedCode = @"
public class TestClass
{
    public bool Test(SomeType value) => value is { Nested.Value: 1, Other.Value: 2, Third.Value: 3 };
}

public class SomeType
{
    public SomeType Nested { get; set; }
    public SomeType Other { get; set; }
    public SomeType Third { get; set; }
    public SomeType Child { get; set; }
    public int Value { get; set; }
}";

            DiagnosticResult[] expected =
            {
                Diagnostic(DescriptorNotPreceded).WithLocation(0),
                Diagnostic(DescriptorFollowed).WithLocation(1),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
