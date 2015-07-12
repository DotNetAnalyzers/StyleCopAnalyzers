namespace StyleCop.Analyzers.SpacingRules
{
    using System.Collections.Generic;
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
    /// Implements a code fix for <see cref="SA1010OpeningSquareBracketsMustBeSpacedCorrectly"/>.
    /// </summary>
    /// <remarks>
    /// <para>To fix a violation of this rule, ensure that there is no whitespace on either side of the opening square
    /// bracket.</para>
    /// </remarks>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1010CodeFixProvider))]
    [Shared]
    public class SA1010CodeFixProvider : CodeFixProvider
    {
        private static readonly ImmutableArray<string> FixableDiagnostics =
            ImmutableArray.Create(SA1010OpeningSquareBracketsMustBeSpacedCorrectly.DiagnosticId);

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
            foreach (var diagnostic in context.Diagnostics)
            {
                if (!diagnostic.Id.Equals(SA1010OpeningSquareBracketsMustBeSpacedCorrectly.DiagnosticId))
                {
                    continue;
                }

                context.RegisterCodeFix(
                    CodeAction.Create(SpacingResources.SA1010CodeFix,
                        cancellationToken => GetTransformedDocumentAsync(context.Document, diagnostic, cancellationToken)),
                        diagnostic);
            }

            return SpecializedTasks.CompletedTask;
        }

        private static async Task<Document> GetTransformedDocumentAsync(Document document, Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            SyntaxToken token = root.FindToken(diagnostic.Location.SourceSpan.Start);
            var replacements = new List<SyntaxToken>(2);

            if (!token.IsFirstInLine())
            {
                SyntaxToken precedingToken = token.GetPreviousToken();
                if (precedingToken.TrailingTrivia.Any(SyntaxKind.WhitespaceTrivia))
                {
                    replacements.Add(precedingToken);
                }
            }

            SyntaxTriviaList trailingTrivia = token.TrailingTrivia;
            if (!trailingTrivia.Any(SyntaxKind.EndOfLineTrivia) && trailingTrivia.Any(SyntaxKind.WhitespaceTrivia))
            {
                replacements.Add(token);
            }

            if (replacements.Count == 0)
            {
                return document;
            }

            var transformed = root.ReplaceTokens(replacements, (original, maybeRewritten) => maybeRewritten.WithoutTrailingWhitespace().WithoutFormatting());
            Document updatedDocument = document.WithSyntaxRoot(transformed);

            return updatedDocument;
        }
    }
}
