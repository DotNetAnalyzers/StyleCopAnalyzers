namespace StyleCop.Analyzers.LayoutRules
{
    using System.Collections.Immutable;
    using System.Composition;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Text;
    using StyleCop.Analyzers.Helpers;

    /// <summary>
    /// Implements a code fix for <see cref="SA1507CodeMustNotContainMultipleBlankLinesInARow"/>.
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1507CodeFixProvider))]
    [Shared]
    public class SA1507CodeFixProvider : CodeFixProvider
    {
        private static readonly ImmutableArray<string> FixableDiagnostics =
            ImmutableArray.Create(SA1507CodeMustNotContainMultipleBlankLinesInARow.DiagnosticId);

        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds => FixableDiagnostics;

        /// <inheritdoc/>
        public override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        /// <inheritdoc/>
        public override Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            foreach (Diagnostic diagnostic in context.Diagnostics.Where(d => FixableDiagnostics.Contains(d.Id)))
            {
                context.RegisterCodeFix(CodeAction.Create(LayoutResources.SA1507CodeFix, token => GetTransformedDocumentAsync(context.Document, diagnostic, token)), diagnostic);
            }

            return SpecializedTasks.CompletedTask;
        }

        private static async Task<Document> GetTransformedDocumentAsync(Document document, Diagnostic diagnostic, CancellationToken token)
        {
            var sourceText = await document.GetTextAsync(token).ConfigureAwait(false);

            var startIndex = sourceText.Lines.IndexOf(diagnostic.Location.SourceSpan.Start);
            int endIndex = startIndex;

            for (var i = startIndex + 1; i < sourceText.Lines.Count; i++)
            {
                if (!string.IsNullOrWhiteSpace(sourceText.Lines[i].ToString()))
                {
                    endIndex = i - 1;
                    break;
                }
            }

            if (endIndex >= (startIndex + 1))
            {
                var replaceSpan = TextSpan.FromBounds(sourceText.Lines[startIndex + 1].SpanIncludingLineBreak.Start, sourceText.Lines[endIndex].SpanIncludingLineBreak.End);
                var newSourceText = sourceText.Replace(replaceSpan, string.Empty);
                return document.WithText(newSourceText);
            }

            return document;
        }
    }
}
