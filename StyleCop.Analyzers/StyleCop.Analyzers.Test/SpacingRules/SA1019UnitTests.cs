// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.SpacingRules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using StyleCop.Analyzers.SpacingRules;
    using TestHelper;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.SpacingRules.SA1019MemberAccessSymbolsMustBeSpacedCorrectly,
        StyleCop.Analyzers.SpacingRules.TokenSpacingCodeFixProvider>;

    /// <summary>
    /// This class contains unit tests for the <see cref="SA1019MemberAccessSymbolsMustBeSpacedCorrectly"/> class.
    /// </summary>
    public class SA1019UnitTests
    {
        /// <summary>
        /// Gets the members access operators to test.
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

            DiagnosticResult expected = Diagnostic().WithLocation(16, 27).WithArguments(op[0], "preceded");

            await VerifyCSharpFixAsync(template, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult expected = Diagnostic().WithLocation(16, 25 + op.Length).WithArguments(op.Last(), "followed");
            await VerifyCSharpFixAsync(template, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
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
            await VerifyCSharpDiagnosticAsync(template, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
            await VerifyCSharpDiagnosticAsync(template, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
            await VerifyCSharpDiagnosticAsync(template, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
            await VerifyCSharpDiagnosticAsync(template, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
            await VerifyCSharpDiagnosticAsync(template, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
            DiagnosticResult expected = Diagnostic().WithLocation(16, 25 + op.Length).WithArguments(".", "followed");
            await VerifyCSharpDiagnosticAsync(template, expected, CancellationToken.None).ConfigureAwait(false);
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
            await VerifyCSharpDiagnosticAsync(template, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            await VerifyCSharpDiagnosticAsync(template, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(2471, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2471")]
        public async Task TestUnaryMemberAccessAsync()
        {
            var testCode = @"public class ClassName
{
    public unsafe void MethodName()
    {
        int? x = 0;
        int* y = null;

        x-- . ToString();
        x-- .ToString();
        x--. ToString();

        x++ . ToString();
        x++ .ToString();
        x++. ToString();

        x-- ?. ToString();
        x-- ?.ToString();
        x--?. ToString();

        x++ ?. ToString();
        x++ ?.ToString();
        x++?. ToString();

        y-- -> ToString();
        y-- ->ToString();
        y---> ToString();

        y++ -> ToString();
        y++ ->ToString();
        y++-> ToString();
    }
}
";
            var fixedTestCode = @"public class ClassName
{
    public unsafe void MethodName()
    {
        int? x = 0;
        int* y = null;

        x--.ToString();
        x--.ToString();
        x--.ToString();

        x++.ToString();
        x++.ToString();
        x++.ToString();

        x--?.ToString();
        x--?.ToString();
        x--?.ToString();

        x++?.ToString();
        x++?.ToString();
        x++?.ToString();

        y--->ToString();
        y--->ToString();
        y--->ToString();

        y++->ToString();
        y++->ToString();
        y++->ToString();
    }
}
";
            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(8, 13).WithArguments(".", "preceded"),
                Diagnostic().WithLocation(8, 13).WithArguments(".", "followed"),
                Diagnostic().WithLocation(9, 13).WithArguments(".", "preceded"),
                Diagnostic().WithLocation(10, 12).WithArguments(".", "followed"),

                Diagnostic().WithLocation(12, 13).WithArguments(".", "preceded"),
                Diagnostic().WithLocation(12, 13).WithArguments(".", "followed"),
                Diagnostic().WithLocation(13, 13).WithArguments(".", "preceded"),
                Diagnostic().WithLocation(14, 12).WithArguments(".", "followed"),

                Diagnostic().WithLocation(16, 13).WithArguments("?", "preceded"),
                Diagnostic().WithLocation(16, 14).WithArguments(".", "followed"),
                Diagnostic().WithLocation(17, 13).WithArguments("?", "preceded"),
                Diagnostic().WithLocation(18, 13).WithArguments(".", "followed"),

                Diagnostic().WithLocation(20, 13).WithArguments("?", "preceded"),
                Diagnostic().WithLocation(20, 14).WithArguments(".", "followed"),
                Diagnostic().WithLocation(21, 13).WithArguments("?", "preceded"),
                Diagnostic().WithLocation(22, 13).WithArguments(".", "followed"),

                Diagnostic().WithLocation(24, 13).WithArguments("->", "preceded"),
                Diagnostic().WithLocation(24, 13).WithArguments("->", "followed"),
                Diagnostic().WithLocation(25, 13).WithArguments("->", "preceded"),
                Diagnostic().WithLocation(26, 12).WithArguments("->", "followed"),

                Diagnostic().WithLocation(28, 13).WithArguments("->", "preceded"),
                Diagnostic().WithLocation(28, 13).WithArguments("->", "followed"),
                Diagnostic().WithLocation(29, 13).WithArguments("->", "preceded"),
                Diagnostic().WithLocation(30, 12).WithArguments("->", "followed"),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
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
