// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp9.SpacingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using StyleCop.Analyzers.Test.CSharp8.SpacingRules;
    using Xunit;
    using static StyleCop.Analyzers.SpacingRules.SA1015ClosingGenericBracketsMustBeSpacedCorrectly;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.SpacingRules.SA1015ClosingGenericBracketsMustBeSpacedCorrectly,
        StyleCop.Analyzers.SpacingRules.TokenSpacingCodeFixProvider>;

    public partial class SA1015CSharp9UnitTests : SA1015CSharp8UnitTests
    {
        [Fact]
        [WorkItem(3970, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3970")]
        public async Task TestFunctionPointerClosingBracketSpacingAsync()
        {
            var testCode = @"public class TestClass
{
    private unsafe delegate*<int, void {|#0:>|}field1;
    private unsafe delegate*<int, void {|#1:>|} field2;
}
";

            var fixedCode = @"public class TestClass
{
    private unsafe delegate*<int, void> field1;
    private unsafe delegate*<int, void> field2;
}
";

            var expected = new[]
            {
                Diagnostic(DescriptorNotPreceded).WithLocation(0),
                Diagnostic(DescriptorFollowed).WithLocation(0),
                Diagnostic(DescriptorNotPreceded).WithLocation(1),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
