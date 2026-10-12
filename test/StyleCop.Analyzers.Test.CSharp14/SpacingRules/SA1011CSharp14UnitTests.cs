// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp14.SpacingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using StyleCop.Analyzers.Test.CSharp13.SpacingRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.SpacingRules.SA1011ClosingSquareBracketsMustBeSpacedCorrectly,
        StyleCop.Analyzers.SpacingRules.TokenSpacingCodeFixProvider>;

    public partial class SA1011CSharp14UnitTests : SA1011CSharp13UnitTests
    {
        [Fact]
        [WorkItem(4028, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4028")]
        public async Task TestFieldIndexerAsync()
        {
            var testCode = @"
public class TestClass
{
    private int result;

    public int[] Items
    {
        get => field;
        set
        {
            field = value;
            this.result = field[0 {|#0:]|};
        }
    }
}
";

            var fixedCode = @"
public class TestClass
{
    private int result;

    public int[] Items
    {
        get => field;
        set
        {
            field = value;
            this.result = field[0];
        }
    }
}
";

            var expected = Diagnostic().WithLocation(0).WithArguments(" not", "preceded");

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
