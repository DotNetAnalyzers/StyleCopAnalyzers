// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.SpacingRules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.SpacingRules;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// This class contains unit tests for the <see cref="SA1019MemberAccessSymbolsMustBeSpacedCorrectly"/> class.
    /// </summary>
    public class SA1019UnitTests : CodeFixVerifier
    {
        /// <summary>
        /// The members access operators to test.
        /// </summary>
        /// <value>The members access operators to test.</value>
        public static IEnumerable<object[]> Operators
        {
            get
            {
                yield return new object[] { "." };
                yield return new object[] { "?." };
            }
        }

        /// <summary>
        /// Asserts that a space before a member access operator reports correctly.
        /// </summary>
        /// <param name="op">The member access operator.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [MemberData(nameof(Operators))]
        public async Task TestSpaceBeforeOperatorAsync(string op)
        {
            string template = this.GetTemplate($" {op}");
            var fixedCode = this.GetTemplate($"{op}");

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(16, 27).WithArguments(op[0], "preceded");

            await this.VerifyCSharpDiagnosticAsync(template, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(template, fixedCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Asserts that a space after a member access operator reports correctly.
        /// </summary>
        /// <param name="op">The member access operator.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [MemberData(nameof(Operators))]
        public async Task TestSpaceAfterOperatorAsync(string op)
        {
            string template = this.GetTemplate($"{op} ");
            string fixedCode = this.GetTemplate($"{op}");

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(16, 25 + op.Length).WithArguments(op.Last(), "followed");
            await this.VerifyCSharpDiagnosticAsync(template, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(template, fixedCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Asserts that a member access operator at the end of a line does not report.
        /// </summary>
        /// <param name="op">The member access operator.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [MemberData(nameof(Operators))]
        public async Task TestOperatorAtEndOfLineDoesNotReportAsync(string op)
        {
            string template = this.GetTemplate($"{op}{Environment.NewLine}");
            await this.VerifyCSharpDiagnosticAsync(template, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Asserts that a member access operator at the start of a line does not report.
        /// </summary>
        /// <param name="op">The member access operator.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [MemberData(nameof(Operators))]
        public async Task TestOperatorAtStartOfLineDoesNotReportAsync(string op)
        {
            string template = this.GetTemplate($"{Environment.NewLine}{op}");
            await this.VerifyCSharpDiagnosticAsync(template, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Asserts that block comments either side of the operator does not report a diagnostic.
        /// </summary>
        /// <param name="op">The member access operator.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [MemberData(nameof(Operators))]
        public async Task TestBlockCommentsEitherSideOfOperatorDoesNotReportAsync(string op)
        {
            string template = this.GetTemplate($"/**/{op}/**/");
            await this.VerifyCSharpDiagnosticAsync(template, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Asserts that a comment on the line preceding the operator does not report.
        /// </summary>
        /// <param name="op">The member access operator.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [MemberData(nameof(Operators))]
        public async Task TestCommentOnLinePrecedingOperatorDoesNotReportAsync(string op)
        {
            string template = this.GetTemplate($"// This is a comment{Environment.NewLine}{op}");
            await this.VerifyCSharpDiagnosticAsync(template, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Asserts that a comment on the line following the operator does not report.
        /// </summary>
        /// <param name="op">The member access operator.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [MemberData(nameof(Operators))]
        public async Task TestCommentOnLineFollowingOperatorDoesNotReportAsync(string op)
        {
            string template = this.GetTemplate($"{op}{Environment.NewLine}// This is a comment{Environment.NewLine}");
            await this.VerifyCSharpDiagnosticAsync(template, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Asserts that a comment on the same line as the operator separated by whitespace does report a diagnostic.
        /// </summary>
        /// <param name="op">The member access operator.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [MemberData(nameof(Operators))]
        public async Task TestCommentOnSameLineSeparatedByWhitespaceReportsAsync(string op)
        {
            string template = this.GetTemplate($"{op} // This is a comment{Environment.NewLine}");
            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(16, 25 + op.Length).WithArguments(".", "followed");
            await this.VerifyCSharpDiagnosticAsync(template, expected, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Asserts that a comment on the same line as and immediately after the operator does report a diagnostic.
        /// </summary>
        /// <param name="op">The member access operator.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [MemberData(nameof(Operators))]
        public async Task TestCommentOnSameLineImmediatelyAfterOperatorDoesNotReportsAsync(string op)
        {
            string template = this.GetTemplate($"{op}// This is a comment{Environment.NewLine}");
            await this.VerifyCSharpDiagnosticAsync(template, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Asserts that a ternary operator does not report.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestTernaryDoesNotReportAsync()
        {
            string template =
@"namespace SA1019
{
    class Test
    {
        public void TestMethod()
        {
            var number = true ? 1 : 2;
        }
    }
}";

            await this.VerifyCSharpDiagnosticAsync(template, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the C# analyzers being tested
        /// </summary>
        /// <returns>
        /// New instances of all the C# analyzers being tested.
        /// </returns>
        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1019MemberAccessSymbolsMustBeSpacedCorrectly();
        }

        /// <inheritdoc/>
        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new TokenSpacingCodeFixProvider();
        }

        /// <summary>
        /// Retrieves the template used for testing.
        /// </summary>
        /// <param name="accessSymbol">The access symbol to use, including any spacing or newline characters.</param>
        /// <returns>The template to be tested.</returns>
        private string GetTemplate(string accessSymbol)
        {
            string template =
@"namespace SA1019 
{{     
    class Foo
    {{
        public bool Bar() 
        {{
            return true;
        }}
    }}

    class Test
    {{
        public void TestMethod()
        {{
            var foo = new Foo();
            var bar = foo{0}Bar();
        }}
    }}
}}";

            return string.Format(template, accessSymbol);
        }
    }
}
