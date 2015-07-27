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
    /// Implements a code fix for <see cref="SA1024ColonsMustBeSpacedCorrectly"/>.
    /// </summary>
    /// <remarks>
    /// <para>To fix a violation of this rule, ensure that the spacing around the colon follows the rule.</para>
    /// </remarks>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1024CodeFixProvider))]
    [Shared]
    public class SA1024CodeFixProvider : CodeFixProvider
    {
        private static readonly ImmutableArray<string> FixableDiagnostics =
            ImmutableArray.Create(SA1024ColonsMustBeSpacedCorrectly.DiagnosticId);

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
                if (!diagnostic.Id.Equals(SA1024ColonsMustBeSpacedCorrectly.DiagnosticId))
                {
                    continue;
                }

                context.RegisterCodeFix(
                    CodeAction.Create(
                        SpacingResources.SA1024CodeFix,
                        cancellation => GetTransformedDocumentAsync(context.Document, diagnostic.Location.SourceSpan.Start, cancellation),
                        equivalenceKey: nameof(SA1024CodeFixProvider)),
                    diagnostic);
            }

            return SpecializedTasks.CompletedTask;
        }

        private static async Task<Document> GetTransformedDocumentAsync(Document document, int position, CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            SyntaxToken token = root.FindToken(position);
            bool requireBefore = true;

            switch (token.Parent.Kind())
            {
            case SyntaxKind.LabeledStatement:
            case SyntaxKind.CaseSwitchLabel:
            case SyntaxKind.DefaultSwitchLabel:
            // NameColon is not explicitly listed in the description of this warning, but the behavior is inferred
            case SyntaxKind.NameColon:
                requireBefore = false;
                break;
            }

            // check for a following space
            bool missingFollowingSpace = true;
            if (token.HasTrailingTrivia)
            {
                if (token.TrailingTrivia.First().IsKind(SyntaxKind.WhitespaceTrivia))
                {
                    missingFollowingSpace = false;
                }
                else if (token.TrailingTrivia.First().IsKind(SyntaxKind.EndOfLineTrivia))
                {
                    missingFollowingSpace = false;
                }
            }

            bool hasPrecedingSpace = token.HasLeadingTrivia;
            if (!hasPrecedingSpace)
            {
                // only the first token on the line has leading trivia, and those are ignored
                SyntaxToken precedingToken = token.GetPreviousToken();
                SyntaxTriviaList combinedTrivia = precedingToken.TrailingTrivia.AddRange(token.LeadingTrivia);
                if (combinedTrivia.Count > 0 && !combinedTrivia.Last().IsKind(SyntaxKind.MultiLineCommentTrivia))
                {
                    hasPrecedingSpace = true;
                }
            }

            if (missingFollowingSpace && requireBefore && !hasPrecedingSpace)
            {
                SyntaxTrivia whitespace = SyntaxFactory.Space;
                SyntaxToken corrected = token.WithTrailingTrivia(token.TrailingTrivia.Insert(0, whitespace))
                                             .WithLeadingTrivia(token.LeadingTrivia.Add(whitespace));
                Document updatedDocument = document.WithSyntaxRoot(root.ReplaceToken(token, corrected));
                return updatedDocument;
            }

            if (missingFollowingSpace)
            {
                SyntaxTrivia whitespace = SyntaxFactory.Space;
                SyntaxToken corrected = token.WithTrailingTrivia(token.TrailingTrivia.Insert(0, whitespace));
                Document updatedDocument = document.WithSyntaxRoot(root.ReplaceToken(token, corrected));
                return updatedDocument;
            }

            if (hasPrecedingSpace != requireBefore)
            {
                SyntaxToken corrected;

                if (requireBefore)
                {
                    corrected = token.WithLeadingTrivia(token.LeadingTrivia.Add(SyntaxFactory.Space));
                }
                else
                {
                    token = token.GetPreviousToken();
                    corrected = token.WithoutTrailingWhitespace();
                }

                Document updatedDocument = document.WithSyntaxRoot(root.ReplaceToken(token, corrected));
                return updatedDocument;
            }

            return document;
        }
    }
}
