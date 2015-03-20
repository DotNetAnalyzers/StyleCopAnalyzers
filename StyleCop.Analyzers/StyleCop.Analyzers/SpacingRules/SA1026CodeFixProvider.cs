namespace StyleCop.Analyzers.SpacingRules
{
    using System.Collections.Immutable;
    using System.Composition;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;

    /// <summary>
    /// Implements a code fix for <see cref="SA1026CodeMustNotContainSpaceAfterNewKeywordInImplicitlyTypedArrayAllocation"/>.
    /// </summary>
    /// <remarks>
    /// <para>To fix a violation of this rule, remove any whitespace between the new keyword and the opening array
    /// bracket.</para>
    /// </remarks>
    [ExportCodeFixProvider(nameof(SA1026CodeFixProvider), LanguageNames.CSharp)]
    [Shared]
    public class SA1026CodeFixProvider : CodeFixProvider
    {
        private static readonly ImmutableArray<string> FixableDiagnostics =
            ImmutableArray.Create(SA1026CodeMustNotContainSpaceAfterNewKeywordInImplicitlyTypedArrayAllocation.DiagnosticId);

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
                if (!diagnostic.Id.Equals(SA1026CodeMustNotContainSpaceAfterNewKeywordInImplicitlyTypedArrayAllocation.DiagnosticId))
                    continue;

                SyntaxToken token = root.FindToken(diagnostic.Location.SourceSpan.Start);
                if (token.IsMissing)
                    continue;

                context.RegisterCodeFix(CodeAction.Create("Remove space", t => GetTransformedDocumentAsync(context.Document, root, token)), diagnostic);
            }
        }

        private static Task<Document> GetTransformedDocumentAsync(Document document, SyntaxNode root, SyntaxToken token)
        {
            SyntaxToken corrected = token.WithoutTrailingWhitespace().WithoutFormatting();
            Document updatedDocument = document.WithSyntaxRoot(root.ReplaceToken(token, corrected));

            return Task.FromResult(updatedDocument);
        }
    }
}
