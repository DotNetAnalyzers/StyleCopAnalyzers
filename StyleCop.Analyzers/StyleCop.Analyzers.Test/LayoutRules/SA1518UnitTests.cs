// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.LayoutRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.LayoutRules;
    using StyleCop.Analyzers.Settings.ObjectModel;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// Unit tests for <see cref="SA1518UseLineEndingsCorrectlyAtEndOfFile"/>.
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

        private OptionSetting? newlineAtEndOfFile;

        /// <summary>
        /// Verifies that blank lines at the end of the file will produce a warning.
        /// </summary>
        /// <param name="newlineAtEndOfFile">The effective <see cref="OptionSetting"/> setting.</param>
        /// <param name="result">The expected text to appear at the end of the file.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData(null, "\r\n")]
        [InlineData(OptionSetting.Allow, "\r\n")]
        [InlineData(OptionSetting.Require, "\r\n")]
        [InlineData(OptionSetting.Omit, "")]
        internal async Task TestWithBlankLinesAtEndOfFileAsync(OptionSetting? newlineAtEndOfFile, string result)
        {
            this.newlineAtEndOfFile = newlineAtEndOfFile;

            var testCode = BaseCode + "\r\n\r\n";
            var fixedCode = BaseCode + result;

            var expected = this.CSharpDiagnostic(this.GetDescriptor(newlineAtEndOfFile)).WithLocation(8, 2);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that linefeed only blank lines at the end of the file will produce a warning.
        /// </summary>
        /// <param name="newlineAtEndOfFile">The effective <see cref="OptionSetting"/> setting.</param>
        /// <param name="result">The expected text to appear at the end of the file.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData(null, "\r\n")]
        [InlineData(OptionSetting.Allow, "\r\n")]
        [InlineData(OptionSetting.Require, "\r\n")]
        [InlineData(OptionSetting.Omit, "")]
        internal async Task TestWithLineFeedOnlyBlankLinesAtEndOfFileAsync(OptionSetting? newlineAtEndOfFile, string result)
        {
            this.newlineAtEndOfFile = newlineAtEndOfFile;

            var testCode = BaseCode + "\n\n";
            var fixedCode = BaseCode + result;

            var expected = this.CSharpDiagnostic(this.GetDescriptor(newlineAtEndOfFile)).WithLocation(8, 2);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a single carriage return / linefeed at the end of the file will not produce a warning.
        /// </summary>
        /// <param name="newlineAtEndOfFile">The effective <see cref="OptionSetting"/> setting.</param>
        /// <param name="expectedText">The expected text to appear at the end of the file, or <see langword="null"/> if
        /// the input text is already correct.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData(null, null)]
        [InlineData(OptionSetting.Allow, null)]
        [InlineData(OptionSetting.Require, null)]
        [InlineData(OptionSetting.Omit, "")]
        internal async Task TestWithSingleCarriageReturnLineFeedAtEndOfFileAsync(OptionSetting? newlineAtEndOfFile, string expectedText)
        {
            this.newlineAtEndOfFile = newlineAtEndOfFile;

            var testCode = BaseCode + "\r\n";
            var fixedCode = BaseCode + expectedText;

            if (expectedText == null)
            {
                await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            }
            else
            {
                var expected = this.CSharpDiagnostic(this.GetDescriptor(newlineAtEndOfFile)).WithLocation(8, 2);
                await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
                await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
                await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Verifies that a single linefeed at the end of the file will not produce a warning.
        /// </summary>
        /// <param name="newlineAtEndOfFile">The effective <see cref="OptionSetting"/> setting.</param>
        /// <param name="expectedText">The expected text to appear at the end of the file, or <see langword="null"/> if
        /// the input text is already correct.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData(null, null)]
        [InlineData(OptionSetting.Allow, null)]
        [InlineData(OptionSetting.Require, null)]
        [InlineData(OptionSetting.Omit, "")]
        internal async Task TestWithSingleLineFeedAtEndOfFileAsync(OptionSetting? newlineAtEndOfFile, string expectedText)
        {
            this.newlineAtEndOfFile = newlineAtEndOfFile;

            var testCode = BaseCode + "\n";
            var fixedCode = BaseCode + expectedText;

            if (expectedText == null)
            {
                await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            }
            else
            {
                var expected = this.CSharpDiagnostic(this.GetDescriptor(newlineAtEndOfFile)).WithLocation(8, 2);
                await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
                await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
                await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Verifies that a source file that ends without a carriage return / linefeed at the end of the file will not produce a warning.
        /// </summary>
        /// <param name="newlineAtEndOfFile">The effective <see cref="OptionSetting"/> setting.</param>
        /// <param name="expectedText">The expected text to appear at the end of the file, or <see langword="null"/> if
        /// the input text is already correct.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData(null, null)]
        [InlineData(OptionSetting.Allow, null)]
        [InlineData(OptionSetting.Require, "\r\n")]
        [InlineData(OptionSetting.Omit, null)]
        internal async Task TestWithoutCarriageReturnLineFeedAtEndOfFileAsync(OptionSetting? newlineAtEndOfFile, string expectedText)
        {
            this.newlineAtEndOfFile = newlineAtEndOfFile;

            var testCode = BaseCode;
            var fixedCode = BaseCode + expectedText;

            if (expectedText == null)
            {
                await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            }
            else
            {
                var expected = this.CSharpDiagnostic(this.GetDescriptor(newlineAtEndOfFile)).WithLocation(8, 2);
                await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
                await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
                await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Verifies that a source file that ends with spaces will produce a warning.
        /// </summary>
        /// <param name="newlineAtEndOfFile">The effective <see cref="OptionSetting"/> setting.</param>
        /// <param name="expectedText">The expected text to appear at the end of the file.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData(null, "\r\n")]
        [InlineData(OptionSetting.Allow, "\r\n")]
        [InlineData(OptionSetting.Require, "\r\n")]
        [InlineData(OptionSetting.Omit, "")]
        internal async Task TestFileEndsWithSpacesAsync(OptionSetting? newlineAtEndOfFile, string expectedText)
        {
            this.newlineAtEndOfFile = newlineAtEndOfFile;

            var testCode = BaseCode + "\r\n          ";
            var fixedCode = BaseCode + expectedText;

            var expected = this.CSharpDiagnostic(this.GetDescriptor(newlineAtEndOfFile)).WithLocation(8, 2);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a comment at the end of the file is not flagged.
        /// </summary>
        /// <param name="newlineAtEndOfFile">The effective <see cref="OptionSetting"/> setting.</param>
        /// <param name="expectedText">The expected text to appear at the end of the file, or <see langword="null"/> if
        /// the input text is already correct.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData(null, null)]
        [InlineData(OptionSetting.Allow, null)]
        [InlineData(OptionSetting.Require, "\r\n")]
        [InlineData(OptionSetting.Omit, null)]
        internal async Task TestFileEndingWithCommentAsync(OptionSetting? newlineAtEndOfFile, string expectedText)
        {
            this.newlineAtEndOfFile = newlineAtEndOfFile;

            var testCode = BaseCode + "\r\n// Test comment";
            var fixedCode = BaseCode + "\r\n// Test comment" + expectedText;

            if (expectedText == null)
            {
                await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            }
            else
            {
                var expected = this.CSharpDiagnostic(this.GetDescriptor(newlineAtEndOfFile)).WithLocation(9, 16);
                await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
                await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
                await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Verifies that spurious end of lines after a comment at the end of the file are flagged.
        /// </summary>
        /// <param name="newlineAtEndOfFile">The effective <see cref="OptionSetting"/> setting.</param>
        /// <param name="expectedText">The expected text to appear at the end of the file.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData(null, "\r\n")]
        [InlineData(OptionSetting.Allow, "\r\n")]
        [InlineData(OptionSetting.Require, "\r\n")]
        [InlineData(OptionSetting.Omit, "")]
        internal async Task TestFileEndingWithCommentAndSpuriousWhitespaceAsync(OptionSetting? newlineAtEndOfFile, string expectedText)
        {
            this.newlineAtEndOfFile = newlineAtEndOfFile;

            var testCode = BaseCode + "\r\n// Test comment\r\n   \r\n";
            var fixedCode = BaseCode + "\r\n// Test comment" + expectedText;

            var expected = this.CSharpDiagnostic(this.GetDescriptor(newlineAtEndOfFile)).WithLocation(9, 16);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that an endif at the end of the file is not flagged.
        /// </summary>
        /// <param name="newlineAtEndOfFile">The effective <see cref="OptionSetting"/> setting.</param>
        /// <param name="expectedText">The expected text to appear at the end of the file, or <see langword="null"/> if
        /// the input text is already correct.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData(null, null)]
        [InlineData(OptionSetting.Allow, null)]
        [InlineData(OptionSetting.Require, null)]
        [InlineData(OptionSetting.Omit, "")]
        internal async Task TestFileEndingWithEndIfAsync(OptionSetting? newlineAtEndOfFile, string expectedText)
        {
            this.newlineAtEndOfFile = newlineAtEndOfFile;

            var testCode = "#if true\r\n" + BaseCode + "\r\n#endif\r\n";
            var fixedCode = "#if true\r\n" + BaseCode + "\r\n#endif" + expectedText;

            if (expectedText == null)
            {
                await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            }
            else
            {
                var expected = this.CSharpDiagnostic(this.GetDescriptor(newlineAtEndOfFile)).WithLocation(10, 7);
                await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
                await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
                await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Verifies that an endif at the end of the file is not flagged.
        /// </summary>
        /// <param name="newlineAtEndOfFile">The effective <see cref="OptionSetting"/> setting.</param>
        /// <param name="expectedText">The expected text to appear at the end of the file.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData(null, "\r\n")]
        [InlineData(OptionSetting.Allow, "\r\n")]
        [InlineData(OptionSetting.Require, "\r\n")]
        [InlineData(OptionSetting.Omit, "")]
        internal async Task TestFileEndingWithEndIfWithSpuriousWhitespaceAsync(OptionSetting? newlineAtEndOfFile, string expectedText)
        {
            this.newlineAtEndOfFile = newlineAtEndOfFile;

            var testCode = "#if true\r\n" + BaseCode + "\r\n#endif\r\n   \r\n";
            var fixedCode = "#if true\r\n" + BaseCode + "\r\n#endif" + expectedText;

            var expected = this.CSharpDiagnostic(this.GetDescriptor(newlineAtEndOfFile)).WithLocation(10, 7);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the code fix provider will strip trailing blank lines.
        /// </summary>
        /// <param name="newlineAtEndOfFile">The effective <see cref="OptionSetting"/> setting.</param>
        /// <param name="expectedText">The expected text to appear at the end of the file.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData(null, "\r\n")]
        [InlineData(OptionSetting.Allow, "\r\n")]
        [InlineData(OptionSetting.Require, "\r\n")]
        [InlineData(OptionSetting.Omit, "")]
        internal async Task TestCodeFixProviderStripsTrailingBlankLinesAsync(OptionSetting? newlineAtEndOfFile, string expectedText)
        {
            this.newlineAtEndOfFile = newlineAtEndOfFile;

            var testCode = BaseCode + "\r\n\r\n";
            var fixedCode = BaseCode + expectedText;

            var expected = this.CSharpDiagnostic(this.GetDescriptor(newlineAtEndOfFile)).WithLocation(8, 2);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the code fix provider will strip trailing blank lines that include whitespace.
        /// </summary>
        /// <param name="newlineAtEndOfFile">The effective <see cref="OptionSetting"/> setting.</param>
        /// <param name="expectedText">The expected text to appear at the end of the file.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData(null, "\r\n")]
        [InlineData(OptionSetting.Allow, "\r\n")]
        [InlineData(OptionSetting.Require, "\r\n")]
        [InlineData(OptionSetting.Omit, "")]
        internal async Task TestCodeFixProviderStripsTrailingBlankLinesIncludingWhitespaceAsync(OptionSetting? newlineAtEndOfFile, string expectedText)
        {
            this.newlineAtEndOfFile = newlineAtEndOfFile;

            var testCode = BaseCode + "\r\n   \r\n   \r\n";
            var fixedCode = BaseCode + expectedText;

            var expected = this.CSharpDiagnostic(this.GetDescriptor(newlineAtEndOfFile)).WithLocation(8, 2);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the code fix provider will strip trailing linefeed only blank lines that include whitespace.
        /// </summary>
        /// <param name="newlineAtEndOfFile">The effective <see cref="OptionSetting"/> setting.</param>
        /// <param name="expectedText">The expected text to appear at the end of the file.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData(null, "\r\n")]
        [InlineData(OptionSetting.Allow, "\r\n")]
        [InlineData(OptionSetting.Require, "\r\n")]
        [InlineData(OptionSetting.Omit, "")]
        internal async Task TestCodeFixProviderStripsTrailingLinefeedOnlyBlankLinesIncludingWhitespaceAsync(OptionSetting? newlineAtEndOfFile, string expectedText)
        {
            this.newlineAtEndOfFile = newlineAtEndOfFile;

            var testCode = BaseCode + "\n   \n   \n";
            var fixedCode = BaseCode + expectedText;

            var expected = this.CSharpDiagnostic(this.GetDescriptor(newlineAtEndOfFile)).WithLocation(8, 2);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the code fix provider will strip only trailing blank lines.
        /// </summary>
        /// <param name="newlineAtEndOfFile">The effective <see cref="OptionSetting"/> setting.</param>
        /// <param name="expectedText">The expected text to appear at the end of the file.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData(null, "\r\n")]
        [InlineData(OptionSetting.Allow, "\r\n")]
        [InlineData(OptionSetting.Require, "\r\n")]
        [InlineData(OptionSetting.Omit, "")]
        internal async Task TestCodeFixProviderOnlyStripsTrailingBlankLinesAsync(OptionSetting? newlineAtEndOfFile, string expectedText)
        {
            this.newlineAtEndOfFile = newlineAtEndOfFile;

            var testCode = "#if true\r\n" + BaseCode + "\r\n#endif\r\n   \r\n";
            var fixedCode = "#if true\r\n" + BaseCode + "\r\n#endif" + expectedText;

            var expected = this.CSharpDiagnostic(this.GetDescriptor(newlineAtEndOfFile)).WithLocation(10, 7);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        protected override string GetSettings()
        {
            if (this.newlineAtEndOfFile == null)
            {
                return base.GetSettings();
            }

            return $@"
{{
  ""settings"": {{
    ""layoutRules"": {{
      ""newlineAtEndOfFile"": ""{this.newlineAtEndOfFile.ToString().ToLowerInvariant()}""
    }}
  }}
}}";
        }

        /// <inheritdoc/>
        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1518UseLineEndingsCorrectlyAtEndOfFile();
        }

        /// <inheritdoc/>
        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new SA1518CodeFixProvider();
        }

        private DiagnosticDescriptor GetDescriptor(OptionSetting? endOfFileHandling)
        {
            switch (endOfFileHandling)
            {
            case OptionSetting.Require:
                return SA1518UseLineEndingsCorrectlyAtEndOfFile.DescriptorRequire;

            case OptionSetting.Omit:
                return SA1518UseLineEndingsCorrectlyAtEndOfFile.DescriptorOmit;

            case OptionSetting.Allow:
            case null:
            default:
                return SA1518UseLineEndingsCorrectlyAtEndOfFile.DescriptorAllow;
            }
        }
    }
}
