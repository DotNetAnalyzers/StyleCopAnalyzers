namespace StyleCop.Analyzers.Test.SpacingRules
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

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
        /// Verifies that the analyzer will properly handle an empty source.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestEmptySourceAsync()
        {
            var testCode = string.Empty;
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Asserts that a space before a period member access symbol reports correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestSpaceBeforeDotAsync()
        {
            string template = this.GetTemplate(" .");
            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(16, 27).WithArguments(".", "preceded");
            await this.VerifyCSharpDiagnosticAsync(template, expected, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Asserts that a space after a period member access symbol reports correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestSpaceAfterDotAsync()
        {
            string template = this.GetTemplate(". ");
            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(16, 26).WithArguments(".", "followed");
            await this.VerifyCSharpDiagnosticAsync(template, expected, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Asserts that a space before a null conditional access symbol reports correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestSpaceBeforeNullConditionalAsync()
        {
            string template = this.GetTemplate(" ?.");
            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(16, 27).WithArguments("?", "preceded");
            await this.VerifyCSharpDiagnosticAsync(template, expected, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Asserts that a space after a null conditional access symbol reports correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestSpaceAfterNullConditionalAsync()
        {
            string template = this.GetTemplate("?. ");
            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(16, 27).WithArguments(".", "followed");
            await this.VerifyCSharpDiagnosticAsync(template, expected, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Asserts that a dot at the end of a line does not report.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestDotAtEndOfLineDoesNotReportAsync()
        {
            string template = this.GetTemplate($".{Environment.NewLine}");
            await this.VerifyCSharpDiagnosticAsync(template, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Asserts that a dot at the start of a line does not report.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestDotAtStartOfLineDoesNotReportAsync()
        {
            string template = this.GetTemplate($"{Environment.NewLine}.");
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
