// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp8.LayoutRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using StyleCop.Analyzers.Test.CSharp7.LayoutRules;
    using Xunit;

    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.LayoutRules.SA1508ClosingBracesMustNotBePrecededByBlankLine,
        StyleCop.Analyzers.LayoutRules.SA1508CodeFixProvider>;

    public partial class SA1508CSharp8UnitTests : SA1508CSharp7UnitTests
    {
        [Fact]
        [WorkItem(3003, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3003")]
        public async Task TestSwitchExpressionClosingBracePrecededByBlankLineAsync()
        {
            var testCode = @"
public class TestClass
{
    public int Test(Wrapper value)
    {
        return value switch
        {
            { X: 1 } => 1,
            _ => 0,

        {|#0:}|};
    }
}

public class Wrapper
{
    public int X { get; set; }
}
";
            var fixedCode = @"
public class TestClass
{
    public int Test(Wrapper value)
    {
        return value switch
        {
            { X: 1 } => 1,
            _ => 0,
        };
    }
}

public class Wrapper
{
    public int X { get; set; }
}
";

            var expected = Diagnostic().WithLocation(0);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3003, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3003")]
        public async Task TestPropertyPatternClosingBracePrecededByBlankLineAsync()
        {
            var testCode = @"
public class TestClass
{
    public bool Test(Wrapper value)
    {
        return value is Wrapper
        {
            X: 1,

        {|#0:}|};
    }
}

public class Wrapper
{
    public int X { get; set; }
}
";
            var fixedCode = @"
public class TestClass
{
    public bool Test(Wrapper value)
    {
        return value is Wrapper
        {
            X: 1,
        };
    }
}

public class Wrapper
{
    public int X { get; set; }
}
";

            var expected = Diagnostic().WithLocation(0);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
