namespace StyleCop.Analyzers.SpacingRules
{
    using System.Collections.Immutable;
    using System.Composition;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using StyleCop.Analyzers.Helpers;

    /// <summary>
    /// Implements a code fix for <see cref="SA1017ClosingAttributeBracketsMustBeSpacedCorrectly"/>.
    /// </summary>
    /// <remarks>
    /// <para>To fix a violation of this rule, ensure that there is no whitespace before the closing attribute
    /// bracket.</para>
    /// </remarks>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1017CodeFixProvider))]
    [Shared]
    public class SA1017CodeFixProvider : CodeFixProvider
    {
        private static readonly ImmutableArray<string> FixableDiagnostics =
            ImmutableArray.Create(SA1017ClosingAttributeBracketsMustBeSpacedCorrectly.DiagnosticId);

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
            foreach (var diagnostic in context.Diagnostics)
            {
                if (!diagnostic.Id.Equals(SA1017ClosingAttributeBracketsMustBeSpacedCorrectly.DiagnosticId))
                {
                    continue;
                }

                context.RegisterCodeFix(CodeAction.Create(SpacingResources.SA1017CodeFix, cancellationToken => GetTransformedDocumentAsync(context.Document, diagnostic, cancellationToken), equivalenceKey: nameof(SA1017CodeFixProvider)), diagnostic);
            }

            return SpecializedTasks.CompletedTask;
        }

        private static async Task<Document> GetTransformedDocumentAsync(Document document, Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            SyntaxToken token = root.FindToken(diagnostic.Location.SourceSpan.Start);
            if (!token.IsKind(SyntaxKind.CloseBracketToken))
            {
                return document;
            }

            if (token.IsFirstInLine())
            {
                return document;
            }

            SyntaxToken precedingToken = token.GetPreviousToken();
            if (!precedingToken.TrailingTrivia.Any(SyntaxKind.WhitespaceTrivia))
            {
                return document;
            }

            SyntaxToken corrected = precedingToken.WithoutTrailingWhitespace().WithoutFormatting();
            SyntaxNode transformed = root.ReplaceToken(precedingToken, corrected);
            Document updatedDocument = document.WithSyntaxRoot(transformed);

            return updatedDocument;
        }
    }
}
