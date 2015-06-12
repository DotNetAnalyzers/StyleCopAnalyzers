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
    using Microsoft.CodeAnalysis.CSharp;

    /// <summary>
    /// Implements a code fix for <see cref="SA1514ElementDocumentationHeaderMustBePrecededByBlankLine"/>.
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1514CodeFixProvider))]
    [Shared]
    public class SA1514CodeFixProvider : CodeFixProvider
    {
        private static readonly ImmutableArray<string> FixableDiagnostics =
            ImmutableArray.Create(SA1514ElementDocumentationHeaderMustBePrecededByBlankLine.DiagnosticId);

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
                context.RegisterCodeFix(CodeAction.Create(LayoutResources.SA1514CodeFix, token => GetTransformedDocumentAsync(context.Document, diagnostic, token)), diagnostic);
            }

            return Task.FromResult(true);
        }

        private static async Task<Document> GetTransformedDocumentAsync(Document document, Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            var syntaxRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            var documentationHeader = syntaxRoot.FindTrivia(diagnostic.Location.SourceSpan.Start);
            var triviaList = documentationHeader.Token.LeadingTrivia;
            var documentationHeaderIndex = triviaList.IndexOf(documentationHeader);

            // Keep any leading whitespace with the documentation header
            var index = documentationHeaderIndex - 1;
            while ((index >= 0) && triviaList[index].IsKind(SyntaxKind.WhitespaceTrivia))
            {
                index--;
            }

            var newLeadingTrivia = documentationHeader.Token.LeadingTrivia.Insert(index + 1, SyntaxFactory.CarriageReturnLineFeed);
            var newSyntaxRoot = syntaxRoot.ReplaceToken(documentationHeader.Token, documentationHeader.Token.WithLeadingTrivia(newLeadingTrivia));
            return document.WithSyntaxRoot(newSyntaxRoot);
        }
    }
}
