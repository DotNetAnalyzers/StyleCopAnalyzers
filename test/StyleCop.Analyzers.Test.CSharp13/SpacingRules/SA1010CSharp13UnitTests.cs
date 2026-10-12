// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp13.SpacingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using StyleCop.Analyzers.Test.CSharp12.SpacingRules;
    using Xunit;
    using static StyleCop.Analyzers.SpacingRules.SA1010OpeningSquareBracketsMustBeSpacedCorrectly;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.SpacingRules.SA1010OpeningSquareBracketsMustBeSpacedCorrectly,
        StyleCop.Analyzers.SpacingRules.TokenSpacingCodeFixProvider>;

    public partial class SA1010CSharp13UnitTests : SA1010CSharp12UnitTests
    {
        [Fact]
        [WorkItem(4017, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4017")]
        public async Task TestImplicitIndexInitializerAsync()
        {
            var testCode = @"
public class TimerRemaining
{
    public int[] Buffer { get; set; } = new int[10];
}

public class TestClass
{
    public void TestMethod()
    {
        var countdown = new TimerRemaining()
        {
            Buffer =
            {
                {|#0:[|} ^1] = 0,
            },
        };
    }
}
";

            var fixedTestCode = @"
public class TimerRemaining
{
    public int[] Buffer { get; set; } = new int[10];
}

public class TestClass
{
    public void TestMethod()
    {
        var countdown = new TimerRemaining()
        {
            Buffer =
            {
                [^1] = 0,
            },
        };
    }
}
";

            var expected = Diagnostic(DescriptorNotFollowed).WithLocation(0);

            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
