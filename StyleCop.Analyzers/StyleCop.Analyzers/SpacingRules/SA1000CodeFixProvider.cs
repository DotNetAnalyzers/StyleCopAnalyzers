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
    /// Implements a code fix for <see cref="SA1000KeywordsMustBeSpacedCorrectly"/>.
    /// </summary>
    /// <remarks>
    /// <para>To fix a violation of this rule, add or remove a space after the keyword, according to the description
    /// above.</para>
    /// </remarks>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1000CodeFixProvider))]
    [Shared]
    public class SA1000CodeFixProvider : CodeFixProvider
    {
        private static readonly ImmutableArray<string> FixableDiagnostics =
            ImmutableArray.Create(SA1000KeywordsMustBeSpacedCorrectly.DiagnosticId);

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
                if (!diagnostic.Id.Equals(SA1000KeywordsMustBeSpacedCorrectly.DiagnosticId))
                {
                    continue;
                }

                SyntaxToken token = root.FindToken(diagnostic.Location.SourceSpan.Start);
                if (token.IsMissing)
                {
                    continue;
                }

                context.RegisterCodeFix(CodeAction.Create(SpacingResources.SA1000CodeFix, t => GetTransformedDocumentAsync(context.Document, root, token), equivalenceKey: nameof(SA1000CodeFixProvider)), diagnostic);
            }
        }

        private static Task<Document> GetTransformedDocumentAsync(Document document, SyntaxNode root, SyntaxToken token)
        {
            bool isAddingSpace = true;
            switch (token.Kind())
            {
            case SyntaxKind.NewKeyword:
                {
                    SyntaxToken nextToken = token.GetNextToken();
                    if (nextToken.IsKind(SyntaxKind.OpenBracketToken) || nextToken.IsKind(SyntaxKind.OpenParenToken))
                    {
                        isAddingSpace = false;
                    }
                }

                break;

            case SyntaxKind.ReturnKeyword:
            case SyntaxKind.ThrowKeyword:
                {
                    SyntaxToken nextToken = token.GetNextToken();
                    if (nextToken.IsKind(SyntaxKind.SemicolonToken))
                    {
                        isAddingSpace = false;
                    }
                }

                break;

            case SyntaxKind.CheckedKeyword:
            case SyntaxKind.UncheckedKeyword:
                isAddingSpace = token.GetNextToken().IsKind(SyntaxKind.OpenBraceToken);
                break;

            case SyntaxKind.DefaultKeyword:
            case SyntaxKind.NameOfKeyword:
            case SyntaxKind.SizeOfKeyword:
            case SyntaxKind.TypeOfKeyword:
                isAddingSpace = false;
                break;

            case SyntaxKind.IdentifierToken:
                if (token.Text == "nameof")
                {
                    // SA1000 would only have been reported for a nameof expression. No need to verify.
                    goto case SyntaxKind.NameOfKeyword;
                }

                return Task.FromResult(document);

            default:
                break;
            }

            if (isAddingSpace == token.HasTrailingTrivia)
            {
                return Task.FromResult(document);
            }

            if (isAddingSpace)
            {
                SyntaxTrivia whitespace = SyntaxFactory.Space;
                SyntaxToken corrected = token.WithTrailingTrivia(token.TrailingTrivia.Insert(0, whitespace));
                Document updatedDocument = document.WithSyntaxRoot(root.ReplaceToken(token, corrected));
                return Task.FromResult(updatedDocument);
            }
            else
            {
                SyntaxToken corrected = token.WithoutTrailingWhitespace().WithoutFormatting();
                Document updatedDocument = document.WithSyntaxRoot(root.ReplaceToken(token, corrected));
                return Task.FromResult(updatedDocument);
            }
        }
    }
}
