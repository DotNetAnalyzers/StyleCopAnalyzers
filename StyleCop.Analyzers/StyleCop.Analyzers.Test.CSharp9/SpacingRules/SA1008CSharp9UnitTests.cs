// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp9.SpacingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp8.SpacingRules;
    using Xunit;
    using static StyleCop.Analyzers.SpacingRules.SA1008OpeningParenthesisMustBeSpacedCorrectly;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.SpacingRules.SA1008OpeningParenthesisMustBeSpacedCorrectly,
        StyleCop.Analyzers.SpacingRules.TokenSpacingCodeFixProvider>;

    public class SA1008CSharp9UnitTests : SA1008CSharp8UnitTests
    {
        [Fact]
        [WorkItem(3230, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3230")]
        public async Task TestParenthesizedPatternAsync()
        {
            const string testCode = @"
class C
{
    void Method(int b)
    {
        _ = b is{|#0:(|} >= 0 and <= 31) or 127;
        _ = b is{|#1:(|}>= 0 and <= 31) or 127;
        _ = b is {|#2:(|} >= 0 and <= 31) or 127;
    }
}";
            const string fixedCode = @"
class C
{
    void Method(int b)
    {
        _ = b is (>= 0 and <= 31) or 127;
        _ = b is (>= 0 and <= 31) or 127;
        _ = b is (>= 0 and <= 31) or 127;
    }
}";

            await new CSharpTest(LanguageVersion.CSharp9)
            {
                ReferenceAssemblies = ReferenceAssemblies.Net.Net50,
                ExpectedDiagnostics =
                {
                    // /0/Test0.cs(6,17): warning SA1008: Opening parenthesis should be preceded by a space.
                    Diagnostic(DescriptorPreceded).WithLocation(0),

                    // /0/Test0.cs(6,17): warning SA1008: Opening parenthesis should not be followed by a space.
                    Diagnostic(DescriptorNotFollowed).WithLocation(0),

                    // /0/Test0.cs(7,17): warning SA1008: Opening parenthesis should be preceded by a space.
                    Diagnostic(DescriptorPreceded).WithLocation(1),

                    // /0/Test0.cs(8,18): warning SA1008: Opening parenthesis should not be followed by a space.
                    Diagnostic(DescriptorNotFollowed).WithLocation(2),
                },
                TestCode = testCode,
                FixedCode = fixedCode,
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }
    }
}
