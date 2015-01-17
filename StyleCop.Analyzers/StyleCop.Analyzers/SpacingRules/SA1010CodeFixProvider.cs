namespace StyleCop.Analyzers.SpacingRules
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Composition;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;

    /// <summary>
    /// Implements a code fix for <see cref="SA1010OpeningSquareBracketsMustBeSpacedCorrectly"/>.
    /// </summary>
    /// <remarks>
    /// <para>To fix a violation of this rule, ensure that there is no whitespace on either side of the opening square
    /// bracket.</para>
    /// </remarks>
    [ExportCodeFixProvider(nameof(SA1010CodeFixProvider), LanguageNames.CSharp)]
    [Shared]
    public class SA1010CodeFixProvider : CodeFixProvider
    {
        private static readonly ImmutableArray<string> _fixableDiagnostics =
            ImmutableArray.Create(SA1010OpeningSquareBracketsMustBeSpacedCorrectly.DiagnosticId);

        /// <inheritdoc/>
        public override ImmutableArray<string> GetFixableDiagnosticIds()
        {
            return _fixableDiagnostics;
        }

        /// <inheritdoc/>
        public override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        /// <inheritdoc/>
        public override async Task ComputeFixesAsync(CodeFixContext context)
        {
            foreach (var diagnostic in context.Diagnostics)
            {
                if (!diagnostic.Id.Equals(SA1010OpeningSquareBracketsMustBeSpacedCorrectly.DiagnosticId))
                    continue;

                var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
                SyntaxToken token = root.FindToken(diagnostic.Location.SourceSpan.Start);
                if (!token.IsKind(SyntaxKind.OpenBracketToken))
                    continue;

                Dictionary<SyntaxToken, SyntaxToken> replacements = new Dictionary<SyntaxToken, SyntaxToken>();

                bool firstInLine = token.HasLeadingTrivia || token.GetLocation()?.GetMappedLineSpan().StartLinePosition.Character == 0;
                if (!firstInLine)
                {
                    SyntaxToken precedingToken = token.GetPreviousToken();
                    if (precedingToken.TrailingTrivia.Any(SyntaxKind.WhitespaceTrivia))
                    {
                        SyntaxToken corrected = precedingToken.WithoutTrailingWhitespace().WithoutFormatting();
                        replacements[precedingToken] = corrected;
                    }
                }

                if (!token.TrailingTrivia.Any(SyntaxKind.EndOfLineTrivia) && token.TrailingTrivia.Any(SyntaxKind.WhitespaceTrivia))
                {
                    SyntaxToken corrected = token.WithoutTrailingWhitespace().WithoutFormatting();
                    replacements[token] = corrected;
                }

                if (replacements.Count == 0)
                    continue;

                var transformed = root.ReplaceTokens(replacements.Keys, (original, maybeRewritten) => replacements[original]);
                Document updatedDocument = context.Document.WithSyntaxRoot(transformed);
                context.RegisterFix(CodeAction.Create("Fix spacing", updatedDocument), diagnostic);
            }
        }
    }
}
