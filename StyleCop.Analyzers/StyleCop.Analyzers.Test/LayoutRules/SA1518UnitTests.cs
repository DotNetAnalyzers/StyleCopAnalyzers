using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StyleCop.Analyzers.LayoutRules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TestHelper;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.CodeFixes;

namespace StyleCop.Analyzers.Test.LayoutRules
{
    /// <summary>
    /// Unit tests for the SA1518: CodeMustNotContainBlankLinesAtEndOfFile
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
        /// <returns></returns>
        [TestMethod, TestCategory("MaintainabilityRules/SA1518")]
        public async Task TestEmptySource()
        {
            var testCode = string.Empty;
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        /// <summary>
        /// Verifies that blank lines at the end of the file will produce a warning.
        /// </summary>
        /// <returns></returns>
        [TestMethod, TestCategory("MaintainabilityRules/SA1518")]
        public async Task TestWithBlankLinesAtEndOfFile()
        {
            var testCode = BaseCode + "\r\n\r\n";

            var expected = new[]
            {
                new DiagnosticResult
                {
                    Id = DiagnosticId,
                    Message = "Code must not contain blank lines at end of file",
                    Severity = DiagnosticSeverity.Warning,
                    Locations = new[] { new DiagnosticResultLocation("Test0.cs", 9, 1) }
                }
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        /// <summary>
        /// Verifies that a single carriage return / linefeed at the end of the file will not produce a warning.
        /// </summary>
        /// <returns></returns>
        [TestMethod, TestCategory("MaintainabilityRules/SA1518")]
        public async Task TestWithSingleCarriageReturnLineFeedAtEndOfFile()
        {
            var testCode = BaseCode + "\r\n";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        /// <summary>
        /// Verifies that a no carriage return / linefeed at the end of the file will not produce a warning.
        /// </summary>
        /// <returns></returns>
        [TestMethod, TestCategory("MaintainabilityRules/SA1518")]
        public async Task TestWithoutCarriageReturnLineFeedAtEndOfFile()
        {
            await this.VerifyCSharpDiagnosticAsync(BaseCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        /// <summary>
        /// Verifies that a single carriage return / linefeed at the end of the file will not produce a warning.
        /// </summary>
        /// <returns></returns>
        [TestMethod, TestCategory("MaintainabilityRules/SA1518")]
        public async Task TestFileEndsWithSpaces()
        {
            var testCode = BaseCode + "\r\n          ";

            var expected = new[]
            {
                new DiagnosticResult
                {
                    Id = DiagnosticId,
                    Message = "Code must not contain blank lines at end of file",
                    Severity = DiagnosticSeverity.Warning,
                    Locations = new[] { new DiagnosticResultLocation("Test0.cs", 9, 1) }
                }
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        /// <summary>
        /// Verifies that the code fix provider will strip trailing blank lines.
        /// </summary>
        /// <remarks>The CRLF after the last curly bracket will not be stripped!</remarks>
        /// <returns></returns>
        [TestMethod, TestCategory("MaintainabilityRules/SA1518")]
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
        /// <returns></returns>
        [TestMethod, TestCategory("MaintainabilityRules/SA1518")]
        public async Task TestCodeFixProviderStripsTrailingBlankLinesIncludingWhitespace()
        {
            var testCode = BaseCode + "\r\n   \r\n   \r\n";
            var fixedTestCode = BaseCode + "\r\n";

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
    }
}
