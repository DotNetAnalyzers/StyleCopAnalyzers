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
    using Microsoft.CodeAnalysis.Text;
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
            return CustomFixAllProviders.BatchFixer;
        }

        /// <inheritdoc/>
        public override Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            foreach (Diagnostic diagnostic in context.Diagnostics.Where(d => FixableDiagnostics.Contains(d.Id)))
            {
                context.RegisterCodeFix(CodeAction.Create(SpacingResources.SA1027CodeFix, token => GetTransformedDocumentAsync(context.Document, diagnostic, token), equivalenceKey: nameof(SA1027CodeFixProvider)), diagnostic);
            }

            return SpecializedTasks.CompletedTask;
        }

        private static async Task<Document> GetTransformedDocumentAsync(Document document, Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            var indentationOptions = IndentationOptions.FromDocument(document);
            var syntaxRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            // Find trivia at specific location of diagnostic. We do not need the token whose full span
            // includes the trivia, e.g. when looking for DocummentationCommentExteriorTrivia which can be
            // part of SingleLineDocumentationTrivia. This would lead to incorrect replacements of tabs here.
            // So we are looking for tokens that are part of the trivia.
            var violatingTrivia = syntaxRoot.FindTrivia(diagnostic.Location.SourceSpan.Start, true);

            var stringBuilder = new StringBuilder();

            int firstTriviaIndex = violatingTrivia.GetLineSpan().StartLinePosition.Character;
            string relevantText;
            if (firstTriviaIndex == 0)
            {
                relevantText = violatingTrivia.ToFullString();
            }
            else
            {
                SourceText sourceText = await document.GetTextAsync(cancellationToken).ConfigureAwait(false);
                relevantText = sourceText.ToString(new TextSpan(violatingTrivia.FullSpan.Start - firstTriviaIndex, firstTriviaIndex + violatingTrivia.FullSpan.Length));
            }

            // The column is used to count characters before a tab occurs. The tab will then be replaced by that number
            // of whitespaces, that results from the tab size reduced by the number of preceding characters (column).
            // So it will be ensured, that tab indention is always of consistent length.
            int column = 0;

            for (int i = 0; i < relevantText.Length; i++)
            {
                char c = relevantText[i];
                if (c == '\t')
                {
                    var offsetWithinTabColumn = column % indentationOptions.TabSize;
                    var spaceCount = indentationOptions.TabSize - offsetWithinTabColumn;

                    if (i >= firstTriviaIndex)
                    {
                        stringBuilder.Append(' ', spaceCount);
                    }

                    column += spaceCount;
                }
                else
                {
                    if (i >= firstTriviaIndex)
                    {
                        stringBuilder.Append(c);
                    }

                    column++;
                }
            }

            var newSyntaxRoot = syntaxRoot.ReplaceTrivia(violatingTrivia, SyntaxFactory.Whitespace(stringBuilder.ToString()));
            return document.WithSyntaxRoot(newSyntaxRoot);
        }
    }
}
