// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.LayoutRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.LayoutRules;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// Unit tests for <see cref="SA1518CodeMustNotContainBlankLinesAtEndOfFile"/>.
    /// </summary>
    public class SA1518UnitTests : CodeFixVerifier
    {
        private const string BaseCode = @"using System.Diagnostics;
public class Foo
{
    public void Bar(int i)
    {
        Debug.Assert(true);
    }
}";

        /// <summary>
        /// Verifies that blank lines at the end of the file will produce a warning.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestWithBlankLinesAtEndOfFileAsync()
        {
            var testCode = BaseCode + "\r\n\r\n";
            await this.VerifyCSharpDiagnosticAsync(testCode, this.GenerateExpectedWarning(9, 1), CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that linefeed only blank lines at the end of the file will produce a warning.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestWithLineFeedOnlyBlankLinesAtEndOfFileAsync()
        {
            var testCode = BaseCode + "\n\n";
            await this.VerifyCSharpDiagnosticAsync(testCode, this.GenerateExpectedWarning(9, 1), CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a single carriage return / linefeed at the end of the file will not produce a warning.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestWithSingleCarriageReturnLineFeedAtEndOfFileAsync()
        {
            var testCode = BaseCode + "\r\n";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a single linefeed at the end of the file will not produce a warning.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestWithSingleLineFeedAtEndOfFileAsync()
        {
            var testCode = BaseCode + "\n";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a source file that ends without a carriage return / linefeed at the end of the file will not produce a warning.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestWithoutCarriageReturnLineFeedAtEndOfFileAsync()
        {
            await this.VerifyCSharpDiagnosticAsync(BaseCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a source file that ends with spaces will produce a warning.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestFileEndsWithSpacesAsync()
        {
            var testCode = BaseCode + "\r\n          ";
            await this.VerifyCSharpDiagnosticAsync(testCode, this.GenerateExpectedWarning(9, 1), CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a comment at the end of the file is not flagged.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestFileEndingWithCommentAsync()
        {
            var testCode = BaseCode + "\r\n// Test comment";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that spurious end of lines after a comment at the end of the file are flagged.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestFileEndingWithCommentAndSpuriousWhitespaceAsync()
        {
            var testCode = BaseCode + "\r\n// Test comment\r\n   \r\n";
            await this.VerifyCSharpDiagnosticAsync(testCode, this.GenerateExpectedWarning(10, 1), CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that an endif at the end of the file is not flagged.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestFileEndingWithEndIfAsync()
        {
            var testCode = "#if true\r\n" + BaseCode + "\r\n#endif\r\n";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that an endif at the end of the file is not flagged.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestFileEndingWithEndIfWithSpuriousWhitespaceAsync()
        {
            var testCode = "#if true\r\n" + BaseCode + "\r\n#endif\r\n   \r\n";
            await this.VerifyCSharpDiagnosticAsync(testCode, this.GenerateExpectedWarning(11, 1), CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the code fix provider will strip trailing blank lines.
        /// </summary>
        /// <remarks>The CRLF after the last curly bracket will not be stripped!</remarks>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestCodeFixProviderStripsTrailingBlankLinesAsync()
        {
            var testCode = BaseCode + "\r\n\r\n";
            var fixedTestCode = BaseCode + "\r\n";

            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the code fix provider will strip trailing blank lines that include whitespace.
        /// </summary>
        /// <remarks>The CRLF after the last curly bracket will not be stripped!</remarks>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestCodeFixProviderStripsTrailingBlankLinesIncludingWhitespaceAsync()
        {
            var testCode = BaseCode + "\r\n   \r\n   \r\n";
            var fixedTestCode = BaseCode + "\r\n";

            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the code fix provider will strip trailing linefeed only blank lines that include whitespace.
        /// </summary>
        /// <remarks>The LF after the last curly bracket will not be stripped!</remarks>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestCodeFixProviderStripsTrailingLinefeedOnlyBlankLinesIncludingWhitespaceAsync()
        {
            var testCode = BaseCode + "\n   \n   \n";
            var fixedTestCode = BaseCode + "\n";

            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the code fix provider will strip only trailing blank lines.
        /// </summary>
        /// <remarks>The CRLF after the #endif will not be stripped!</remarks>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestCodeFixProviderOnlyStripsTrailingBlankLinesAsync()
        {
            var testCode = "#if true\r\n" + BaseCode + "\r\n#endif\r\n   \r\n";
            var fixedTestCode = "#if true\r\n" + BaseCode + "\r\n#endif\r\n";

            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1518CodeMustNotContainBlankLinesAtEndOfFile();
        }

        /// <inheritdoc/>
        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new SA1518CodeFixProvider();
        }

        private DiagnosticResult GenerateExpectedWarning(int line, int column)
        {
            return this.CSharpDiagnostic().WithLocation(line, column);
        }
    }
}
