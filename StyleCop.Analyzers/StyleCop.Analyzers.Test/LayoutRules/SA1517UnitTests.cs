using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Diagnostics;
using TestHelper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StyleCop.Analyzers.LayoutRules;
using Microsoft.CodeAnalysis;
using System.Threading;
using Microsoft.CodeAnalysis.CodeFixes;

namespace StyleCop.Analyzers.Test.LayoutRules
{
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
        [TestMethod, TestCategory("MaintainabilityRules/SA1517")]
        public async Task TestEmptySource()
        {
            var testCode = string.Empty;
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        /// <summary>
        /// Verifies that blank lines at the start of the file will produce a warning.
        /// </summary>
        /// <returns></returns>
        [TestMethod, TestCategory("MaintainabilityRules/SA1517")]
        public async Task TestWithBlankLinesAtStartOfFile()
        {
            var testCode = "\r\n\r\n" + BaseCode;

            var expected = new[]
            {
                new DiagnosticResult
                {
                    Id = DiagnosticId,
                    Message = "Code must not contain blank lines at start of file",
                    Severity = DiagnosticSeverity.Warning,
                    Locations = new[] { new DiagnosticResultLocation("Test0.cs", 1, 1) }
                }
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        /// <summary>
        /// Verifies that no blank lines at the start of the file will not produce a warning.
        /// </summary>
        /// <returns></returns>
        [TestMethod, TestCategory("MaintainabilityRules/SA1517")]
        public async Task TestWithoutCarriageReturnLineFeedAtStartOfFile()
        {
            await this.VerifyCSharpDiagnosticAsync(BaseCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        /// <summary>
        /// Verifies that a invalid spacing will not trigger SA1517.
        /// </summary>
        /// <returns></returns>
        [TestMethod, TestCategory("MaintainabilityRules/SA1517")]
        public async Task TestWithInvalidSpacing()
        {
            var testCode = "    " + BaseCode;
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        /// <summary>
        /// Verifies that the code fix provider will strip leading blank lines.
        /// </summary>
        /// <returns></returns>
        [TestMethod, TestCategory("MaintainabilityRules/SA1517")]
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
        [TestMethod, TestCategory("MaintainabilityRules/SA1517")]
        public async Task TestCodeFixProviderHandlesWhitespaceProperly()
        {
            var testCode = "\r\n   " + BaseCode;
            var fixedTestCode = "   " + BaseCode;

            await this.VerifyCSharpFixAsync(testCode,  fixedTestCode);
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
    }
}
