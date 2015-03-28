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
    /// Implements a code fix for <see cref="SA1018NullableTypeSymbolsMustNotBePrecededBySpace"/>.
    /// </summary>
    /// <remarks>
    /// <para>To fix a violation of this rule, ensure that there is no whitespace before the nullable type
    /// symbol.</para>
    /// </remarks>
    [ExportCodeFixProvider(nameof(SA1018CodeFixProvider), LanguageNames.CSharp)]
    [Shared]
    public class SA1018CodeFixProvider : CodeFixProvider
    {
        private static readonly ImmutableArray<string> FixableDiagnostics =
            ImmutableArray.Create(SA1018NullableTypeSymbolsMustNotBePrecededBySpace.DiagnosticId);

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
                if (!diagnostic.Id.Equals(SA1018NullableTypeSymbolsMustNotBePrecededBySpace.DiagnosticId))
                    continue;

                SyntaxToken token = root.FindToken(diagnostic.Location.SourceSpan.Start);
                if (!token.IsKind(SyntaxKind.QuestionToken))
                    continue;

                bool firstInLine = token.HasLeadingTrivia || token.GetLocation()?.GetMappedLineSpan().StartLinePosition.Character == 0;
                if (firstInLine)
                    continue;

                SyntaxToken previousToken = token.GetPreviousToken();
                if (!previousToken.HasTrailingTrivia)
                    continue;

                context.RegisterCodeFix(CodeAction.Create("Remove space", t => GetTransformedDocument(context.Document, root, previousToken)), diagnostic);
            }
        }

        private static Task<Document> GetTransformedDocument(Document document, SyntaxNode root, SyntaxToken previousToken)
        {
            SyntaxToken corrected = previousToken.WithoutTrailingWhitespace().WithoutFormatting();
            Document updatedDocument = document.WithSyntaxRoot(root.ReplaceToken(previousToken, corrected));

            return Task.FromResult(updatedDocument);
        }
    }
}
