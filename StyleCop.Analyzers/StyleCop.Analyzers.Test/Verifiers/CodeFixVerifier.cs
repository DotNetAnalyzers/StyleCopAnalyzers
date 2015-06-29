namespace TestHelper
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.Formatting;
    using Microsoft.CodeAnalysis.Text;
    using Xunit;

    /// <summary>
    /// Superclass of all unit tests made for diagnostics with code fixes.
    /// Contains methods used to verify correctness of code fixes.
    /// </summary>
    public abstract partial class CodeFixVerifier : DiagnosticVerifier
    {
        /// <summary>
        /// Returns the code fix being tested (C#) - to be implemented in non-abstract class.
        /// </summary>
        /// <returns>The <see cref="CodeFixProvider"/> to be used for C# code.</returns>
        protected virtual CodeFixProvider GetCSharpCodeFixProvider()
        {
            return null;
        }

        /// <summary>
        /// Called to test a C# code fix when applied on the input source as a string.
        /// </summary>
        /// <param name="oldSource">A class in the form of a string before the code fix was applied to it.</param>
        /// <param name="newSource">A class in the form of a string after the code fix was applied to it.</param>
        /// <param name="codeFixIndex">Index determining which code fix to apply if there are multiple.</param>
        /// <param name="allowNewCompilerDiagnostics">A value indicating whether or not the test will fail if the code fix introduces other warnings after being applied.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that the task will observe.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        protected Task VerifyCSharpFixAsync(string oldSource, string newSource, int? codeFixIndex = null, bool allowNewCompilerDiagnostics = false, CancellationToken cancellationToken = default(CancellationToken))
        {
            return this.VerifyFixAsync(LanguageNames.CSharp, this.GetCSharpDiagnosticAnalyzers().ToImmutableArray(), this.GetCSharpCodeFixProvider(), oldSource, newSource, codeFixIndex, allowNewCompilerDiagnostics, cancellationToken);
        }

        /// <summary>
        /// General verifier for code fixes.
        /// Creates a <see cref="Document"/> from the source string, then gets <see cref="Diagnostic"/>s on it and
        /// applies the relevant code fixes. Then gets the string after the code fix is applied and compares it with the
        /// expected result.
        /// <note type="note">
        /// <para>If any code fix causes new diagnostics to show up, the test fails unless
        /// <paramref name="allowNewCompilerDiagnostics"/> is set to <see langword="true"/>.</para>
        /// </note>
        /// </summary>
        /// <param name="language">The language the source classes are in. Values may be taken from the
        /// <see cref="LanguageNames"/> class.</param>
        /// <param name="analyzers">The analyzer to be applied to the source code.</param>
        /// <param name="codeFixProvider">The code fix to be applied to the code wherever the relevant
        /// <see cref="Diagnostic"/> is found.</param>
        /// <param name="oldSource">A class in the form of a string before the code fix was applied to it.</param>
        /// <param name="newSource">A class in the form of a string after the code fix was applied to it.</param>
        /// <param name="codeFixIndex">Index determining which code fix to apply if there are multiple.</param>
        /// <param name="allowNewCompilerDiagnostics">A value indicating whether or not the test will fail if the code
        /// fix introduces other warnings after being applied.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that the task will observe.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        private async Task VerifyFixAsync(string language, ImmutableArray<DiagnosticAnalyzer> analyzers, CodeFixProvider codeFixProvider, string oldSource, string newSource, int? codeFixIndex, bool allowNewCompilerDiagnostics, CancellationToken cancellationToken)
        {
            var document = CreateDocument(oldSource, language);
            var compilerDiagnostics = await GetCompilerDiagnosticsAsync(document, cancellationToken).ConfigureAwait(false);

            var previousDiagnostics = ImmutableArray.Create<Diagnostic>();

            bool done;
            do
            {
                var analyzerDiagnostics = await GetSortedDiagnosticsFromDocumentsAsync(analyzers, new[] { document }, cancellationToken).ConfigureAwait(false);
                if (analyzerDiagnostics.Length == 0)
                {
                    break;
                }

                Assert.True(AreDiagnosticsDifferent(analyzerDiagnostics, previousDiagnostics), "Unmodified diagnostics detected!");
                previousDiagnostics = analyzerDiagnostics;

                done = true;
                for (var i = 0; i < analyzerDiagnostics.Length; i++)
                {
                    var actions = new List<CodeAction>();
                    var context = new CodeFixContext(document, analyzerDiagnostics[i], (a, d) => actions.Add(a), cancellationToken);
                    await codeFixProvider.RegisterCodeFixesAsync(context).ConfigureAwait(false);

                    if (actions.Count > 0)
                    {
                        SourceText oldText = await document.GetTextAsync(cancellationToken).ConfigureAwait(false);
                        document = await ApplyFixAsync(document, actions.ElementAt(codeFixIndex.GetValueOrDefault(0)), cancellationToken).ConfigureAwait(false);
                        done = oldText.ContentEquals(await document.GetTextAsync(cancellationToken).ConfigureAwait(false));
                        break;
                    }
                }
            }
            while (!done);

            var newCompilerDiagnostics = GetNewDiagnostics(compilerDiagnostics, await GetCompilerDiagnosticsAsync(document, cancellationToken).ConfigureAwait(false));

            // check if applying the code fix introduced any new compiler diagnostics
            if (!allowNewCompilerDiagnostics && newCompilerDiagnostics.Any())
            {
                // Format and get the compiler diagnostics again so that the locations make sense in the output
                document = await Formatter.FormatAsync(document, Formatter.Annotation, cancellationToken: cancellationToken).ConfigureAwait(false);
                newCompilerDiagnostics = GetNewDiagnostics(compilerDiagnostics, await GetCompilerDiagnosticsAsync(document, cancellationToken).ConfigureAwait(false));

                string message =
                    string.Format("Fix introduced new compiler diagnostics:\r\n{0}\r\n\r\nNew document:\r\n{1}\r\n",
                        string.Join("\r\n", newCompilerDiagnostics.Select(d => d.ToString())),
                        (await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false)).ToFullString());
                Assert.True(false, message);
            }

            // after applying all of the code fixes, compare the resulting string to the inputted one
            var actual = await GetStringFromDocumentAsync(document, cancellationToken).ConfigureAwait(false);
            Assert.Equal(newSource, actual);
        }

        private static bool AreDiagnosticsDifferent(ImmutableArray<Diagnostic> analyzerDiagnostics, ImmutableArray<Diagnostic> previousDiagnostics)
        {
            if (analyzerDiagnostics.Length != previousDiagnostics.Length)
            {
                return true;
            }

            for (var i = 0; i < analyzerDiagnostics.Length; i++)
            {
                if (analyzerDiagnostics[i].Id != previousDiagnostics[i].Id)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
