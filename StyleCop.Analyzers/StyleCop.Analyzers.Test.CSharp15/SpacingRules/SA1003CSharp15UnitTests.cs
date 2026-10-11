// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp15.SpacingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp14.SpacingRules;
    using Xunit;
    using static StyleCop.Analyzers.SpacingRules.SA1003SymbolsMustBeSpacedCorrectly;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.SpacingRules.SA1003SymbolsMustBeSpacedCorrectly,
        StyleCop.Analyzers.SpacingRules.SA1003CodeFixProvider>;

    public partial class SA1003CSharp15UnitTests : SA1003CSharp14UnitTests
    {
        /// <summary>
        /// Verifies that an operator in a union member is reported exactly once. The analyzer driver in Roslyn 5.9 ran
        /// syntax node actions on union members three times (dotnet/roslyn#84570), which reported each of these
        /// diagnostics three times.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(4182, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4182")]
        public async Task TestOperatorInUnionMemberReportedOnceAsync()
        {
            var testCode = @"
public union Pet(int, string)
{
    public int Add(int x)
    {
        return x{|#0:+|}3;
    }
}
";

            var fixedCode = @"
public union Pet(int, string)
{
    public int Add(int x)
    {
        return x + 3;
    }
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic(DescriptorPrecededByWhitespace).WithLocation(0).WithArguments("+"),
                Diagnostic(DescriptorFollowedByWhitespace).WithLocation(0).WithArguments("+"),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
