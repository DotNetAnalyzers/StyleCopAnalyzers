// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.ReadabilityRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.ReadabilityRules;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// Unit tests for the <see cref="SA1136EnumValuesShouldBeOnSeparateLines"/> analyzer.
    /// </summary>
    public class SA1136UnitTests : CodeFixVerifier
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
                this.CSharpDiagnostic().WithLocation(4, 17),
                this.CSharpDiagnostic().WithLocation(4, 30),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
                this.CSharpDiagnostic().WithLocation(4, 31),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
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
                this.CSharpDiagnostic().WithLocation(6, 18),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
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
                this.CSharpCompilerError("CS1513").WithMessage("} expected").WithLocation(2, 21),
                this.CSharpCompilerError("CS1514").WithMessage("{ expected").WithLocation(2, 21),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new SA1136CodeFixProvider();
        }

        /// <inheritdoc/>
        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1136EnumValuesShouldBeOnSeparateLines();
        }
    }
}
