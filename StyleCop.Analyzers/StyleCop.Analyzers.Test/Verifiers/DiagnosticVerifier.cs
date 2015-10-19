// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace TestHelper
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Xunit;

    /// <summary>
    /// Superclass of all unit tests for <see cref="DiagnosticAnalyzer"/>s.
    /// </summary>
    public abstract partial class DiagnosticVerifier
    {
        protected static DiagnosticResult[] EmptyDiagnosticResults { get; } = { };

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
        /// Verifies that each diagnostics contains a <see cref="DiagnosticDescriptor.HelpLinkUri"/> in the expected
        /// format.
        /// </summary>
        [Fact]
        public void TestHelpLink()
        {
            foreach (var diagnosticAnalyzer in this.GetCSharpDiagnosticAnalyzers())
            {
                foreach (var diagnostic in diagnosticAnalyzer.SupportedDiagnostics)
                {
                    if (diagnostic.DefaultSeverity == DiagnosticSeverity.Hidden && diagnostic.CustomTags.Contains(WellKnownDiagnosticTags.NotConfigurable))
                    {
                        // This diagnostic will never appear in the UI.
                        continue;
                    }

                    string expected = $"https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/{diagnostic.Id}.md";
                    Assert.Equal(expected, diagnostic.HelpLinkUri);
                }
            }
        }

        /// <summary>
        /// Gets the C# analyzers being tested
        /// </summary>
        /// <returns>
        /// New instances of all the C# analyzers being tested.
        /// </returns>
        protected abstract IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers();

        /// <summary>
        /// Called to test a C# <see cref="DiagnosticAnalyzer"/> when applied on the single input source as a string.
        /// <note type="note">
        /// <para>Input a <see cref="DiagnosticResult"/> for the expected <see cref="Diagnostic"/>.</para>
        /// </note>
        /// </summary>
        /// <param name="source">A class in the form of a string to run the analyzer on.</param>
        /// <param name="expected">A <see cref="DiagnosticResult"/>s describing the <see cref="Diagnostic"/> that should
        /// be reported by the analyzer for the specified source.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that the task will observe.</param>
        /// <param name="filename">The filename or null if the default filename should be used</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        protected Task VerifyCSharpDiagnosticAsync(string source, DiagnosticResult expected, CancellationToken cancellationToken, string filename = null)
        {
            return this.VerifyCSharpDiagnosticAsync(source, new[] { expected }, cancellationToken, filename);
        }

        /// <summary>
        /// Called to test a C# <see cref="DiagnosticAnalyzer"/> when applied on the single input source as a string.
        /// <note type="note">
        /// <para>Input a <see cref="DiagnosticResult"/> for each <see cref="Diagnostic"/> expected.</para>
        /// </note>
        /// </summary>
        /// <param name="source">A class in the form of a string to run the analyzer on.</param>
        /// <param name="expected">A collection of <see cref="DiagnosticResult"/>s describing the
        /// <see cref="Diagnostic"/>s that should be reported by the analyzer for the specified source.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that the task will observe.</param>
        /// <param name="filename">The filename or null if the default filename should be used</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        protected Task VerifyCSharpDiagnosticAsync(string source, DiagnosticResult[] expected, CancellationToken cancellationToken, string filename = null)
        {
            return this.VerifyDiagnosticsAsync(new[] { source }, LanguageNames.CSharp, this.GetCSharpDiagnosticAnalyzers().ToImmutableArray(), expected, cancellationToken, new[] { filename });
        }

        /// <summary>
        /// Called to test a C# <see cref="DiagnosticAnalyzer"/> when applied on the input strings as sources.
        /// <note type="note">
        /// <para>Input a <see cref="DiagnosticResult"/> for each <see cref="Diagnostic"/> expected.</para>
        /// </note>
        /// </summary>
        /// <param name="sources">A collection of strings to create source documents from to run the analyzers
        /// on.</param>
        /// <param name="expected">A collection of <see cref="DiagnosticResult"/>s describing the
        /// <see cref="Diagnostic"/>s that should be reported by the analyzer for the specified sources.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that the task will observe.</param>
        /// <param name="filenames">The filenames or null if the default filename should be used</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        protected Task VerifyCSharpDiagnosticAsync(string[] sources, DiagnosticResult[] expected, CancellationToken cancellationToken, string[] filenames = null)
        {
            return this.VerifyDiagnosticsAsync(sources, LanguageNames.CSharp, this.GetCSharpDiagnosticAnalyzers().ToImmutableArray(), expected, cancellationToken, filenames);
        }

        /// <summary>
        /// Checks each of the actual <see cref="Diagnostic"/>s found and compares them with the corresponding
        /// <see cref="DiagnosticResult"/> in the array of expected results. <see cref="Diagnostic"/>s are considered
        /// equal only if the <see cref="DiagnosticResult.Locations"/>, <see cref="DiagnosticResult.Id"/>,
        /// <see cref="DiagnosticResult.Severity"/>, and <see cref="DiagnosticResult.Message"/> of the
        /// <see cref="DiagnosticResult"/> match the actual <see cref="Diagnostic"/>.
        /// </summary>
        /// <param name="actualResults">The <see cref="Diagnostic"/>s found by the compiler after running the analyzer
        /// on the source code.</param>
        /// <param name="analyzers">The analyzers that have been run on the sources.</param>
        /// <param name="expectedResults">A collection of <see cref="DiagnosticResult"/>s describing the expected
        /// diagnostics for the sources.</param>
        private static void VerifyDiagnosticResults(IEnumerable<Diagnostic> actualResults, ImmutableArray<DiagnosticAnalyzer> analyzers, DiagnosticResult[] expectedResults)
        {
            int expectedCount = expectedResults.Length;
            int actualCount = actualResults.Count();

            if (expectedCount != actualCount)
            {
                string diagnosticsOutput = actualResults.Any() ? FormatDiagnostics(analyzers, actualResults.ToArray()) : "    NONE.";

                Assert.True(
                    false,
                    string.Format("Mismatch between number of diagnostics returned, expected \"{0}\" actual \"{1}\"\r\n\r\nDiagnostics:\r\n{2}\r\n", expectedCount, actualCount, diagnosticsOutput));
            }

            for (int i = 0; i < expectedResults.Length; i++)
            {
                var actual = actualResults.ElementAt(i);
                var expected = expectedResults[i];

                if (expected.Line == -1 && expected.Column == -1)
                {
                    if (actual.Location != Location.None)
                    {
                        string message =
                            string.Format(
                                "Expected:\nA project diagnostic with No location\nActual:\n{0}",
                                FormatDiagnostics(analyzers, actual));
                        Assert.True(false, message);
                    }
                }
                else
                {
                    VerifyDiagnosticLocation(analyzers, actual, actual.Location, expected.Locations.First());
                    var additionalLocations = actual.AdditionalLocations.ToArray();

                    if (additionalLocations.Length != expected.Locations.Length - 1)
                    {
                        string message =
                            string.Format(
                                "Expected {0} additional locations but got {1} for Diagnostic:\r\n    {2}\r\n",
                                expected.Locations.Length - 1,
                                additionalLocations.Length,
                                FormatDiagnostics(analyzers, actual));
                        Assert.True(false, message);
                    }

                    for (int j = 0; j < additionalLocations.Length; ++j)
                    {
                        VerifyDiagnosticLocation(analyzers, actual, additionalLocations[j], expected.Locations[j + 1]);
                    }
                }

                if (actual.Id != expected.Id)
                {
                    string message =
                        string.Format(
                            "Expected diagnostic id to be \"{0}\" was \"{1}\"\r\n\r\nDiagnostic:\r\n    {2}\r\n",
                            expected.Id,
                            actual.Id,
                            FormatDiagnostics(analyzers, actual));
                    Assert.True(false, message);
                }

                if (actual.Severity != expected.Severity)
                {
                    string message =
                        string.Format(
                            "Expected diagnostic severity to be \"{0}\" was \"{1}\"\r\n\r\nDiagnostic:\r\n    {2}\r\n",
                            expected.Severity,
                            actual.Severity,
                            FormatDiagnostics(analyzers, actual));
                    Assert.True(false, message);
                }

                if (actual.GetMessage() != expected.Message)
                {
                    string message =
                        string.Format(
                            "Expected diagnostic message to be \"{0}\" was \"{1}\"\r\n\r\nDiagnostic:\r\n    {2}\r\n",
                            expected.Message,
                            actual.GetMessage(),
                            FormatDiagnostics(analyzers, actual));
                    Assert.True(false, message);
                }
            }
        }

        /// <summary>
        /// Helper method to <see cref="VerifyDiagnosticResults"/> that checks the location of a
        /// <see cref="Diagnostic"/> and compares it with the location described by a
        /// <see cref="DiagnosticResultLocation"/>.
        /// </summary>
        /// <param name="analyzers">The analyzer that have been run on the sources.</param>
        /// <param name="diagnostic">The diagnostic that was found in the code.</param>
        /// <param name="actual">The location of the diagnostic found in the code.</param>
        /// <param name="expected">The <see cref="DiagnosticResultLocation"/> describing the expected location of the
        /// diagnostic.</param>
        private static void VerifyDiagnosticLocation(ImmutableArray<DiagnosticAnalyzer> analyzers, Diagnostic diagnostic, Location actual, DiagnosticResultLocation expected)
        {
            var actualSpan = actual.GetLineSpan();

            string message =
                string.Format(
                    "Expected diagnostic to be in file \"{0}\" was actually in file \"{1}\"\r\n\r\nDiagnostic:\r\n    {2}\r\n",
                    expected.Path,
                    actualSpan.Path,
                    FormatDiagnostics(analyzers, diagnostic));
            Assert.True(
                actualSpan.Path == expected.Path || (actualSpan.Path != null && actualSpan.Path.Contains("Test0.") && expected.Path.Contains("Test.")),
                message);

            var actualLinePosition = actualSpan.StartLinePosition;

            // Only check line position if it matters
            if (expected.Line > 0)
            {
                if (actualLinePosition.Line + 1 != expected.Line)
                {
                    string message2 =
                        string.Format(
                            "Expected diagnostic to be on line \"{0}\" was actually on line \"{1}\"\r\n\r\nDiagnostic:\r\n    {2}\r\n",
                            expected.Line,
                            actualLinePosition.Line + 1,
                            FormatDiagnostics(analyzers, diagnostic));
                    Assert.True(false, message2);
                }
            }

            // Only check column position if it matters
            if (expected.Column > 0)
            {
                if (actualLinePosition.Character + 1 != expected.Column)
                {
                    string message2 =
                        string.Format(
                            "Expected diagnostic to start at column \"{0}\" was actually at column \"{1}\"\r\n\r\nDiagnostic:\r\n    {2}\r\n",
                            expected.Column,
                            actualLinePosition.Character + 1,
                            FormatDiagnostics(analyzers, diagnostic));
                    Assert.True(false, message2);
                }
            }
        }

        /// <summary>
        /// Helper method to format a <see cref="Diagnostic"/> into an easily readable string.
        /// </summary>
        /// <param name="analyzers">The analyzers that this verifier tests.</param>
        /// <param name="diagnostics">A collection of <see cref="Diagnostic"/>s to be formatted.</param>
        /// <returns>The <paramref name="diagnostics"/> formatted as a string.</returns>
        private static string FormatDiagnostics(ImmutableArray<DiagnosticAnalyzer> analyzers, params Diagnostic[] diagnostics)
        {
            var builder = new StringBuilder();
            for (int i = 0; i < diagnostics.Length; ++i)
            {
                var diagnosticsId = diagnostics[i].Id;

                builder.Append("// ").AppendLine(diagnostics[i].ToString());

                var applicableAnalyzer = analyzers.FirstOrDefault(a => a.SupportedDiagnostics.Any(dd => dd.Id == diagnosticsId));
                if (applicableAnalyzer != null)
                {
                    var analyzerType = applicableAnalyzer.GetType();

                    var location = diagnostics[i].Location;
                    if (location == Location.None)
                    {
                        builder.AppendFormat("GetGlobalResult({0}.{1})", analyzerType.Name, diagnosticsId);
                    }
                    else
                    {
                        Assert.True(
                            location.IsInSource,
                            string.Format("Test base does not currently handle diagnostics in metadata locations. Diagnostic in metadata:\r\n{0}", diagnostics[i]));

                        string resultMethodName = diagnostics[i].Location.SourceTree.FilePath.EndsWith(".cs") ? "GetCSharpResultAt" : "GetBasicResultAt";
                        var linePosition = diagnostics[i].Location.GetLineSpan().StartLinePosition;

                        builder.AppendFormat(
                            "{0}({1}, {2}, {3}.{4})",
                            resultMethodName,
                            linePosition.Line + 1,
                            linePosition.Character + 1,
                            analyzerType.Name,
                            diagnosticsId);
                    }

                    if (i != diagnostics.Length - 1)
                    {
                        builder.Append(',');
                    }

                    builder.AppendLine();
                }
            }

            return builder.ToString();
        }

        /// <summary>
        /// General method that gets a collection of actual <see cref="Diagnostic"/>s found in the source after the
        /// analyzer is run, then verifies each of them.
        /// </summary>
        /// <param name="sources">An array of strings to create source documents from to run the analyzers on.</param>
        /// <param name="language">The language of the classes represented by the source strings.</param>
        /// <param name="analyzers">The analyzers to be run on the source code.</param>
        /// <param name="expected">A collection of <see cref="DiagnosticResult"/>s that should appear after the analyzer
        /// is run on the sources.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that the task will observe.</param>
        /// <param name="filenames">The filenames or null if the default filename should be used</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        private async Task VerifyDiagnosticsAsync(string[] sources, string language, ImmutableArray<DiagnosticAnalyzer> analyzers, DiagnosticResult[] expected, CancellationToken cancellationToken, string[] filenames)
        {
            VerifyDiagnosticResults(await this.GetSortedDiagnosticsAsync(sources, language, analyzers, cancellationToken, filenames).ConfigureAwait(false), analyzers, expected);

            // If filenames is null we want to test for exclusions too
            if (filenames == null)
            {
                // Also check if the analyzer honors exclusions
                if (expected.Any(x => x.Id.StartsWith("SA", StringComparison.Ordinal) || x.Id.StartsWith("SX", StringComparison.Ordinal)))
                {
                    // We want to look at non-stylecop diagnostics only. We also insert a new line at the beginning
                    // so we have to move all diagnostic location down by one line
                    var expectedResults = expected
                        .Where(x => !x.Id.StartsWith("SA", StringComparison.Ordinal) && !x.Id.StartsWith("SX", StringComparison.Ordinal))
                        .Select(x => x.WithLineOffset(1))
                        .ToArray();

                    VerifyDiagnosticResults(await this.GetSortedDiagnosticsAsync(sources.Select(x => " // <auto-generated>\r\n" + x).ToArray(), language, analyzers, cancellationToken, null).ConfigureAwait(false), analyzers, expectedResults);
                }
            }
        }
    }
}
