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
    /// Implements a code fix for <see cref="SA1006PreprocessorKeywordsMustNotBePrecededBySpace"/>.
    /// </summary>
    /// <remarks>
    /// <para>To fix a violation of this rule, ensure that there is no whitespace between the opening hash mark and the
    /// preprocessor-type keyword.</para>
    /// </remarks>
    [ExportCodeFixProvider(nameof(SA1006CodeFixProvider), LanguageNames.CSharp)]
    [Shared]
    public class SA1006CodeFixProvider : CodeFixProvider
    {
        private static readonly ImmutableArray<string> FixableDiagnostics =
            ImmutableArray.Create(SA1006PreprocessorKeywordsMustNotBePrecededBySpace.DiagnosticId);

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
                if (!diagnostic.Id.Equals(SA1006PreprocessorKeywordsMustNotBePrecededBySpace.DiagnosticId))
                    continue;

                SyntaxToken keywordToken = root.FindToken(diagnostic.Location.SourceSpan.Start, findInsideTrivia: true);
                if (keywordToken.IsMissing)
                    continue;

                SyntaxToken hashToken = keywordToken.GetPreviousToken(includeDirectives: true);
                if (!hashToken.IsKind(SyntaxKind.HashToken))
                    continue;

                context.RegisterCodeFix(CodeAction.Create("Remove space", t => GetTransformedDocument(context.Document, root, hashToken)), diagnostic);
            }
        }

        private static Task<Document> GetTransformedDocument(Document document, SyntaxNode root, SyntaxToken hashToken)
        {
            SyntaxToken corrected = hashToken.WithoutTrailingWhitespace().WithoutFormatting();
            Document updatedDocument = document.WithSyntaxRoot(root.ReplaceToken(hashToken, corrected));
            return Task.FromResult(updatedDocument);
        }
    }
}
