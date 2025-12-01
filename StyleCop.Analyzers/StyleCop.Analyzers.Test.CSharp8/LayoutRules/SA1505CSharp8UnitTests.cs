// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp8.LayoutRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp7.LayoutRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.LayoutRules.SA1505OpeningBracesMustNotBeFollowedByBlankLine,
        StyleCop.Analyzers.LayoutRules.SA1505CodeFixProvider>;

    public partial class SA1505CSharp8UnitTests : SA1505CSharp7UnitTests
    {
        [Fact]
        [WorkItem(3003, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3003")]
        public async Task TestSwitchExpressionOpeningBraceFollowedByBlankLineAsync()
        {
            var testCode = @"
public class TestClass
{
    public int Test(Wrapper value)
    {
        return value switch
        {|#0:{|}

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
        public async Task TestPropertyPatternOpeningBraceFollowedByBlankLineAsync()
        {
            var testCode = @"
public class TestClass
{
    public bool Test(Wrapper value)
    {
        return value is Wrapper
        {|#0:{|}

            X: 1,
            Y: 2,
        };
    }
}

public class Wrapper
{
    public int X { get; set; }

    public int Y { get; set; }
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
            Y: 2,
        };
    }
}

public class Wrapper
{
    public int X { get; set; }

    public int Y { get; set; }
}
";

            var expected = Diagnostic().WithLocation(0);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
