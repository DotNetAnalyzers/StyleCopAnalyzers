// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp11.SpacingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using StyleCop.Analyzers.Test.CSharp10.SpacingRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.SpacingRules.SA1001CommasMustBeSpacedCorrectly,
        StyleCop.Analyzers.SpacingRules.TokenSpacingCodeFixProvider>;

    public partial class SA1001CSharp11UnitTests : SA1001CSharp10UnitTests
    {
        [Fact]
        [WorkItem(4005, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4005")]
        public async Task TestScopedParameterAsync()
        {
            var testCode = @"
public class TestClass
{
    public void Method(int a{|#0:,|}scoped System.Span<int> b)
    {
    }
}
";

            var fixedCode = @"
public class TestClass
{
    public void Method(int a, scoped System.Span<int> b)
    {
    }
}
";

            var expected = Diagnostic().WithArguments(string.Empty, "followed").WithLocation(0);

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
