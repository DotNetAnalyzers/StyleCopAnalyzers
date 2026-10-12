// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp14.SpacingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using StyleCop.Analyzers.Test.CSharp13.SpacingRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.SpacingRules.SA1020IncrementDecrementSymbolsMustBeSpacedCorrectly,
        StyleCop.Analyzers.SpacingRules.TokenSpacingCodeFixProvider>;

    public partial class SA1020CSharp14UnitTests : SA1020CSharp13UnitTests
    {
        [Fact]
        [WorkItem(4028, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4028")]
        public async Task TestFieldPostfixIncrementAsync()
        {
            var testCode = @"
public class TestClass
{
    public int Prop
    {
        get => field;
        set
        {
            field = value;
            field {|#0:++|};
        }
    }
}
";

            var fixedCode = @"
public class TestClass
{
    public int Prop
    {
        get => field;
        set
        {
            field = value;
            field++;
        }
    }
}
";

            var expected = Diagnostic().WithLocation(0).WithArguments("Increment", "++", "preceded");

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(4028, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4028")]
        public async Task TestFieldPrefixIncrementAsync()
        {
            var testCode = @"
public class TestClass
{
    public int Prop
    {
        get => field;
        set
        {
            field = value;
            {|#0:++|} field;
        }
    }
}
";

            var fixedCode = @"
public class TestClass
{
    public int Prop
    {
        get => field;
        set
        {
            field = value;
            ++field;
        }
    }
}
";

            var expected = Diagnostic().WithLocation(0).WithArguments("Increment", "++", "followed");

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
