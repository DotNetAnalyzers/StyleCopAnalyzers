// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace TestHelper
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.Formatting;
    using Microsoft.CodeAnalysis.Simplification;
    using Microsoft.CodeAnalysis.Text;

    /// <summary>
    /// Diagnostic Producer class with extra methods dealing with applying code fixes.
    /// All methods are static
    /// </summary>
    public abstract partial class CodeFixVerifier : DiagnosticVerifier
    {
        /// <summary>
        /// Apply the inputted <see cref="CodeAction"/> to the inputted document.
        /// Meant to be used to apply code fixes.
        /// </summary>
        /// <param name="project">The <see cref="Project"/> to apply the fix on</param>
        /// <param name="codeAction">A <see cref="CodeAction"/> that will be applied to the
        /// <paramref name="project"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that the task will observe.</param>
        /// <returns>A <see cref="Project"/> with the changes from the <see cref="CodeAction"/>.</returns>
        private static async Task<Project> ApplyFixAsync(Project project, CodeAction codeAction, CancellationToken cancellationToken)
        {
            var operations = await codeAction.GetOperationsAsync(cancellationToken).ConfigureAwait(false);
            var solution = operations.OfType<ApplyChangesOperation>().Single().ChangedSolution;
            return solution.GetProject(project.Id);
        }

        /// <summary>
        /// Compare two collections of <see cref="Diagnostic"/>s, and return a list of any new diagnostics that appear
        /// only in the second collection.
        /// <note type="note">
        /// <para>Considers <see cref="Diagnostic"/> to be the same if they have the same <see cref="Diagnostic.Id"/>s.
        /// In the case of multiple diagnostics with the same <see cref="Diagnostic.Id"/> in a row, this method may not
        /// necessarily return the new one.</para>
        /// </note>
        /// </summary>
        /// <param name="diagnostics">The <see cref="Diagnostic"/>s that existed in the code before the code fix was
        /// applied.</param>
        /// <param name="newDiagnostics">The <see cref="Diagnostic"/>s that exist in the code after the code fix was
        /// applied.</param>
        /// <returns>A list of <see cref="Diagnostic"/>s that only surfaced in the code after the code fix was
        /// applied.</returns>
        private static IEnumerable<Diagnostic> GetNewDiagnostics(IEnumerable<Diagnostic> diagnostics, IEnumerable<Diagnostic> newDiagnostics)
        {
            var oldArray = diagnostics.OrderBy(d => d.Location.SourceSpan.Start).ToArray();
            var newArray = newDiagnostics.OrderBy(d => d.Location.SourceSpan.Start).ToArray();

            int oldIndex = 0;
            int newIndex = 0;

            while (newIndex < newArray.Length)
            {
                if (oldIndex < oldArray.Length && oldArray[oldIndex].Id == newArray[newIndex].Id)
                {
                    ++oldIndex;
                    ++newIndex;
                }
                else
                {
                    yield return newArray[newIndex++];
                }
            }
        }

        /// <summary>
        /// Get the existing compiler diagnostics on the input document.
        /// </summary>
        /// <param name="project">The <see cref="Project"/> to run the compiler diagnostic analyzers on.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that the task will observe.</param>
        /// <returns>The compiler diagnostics that were found in the code.</returns>
        private static async Task<ImmutableArray<Diagnostic>> GetCompilerDiagnosticsAsync(Project project, CancellationToken cancellationToken)
        {
            var allDiagnostics = ImmutableArray.Create<Diagnostic>();

            foreach (var document in project.Documents)
            {
                var semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);
                allDiagnostics = allDiagnostics.AddRange(semanticModel.GetDiagnostics(cancellationToken: cancellationToken));
            }

            return allDiagnostics;
        }

        /// <summary>
        /// Given a document, turn it into a string based on the syntax root.
        /// </summary>
        /// <param name="document">The <see cref="Document"/> to be converted to a string.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that the task will observe.</param>
        /// <returns>A string containing the syntax of the <see cref="Document"/> after formatting.</returns>
        private static async Task<string> GetStringFromDocumentAsync(Document document, CancellationToken cancellationToken)
        {
            var simplifiedDoc = await Simplifier.ReduceAsync(document, Simplifier.Annotation, cancellationToken: cancellationToken).ConfigureAwait(false);
            var formatted = await Formatter.FormatAsync(simplifiedDoc, Formatter.Annotation, cancellationToken: cancellationToken).ConfigureAwait(false);
            var sourceText = await formatted.GetTextAsync(cancellationToken).ConfigureAwait(false);
            return sourceText.ToString();
        }

        /// <summary>
        /// Implements a workaround for issue #936, force re-parsing to get the same sort of syntax tree as the original document.
        /// </summary>
        /// <param name="project">The project to update.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The updated <see cref="Project"/>.</returns>
        private static async Task<Project> RecreateProjectDocumentsAsync(Project project, CancellationToken cancellationToken)
        {
            foreach (var documentId in project.DocumentIds)
            {
                var document = project.GetDocument(documentId);
                document = await RecreateDocumentAsync(document, cancellationToken).ConfigureAwait(false);
                project = document.Project;
            }

            return project;
        }

        private static async Task<Document> RecreateDocumentAsync(Document document, CancellationToken cancellationToken)
        {
            var newText = await document.GetTextAsync(cancellationToken).ConfigureAwait(false);
            newText = newText.WithChanges(new TextChange(new TextSpan(0, 0), " "));
            newText = newText.WithChanges(new TextChange(new TextSpan(0, 1), string.Empty));
            return document.WithText(newText);
        }

        /// <summary>
        /// Formats the whitespace in all documents of the specified <see cref="Project"/>.
        /// </summary>
        /// <param name="project">The project to update.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The updated <see cref="Project"/>.</returns>
        private static async Task<Project> ReformatProjectDocumentsAsync(Project project, CancellationToken cancellationToken)
        {
            foreach (var documentId in project.DocumentIds)
            {
                var document = project.GetDocument(documentId);
                document = await Formatter.FormatAsync(document, Formatter.Annotation, cancellationToken: cancellationToken).ConfigureAwait(false);
                project = document.Project;
            }

            return project;
        }
    }
}
