﻿using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Xunit;
using Xunit.Sdk;

namespace TestHelper
{
    /// <summary>
    /// Superclass of all unit tests for <see cref="DiagnosticAnalyzer"/>s.
    /// </summary>
    public abstract partial class DiagnosticVerifier
    {
        protected static DiagnosticResult[] EmptyDiagnosticResults { get; } = { };

        #region To be implemented by Test classes
        /// <summary>
        /// Get the C# analyzer being tested - to be implemented in non-abstract class.
        /// </summary>
        protected virtual DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return null;
        }

        /// <summary>
        /// Get the Visual Basic analyzer being tested - to be implemented in non-abstract class.
        /// </summary>
        protected virtual DiagnosticAnalyzer GetBasicDiagnosticAnalyzer()
        {
            return null;
        }
        #endregion

        #region Verifier wrappers

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
        protected Task VerifyCSharpDiagnosticAsync(string source, DiagnosticResult[] expected, CancellationToken cancellationToken)
        {
            return this.VerifyDiagnosticsAsync(new[] { source }, LanguageNames.CSharp, this.GetCSharpDiagnosticAnalyzer(), expected, cancellationToken);
        }

        /// <summary>
        /// Called to test a Visual Basic <see cref="DiagnosticAnalyzer"/> when applied on the single input source as a
        /// string.
        /// <note type="note">
        /// <para>Input a <see cref="DiagnosticResult"/> for each <see cref="Diagnostic"/> expected.</para>
        /// </note>
        /// </summary>
        /// <param name="source">A class in the form of a string to run the analyzer on.</param>
        /// <param name="expected">A collection of <see cref="DiagnosticResult"/>s describing the
        /// <see cref="Diagnostic"/>s that should be reported by the analyzer for the specified source.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that the task will observe.</param>
        protected Task VerifyBasicDiagnosticAsync(string source, DiagnosticResult[] expected, CancellationToken cancellationToken)
        {
            return this.VerifyDiagnosticsAsync(new[] { source }, LanguageNames.VisualBasic, this.GetBasicDiagnosticAnalyzer(), expected, cancellationToken);
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
        protected Task VerifyCSharpDiagnosticAsync(string[] sources, DiagnosticResult[] expected, CancellationToken cancellationToken)
        {
            return this.VerifyDiagnosticsAsync(sources, LanguageNames.CSharp, this.GetCSharpDiagnosticAnalyzer(), expected, cancellationToken);
        }

        /// <summary>
        /// Called to test a Visual Basic <see cref="DiagnosticAnalyzer"/> when applied on the input strings as sources.
        /// <note type="note">
        /// <para>Input a <see cref="DiagnosticResult"/> for each <see cref="Diagnostic"/> expected.</para>
        /// </note>
        /// </summary>
        /// <param name="sources">A collection of strings to create source documents from to run the analyzers
        /// on.</param>
        /// <param name="expected">A collection of <see cref="DiagnosticResult"/>s describing the
        /// <see cref="Diagnostic"/>s that should be reported by the analyzer for the specified sources.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that the task will observe.</param>
        protected Task VerifyBasicDiagnosticAsync(string[] sources, DiagnosticResult[] expected, CancellationToken cancellationToken)
        {
            return this.VerifyDiagnosticsAsync(sources, LanguageNames.VisualBasic, this.GetBasicDiagnosticAnalyzer(), expected, cancellationToken);
        }

        /// <summary>
        /// General method that gets a collection of actual <see cref="Diagnostic"/>s found in the source after the
        /// analyzer is run, then verifies each of them.
        /// </summary>
        /// <param name="sources">An array of strings to create source documents from to run the analyzers on.</param>
        /// <param name="language">The language of the classes represented by the source strings.</param>
        /// <param name="analyzer">The analyzer to be run on the source code.</param>
        /// <param name="expected">A collection of <see cref="DiagnosticResult"/>s that should appear after the analyzer
        /// is run on the sources.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that the task will observe.</param>
        private async Task VerifyDiagnosticsAsync(string[] sources, string language, DiagnosticAnalyzer analyzer, DiagnosticResult[] expected, CancellationToken cancellationToken)
        {
            var diagnostics = await GetSortedDiagnosticsAsync(sources, language, analyzer, cancellationToken);
            VerifyDiagnosticResults(diagnostics, analyzer, expected);
        }

        #endregion

        #region Actual comparisons and verifications
        /// <summary>
        /// Checks each of the actual <see cref="Diagnostic"/>s found and compares them with the corresponding
        /// <see cref="DiagnosticResult"/> in the array of expected results. <see cref="Diagnostic"/>s are considered
        /// equal only if the <see cref="DiagnosticResult.Locations"/>, <see cref="DiagnosticResult.Id"/>,
        /// <see cref="DiagnosticResult.Severity"/>, and <see cref="DiagnosticResult.Message"/> of the
        /// <see cref="DiagnosticResult"/> match the actual <see cref="Diagnostic"/>.
        /// </summary>
        /// <param name="actualResults">The <see cref="Diagnostic"/>s found by the compiler after running the analyzer
        /// on the source code.</param>
        /// <param name="analyzer">The analyzer that was being run on the sources</param>
        /// <param name="expectedResults">A collection of <see cref="DiagnosticResult"/>s describing the expected
        /// diagnostics for the sources.</param>
        private static void VerifyDiagnosticResults(IEnumerable<Diagnostic> actualResults, DiagnosticAnalyzer analyzer, params DiagnosticResult[] expectedResults)
        {
            int expectedCount = expectedResults.Count();
            int actualCount = actualResults.Count();
                StringBuilder errorStringBuilder = new StringBuilder();

            if (expectedCount != actualCount)
            {
                errorStringBuilder.AppendLine($"Mismatch between number of diagnostics returned, expected \"{expectedCount}\" actual \"{actualCount}\"");
            }

            AppendMissmatchErrorText(actualResults, analyzer, expectedResults, errorStringBuilder);

            string errorString = errorStringBuilder.ToString();
            if (!string.IsNullOrEmpty(errorString))
            {
                Assert.True(false, errorString);
            }

            //The only thing that could still be wrong is that the duplicated entries in actualResult and expectedResults
            // don't match. Example:
            // Let a, b, c be diagnostics and
            // actual: a, b, b, c
            // expected: a, b, c, c
            // Then every actual result was expected and every expected result was reported, but they still don't match.
            // We shouldn't report multiple diagnostics for a single location so this should not happen, but for the sake
            // of correctness of this method we should still check it.
            for (int i = 0; i < expectedResults.Length; i++)
            {
                var actual = actualResults.ElementAt(i);
                var expected = expectedResults[i];

                AssertDiagnosticsMatch(analyzer, actual, expected);
            }
        }

        private static void AppendMissmatchErrorText(IEnumerable<Diagnostic> actualResults, DiagnosticAnalyzer analyzer, DiagnosticResult[] expectedResults, StringBuilder errorStringBuilder)
        {
            foreach (var actual in actualResults)
            {
                bool success = false;
                foreach (var expected in expectedResults)
                {
                    try
                    {
                        AssertDiagnosticsMatch(analyzer, actual, expected);
                        success = true;
                        break;
                    }
                    catch (XunitException)
                    {
                        // If the diagnostic does not match any expected result we will get an Exception for all
                        // expected diagnostics.
                    }
                }
                if (!success)
                {
                    errorStringBuilder.AppendLine("The diagnostic is reporting this diagnostic, but it is not expected:");
                    errorStringBuilder.AppendLine(FormatDiagnostics(analyzer, actual));
                }
            }

            foreach (var expected in expectedResults)
            {
                bool success = false;
                foreach (var actual in actualResults)
                {
                    try
                    {
                        AssertDiagnosticsMatch(analyzer, actual, expected);
                        success = true;
                        break;
                    }
                    catch (XunitException)
                    {
                        // If the expected result does not match any diagnostic we will get an Exception for all
                        // diagnostics.
                    }
                }
                if (!success)
                {
                    errorStringBuilder.AppendLine("The test is expecting this diagnostic, but it is not reported:");
                    errorStringBuilder.AppendLine(FormatDiagnostics(analyzer, expected));
                }
            }
        }

        private static void AssertDiagnosticsMatch(DiagnosticAnalyzer analyzer, Diagnostic actual, DiagnosticResult expected)
        {
            if (expected.Line == -1 && expected.Column == -1)
            {
                if (actual.Location != Location.None)
                {
                    Assert.True(false,
                        string.Format("Expected:\nA project diagnostic with No location\nActual:\n{0}",
                        FormatDiagnostics(analyzer, actual)));
                }
            }
            else
            {
                VerifyDiagnosticLocation(analyzer, actual, actual.Location, expected.Locations.First());
                var additionalLocations = actual.AdditionalLocations.ToArray();

                if (additionalLocations.Length != expected.Locations.Length - 1)
                {
                    Assert.True(false,
                        string.Format("Expected {0} additional locations but got {1} for Diagnostic:\r\n    {2}\r\n",
                            expected.Locations.Length - 1, additionalLocations.Length,
                            FormatDiagnostics(analyzer, actual)));
                }

                for (int j = 0; j < additionalLocations.Length; ++j)
                {
                    VerifyDiagnosticLocation(analyzer, actual, additionalLocations[j], expected.Locations[j + 1]);
                }
            }

            if (actual.Id != expected.Id)
            {
                Assert.True(false,
                    string.Format("Expected diagnostic id to be \"{0}\" was \"{1}\"\r\n\r\nDiagnostic:\r\n    {2}\r\n",
                        expected.Id, actual.Id, FormatDiagnostics(analyzer, actual)));
            }

            if (actual.Severity != expected.Severity)
            {
                Assert.True(false,
                    string.Format("Expected diagnostic severity to be \"{0}\" was \"{1}\"\r\n\r\nDiagnostic:\r\n    {2}\r\n",
                        expected.Severity, actual.Severity, FormatDiagnostics(analyzer, actual)));
            }

            if (actual.GetMessage() != expected.Message)
            {
                Assert.True(false,
                    string.Format("Expected diagnostic message to be \"{0}\" was \"{1}\"\r\n\r\nDiagnostic:\r\n    {2}\r\n",
                        expected.Message, actual.GetMessage(), FormatDiagnostics(analyzer, actual)));
            }
        }


        /// <summary>
        /// Helper method to <see cref="VerifyDiagnosticResults"/> that checks the location of a
        /// <see cref="Diagnostic"/> and compares it with the location described by a
        /// <see cref="DiagnosticResultLocation"/>.
        /// </summary>
        /// <param name="analyzer">The analyzer that was being run on the sources.</param>
        /// <param name="diagnostic">The diagnostic that was found in the code.</param>
        /// <param name="actual">The location of the diagnostic found in the code.</param>
        /// <param name="expected">The <see cref="DiagnosticResultLocation"/> describing the expected location of the
        /// diagnostic.</param>
        private static void VerifyDiagnosticLocation(DiagnosticAnalyzer analyzer, Diagnostic diagnostic, Location actual, DiagnosticResultLocation expected)
        {
            var actualSpan = actual.GetLineSpan();

            Assert.True(actualSpan.Path == expected.Path || (actualSpan.Path != null && actualSpan.Path.Contains("Test0.") && expected.Path.Contains("Test.")),
                string.Format("Expected diagnostic to be in file \"{0}\" was actually in file \"{1}\"\r\n\r\nDiagnostic:\r\n    {2}\r\n",
                    expected.Path, actualSpan.Path, FormatDiagnostics(analyzer, diagnostic)));

            var actualLinePosition = actualSpan.StartLinePosition;

            // Only check line position if there is an actual line in the real diagnostic
            if (actualLinePosition.Line > 0)
            {
                if (actualLinePosition.Line + 1 != expected.Line)
                {
                    Assert.True(false,
                        string.Format("Expected diagnostic to be on line \"{0}\" was actually on line \"{1}\"\r\n\r\nDiagnostic:\r\n    {2}\r\n",
                            expected.Line, actualLinePosition.Line + 1, FormatDiagnostics(analyzer, diagnostic)));
                }
            }

            // Only check column position if there is an actual column position in the real diagnostic
            if (actualLinePosition.Character > 0)
            {
                if (actualLinePosition.Character + 1 != expected.Column)
                {
                    Assert.True(false,
                        string.Format("Expected diagnostic to start at column \"{0}\" was actually at column \"{1}\"\r\n\r\nDiagnostic:\r\n    {2}\r\n",
                            expected.Column, actualLinePosition.Character + 1, FormatDiagnostics(analyzer, diagnostic)));
                }
            }
        }
        #endregion

        #region Formatting Diagnostics
        /// <summary>
        /// Helper method to format a <see cref="Diagnostic"/> into an easily readable string.
        /// </summary>
        /// <param name="analyzer">The analyzer that this verifier tests.</param>
        /// <param name="diagnostics">A collection of <see cref="Diagnostic"/>s to be formatted.</param>
        /// <returns>The <paramref name="diagnostics"/> formatted as a string.</returns>
        private static string FormatDiagnostics(DiagnosticAnalyzer analyzer, params Diagnostic[] diagnostics)
        {
            var builder = new StringBuilder();
            for (int i = 0; i < diagnostics.Length; ++i)
            {
                builder.AppendLine("// " + diagnostics[i].ToString());

                var analyzerType = analyzer.GetType();
                var rules = analyzer.SupportedDiagnostics;

                foreach (var rule in rules)
                {
                    if (rule != null && rule.Id == diagnostics[i].Id)
                    {
                        var location = diagnostics[i].Location;
                        if (location == Location.None)
                        {
                            builder.AppendFormat("GetGlobalResult({0}.{1})", analyzerType.Name, rule.Id);
                        }
                        else
                        {
                            Assert.True(location.IsInSource,
                                string.Format("Test base does not currently handle diagnostics in metadata locations. Diagnostic in metadata:\r\n", diagnostics[i]));

                            string resultMethodName = diagnostics[i].Location.SourceTree.FilePath.EndsWith(".cs") ? "GetCSharpResultAt" : "GetBasicResultAt";
                            var linePosition = diagnostics[i].Location.GetLineSpan().StartLinePosition;

                            builder.AppendFormat("{0}({1}, {2}, {3}.{4})",
                                resultMethodName,
                                linePosition.Line + 1,
                                linePosition.Character + 1,
                                analyzerType.Name,
                                rule.Id);
                        }

                        if (i != diagnostics.Length - 1)
                        {
                            builder.Append(',');
                        }

                        builder.AppendLine();
                        break;
                    }
                }
            }
            return builder.ToString();
        }

        /// <summary>
        /// Helper method to format a <see cref="Diagnostic"/> into an easily readable string.
        /// </summary>
        /// <param name="analyzer">The analyzer that this verifier tests.</param>
        /// <param name="diagnostics">A collection of <see cref="Diagnostic"/>s to be formatted.</param>
        /// <returns>The <paramref name="diagnostics"/> formatted as a string.</returns>
        private static string FormatDiagnostics(DiagnosticAnalyzer analyzer, params DiagnosticResult[] diagnostics)
        {
            var builder = new StringBuilder();
            for (int i = 0; i < diagnostics.Length; ++i)
            {
                builder.AppendLine("// " + diagnostics[i].ToString());

                var analyzerType = analyzer.GetType();
                var rules = analyzer.SupportedDiagnostics;

                foreach (var rule in rules)
                {
                    if (rule != null && rule.Id == diagnostics[i].Id)
                    {
                        var location = diagnostics[i].Locations[0];

                        string resultMethodName = "GetCSharpResultAt";

                        builder.AppendFormat("{0}({1}, {2}, {3}.{4})",
                            resultMethodName,
                            location.Line,
                            location.Column,
                            analyzerType.Name,
                            rule.Id);

                        if (i != diagnostics.Length - 1)
                        {
                            builder.Append(',');
                        }

                        builder.AppendLine();
                        break;
                    }
                }
            }
            return builder.ToString();
        }
        #endregion
    }
}
