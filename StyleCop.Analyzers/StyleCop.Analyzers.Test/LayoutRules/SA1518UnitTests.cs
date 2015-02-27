using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StyleCop.Analyzers.LayoutRules;
using System.Threading;
using System.Threading.Tasks;
using TestHelper;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.CodeFixes;

namespace StyleCop.Analyzers.Test.LayoutRules
{
    /// <summary>
    /// Unit tests for <see cref="SA1518CodeMustNotContainBlankLinesAtEndOfFile"/>.
    /// </summary>
    [TestClass]
    public class SA1518UnitTests : CodeFixVerifier
    {
        private const string DiagnosticId = SA1518CodeMustNotContainBlankLinesAtEndOfFile.DiagnosticId;

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
        [TestMethod]
        public async Task TestEmptySource()
        {
            var testCode = string.Empty;
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        /// <summary>
        /// Verifies that blank lines at the end of the file will produce a warning.
        /// </summary>
        [TestMethod]
        public async Task TestWithBlankLinesAtEndOfFile()
        {
            var testCode = BaseCode + "\r\n\r\n";
            await this.VerifyCSharpDiagnosticAsync(testCode, this.GenerateExpectedWarning(9, 1), CancellationToken.None);
        }

        /// <summary>
        /// Verifies that linefeed only blank lines at the end of the file will produce a warning.
        /// </summary>
        [TestMethod]
        public async Task TestWithLineFeedOnlyBlankLinesAtEndOfFile()
        {
            var testCode = BaseCode + "\n\n";
            await this.VerifyCSharpDiagnosticAsync(testCode, this.GenerateExpectedWarning(9, 1), CancellationToken.None);
        }

        /// <summary>
        /// Verifies that a single carriage return / linefeed at the end of the file will not produce a warning.
        /// </summary>
        [TestMethod]
        public async Task TestWithSingleCarriageReturnLineFeedAtEndOfFile()
        {
            var testCode = BaseCode + "\r\n";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        /// <summary>
        /// Verifies that a single linefeed at the end of the file will not produce a warning.
        /// </summary>
        [TestMethod]
        public async Task TestWithSingleLineFeedAtEndOfFile()
        {
            var testCode = BaseCode + "\n";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        /// <summary>
        /// Verifies that a source file that ends without a carriage return / linefeed at the end of the file will not produce a warning.
        /// </summary>
        [TestMethod]
        public async Task TestWithoutCarriageReturnLineFeedAtEndOfFile()
        {
            await this.VerifyCSharpDiagnosticAsync(BaseCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        /// <summary>
        /// Verifies that a source file that ends with spaces will produce a warning.
        /// </summary>
        [TestMethod]
        public async Task TestFileEndsWithSpaces()
        {
            var testCode = BaseCode + "\r\n          ";
            await this.VerifyCSharpDiagnosticAsync(testCode, this.GenerateExpectedWarning(9, 1), CancellationToken.None);
        }

        /// <summary>
        /// Verifies that a comment at the end of the file is not flagged.
        /// </summary>
        [TestMethod]
        public async Task TestFileEndingWithComment()
        {
            var testCode = BaseCode + "\r\n// Test comment";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        /// <summary>
        /// Verifies that spurious end of lines after a comment at the end of the file are flagged.
        /// </summary>
        [TestMethod]
        public async Task TestFileEndingWithCommentAndSpuriousWhitespace()
        {
            var testCode = BaseCode + "\r\n// Test comment\r\n   \r\n";
            await this.VerifyCSharpDiagnosticAsync(testCode, this.GenerateExpectedWarning(10, 1), CancellationToken.None);
        }

        /// <summary>
        /// Verifies that an endif at the end of the file is not flagged.
        /// </summary>
        [TestMethod]
        public async Task TestFileEndingWithEndIf()
        {
            var testCode = "#if true\r\n" + BaseCode + "\r\n#endif\r\n";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        /// <summary>
        /// Verifies that an endif at the end of the file is not flagged.
        /// </summary>
        [TestMethod]
        public async Task TestFileEndingWithEndIfWithSpuriousWhitespace()
        {
            var testCode = "#if true\r\n" + BaseCode + "\r\n#endif\r\n   \r\n";
            await this.VerifyCSharpDiagnosticAsync(testCode, this.GenerateExpectedWarning(11, 1), CancellationToken.None);
        }

        /// <summary>
        /// Verifies that the code fix provider will strip trailing blank lines.
        /// </summary>
        /// <remarks>The CRLF after the last curly bracket will not be stripped!</remarks>
        [TestMethod]
        public async Task TestCodeFixProviderStripsTrailingBlankLines()
        {
            var testCode = BaseCode + "\r\n\r\n";
            var fixedTestCode = BaseCode + "\r\n";

            await this.VerifyCSharpFixAsync(testCode, fixedTestCode);
        }

        /// <summary>
        /// Verifies that the code fix provider will strip trailing blank lines that include whitespace.
        /// </summary>
        /// <remarks>The CRLF after the last curly bracket will not be stripped!</remarks>
        [TestMethod]
        public async Task TestCodeFixProviderStripsTrailingBlankLinesIncludingWhitespace()
        {
            var testCode = BaseCode + "\r\n   \r\n   \r\n";
            var fixedTestCode = BaseCode + "\r\n";

            await this.VerifyCSharpFixAsync(testCode, fixedTestCode);
        }

        /// <summary>
        /// Verifies that the code fix provider will strip trailing linefeed only blank lines that include whitespace.
        /// </summary>
        /// <remarks>The LF after the last curly bracket will not be stripped!</remarks>
        [TestMethod]
        public async Task TestCodeFixProviderStripsTrailingLinefeedOnlyBlankLinesIncludingWhitespace()
        {
            var testCode = BaseCode + "\n   \n   \n";
            var fixedTestCode = BaseCode + "\n";

            await this.VerifyCSharpFixAsync(testCode, fixedTestCode);
        }

        /// <summary>
        /// Verifies that the code fix provider will strip only trailing blank lines.
        /// </summary>
        /// <remarks>The CRLF after the #endif will not be stripped!</remarks>
        [TestMethod]
        public async Task TestCodeFixProviderOnlyStripsTrailingBlankLines()
        {
            var testCode = "#if true\r\n" + BaseCode + "\r\n#endif\r\n   \r\n";
            var fixedTestCode = "#if true\r\n" + BaseCode + "\r\n#endif\r\n";

            await this.VerifyCSharpFixAsync(testCode, fixedTestCode);
        }

        /// <inheritdoc/>
        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new SA1518CodeMustNotContainBlankLinesAtEndOfFile();
        }

        /// <inheritdoc/>
        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new SA1518CodeFixProvider();
        }

        private DiagnosticResult[] GenerateExpectedWarning(int line, int column)
        {
            return new[]
            {
                new DiagnosticResult
                {
                    Id = DiagnosticId,
                    Message = "Code must not contain blank lines at end of file",
                    Severity = DiagnosticSeverity.Warning,
                    Locations = new[] { new DiagnosticResultLocation("Test0.cs", line, column) }
                }
            };
        }
    }
}
