namespace StyleCop.Analyzers.SpacingRules
{
    using System;
    using System.Collections.Immutable;
    using System.Composition;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;

    /// <summary>
    /// Implements a code fix for <see cref="SA1028NoTrailingWhitespace"/>.
    /// </summary>
    /// <remarks>
    /// <para>To fix a violation of this rule, remove any whitespace at the end of a line of code.</para>
    /// </remarks>
    [ExportCodeFixProvider("WhitespaceDiagnosticCodeFixProvider", LanguageNames.CSharp), Shared]
    public class SA1028CodeFixProvider : CodeFixProvider
    {
        private static readonly ImmutableArray<string> FixableDiagnostics =
            ImmutableArray.Create(SA1028NoTrailingWhitespace.DiagnosticId);

        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds => FixableDiagnostics;

        /// <inheritdoc/>
        public sealed override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        /// <inheritdoc/>
        public override Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            foreach (var diagnostic in context.Diagnostics)
            {
                context.RegisterCodeFix(
                    CodeAction.Create(
                        "Remove trailing whitespace",
                        ct => RemoveWhitespaceAsync(context.Document, diagnostic, ct)),
                    diagnostic);
            }

            return AnalyzerConstants.CompletedTask;
        }

        /// <summary>
        /// Removes trailing whitespace.
        /// </summary>
        /// <param name="document">The document to be changed.</param>
        /// <param name="diagnostic">The diagnostic to fix.</param>
        /// <param name="cancellationToken">The cancellation token associated with the fix action.</param>
        /// <returns>The transformed document.</returns>
        private static async Task<Document> RemoveWhitespaceAsync(Document document, Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken);
            var diagnosticSpan = diagnostic.Location.SourceSpan;
            var trivia = root.FindTrivia(diagnosticSpan.Start, findInsideTrivia: true);
            var newRoot = root.ReplaceTrivia(trivia, SyntaxTriviaList.Empty);

            var newDocument = document.WithSyntaxRoot(newRoot);
            return newDocument;
        }
    }
}
