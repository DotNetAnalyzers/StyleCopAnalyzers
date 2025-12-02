// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp9.SpacingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp8.SpacingRules;
    using Xunit;
    using static StyleCop.Analyzers.SpacingRules.SA1003SymbolsMustBeSpacedCorrectly;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.SpacingRules.SA1003SymbolsMustBeSpacedCorrectly,
        StyleCop.Analyzers.SpacingRules.SA1003CodeFixProvider>;

    public partial class SA1003CSharp9UnitTests : SA1003CSharp8UnitTests
    {
        [Fact]
        [WorkItem(3968, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3968")]
        public async Task TestRelationalPatternsAreValidatedAsync()
        {
            const string testCode = @"
class C
{
    void M(int value)
    {
        _ = value is {|#0:>|}5;
        _ = value is{|#1:<|} 5;
        _ = value is ( < 5); // Validated by SA1008
        _ = value is (< 5);
        _ = value is {|#2:<=|}5;
        _ = value is {|#3:>=|}5;
        _ = value is {|#4:>|}
            5;
    }
}";

            const string fixedCode = @"
class C
{
    void M(int value)
    {
        _ = value is > 5;
        _ = value is < 5;
        _ = value is ( < 5); // Validated by SA1008
        _ = value is (< 5);
        _ = value is <= 5;
        _ = value is >= 5;
        _ = value is > 5;
    }
}";

            DiagnosticResult[] expected =
            {
                Diagnostic(DescriptorFollowedByWhitespace).WithLocation(0).WithArguments(">"),
                Diagnostic(DescriptorPrecededByWhitespace).WithLocation(1).WithArguments("<"),
                Diagnostic(DescriptorFollowedByWhitespace).WithLocation(2).WithArguments("<="),
                Diagnostic(DescriptorFollowedByWhitespace).WithLocation(3).WithArguments(">="),
                Diagnostic(DescriptorNotAtEndOfLine).WithLocation(4).WithArguments(">"),
            };
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
