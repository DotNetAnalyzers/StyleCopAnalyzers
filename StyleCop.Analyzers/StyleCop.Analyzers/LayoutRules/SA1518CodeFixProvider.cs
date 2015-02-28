namespace StyleCop.Analyzers.LayoutRules
{
    using System.Collections.Immutable;
    using System.Composition;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using StyleCop.Analyzers.Helpers;

    /// <summary>
    /// Implements a code fix for <see cref="SA1518CodeMustNotContainBlankLinesAtEndOfFile"/>.
    /// </summary>
    [ExportCodeFixProvider(nameof(SA1518CodeFixProvider), LanguageNames.CSharp)]
    [Shared]
    public class SA1518CodeFixProvider : CodeFixProvider
    {
        private static readonly ImmutableArray<string> FixableDiagnostics =
            ImmutableArray.Create(SA1518CodeMustNotContainBlankLinesAtEndOfFile.DiagnosticId);

        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds => FixableDiagnostics;

        /// <inheritdoc/>
        public override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        /// <inheritdoc/>
        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            foreach (Diagnostic diagnostic in context.Diagnostics.Where(d => FixableDiagnostics.Contains(d.Id)))
            {
                var syntaxRoot = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

                var lastToken = syntaxRoot.GetLastToken(includeZeroWidth: true);

                var newLastToken = this.StripViolatingWhitespace(lastToken);
                var newSyntaxRoot = syntaxRoot.ReplaceToken(lastToken, newLastToken);
                var newDocument = context.Document.WithSyntaxRoot(newSyntaxRoot);

                context.RegisterCodeFix(CodeAction.Create("Remove blank lines at the end of the file", token => Task.FromResult(newDocument)), diagnostic);
            }
        }

        private SyntaxToken StripViolatingWhitespace(SyntaxToken token)
        {
            SyntaxToken result = token;

            var trailingWhitespaceIndex = TriviaHelper.IndexOfTrailingWhitespace(token.LeadingTrivia);
            if (trailingWhitespaceIndex != -1)
            {
                var newTriviaList = SyntaxFactory.TriviaList(token.LeadingTrivia.Take(trailingWhitespaceIndex));
                result = token.WithLeadingTrivia(newTriviaList);
            }

            return result;
        }
    }
}
