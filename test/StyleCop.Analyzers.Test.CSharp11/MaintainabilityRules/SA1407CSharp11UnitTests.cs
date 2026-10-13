// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp11.MaintainabilityRules;

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Testing;
using StyleCop.Analyzers.Test.CSharp10.MaintainabilityRules;
using Xunit;
using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
    StyleCop.Analyzers.MaintainabilityRules.SA1407ArithmeticExpressionsMustDeclarePrecedence,
    StyleCop.Analyzers.MaintainabilityRules.SA1407SA1408CodeFixProvider>;

public partial class SA1407CSharp11UnitTests : SA1407CSharp10UnitTests
{
    [Fact]
    [WorkItem(3547, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3547")]
    public async Task TestUnsignedRightShiftOperatorPrecedenceAsync()
    {
        var testCode = @"public class Foo
{
    public void Bar()
    {
        int x = 1 >>> {|#0:2 + 3|};
    }
}";
        var expected = Diagnostic().WithLocation(0);

        var fixedCode = @"public class Foo
{
    public void Bar()
    {
        int x = 1 >>> (2 + 3);
    }
}";

        await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
    }

    [Fact]
    [WorkItem(3547, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3547")]
    public async Task TestUnsignedRightShiftSamePrecedenceAndParenthesizedAsync()
    {
        var testCode = @"public class Foo
{
    public void Bar()
    {
        int chained = 1 >> 2 >>> 3;
        int parenthesized = 1 >>> (2 + 3);
    }
}";

        await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
    }
}
