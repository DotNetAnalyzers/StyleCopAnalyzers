// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp8.LayoutRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using StyleCop.Analyzers.Test.CSharp7.LayoutRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.LayoutRules.SA1510ChainedStatementBlocksMustNotBePrecededByBlankLine,
        StyleCop.Analyzers.LayoutRules.SA1510CodeFixProvider>;

    public partial class SA1510CSharp8UnitTests : SA1510CSharp7UnitTests
    {
        [Fact]
        [WorkItem(3003, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3003")]
        public async Task TestCatchAfterSwitchExpressionWithBlankLineAsync()
        {
            var testCode = @"
public class TestClass
{
    public int Test(int value)
    {
        try
        {
            return value switch { 0 => 0, _ => 1 };
        }

        {|#0:catch|} (System.Exception)
        {
            return -1;
        }
    }
}
";
            var fixedCode = @"
public class TestClass
{
    public int Test(int value)
    {
        try
        {
            return value switch { 0 => 0, _ => 1 };
        }
        catch (System.Exception)
        {
            return -1;
        }
    }
}
";

            var expected = Diagnostic().WithLocation(0).WithArguments("catch");
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
