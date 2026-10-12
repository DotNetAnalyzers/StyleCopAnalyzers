// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp9.SpacingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using StyleCop.Analyzers.Test.CSharp8.SpacingRules;
    using Xunit;

    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.SpacingRules.SA1001CommasMustBeSpacedCorrectly,
        StyleCop.Analyzers.SpacingRules.TokenSpacingCodeFixProvider>;

    public partial class SA1001CSharp9UnitTests : SA1001CSharp8UnitTests
    {
        [Fact]
        [WorkItem(3970, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3970")]
        public async Task TestFunctionPointerParameterSpacingAsync()
        {
            var testCode = @"public class TestClass
{
    private unsafe delegate*<int{|#0:,|}int, void> field1;
    private unsafe delegate*<int {|#1:,|}int, void> field2;
}
";

            var fixedCode = @"public class TestClass
{
    private unsafe delegate*<int, int, void> field1;
    private unsafe delegate*<int, int, void> field2;
}
";

            var expected = new[]
            {
                Diagnostic().WithLocation(0).WithArguments(string.Empty, "followed"),
                Diagnostic().WithLocation(1).WithArguments(" not", "preceded"),
                Diagnostic().WithLocation(1).WithArguments(string.Empty, "followed"),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
