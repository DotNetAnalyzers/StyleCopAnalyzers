// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.CSharp8.SpacingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp7.SpacingRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.SpacingRules.SA1012OpeningBracesMustBeSpacedCorrectly,
        StyleCop.Analyzers.SpacingRules.TokenSpacingCodeFixProvider>;

    public class SA1012CSharp8UnitTests : SA1012CSharp7UnitTests
    {
        [Fact]
        [WorkItem(3141, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3141")]
        public async Task TestInPropertyPatternsAsync()
        {
            var testCode = @"
class C
{
    void M((string A, string B) tuple)
    {
        _ = tuple is ( {|#0:{|}Length: 1 },{|#1:{|}Length: 2 });
    }
}
";
            var fixedCode = @"
class C
{
    void M((string A, string B) tuple)
    {
        _ = tuple is ({ Length: 1 }, { Length: 2 });
    }
}
";

            DiagnosticResult[] expectedResults =
            {
                // /0/Test0.cs(6,24): warning SA1012: Opening brace should be followed by a space
                Diagnostic().WithLocation(0).WithArguments(string.Empty, "followed"),

                // /0/Test0.cs(6,24): warning SA1012: Opening brace should not be preceded by a space
                Diagnostic().WithLocation(0).WithArguments(" not", "preceded"),

                // /0/Test0.cs(6,37): warning SA1012: Opening brace should be followed by a space
                Diagnostic().WithLocation(1).WithArguments(string.Empty, "followed"),

                // /0/Test0.cs(6,37): warning SA1012: Opening brace should be preceded by a space
                Diagnostic().WithLocation(1).WithArguments(string.Empty, "preceded"),
            };

            await VerifyCSharpFixAsync(
                testCode,
                expectedResults,
                fixedCode,
                CancellationToken.None).ConfigureAwait(false);
        }
    }
}
