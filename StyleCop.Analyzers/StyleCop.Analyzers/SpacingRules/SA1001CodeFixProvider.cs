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
    using StyleCop.Analyzers.Helpers;

    /// <summary>
    /// Implements a code fix for <see cref="SA1001CommasMustBeSpacedCorrectly"/>.
    /// </summary>
    /// <remarks>
    /// <para>To fix a violation of this rule, ensure that the comma is followed by a single space, and is not preceded
    /// by any space.</para>
    /// </remarks>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1001CodeFixProvider))]
    [Shared]
    public class SA1001CodeFixProvider : CodeFixProvider
    {
        private static readonly ImmutableArray<string> FixableDiagnostics =
            ImmutableArray.Create(SA1001CommasMustBeSpacedCorrectly.DiagnosticId);

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
                if (!diagnostic.Id.Equals(SA1001CommasMustBeSpacedCorrectly.DiagnosticId))
                {
                    continue;
                }

                SyntaxToken token = root.FindToken(diagnostic.Location.SourceSpan.Start);
                if (!token.IsKind(SyntaxKind.CommaToken))
                {
                    continue;
                }

                context.RegisterCodeFix(CodeAction.Create(SpacingResources.SA1001CodeFix, t => GetTransformedDocumentAsync(context.Document, root, token)), diagnostic);
            }
        }

        private static Task<Document> GetTransformedDocumentAsync(Document document, SyntaxNode root, SyntaxToken token)
        {
            Dictionary<SyntaxToken, SyntaxToken> replacements = new Dictionary<SyntaxToken, SyntaxToken>();

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
            else
            {
                SyntaxToken nextToken = token.GetNextToken();
                if (nextToken.IsKind(SyntaxKind.CommaToken) || nextToken.IsKind(SyntaxKind.GreaterThanToken) || nextToken.IsKind(SyntaxKind.CloseBracketToken))
                {
                    // make an exception for things like typeof(Func<,>), typeof(Func<,,>) and int[,] myArray
                    missingFollowingSpace = false;
                }
            }

            if (!token.IsFirstInLine())
            {
                SyntaxToken precedingToken = token.GetPreviousToken();
                if (precedingToken.TrailingTrivia.Any(SyntaxKind.WhitespaceTrivia))
                {
                    SyntaxToken corrected = precedingToken.WithoutTrailingWhitespace().WithoutFormatting();
                    replacements[precedingToken] = corrected;
                }
            }

            if (missingFollowingSpace)
            {
                SyntaxToken intermediate = token.WithoutTrailingWhitespace();
                SyntaxToken corrected =
                    intermediate
                    .WithTrailingTrivia(intermediate.TrailingTrivia.Insert(0, SyntaxFactory.Space));
                replacements[token] = corrected;
            }

            var transformed = root.ReplaceTokens(replacements.Keys, (original, maybeRewritten) => replacements[original]);
            return Task.FromResult(document.WithSyntaxRoot(transformed));
        }
    }
}
