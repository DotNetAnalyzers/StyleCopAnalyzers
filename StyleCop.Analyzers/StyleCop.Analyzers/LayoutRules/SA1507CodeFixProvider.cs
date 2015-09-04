namespace StyleCop.Analyzers.LayoutRules
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Composition;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Helpers;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Formatting;
    using Microsoft.CodeAnalysis.Text;

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
            return FixAll.Instance;
        }

        /// <inheritdoc/>
        public override Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            foreach (Diagnostic diagnostic in context.Diagnostics.Where(d => FixableDiagnostics.Contains(d.Id)))
            {
                context.RegisterCodeFix(CodeAction.Create(LayoutResources.SA1507CodeFix, token => GetTransformedDocumentAsync(context.Document, diagnostic, token), equivalenceKey: nameof(SA1507CodeFixProvider)), diagnostic);
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

        private class FixAll : DocumentBasedFixAllProvider
        {
            public static FixAllProvider Instance { get; } =
                new FixAll();

            protected override string CodeActionTitle =>
                LayoutResources.SA1507CodeFix;

            protected override async Task<SyntaxNode> FixAllInDocumentAsync(FixAllContext fixAllContext, Document document)
            {
                var newLine = fixAllContext.Document.Project.Solution.Workspace.Options.GetOption(FormattingOptions.NewLine, LanguageNames.CSharp);
                var text = await document.GetTextAsync().ConfigureAwait(false);

                List<TextChange> changes = new List<TextChange>();

                foreach (var diagnostic in await fixAllContext.GetDocumentDiagnosticsAsync(document).ConfigureAwait(false))
                {
                    changes.Add(new TextChange(diagnostic.Location.SourceSpan, newLine));
                }

                changes.Sort((left, right) => left.Span.Start.CompareTo(right.Span.Start));

                var tree = await document.GetSyntaxTreeAsync().ConfigureAwait(false);
                return await tree.WithChangedText(text.WithChanges(changes)).GetRootAsync().ConfigureAwait(false);
            }
        }
    }
}
