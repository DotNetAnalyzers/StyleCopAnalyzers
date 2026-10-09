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
        StyleCop.Analyzers.SpacingRules.SA1013ClosingBracesMustBeSpacedCorrectly,
        StyleCop.Analyzers.SpacingRules.TokenSpacingCodeFixProvider>;

    public partial class SA1013CSharp11UnitTests : SA1013CSharp10UnitTests
    {
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
    string M(int x) => $""""""a {x {|#0:}|} b"""""";
}
";

            var fixedCode = @"
class C
{
    string M(int x) => $""""""a {x} b"""""";
}
";

            var expected = Diagnostic().WithLocation(0).WithArguments(" not", "preceded");

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the spacing of a multi-character closing interpolation brace in a raw interpolated string
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
    string M(int x) => $$""""""a {{x }} b"""""";
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
                ExpectedDiagnostics = { Diagnostic().WithLocation(4, 35).WithArguments(" not", "preceded") },
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a closing interpolation brace which is the first token on its line is not reported, using the
        /// newlines in interpolations allowed by C# 11.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(3898, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3898")]
        public async Task TestInterpolationClosingBraceFirstOnLineAsync()
        {
            var testCode = @"
class C
{
    string M(int x) => $""abc {x
        } def"";
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
