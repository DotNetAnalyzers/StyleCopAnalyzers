namespace StyleCop.Analyzers.SpacingRules
{
    using System.Collections.Immutable;
    using System.Composition;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using StyleCop.Analyzers.Helpers;

    /// <summary>
    /// Implements a code fix for <see cref="SA1027TabsMustNotBeUsed"/>.
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1027CodeFixProvider))]
    [Shared]
    public class SA1027CodeFixProvider : CodeFixProvider
    {
        private static readonly ImmutableArray<string> FixableDiagnostics =
            ImmutableArray.Create(SA1027TabsMustNotBeUsed.DiagnosticId);

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
                context.RegisterCodeFix(CodeAction.Create("Replace tabs with spaces", token => GetTransformedDocumentAsync(context.Document, diagnostic, token)), diagnostic);
            }

            return Task.FromResult(true);
        }

        private static async Task<Document> GetTransformedDocumentAsync(Document document, Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            var indentationOptions = IndentationOptions.FromDocument(document);
            var syntaxRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            var violatingTrivia = syntaxRoot.FindTrivia(diagnostic.Location.SourceSpan.Start);

            var stringBuilder = new StringBuilder();

            var column = violatingTrivia.GetLocation().GetLineSpan().StartLinePosition.Character;
            foreach (var c in violatingTrivia.ToFullString())
            {
                if (c == '\t')
                {
                    var offsetWithinTabColumn = column % indentationOptions.TabSize;
                    var spaceCount = indentationOptions.TabSize - offsetWithinTabColumn;

                    stringBuilder.Append(' ', spaceCount);
                    column += spaceCount;
                }
                else
                {
                    stringBuilder.Append(c);
                    column++;
                }
            }

            var newSyntaxRoot = syntaxRoot.ReplaceTrivia(violatingTrivia, SyntaxFactory.Whitespace(stringBuilder.ToString(), elastic: false));
            return document.WithSyntaxRoot(newSyntaxRoot);
        }
    }
}
