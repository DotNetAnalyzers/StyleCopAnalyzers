// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.SpacingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.SpacingRules;
    using TestHelper;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        Analyzers.SpacingRules.SA1020IncrementDecrementSymbolsMustBeSpacedCorrectly,
        Analyzers.SpacingRules.TokenSpacingCodeFixProvider>;

    /// <summary>
    /// Unit tests for <see cref="SA1020IncrementDecrementSymbolsMustBeSpacedCorrectly"/>.
    /// </summary>
    public class SA1020UnitTests
    {
        /// <summary>
        /// Verifies that the analyzer will properly valid symbol spacing.
        /// </summary>
        /// <param name="symbol">The operator to test.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData("++")]
        [InlineData("--")]
        public async Task TestValidSymbolSpacingAsync(string symbol)
        {
            var testCode = $@"
class ClassName
{{
    void MethodName()
    {{
        int x = 0;
        x{symbol};
        {symbol}x;
        for (int y = 0; y < 30; {symbol}x, y{symbol})
        {{
        }}
    }}
}}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will properly invalid symbol spacing.
        /// </summary>
        /// <param name="symbol">The operator to test.</param>
        /// <param name="symbolName">The name of the symbol, as it appears in diagnostics.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData("++", "Increment")]
        [InlineData("--", "Decrement")]
        public async Task TestInvalidSymbolSpacingAsync(string symbol, string symbolName)
        {
            var testCode = $@"
class ClassName
{{
    void MethodName()
    {{
        int x = 0;
        x {symbol};
        {symbol} x;
        for (int y = 0; y < 30; {symbol} x, y {symbol})
        {{
        }}

        x
        {symbol};
        {symbol}
        x;
        for (int y = 0; y < 30; {symbol}
            x,
            y
            {symbol})
        {{
        }}
    }}
}}
";

            var fixedCode = $@"
class ClassName
{{
    void MethodName()
    {{
        int x = 0;
        x{symbol};
        {symbol}x;
        for (int y = 0; y < 30; {symbol}x, y{symbol})
        {{
        }}

        x{symbol};
        {symbol}x;
        for (int y = 0; y < 30; {symbol}x,
            y{symbol})
        {{
        }}
    }}
}}
";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(7, 11).WithArguments(symbolName, symbol, "preceded"),
                Diagnostic().WithLocation(8, 9).WithArguments(symbolName, symbol, "followed"),
                Diagnostic().WithLocation(9, 33).WithArguments(symbolName, symbol, "followed"),
                Diagnostic().WithLocation(9, 41).WithArguments(symbolName, symbol, "preceded"),
                Diagnostic().WithLocation(14, 9).WithArguments(symbolName, symbol, "preceded"),
                Diagnostic().WithLocation(15, 9).WithArguments(symbolName, symbol, "followed"),
                Diagnostic().WithLocation(17, 33).WithArguments(symbolName, symbol, "followed"),
                Diagnostic().WithLocation(20, 13).WithArguments(symbolName, symbol, "preceded"),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
