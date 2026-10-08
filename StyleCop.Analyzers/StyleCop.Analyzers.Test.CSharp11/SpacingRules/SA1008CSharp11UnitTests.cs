// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp11.SpacingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using StyleCop.Analyzers.Test.CSharp10.SpacingRules;
    using Xunit;
    using static StyleCop.Analyzers.SpacingRules.SA1008OpeningParenthesisMustBeSpacedCorrectly;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.SpacingRules.SA1008OpeningParenthesisMustBeSpacedCorrectly,
        StyleCop.Analyzers.SpacingRules.TokenSpacingCodeFixProvider>;

    public partial class SA1008CSharp11UnitTests : SA1008CSharp10UnitTests
    {
        [Fact]
        [WorkItem(4005, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4005")]
        public async Task TestScopedParameterAsync()
        {
            var testCode = @"
public class TestClass
{
    public void Method {|#0:(|}scoped System.Span<int> value)
    {
    }
}
";

            var fixedCode = @"
public class TestClass
{
    public void Method(scoped System.Span<int> value)
    {
    }
}
";

            var expected = Diagnostic(DescriptorNotPreceded).WithLocation(0);

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
