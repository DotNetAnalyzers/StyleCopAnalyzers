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
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using StyleCop.Analyzers.Helpers;

    /// <summary>
    /// Implements a code fix for <see cref="SA1003SymbolsMustBeSpacedCorrectly"/>.
    /// </summary>
    /// <remarks>
    /// <para>To fix a violation of this rule, ensure that the spacing around the symbol follows the rule described in
    /// <see cref="SA1003SymbolsMustBeSpacedCorrectly"/>.</para>
    /// </remarks>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1003CodeFixProvider))]
    [Shared]
    public class SA1003CodeFixProvider : CodeFixProvider
    {
        private static readonly ImmutableArray<string> FixableDiagnostics =
            ImmutableArray.Create(SA1003SymbolsMustBeSpacedCorrectly.DiagnosticId);

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
                if (!diagnostic.Id.Equals(SA1003SymbolsMustBeSpacedCorrectly.DiagnosticId))
                {
                    continue;
                }

                SyntaxToken token = root.FindToken(diagnostic.Location.SourceSpan.Start);
                if (token.IsMissing)
                {
                    continue;
                }

                context.RegisterCodeFix(CodeAction.Create(SpacingResources.SA1003CodeFix, t => GetTransformedDocumentAsync(context.Document, root, token)), diagnostic);
            }
        }

        private static Task<Document> GetTransformedDocumentAsync(Document document, SyntaxNode root, SyntaxToken token)
        {
            Dictionary<SyntaxToken, SyntaxToken> replacements = new Dictionary<SyntaxToken, SyntaxToken>();

            // always a space before unless at the beginning of a line or after certain tokens
            if (!token.IsFirstInLine())
            {
                SyntaxToken precedingToken = token.GetPreviousToken();
                SyntaxToken correctedPrecedingNoSpace = precedingToken.WithoutTrailingWhitespace();
                switch (precedingToken.Kind())
                {
                case SyntaxKind.OpenParenToken:
                case SyntaxKind.CloseParenToken:
                case SyntaxKind.OpenBracketToken:
                case SyntaxKind.CloseBracketToken:
                    // remove any whitespace before
                    replacements[precedingToken] = correctedPrecedingNoSpace;
                    break;

                default:
                    if (!precedingToken.TrailingTrivia.Any(SyntaxKind.WhitespaceTrivia))
                    {
                        SyntaxToken correctedPreceding = correctedPrecedingNoSpace.WithTrailingTrivia(correctedPrecedingNoSpace.TrailingTrivia.Insert(0, SyntaxFactory.ElasticSpace));
                        replacements[precedingToken] = correctedPreceding;
                    }

                    break;
                }
            }

            if (token.Parent is BinaryExpressionSyntax)
            {
                // include a space after unless last on line
                if (!token.TrailingTrivia.Any(SyntaxKind.EndOfLineTrivia))
                {
                    SyntaxToken correctedOperatorNoSpace = token.WithoutTrailingWhitespace();
                    SyntaxToken correctedOperator =
                        correctedOperatorNoSpace
                        .WithTrailingTrivia(correctedOperatorNoSpace.TrailingTrivia.Insert(0, SyntaxFactory.Space));
                    replacements[token] = correctedOperator;
                }
            }
            else if (token.Parent is PrefixUnaryExpressionSyntax)
            {
                // do not include a space after (includes new line characters)
                SyntaxToken correctedOperatorNoSpace = token.WithoutTrailingWhitespace(removeEndOfLineTrivia: true).WithoutFormatting();
                replacements[token] = correctedOperatorNoSpace;
            }

            var transformed = root.ReplaceTokens(replacements.Keys, replacements.GetReplacementToken);
            Document updatedDocument = document.WithSyntaxRoot(transformed);

            return Task.FromResult(updatedDocument);
        }
    }
}
