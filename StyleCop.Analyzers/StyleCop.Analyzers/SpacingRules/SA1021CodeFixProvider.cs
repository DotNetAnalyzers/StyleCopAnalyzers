namespace StyleCop.Analyzers.SpacingRules
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Composition;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;

    /// <summary>
    /// Implements a code fix for <see cref="SA1021NegativeSignsMustBeSpacedCorrectly"/>.
    /// </summary>
    /// <remarks>
    /// <para>To fix a violation of this rule, ensure that the spacing around the negative sign follows the rule
    /// described in <see cref="SA1021NegativeSignsMustBeSpacedCorrectly"/>.</para>
    /// </remarks>
    [ExportCodeFixProvider(nameof(SA1021CodeFixProvider), LanguageNames.CSharp)]
    [Shared]
    public class SA1021CodeFixProvider : CodeFixProvider
    {
        private static readonly ImmutableArray<string> FixableDiagnostics =
            ImmutableArray.Create(SA1021NegativeSignsMustBeSpacedCorrectly.DiagnosticId);

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
                if (!diagnostic.Id.Equals(SA1021NegativeSignsMustBeSpacedCorrectly.DiagnosticId))
                    continue;

                SyntaxToken token = root.FindToken(diagnostic.Location.SourceSpan.Start);
                if (!token.IsKind(SyntaxKind.MinusToken))
                    continue;

                context.RegisterCodeFix(CodeAction.Create("Fix spacing", t => GetTransformedDocument(context.Document, root, token)), diagnostic);
            }
        }

        private static Task<Document> GetTransformedDocument(Document document, SyntaxNode root, SyntaxToken token)
        {
            bool precededBySpace;
            bool firstInLine;
            bool followsSpecialCharacter;

            Dictionary<SyntaxToken, SyntaxToken> replacements = new Dictionary<SyntaxToken, SyntaxToken>();

            firstInLine = token.HasLeadingTrivia || token.GetLocation()?.GetMappedLineSpan().StartLinePosition.Character == 0;
            if (!firstInLine)
            {
                SyntaxToken precedingToken = token.GetPreviousToken();
                precededBySpace = precedingToken.TrailingTrivia.Any(SyntaxKind.WhitespaceTrivia);

                followsSpecialCharacter =
                    precedingToken.IsKind(SyntaxKind.OpenBracketToken)
                    || precedingToken.IsKind(SyntaxKind.OpenParenToken)
                    || precedingToken.IsKind(SyntaxKind.CloseParenToken);

                if (followsSpecialCharacter && precededBySpace)
                {
                    SyntaxToken correctedPreceding = precedingToken.WithoutTrailingWhitespace().WithoutFormatting();
                    replacements.Add(precedingToken, correctedPreceding);
                }
                else if (!followsSpecialCharacter && !precededBySpace)
                {
                    SyntaxToken correctedPreceding = precedingToken.WithoutTrailingWhitespace();
                    SyntaxTrivia whitespace = SyntaxFactory.Whitespace(" ");
                    correctedPreceding =
                        correctedPreceding
                        .WithTrailingTrivia(correctedPreceding.TrailingTrivia.Add(whitespace))
                        .WithoutFormatting();
                    replacements.Add(precedingToken, correctedPreceding);
                }
            }

            if (token.TrailingTrivia.Any(SyntaxKind.WhitespaceTrivia) || token.TrailingTrivia.Any(SyntaxKind.EndOfLineTrivia))
            {
                SyntaxToken corrected = token.WithoutTrailingWhitespace(removeEndOfLineTrivia: true).WithoutFormatting();
                replacements.Add(token, corrected);
            }

            var transformed = root.ReplaceTokens(replacements.Keys, (original, maybeRewritten) => replacements[original]);
            Document updatedDocument = document.WithSyntaxRoot(transformed);

            return Task.FromResult(updatedDocument);
        }
    }
}
