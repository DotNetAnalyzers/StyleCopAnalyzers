// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp8.LayoutRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using StyleCop.Analyzers.Test.CSharp7.LayoutRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.LayoutRules.SA1511WhileDoFooterMustNotBePrecededByBlankLine,
        StyleCop.Analyzers.LayoutRules.SA1511CodeFixProvider>;

    public partial class SA1511CSharp8UnitTests : SA1511CSharp7UnitTests
    {
        [Fact]
        [WorkItem(3003, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3003")]
        public async Task TestDoWhileWithPatternConditionAndBlankLineAsync()
        {
            var testCode = @"
public class TestClass
{
    public void Test(object value)
    {
        do
        {
            value = new object();
        }

        {|#0:while|} (value is int);
    }
}
";
            var fixedCode = @"
public class TestClass
{
    public void Test(object value)
    {
        do
        {
            value = new object();
        }
        while (value is int);
    }
}
";

            var expected = Diagnostic().WithLocation(0);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
