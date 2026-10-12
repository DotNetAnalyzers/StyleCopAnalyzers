// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp8.LayoutRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp7.LayoutRules;
    using StyleCop.Analyzers.Test.Helpers;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.LayoutRules.SA1500BracesForMultiLineStatementsMustNotShareLine,
        StyleCop.Analyzers.LayoutRules.SA1500CodeFixProvider>;

    public partial class SA1500CSharp8UnitTests : SA1500CSharp7UnitTests
    {
        [Fact]
        [WorkItem(3003, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3003")]
        public async Task TestSwitchExpressionBracesMustOwnLineAsync()
        {
            var testCode = @"
public class TestClass
{
    public int Test(Item value)
    {
        return value switch {|#0:{|}
            { Prop: 1 } => 1,
            _ => 0 {|#1:}|};
    }
}

public class Item
{
    public int Prop { get; set; }
}
";

            var fixedCode = @"
public class TestClass
{
    public int Test(Item value)
    {
        return value switch
        {
            { Prop: 1 } => 1,
            _ => 0
        };
    }
}

public class Item
{
    public int Prop { get; set; }
}
";

            DiagnosticResult[] expectedDiagnostics =
            {
                Diagnostic().WithLocation(0),
                Diagnostic().WithLocation(1),
            };

            await VerifyCSharpFixAsync(testCode, expectedDiagnostics, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3003, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3003")]
        public async Task TestPropertyPatternBracesMustOwnLineAsync()
        {
            var testCode = @"
public class TestClass
{
    public bool Test(object value)
    {
        return value is TestType {|#0:{|} A: 1,
            B: 2 {|#1:}|};
    }
}

public class TestType
{
    public int A { get; set; }

    public int B { get; set; }
}
";

            var fixedCode = @"
public class TestClass
{
    public bool Test(object value)
    {
        return value is TestType
        {
            A: 1,
            B: 2
        };
    }
}

public class TestType
{
    public int A { get; set; }

    public int B { get; set; }
}
";

            DiagnosticResult[] expectedDiagnostics =
            {
                Diagnostic().WithLocation(0),
                Diagnostic().WithLocation(1),
            };

            await VerifyCSharpFixAsync(testCode, expectedDiagnostics, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
