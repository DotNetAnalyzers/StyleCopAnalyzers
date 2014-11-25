using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.Simplification;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace TestHelper
{
    /// <summary>
    /// Diagnostic Producer class with extra methods dealing with applying codefixes
    /// All methods are static
    /// </summary>
    public abstract partial class CodeFixVerifier : DiagnosticVerifier
    {
        /// <summary>
        /// Apply the inputted <see cref="CodeAction"/> to the inputted document.
        /// Meant to be used to apply codefixes.
        /// </summary>
        /// <param name="document">The <see cref="Document"/> to apply the fix on</param>
        /// <param name="codeAction">A <see cref="CodeAction"/> that will be applied to the
        /// <paramref name="document"/>.</param>
        /// <returns>A <see cref="Document"/> with the changes from the <see cref="CodeAction"/>.</returns>
        private static Document ApplyFix(Document document, CodeAction codeAction)
        {
            var operations = codeAction.GetOperationsAsync(CancellationToken.None).Result;
            var solution = operations.OfType<ApplyChangesOperation>().Single().ChangedSolution;
            return solution.GetDocument(document.Id);
        }

        /// <summary>
        /// Compare two collections of <see cref="Diagnostic"/>s, and return a list of any new diagnostics that appear
        /// only in the second collection.
        /// <note type="note">
        /// <para>Considers <see cref="Diagnostic"/> to be the same if they have the same <see cref="Diagnostic.Id"/>s.
        /// In the case of mulitple diagnostics with the same <see cref="Diagnostic.Id"/> in a row, this method may not
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
        /// <param name="document">The <see cref="Document"/> to run the compiler diagnostic analyzers on.</param>
        /// <returns>The compiler diagnostics that were found in the code.</returns>
        private static IEnumerable<Diagnostic> GetCompilerDiagnostics(Document document)
        {
            return document.GetSemanticModelAsync().Result.GetDiagnostics();
        }

        /// <summary>
        /// Given a document, turn it into a string based on the syntax root.
        /// </summary>
        /// <param name="document">The <see cref="Document"/> to be converted to a string.</param>
        /// <returns>A string contianing the syntax of the <see cref="Document"/> after formatting.</returns>
        private static string GetStringFromDocument(Document document)
        {
            var simplifiedDoc = Simplifier.ReduceAsync(document, Simplifier.Annotation).Result;
            var root = simplifiedDoc.GetSyntaxRootAsync().Result;
            root = Formatter.Format(root, Formatter.Annotation, simplifiedDoc.Project.Solution.Workspace);
            return root.GetText().ToString();
        }
    }
}
