// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.LayoutRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using StyleCop.Analyzers.LayoutRules;
    using StyleCop.Analyzers.Settings.ObjectModel;
    using StyleCop.Analyzers.Test.Verifiers;
    using TestHelper;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.CustomDiagnosticVerifier<StyleCop.Analyzers.LayoutRules.SA1518UseLineEndingsCorrectlyAtEndOfFile>;

    /// <summary>
    /// Unit tests for <see cref="SA1518UseLineEndingsCorrectlyAtEndOfFile"/>.
    /// </summary>
    public class SA1518UnitTests
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
            var testCode = BaseCode + "\r\n\r\n";
            var fixedCode = BaseCode + result;

            var expected = Diagnostic(this.GetDescriptor(newlineAtEndOfFile)).WithLocation(8, 2);
            await VerifyCSharpFixAsync(newlineAtEndOfFile, testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
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
            var testCode = BaseCode + "\n\n";
            var fixedCode = BaseCode + result;

            var expected = Diagnostic(this.GetDescriptor(newlineAtEndOfFile)).WithLocation(8, 2);
            await VerifyCSharpFixAsync(newlineAtEndOfFile, testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
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
            var testCode = BaseCode + "\r\n";
            var fixedCode = BaseCode + expectedText;

            if (expectedText == null)
            {
                await VerifyCSharpDiagnosticAsync(newlineAtEndOfFile, testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            }
            else
            {
                var expected = Diagnostic(this.GetDescriptor(newlineAtEndOfFile)).WithLocation(8, 2);
                await VerifyCSharpFixAsync(newlineAtEndOfFile, testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
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
            var testCode = BaseCode + "\n";
            var fixedCode = BaseCode + expectedText;

            if (expectedText == null)
            {
                await VerifyCSharpDiagnosticAsync(newlineAtEndOfFile, testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            }
            else
            {
                var expected = Diagnostic(this.GetDescriptor(newlineAtEndOfFile)).WithLocation(8, 2);
                await VerifyCSharpFixAsync(newlineAtEndOfFile, testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
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
            var testCode = BaseCode;
            var fixedCode = BaseCode + expectedText;

            if (expectedText == null)
            {
                await VerifyCSharpDiagnosticAsync(newlineAtEndOfFile, testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            }
            else
            {
                var expected = Diagnostic(this.GetDescriptor(newlineAtEndOfFile)).WithLocation(8, 2);
                await VerifyCSharpFixAsync(newlineAtEndOfFile, testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
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
            var testCode = BaseCode + "\r\n          ";
            var fixedCode = BaseCode + expectedText;

            var expected = Diagnostic(this.GetDescriptor(newlineAtEndOfFile)).WithLocation(8, 2);
            await VerifyCSharpFixAsync(newlineAtEndOfFile, testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
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
            var testCode = BaseCode + "\r\n// Test comment";
            var fixedCode = BaseCode + "\r\n// Test comment" + expectedText;

            if (expectedText == null)
            {
                await VerifyCSharpDiagnosticAsync(newlineAtEndOfFile, testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            }
            else
            {
                var expected = Diagnostic(this.GetDescriptor(newlineAtEndOfFile)).WithLocation(9, 16);
                await VerifyCSharpFixAsync(newlineAtEndOfFile, testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
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
            var testCode = BaseCode + "\r\n// Test comment\r\n   \r\n";
            var fixedCode = BaseCode + "\r\n// Test comment" + expectedText;

            var expected = Diagnostic(this.GetDescriptor(newlineAtEndOfFile)).WithLocation(9, 16);
            await VerifyCSharpFixAsync(newlineAtEndOfFile, testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
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
            var testCode = "#if true\r\n" + BaseCode + "\r\n#endif\r\n";
            var fixedCode = "#if true\r\n" + BaseCode + "\r\n#endif" + expectedText;

            if (expectedText == null)
            {
                await VerifyCSharpDiagnosticAsync(newlineAtEndOfFile, testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            }
            else
            {
                var expected = Diagnostic(this.GetDescriptor(newlineAtEndOfFile)).WithLocation(10, 7);
                await VerifyCSharpFixAsync(newlineAtEndOfFile, testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
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
            var testCode = "#if true\r\n" + BaseCode + "\r\n#endif\r\n   \r\n";
            var fixedCode = "#if true\r\n" + BaseCode + "\r\n#endif" + expectedText;

            var expected = Diagnostic(this.GetDescriptor(newlineAtEndOfFile)).WithLocation(10, 7);
            await VerifyCSharpFixAsync(newlineAtEndOfFile, testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
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
            var testCode = BaseCode + "\r\n\r\n";
            var fixedCode = BaseCode + expectedText;

            var expected = Diagnostic(this.GetDescriptor(newlineAtEndOfFile)).WithLocation(8, 2);
            await VerifyCSharpFixAsync(newlineAtEndOfFile, testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
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
            var testCode = BaseCode + "\r\n   \r\n   \r\n";
            var fixedCode = BaseCode + expectedText;

            var expected = Diagnostic(this.GetDescriptor(newlineAtEndOfFile)).WithLocation(8, 2);
            await VerifyCSharpFixAsync(newlineAtEndOfFile, testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
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
            var testCode = BaseCode + "\n   \n   \n";
            var fixedCode = BaseCode + expectedText;

            var expected = Diagnostic(this.GetDescriptor(newlineAtEndOfFile)).WithLocation(8, 2);
            await VerifyCSharpFixAsync(newlineAtEndOfFile, testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
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
            var testCode = "#if true\r\n" + BaseCode + "\r\n#endif\r\n   \r\n";
            var fixedCode = "#if true\r\n" + BaseCode + "\r\n#endif" + expectedText;

            var expected = Diagnostic(this.GetDescriptor(newlineAtEndOfFile)).WithLocation(10, 7);
            await VerifyCSharpFixAsync(newlineAtEndOfFile, testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        private static Task VerifyCSharpDiagnosticAsync(OptionSetting? newlineAtEndOfFile, string source, DiagnosticResult[] expected, CancellationToken cancellationToken)
        {
            var test = new StyleCopCodeFixVerifier<SA1518UseLineEndingsCorrectlyAtEndOfFile, SA1518CodeFixProvider>.CSharpTest
            {
                TestCode = source,
            };

            if (newlineAtEndOfFile != null)
            {
                test.Settings = GetSettings(newlineAtEndOfFile.Value);
            }

            test.ExpectedDiagnostics.AddRange(expected);
            return test.RunAsync(cancellationToken);
        }

        private static Task VerifyCSharpFixAsync(OptionSetting? newlineAtEndOfFile, string source, DiagnosticResult expected, string fixedSource, CancellationToken cancellationToken)
            => VerifyCSharpFixAsync(newlineAtEndOfFile, source, new[] { expected }, fixedSource, cancellationToken);

        private static Task VerifyCSharpFixAsync(OptionSetting? newlineAtEndOfFile, string source, DiagnosticResult[] expected, string fixedSource, CancellationToken cancellationToken)
        {
            var test = new StyleCopCodeFixVerifier<SA1518UseLineEndingsCorrectlyAtEndOfFile, SA1518CodeFixProvider>.CSharpTest
            {
                TestCode = source,
                FixedCode = fixedSource,
            };

            if (newlineAtEndOfFile != null)
            {
                test.Settings = GetSettings(newlineAtEndOfFile.Value);
            }

            test.ExpectedDiagnostics.AddRange(expected);
            return test.RunAsync(cancellationToken);
        }

        private static string GetSettings(OptionSetting newlineAtEndOfFile)
        {
            return $@"
{{
  ""settings"": {{
    ""layoutRules"": {{
      ""newlineAtEndOfFile"": ""{newlineAtEndOfFile.ToString().ToLowerInvariant()}""
    }}
  }}
}}";
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
