// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp15.MaintainabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp14.MaintainabilityRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.MaintainabilityRules.SA1407ArithmeticExpressionsMustDeclarePrecedence,
        StyleCop.Analyzers.MaintainabilityRules.SA1407SA1408CodeFixProvider>;

    // Union declarations are only parsed with the preview language version. The reference assemblies used by these
    // tests do not define System.Runtime.CompilerServices.IUnion and UnionAttribute, so union declarations produce
    // CS0518 and CS0656, and compiler diagnostics are therefore ignored.
    public partial class SA1407CSharp15UnitTests : SA1407CSharp14UnitTests
    {
        /// <summary>
        /// Verifies that an arithmetic expression in a union member is reported exactly once. The analyzer driver in
        /// Roslyn 5.9 ran syntax node actions on union members three times (dotnet/roslyn#84570), which reported this
        /// diagnostic three times.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(4182, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4182")]
        public async Task TestArithmeticInUnionMemberReportedOnceAsync()
        {
            var testCode = @"
public union Pet(int, string)
{
    public int Compute()
    {
        return 1 + {|#0:2 * 3|};
    }
}
";

            var fixedCode = @"
public union Pet(int, string)
{
    public int Compute()
    {
        return 1 + (2 * 3);
    }
}
";

            await new CSharpTest(LanguageVersion.Preview)
            {
                TestCode = testCode,
                FixedCode = fixedCode,
                ExpectedDiagnostics = { Diagnostic().WithLocation(0) },
                CompilerDiagnostics = CompilerDiagnostics.None,
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }
    }
}
