// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp8.SpacingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using StyleCop.Analyzers.Test.CSharp7.SpacingRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.SpacingRules.SA1000KeywordsMustBeSpacedCorrectly,
        StyleCop.Analyzers.SpacingRules.TokenSpacingCodeFixProvider>;

    public partial class SA1000CSharp8UnitTests : SA1000CSharp7UnitTests
    {
        [Fact]
        [WorkItem(3003, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3003")]
        public async Task TestSwitchExpressionKeywordSpacingAsync()
        {
            var testCode = @"
public class TestClass
{
    public int Test(int value) => value {|#0:switch|}{ 1 => 0, _ => 1 };
}
";
            var fixedCode = @"
public class TestClass
{
    public int Test(int value) => value switch { 1 => 0, _ => 1 };
}
";

            var expected = Diagnostic().WithLocation(0).WithArguments("switch", string.Empty);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3003, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3003")]
        public async Task TestIsKeywordSpacingInPropertyPatternAsync()
        {
            var testCode = @"
public class TestClass
{
    public bool Test(SomeType value) => value {|#0:is|}{ Length: 1 };
}

public class SomeType
{
    public int Length { get; set; }
}
";
            var fixedCode = @"
public class TestClass
{
    public bool Test(SomeType value) => value is { Length: 1 };
}

public class SomeType
{
    public int Length { get; set; }
}
";

            var expected = Diagnostic().WithLocation(0).WithArguments("is", string.Empty);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
