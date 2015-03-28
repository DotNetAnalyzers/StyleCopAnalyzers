namespace StyleCop.Analyzers.SpacingRules
{
    using System.Collections.Immutable;
    using System.Composition;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;

    /// <summary>
    /// Implements a code fix for <see cref="SA1016OpeningAttributeBracketsMustBeSpacedCorrectly"/>.
    /// </summary>
    /// <remarks>
    /// <para>To fix a violation of this rule, ensure that there is no whitespace after the opening attribute
    /// bracket.</para>
    /// </remarks>
    [ExportCodeFixProvider(nameof(SA1016CodeFixProvider), LanguageNames.CSharp)]
    [Shared]
    public class SA1016CodeFixProvider : CodeFixProvider
    {
        private static readonly ImmutableArray<string> FixableDiagnostics =
            ImmutableArray.Create(SA1016OpeningAttributeBracketsMustBeSpacedCorrectly.DiagnosticId);

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
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            foreach (var diagnostic in context.Diagnostics)
            {
                if (!diagnostic.Id.Equals(SA1016OpeningAttributeBracketsMustBeSpacedCorrectly.DiagnosticId))
                {
                    continue;
                }

                SyntaxToken token = root.FindToken(diagnostic.Location.SourceSpan.Start);
                if (!token.IsKind(SyntaxKind.OpenBracketToken))
                {
                    continue;
                }

                if (token.TrailingTrivia.Any(SyntaxKind.EndOfLineTrivia))
                {
                    continue;
                }

                if (!token.TrailingTrivia.Any(SyntaxKind.WhitespaceTrivia))
                {
                    continue;
                }

                context.RegisterCodeFix(CodeAction.Create("Fix spacing", t => GetTransformedDocument(context.Document, root, token)), diagnostic);
            }
        }

        private static Task<Document> GetTransformedDocument(Document document, SyntaxNode root, SyntaxToken token)
        {
            SyntaxToken corrected = token.WithoutTrailingWhitespace().WithoutFormatting();
            SyntaxNode transformed = root.ReplaceToken(token, corrected);
            Document updatedDocument = document.WithSyntaxRoot(transformed);

            return Task.FromResult(updatedDocument);
        }
    }
}
