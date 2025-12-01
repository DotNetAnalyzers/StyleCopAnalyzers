// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp8.SpacingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using StyleCop.Analyzers.Test.CSharp7.SpacingRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.SpacingRules.SA1001CommasMustBeSpacedCorrectly,
        StyleCop.Analyzers.SpacingRules.TokenSpacingCodeFixProvider>;

    public partial class SA1001CSharp8UnitTests : SA1001CSharp7UnitTests
    {
        [Fact]
        [WorkItem(3003, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3003")]
        public async Task TestTuplePatternCommaSpacingAsync()
        {
            var testCode = @"
public class TestClass
{
    public int Test(object value) => value switch
    {
        (1{|#0:,|}2) => 0,
        _ => 1,
    };
}
";
            var fixedCode = @"
public class TestClass
{
    public int Test(object value) => value switch
    {
        (1, 2) => 0,
        _ => 1,
    };
}
";

            var expected = Diagnostic().WithLocation(0).WithArguments(string.Empty, "followed");
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3003, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3003")]
        public async Task TestSwitchExpressionArmSeparatorSpacingAsync()
        {
            var testCode = @"
public class TestClass
{
    public string Test(int value) => value switch { 1 => ""one""{|#0:,|}2 => ""two"", _ => ""other"" };
}
";
            var fixedCode = @"
public class TestClass
{
    public string Test(int value) => value switch { 1 => ""one"", 2 => ""two"", _ => ""other"" };
}
";

            var expected = Diagnostic().WithLocation(0).WithArguments(string.Empty, "followed");
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
