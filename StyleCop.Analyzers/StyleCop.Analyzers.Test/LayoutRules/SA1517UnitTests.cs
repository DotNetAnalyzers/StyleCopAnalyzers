namespace StyleCop.Analyzers.Test.LayoutRules
{
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using StyleCop.Analyzers.LayoutRules;

    using TestHelper;

    /// <summary>
    /// Unit tests for <see cref="SA1517CodeMustNotContainBlankLinesAtStartOfFile"/>.
    /// </summary>
    [TestClass]
    public class SA1517UnitTests : CodeFixVerifier
    {
        private const string DiagnosticId = SA1517CodeMustNotContainBlankLinesAtStartOfFile.DiagnosticId;

        private const string BaseCode = @"using System.Diagnostics;
public class Foo
{
    public void Bar(int i)
    {
        Debug.Assert(true);
    }
}";

        /// <summary>
        /// Verifies that the analyzer will properly handle an empty source.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestEmptySource()
        {
            var testCode = string.Empty;
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        /// <summary>
        /// Verifies that blank lines at the start of the file will produce a warning.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestWithBlankLinesAtStartOfFile()
        {
            var testCode = "\r\n\r\n" + BaseCode;
            await this.VerifyCSharpDiagnosticAsync(testCode, this.GenerateExpectedWarning(1, 1), CancellationToken.None);
        }

        /// <summary>
        /// Verifies that blank linefeed only lines at the start of the file will produce a warning.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestWithBlankLinefeedOnlyLinesAtStartOfFile()
        {
            var testCode = "\n\n" + BaseCode;
            await this.VerifyCSharpDiagnosticAsync(testCode, this.GenerateExpectedWarning(1, 1), CancellationToken.None);
        }

        /// <summary>
        /// Verifies that non-whitespace trivia will not produce a warning.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestWithNonWhitespaceTrivia()
        {
            var testCode = "#if true\r\n" + BaseCode + "\r\n#endif\r\n";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        /// <summary>
        /// Verifies that blank lines followed by non-whitespace trivia will produce a warning.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestWithNonWhitespaceTriviaAndLeadingBlankLines()
        {
            var testCode = "\r\n\r\n#if true\r\n" + BaseCode + "\r\n#endif\r\n";
            await this.VerifyCSharpDiagnosticAsync(testCode, this.GenerateExpectedWarning(1, 1), CancellationToken.None);
        }

        /// <summary>
        /// Verifies that no blank lines at the start of the file will not produce a warning.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestWithoutCarriageReturnLineFeedAtStartOfFile()
        {
            await this.VerifyCSharpDiagnosticAsync(BaseCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        /// <summary>
        /// Verifies that invalid spacing will not trigger SA1517.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestWithInvalidSpacing()
        {
            var testCode = "    " + BaseCode;
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        /// <summary>
        /// Verifies that the code fix provider will strip leading blank lines.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestCodeFixProviderStripsLeadingBlankLines()
        {
            var testCode = "\r\n\r\n" + BaseCode;
            var fixedTestCode = BaseCode;

            await this.VerifyCSharpFixAsync(testCode, fixedTestCode);
        }

        /// <summary>
        /// Verifies that the code fix provider will not strip leading whitespace other than blank lines.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestCodeFixProviderHandlesWhitespaceProperly()
        {
            var testCode = "\r\n   " + BaseCode;
            var fixedTestCode = "   " + BaseCode;

            await this.VerifyCSharpFixAsync(testCode,  fixedTestCode);
        }

        /// <summary>
        /// Verifies that the code fix provider will strip whitespace on blank lines.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestCodeFixProviderHandlesBlankLinesWithWhitespaceProperly()
        {
            var testCode = "   \r\n   \r\n" + BaseCode;
            var fixedTestCode = BaseCode;

            await this.VerifyCSharpFixAsync(testCode, fixedTestCode);
        }

        /// <summary>
        /// Verifies that the code fix provider will not strip non-whitespace trivia.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestCodeFixProviderHandlesNonWhitespaceTriviaProperly()
        {
            var testCode = "\r\n\r\n#if true\r\n" + BaseCode + "\r\n#endif\r\n";
            var fixedTestCode = "#if true\r\n" + BaseCode + "\r\n#endif\r\n";

            await this.VerifyCSharpFixAsync(testCode, fixedTestCode);
        }

        /// <inheritdoc/>
        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new SA1517CodeMustNotContainBlankLinesAtStartOfFile();
        }

        /// <inheritdoc/>
        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new SA1517CodeFixProvider();
        }

        private DiagnosticResult[] GenerateExpectedWarning(int line, int column)
        {
            return new[]
            {
                new DiagnosticResult
                {
                    Id = DiagnosticId,
                    Message = "Code must not contain blank lines at start of file",
                    Severity = DiagnosticSeverity.Warning,
                    Locations = new[] { new DiagnosticResultLocation("Test0.cs", line, column) }
                }
            };
        }
    }
}
