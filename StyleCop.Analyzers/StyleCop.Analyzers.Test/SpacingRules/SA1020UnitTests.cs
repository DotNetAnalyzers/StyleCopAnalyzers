// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.SpacingRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.SpacingRules;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// Unit tests for <see cref="SA1020IncrementDecrementSymbolsMustBeSpacedCorrectly"/>.
    /// </summary>
    public class SA1020UnitTests : CodeFixVerifier
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
                this.CSharpDiagnostic().WithLocation(7, 11).WithArguments(symbolName, symbol, "preceded"),
                this.CSharpDiagnostic().WithLocation(8, 9).WithArguments(symbolName, symbol, "followed"),
                this.CSharpDiagnostic().WithLocation(9, 33).WithArguments(symbolName, symbol, "followed"),
                this.CSharpDiagnostic().WithLocation(9, 41).WithArguments(symbolName, symbol, "preceded"),
                this.CSharpDiagnostic().WithLocation(14, 9).WithArguments(symbolName, symbol, "preceded"),
                this.CSharpDiagnostic().WithLocation(15, 9).WithArguments(symbolName, symbol, "followed"),
                this.CSharpDiagnostic().WithLocation(17, 33).WithArguments(symbolName, symbol, "followed"),
                this.CSharpDiagnostic().WithLocation(20, 13).WithArguments(symbolName, symbol, "preceded"),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1020IncrementDecrementSymbolsMustBeSpacedCorrectly();
        }

        /// <inheritdoc/>
        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new TokenSpacingCodeFixProvider();
        }
    }
}
