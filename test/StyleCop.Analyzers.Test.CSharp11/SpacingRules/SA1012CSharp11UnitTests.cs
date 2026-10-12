// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp11.SpacingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp10.SpacingRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.SpacingRules.SA1012OpeningBracesMustBeSpacedCorrectly,
        StyleCop.Analyzers.SpacingRules.TokenSpacingCodeFixProvider>;

    public partial class SA1012CSharp11UnitTests : SA1012CSharp10UnitTests
    {
        [Fact]
        [WorkItem(3509, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3509")]
        public async Task TestPropertyPatternInsideListPatternAsync()
        {
            var testCode = @"
class C
{
    void M(string[] a)
    {
        _ = a is [ {|#0:{|} Length: 1 }];
        _ = a is [{ Length: 0 },{|#1:{|} Length: 1 }];
    }
}
";

            var fixedCode = @"
class C
{
    void M(string[] a)
    {
        _ = a is [{ Length: 1 }];
        _ = a is [{ Length: 0 }, { Length: 1 }];
    }
}
";

            DiagnosticResult[] expected =
            {
                // Opening brace should not be preceded by a space
                Diagnostic().WithLocation(0).WithArguments(" not", "preceded"),

                // Opening brace should be preceded by a space
                Diagnostic().WithLocation(1).WithArguments(string.Empty, "preceded"),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the spacing of interpolation braces in raw interpolated strings is checked.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(3993, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3993")]
        public async Task TestRawInterpolatedStringAsync()
        {
            var testCode = @"
class C
{
    string M(int x) => $""""""a {|#0:{|} x} b"""""";
}
";

            var fixedCode = @"
class C
{
    string M(int x) => $""""""a {x} b"""""";
}
";

            var expected = Diagnostic().WithLocation(0).WithArguments(" not", "followed");

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the spacing of a multi-character opening interpolation brace in a raw interpolated string
        /// literal with multiple <c>$</c> characters is checked.
        /// Markup is disabled because <c>$$</c> is the position marker of the test markup syntax.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(3993, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3993")]
        public async Task TestRawInterpolatedStringWithMultipleDollarSignsAsync()
        {
            var testCode = @"
class C
{
    string M(int x) => $$""""""a {{ x}} b"""""";
}
";

            var fixedCode = @"
class C
{
    string M(int x) => $$""""""a {{x}} b"""""";
}
";

            await new CSharpTest
            {
                TestCode = testCode,
                FixedCode = fixedCode,
                TestState = { MarkupHandling = MarkupMode.None },
                FixedState = { MarkupHandling = MarkupMode.None },
                ExpectedDiagnostics = { Diagnostic().WithLocation(4, 31).WithArguments(" not", "followed") },
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }
    }
}
