// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp8.LayoutRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using StyleCop.Analyzers.Test.CSharp7.LayoutRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.LayoutRules.SA1506ElementDocumentationHeadersMustNotBeFollowedByBlankLine,
        StyleCop.Analyzers.LayoutRules.SA1506CodeFixProvider>;

    public partial class SA1506CSharp8UnitTests : SA1506CSharp7UnitTests
    {
        [Fact]
        [WorkItem(3003, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3003")]
        public async Task TestDocumentationHeaderBeforeSwitchExpressionAsync()
        {
            var testCode = @"
public class TestClass
{
    /// <summary>
    /// Test method.
    /// </summary>
{|#0:
|}    public int Test(int value) => value switch { 0 => 0, _ => 1 };
}
";
            var fixedCode = @"
public class TestClass
{
    /// <summary>
    /// Test method.
    /// </summary>
    public int Test(int value) => value switch { 0 => 0, _ => 1 };
}
";

            var expected = Diagnostic().WithLocation(0);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
