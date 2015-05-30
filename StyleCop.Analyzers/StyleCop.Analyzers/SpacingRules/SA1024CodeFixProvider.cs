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
        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            foreach (var diagnostic in context.Diagnostics)
            {
                if (!diagnostic.Id.Equals(SA1024ColonsMustBeSpacedCorrectly.DiagnosticId))
                {
                    continue;
                }

                SyntaxToken token = root.FindToken(diagnostic.Location.SourceSpan.Start);
                if (token.IsMissing)
                {
                    continue;
                }

                context.RegisterCodeFix(CodeAction.Create(SpacingResources.SA1024CodeFix, t => GetTransformedDocumentAsync(context.Document, root, token)), diagnostic);
            }
        }

        private static Task<Document> GetTransformedDocumentAsync(Document document, SyntaxNode root, SyntaxToken token)
        {
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
                SyntaxTriviaList triviaList = precedingToken.TrailingTrivia;
                if (triviaList.Count > 0 && !triviaList.Last().IsKind(SyntaxKind.MultiLineCommentTrivia))
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
                return Task.FromResult(updatedDocument);
            }
            else if (missingFollowingSpace)
            {
                SyntaxTrivia whitespace = SyntaxFactory.Space;
                SyntaxToken corrected = token.WithTrailingTrivia(token.TrailingTrivia.Insert(0, whitespace));
                Document updatedDocument = document.WithSyntaxRoot(root.ReplaceToken(token, corrected));
                return Task.FromResult(updatedDocument);
            }
            else if (hasPrecedingSpace != requireBefore)
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
                return Task.FromResult(updatedDocument);
            }

            return Task.FromResult(document);
        }
    }
}
