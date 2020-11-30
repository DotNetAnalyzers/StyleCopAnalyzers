// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.ReadabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.ReadabilityRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.ReadabilityRules.SA1136EnumValuesShouldBeOnSeparateLines,
        StyleCop.Analyzers.ReadabilityRules.SA1136CodeFixProvider>;

    /// <summary>
    /// Unit tests for the <see cref="SA1136EnumValuesShouldBeOnSeparateLines"/> analyzer.
    /// </summary>
    public class SA1136UnitTests
    {
        /// <summary>
        /// Verifies that an enum declaration with all values on the same line will return the expected diagnostics.
        /// This also verifies that an enum declaration with all values on separate lines will not produce diagnostics.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestSingleLineEnumDeclarationAsync()
        {
            var testCode = @"
public enum TestEnum
{
    FirstValue, SecondValue, ThirdValue
}
";

            var fixedTestCode = @"
public enum TestEnum
{
    FirstValue,
    SecondValue,
    ThirdValue
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(4, 17),
                Diagnostic().WithLocation(4, 30),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that comments after a enum value declaration will not produce diagnostics.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestEndOfLineCommentsAsync()
        {
            var testCode = @"
public enum TestEnum
{
    FirstValue, /* comment 1 */
    SecondValue, // comment 2
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that inline comments between enum value declarations are handled properly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestInlineCommentsAsync()
        {
            var testCode = @"
public enum TestEnum
{
    FirstValue, /* comment */ SecondValue,
}
";

            var fixedTestCode = @"
public enum TestEnum
{
    FirstValue, /* comment */
    SecondValue,
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(4, 31),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that values on the same line within a directive trivia are handled correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestDirectiveTriviaAreHandledProperlyAsync()
        {
            var testCode = @"
public enum TestEnum
{
    FirstValue,
#if !TEST
    SecondValue, ThirdValue,
#endif
    FourthValue
}
";

            var fixedTestCode = @"
public enum TestEnum
{
    FirstValue,
#if !TEST
    SecondValue,
    ThirdValue,
#endif
    FourthValue
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(6, 18),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that an enum declaration without a block is handled correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestEnumWithoutBlockAsync()
        {
            var testCode = @"
public enum TestEnum
";

            DiagnosticResult[] expected =
            {
                DiagnosticResult.CompilerError("CS1513").WithMessage("} expected").WithLocation(2, 21),
                DiagnosticResult.CompilerError("CS1514").WithMessage("{ expected").WithLocation(2, 21),
            };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
