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
    /// Implements a code fix for <see cref="SA1011ClosingSquareBracketsMustBeSpacedCorrectly"/>.
    /// </summary>
    /// <remarks>
    /// <para>To fix a violation of this rule, ensure that the spacing around the closing square bracket follows the
    /// rule described in <see cref="SA1011ClosingSquareBracketsMustBeSpacedCorrectly"/>.</para>
    /// </remarks>
    [ExportCodeFixProvider(nameof(SA1011CodeFixProvider), LanguageNames.CSharp)]
    [Shared]
    public class SA1011CodeFixProvider : CodeFixProvider
    {
        private static readonly ImmutableArray<string> FixableDiagnostics =
            ImmutableArray.Create(SA1011ClosingSquareBracketsMustBeSpacedCorrectly.DiagnosticId);

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
            foreach (var diagnostic in context.Diagnostics)
            {
                if (!diagnostic.Id.Equals(SA1011ClosingSquareBracketsMustBeSpacedCorrectly.DiagnosticId))
                    continue;

                var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
                SyntaxToken token = root.FindToken(diagnostic.Location.SourceSpan.Start);
                if (!token.IsKind(SyntaxKind.CloseBracketToken))
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
                    bool ignoreTrailingWhitespace;
                    SyntaxToken nextToken = token.GetNextToken();
                    switch (nextToken.Kind())
                    {
                    case SyntaxKind.CloseBracketToken:
                    case SyntaxKind.OpenParenToken:
                    case SyntaxKind.CommaToken:
                    case SyntaxKind.SemicolonToken:
                    // TODO: "certain types of operator symbols"
                    case SyntaxKind.DotToken:
                    case SyntaxKind.OpenBracketToken:
                    case SyntaxKind.CloseParenToken:
                        ignoreTrailingWhitespace = true;
                        break;

                    case SyntaxKind.GreaterThanToken:
                        ignoreTrailingWhitespace = nextToken.Parent.IsKind(SyntaxKind.TypeArgumentList);
                        break;

                    case SyntaxKind.QuestionToken:
                        ignoreTrailingWhitespace = nextToken.Parent.IsKind(SyntaxKind.ConditionalAccessExpression);
                        break;

                    default:
                        ignoreTrailingWhitespace = false;
                        break;
                    }

                    if (!ignoreTrailingWhitespace)
                    {
                        SyntaxToken corrected = token.WithoutTrailingWhitespace().WithoutFormatting();
                        replacements[token] = corrected;
                    }
                }

                if (replacements.Count == 0)
                    continue;

                var transformed = root.ReplaceTokens(replacements.Keys, (original, maybeRewritten) => replacements[original]);
                Document updatedDocument = context.Document.WithSyntaxRoot(transformed);
                context.RegisterCodeFix(CodeAction.Create("Fix spacing", t => Task.FromResult(updatedDocument)), diagnostic);
            }
        }
    }
}
