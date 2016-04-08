// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace TestHelper
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.Formatting;
    using StyleCop.Analyzers.Test.Helpers;
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
        protected abstract CodeFixProvider GetCSharpCodeFixProvider();

        /// <summary>
        /// Called to test a C# code fix when applied on the input source as a string.
        /// </summary>
        /// <param name="oldSource">A class in the form of a string before the code fix was applied to it.</param>
        /// <param name="newSource">A class in the form of a string after the code fix was applied to it.</param>
        /// <param name="batchNewSource">A class in the form of a string after the batch fixer was applied to it.</param>
        /// <param name="codeFixIndex">Index determining which code fix to apply if there are multiple.</param>
        /// <param name="allowNewCompilerDiagnostics">A value indicating whether or not the test will fail if the code fix introduces other warnings after being applied.</param>
        /// <param name="numberOfIncrementalIterations">The number of iterations the incremental fixer will be called.
        /// If this value is less than 0, the negated value is treated as an upper limit as opposed to an exact
        /// value.</param>
        /// <param name="numberOfFixAllIterations">The number of iterations the Fix All fixer will be called. If this
        /// value is less than 0, the negated value is treated as an upper limit as opposed to an exact value.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that the task will observe.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        protected async Task VerifyCSharpFixAsync(string oldSource, string newSource, string batchNewSource = null, int? codeFixIndex = null, bool allowNewCompilerDiagnostics = false, int numberOfIncrementalIterations = -int.MaxValue, int numberOfFixAllIterations = 1, CancellationToken cancellationToken = default(CancellationToken))
        {
            var t1 = this.VerifyFixInternalAsync(LanguageNames.CSharp, this.GetCSharpDiagnosticAnalyzers().ToImmutableArray(), this.GetCSharpCodeFixProvider(), oldSource, newSource, codeFixIndex, allowNewCompilerDiagnostics, numberOfIncrementalIterations, GetSingleAnalyzerDocumentAsync, cancellationToken).ConfigureAwait(false);

            var fixAllProvider = this.GetCSharpCodeFixProvider().GetFixAllProvider();
            Assert.NotEqual(WellKnownFixAllProviders.BatchFixer, fixAllProvider);

            if (fixAllProvider == null)
            {
                await t1;
            }
            else
            {
                if (Debugger.IsAttached)
                {
                    await t1;
                }

                var t2 = this.VerifyFixInternalAsync(LanguageNames.CSharp, this.GetCSharpDiagnosticAnalyzers().ToImmutableArray(), this.GetCSharpCodeFixProvider(), oldSource, batchNewSource ?? newSource, codeFixIndex, allowNewCompilerDiagnostics, numberOfFixAllIterations, GetFixAllAnalyzerDocumentAsync, cancellationToken).ConfigureAwait(false);
                if (Debugger.IsAttached)
                {
                    await t2;
                }

                var t3 = this.VerifyFixInternalAsync(LanguageNames.CSharp, this.GetCSharpDiagnosticAnalyzers().ToImmutableArray(), this.GetCSharpCodeFixProvider(), oldSource, batchNewSource ?? newSource, codeFixIndex, allowNewCompilerDiagnostics, numberOfFixAllIterations, GetFixAllAnalyzerProjectAsync, cancellationToken).ConfigureAwait(false);
                if (Debugger.IsAttached)
                {
                    await t3;
                }

                var t4 = this.VerifyFixInternalAsync(LanguageNames.CSharp, this.GetCSharpDiagnosticAnalyzers().ToImmutableArray(), this.GetCSharpCodeFixProvider(), oldSource, batchNewSource ?? newSource, codeFixIndex, allowNewCompilerDiagnostics, numberOfFixAllIterations, GetFixAllAnalyzerSolutionAsync, cancellationToken).ConfigureAwait(false);
                if (Debugger.IsAttached)
                {
                    await t4;
                }

                if (!Debugger.IsAttached)
                {
                    // Allow the operations to run in parallel
                    await t1;
                    await t2;
                    await t3;
                    await t4;
                }
            }
        }

        /// <summary>
        /// Called to test a C# fix all provider when applied on the input source as a string.
        /// </summary>
        /// <param name="oldSource">A class in the form of a string before the code fix was applied to it.</param>
        /// <param name="newSource">A class in the form of a string after the code fix was applied to it.</param>
        /// <param name="codeFixIndex">Index determining which code fix to apply if there are multiple.</param>
        /// <param name="allowNewCompilerDiagnostics">A value indicating whether or not the test will fail if the code fix introduces other warnings after being applied.</param>
        /// <param name="numberOfIterations">The number of iterations the fixer will be called. If this value is less
        /// than 0, the negated value is treated as an upper limit as opposed to an exact value.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that the task will observe.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        protected async Task VerifyCSharpFixAllFixAsync(string oldSource, string newSource, int? codeFixIndex = null, bool allowNewCompilerDiagnostics = false, int numberOfIterations = 1, CancellationToken cancellationToken = default(CancellationToken))
        {
            await this.VerifyFixInternalAsync(LanguageNames.CSharp, this.GetCSharpDiagnosticAnalyzers().ToImmutableArray(), this.GetCSharpCodeFixProvider(), oldSource, newSource, codeFixIndex, allowNewCompilerDiagnostics, numberOfIterations, GetFixAllAnalyzerDocumentAsync, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets all offered code fixes for the specified diagnostic within the given source.
        /// </summary>
        /// <param name="source">A valid C# source file in the form of a string.</param>
        /// <param name="diagnosticIndex">Index determining which diagnostic to use for determining the offered code fixes. Uses the first diagnostic if null.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that the task will observe.</param>
        /// <returns>The collection of offered code actions. This collection may be empty.</returns>
        protected async Task<ImmutableArray<CodeAction>> GetOfferedCSharpFixesAsync(string source, int? diagnosticIndex = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await this.GetOfferedFixesInternalAsync(LanguageNames.CSharp, source, diagnosticIndex, this.GetCSharpDiagnosticAnalyzers().ToImmutableArray(), this.GetCSharpCodeFixProvider(), cancellationToken).ConfigureAwait(false);
        }

        private static async Task<Document> GetSingleAnalyzerDocumentAsync(ImmutableArray<DiagnosticAnalyzer> analyzers, CodeFixProvider codeFixProvider, int? codeFixIndex, Document document, int numberOfIterations, CancellationToken cancellationToken)
        {
            int expectedNumberOfIterations = numberOfIterations;
            if (numberOfIterations < 0)
            {
                numberOfIterations = -numberOfIterations;
            }

            var previousDiagnostics = ImmutableArray.Create<Diagnostic>();

            bool done;
            do
            {
                var analyzerDiagnostics = await GetSortedDiagnosticsFromDocumentsAsync(analyzers, new[] { document }, cancellationToken).ConfigureAwait(false);
                if (analyzerDiagnostics.Length == 0)
                {
                    break;
                }

                if (!AreDiagnosticsDifferent(analyzerDiagnostics, previousDiagnostics))
                {
                    break;
                }

                if (--numberOfIterations < 0)
                {
                    Assert.True(false, "The upper limit for the number of code fix iterations was exceeded");
                }

                previousDiagnostics = analyzerDiagnostics;

                done = true;
                for (var i = 0; i < analyzerDiagnostics.Length; i++)
                {
                    if (!codeFixProvider.FixableDiagnosticIds.Contains(analyzerDiagnostics[i].Id))
                    {
                        // do not pass unsupported diagnostics to a code fix provider
                        continue;
                    }

                    var actions = new List<CodeAction>();
                    var context = new CodeFixContext(document, analyzerDiagnostics[i], (a, d) => actions.Add(a), cancellationToken);
                    await codeFixProvider.RegisterCodeFixesAsync(context).ConfigureAwait(false);

                    if (actions.Count > 0)
                    {
                        var fixedDocument = await ApplyFixAsync(document, actions.ElementAt(codeFixIndex.GetValueOrDefault(0)), cancellationToken).ConfigureAwait(false);
                        if (fixedDocument != document)
                        {
                            done = false;
                            var newText = await fixedDocument.GetTextAsync(cancellationToken).ConfigureAwait(false);

                            // workaround for issue #936 - force re-parsing to get the same sort of syntax tree as the original document.
                            document = document.WithText(newText);
                            break;
                        }
                    }
                }
            }
            while (!done);

            if (expectedNumberOfIterations >= 0)
            {
                Assert.Equal($"{expectedNumberOfIterations} iterations", $"{expectedNumberOfIterations - numberOfIterations} iterations");
            }

            return document;
        }

        private static Task<Document> GetFixAllAnalyzerDocumentAsync(ImmutableArray<DiagnosticAnalyzer> analyzers, CodeFixProvider codeFixProvider, int? codeFixIndex, Document document, int numberOfIterations, CancellationToken cancellationToken)
        {
            return GetFixAllAnalyzerAsync(FixAllScope.Document, analyzers, codeFixProvider, codeFixIndex, document, numberOfIterations, cancellationToken);
        }

        private static Task<Document> GetFixAllAnalyzerProjectAsync(ImmutableArray<DiagnosticAnalyzer> analyzers, CodeFixProvider codeFixProvider, int? codeFixIndex, Document document, int numberOfIterations, CancellationToken cancellationToken)
        {
            return GetFixAllAnalyzerAsync(FixAllScope.Project, analyzers, codeFixProvider, codeFixIndex, document, numberOfIterations, cancellationToken);
        }

        private static Task<Document> GetFixAllAnalyzerSolutionAsync(ImmutableArray<DiagnosticAnalyzer> analyzers, CodeFixProvider codeFixProvider, int? codeFixIndex, Document document, int numberOfIterations, CancellationToken cancellationToken)
        {
            return GetFixAllAnalyzerAsync(FixAllScope.Solution, analyzers, codeFixProvider, codeFixIndex, document, numberOfIterations, cancellationToken);
        }

        private static async Task<Document> GetFixAllAnalyzerAsync(FixAllScope scope, ImmutableArray<DiagnosticAnalyzer> analyzers, CodeFixProvider codeFixProvider, int? codeFixIndex, Document document, int numberOfIterations, CancellationToken cancellationToken)
        {
            int expectedNumberOfIterations = numberOfIterations;
            if (numberOfIterations < 0)
            {
                numberOfIterations = -numberOfIterations;
            }

            var previousDiagnostics = ImmutableArray.Create<Diagnostic>();

            var fixAllProvider = codeFixProvider.GetFixAllProvider();

            if (fixAllProvider == null)
            {
                return null;
            }

            bool done;
            do
            {
                var analyzerDiagnostics = await GetSortedDiagnosticsFromDocumentsAsync(analyzers, new[] { document }, cancellationToken).ConfigureAwait(false);
                if (analyzerDiagnostics.Length == 0)
                {
                    break;
                }

                if (!AreDiagnosticsDifferent(analyzerDiagnostics, previousDiagnostics))
                {
                    break;
                }

                if (--numberOfIterations < 0)
                {
                    Assert.True(false, "The upper limit for the number of fix all iterations was exceeded");
                }

                string equivalenceKey = null;
                foreach (var diagnostic in analyzerDiagnostics)
                {
                    if (!codeFixProvider.FixableDiagnosticIds.Contains(diagnostic.Id))
                    {
                        // do not pass unsupported diagnostics to a code fix provider
                        continue;
                    }

                    var actions = new List<CodeAction>();
                    var context = new CodeFixContext(document, diagnostic, (a, d) => actions.Add(a), cancellationToken);
                    await codeFixProvider.RegisterCodeFixesAsync(context).ConfigureAwait(false);
                    if (actions.Count > (codeFixIndex ?? 0))
                    {
                        equivalenceKey = actions[codeFixIndex ?? 0].EquivalenceKey;
                        break;
                    }
                }

                previousDiagnostics = analyzerDiagnostics;

                done = true;

                FixAllContext.DiagnosticProvider fixAllDiagnosticProvider = TestDiagnosticProvider.Create(analyzerDiagnostics);

                IEnumerable<string> analyzerDiagnosticIds = analyzers.SelectMany(x => x.SupportedDiagnostics).Select(x => x.Id);
                IEnumerable<string> compilerDiagnosticIds = codeFixProvider.FixableDiagnosticIds.Where(x => x.StartsWith("CS", StringComparison.Ordinal));
                IEnumerable<string> disabledDiagnosticIds = document.Project.CompilationOptions.SpecificDiagnosticOptions.Where(x => x.Value == ReportDiagnostic.Suppress).Select(x => x.Key);
                IEnumerable<string> relevantIds = analyzerDiagnosticIds.Concat(compilerDiagnosticIds).Except(disabledDiagnosticIds).Distinct();
                FixAllContext fixAllContext = new FixAllContext(document, codeFixProvider, scope, equivalenceKey, relevantIds, fixAllDiagnosticProvider, cancellationToken);

                CodeAction action = await fixAllProvider.GetFixAsync(fixAllContext).ConfigureAwait(false);
                if (action == null)
                {
                    return document;
                }

                var fixedDocument = await ApplyFixAsync(document, action, cancellationToken).ConfigureAwait(false);
                if (fixedDocument != document)
                {
                    done = false;
                    var newText = await fixedDocument.GetTextAsync(cancellationToken).ConfigureAwait(false);

                    // workaround for issue #936 - force re-parsing to get the same sort of syntax tree as the original document.
                    document = document.WithText(newText);
                }
            }
            while (!done);

            if (expectedNumberOfIterations >= 0)
            {
                Assert.Equal($"{expectedNumberOfIterations} iterations", $"{expectedNumberOfIterations - numberOfIterations} iterations");
            }

            return document;
        }

        private static bool AreDiagnosticsDifferent(ImmutableArray<Diagnostic> analyzerDiagnostics, ImmutableArray<Diagnostic> previousDiagnostics)
        {
            if (analyzerDiagnostics.Length != previousDiagnostics.Length)
            {
                return true;
            }

            for (var i = 0; i < analyzerDiagnostics.Length; i++)
            {
                if ((analyzerDiagnostics[i].Id != previousDiagnostics[i].Id)
                    || (analyzerDiagnostics[i].Location.SourceSpan != previousDiagnostics[i].Location.SourceSpan))
                {
                    return true;
                }
            }

            return false;
        }

        private async Task VerifyFixInternalAsync(
            string language,
            ImmutableArray<DiagnosticAnalyzer> analyzers,
            CodeFixProvider codeFixProvider,
            string oldSource,
            string newSource,
            int? codeFixIndex,
            bool allowNewCompilerDiagnostics,
            int numberOfIterations,
            Func<ImmutableArray<DiagnosticAnalyzer>, CodeFixProvider, int?, Document, int, CancellationToken, Task<Document>> getFixedDocument,
            CancellationToken cancellationToken)
        {
            var document = this.CreateDocument(oldSource, language);
            var compilerDiagnostics = await GetCompilerDiagnosticsAsync(document, cancellationToken).ConfigureAwait(false);

            document = await getFixedDocument(analyzers, codeFixProvider, codeFixIndex, document, numberOfIterations, cancellationToken).ConfigureAwait(false);

            var newCompilerDiagnostics = GetNewDiagnostics(compilerDiagnostics, await GetCompilerDiagnosticsAsync(document, cancellationToken).ConfigureAwait(false));

            // check if applying the code fix introduced any new compiler diagnostics
            if (!allowNewCompilerDiagnostics && newCompilerDiagnostics.Any())
            {
                // Format and get the compiler diagnostics again so that the locations make sense in the output
                document = await Formatter.FormatAsync(document, Formatter.Annotation, cancellationToken: cancellationToken).ConfigureAwait(false);
                newCompilerDiagnostics = GetNewDiagnostics(compilerDiagnostics, await GetCompilerDiagnosticsAsync(document, cancellationToken).ConfigureAwait(false));

                string message =
                    string.Format(
                        "Fix introduced new compiler diagnostics:\r\n{0}\r\n\r\nNew document:\r\n{1}\r\n",
                        string.Join("\r\n", newCompilerDiagnostics.Select(d => d.ToString())),
                        (await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false)).ToFullString());
                Assert.True(false, message);
            }

            // after applying all of the code fixes, compare the resulting string to the inputted one
            var actual = await GetStringFromDocumentAsync(document, cancellationToken).ConfigureAwait(false);
            Assert.Equal(newSource, actual);
        }

        private async Task<ImmutableArray<CodeAction>> GetOfferedFixesInternalAsync(string language, string source, int? diagnosticIndex, ImmutableArray<DiagnosticAnalyzer> analyzers, CodeFixProvider codeFixProvider, CancellationToken cancellationToken)
        {
            var document = this.CreateDocument(source, language);
            var analyzerDiagnostics = await GetSortedDiagnosticsFromDocumentsAsync(analyzers, new[] { document }, cancellationToken).ConfigureAwait(false);

            var index = diagnosticIndex.HasValue ? diagnosticIndex.Value : 0;

            Assert.True(index < analyzerDiagnostics.Count());

            var actions = new List<CodeAction>();

            // do not pass unsupported diagnostics to a code fix provider
            if (codeFixProvider.FixableDiagnosticIds.Contains(analyzerDiagnostics[index].Id))
            {
                var context = new CodeFixContext(document, analyzerDiagnostics[index], (a, d) => actions.Add(a), cancellationToken);
                await codeFixProvider.RegisterCodeFixesAsync(context).ConfigureAwait(false);
            }

            return actions.ToImmutableArray();
        }
    }
}
