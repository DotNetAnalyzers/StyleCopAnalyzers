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
    /// Implements a code fix for <see cref="SA1001CommasMustBeSpacedCorrectly"/>.
    /// </summary>
    /// <remarks>
    /// <para>To fix a violation of this rule, ensure that the comma is followed by a single space, and is not preceded
    /// by any space.</para>
    /// </remarks>
    [ExportCodeFixProvider(nameof(SA1001CodeFixProvider), LanguageNames.CSharp)]
    [Shared]
    public class SA1001CodeFixProvider : CodeFixProvider
    {
        private static readonly ImmutableArray<string> FixableDiagnostics =
            ImmutableArray.Create(SA1001CommasMustBeSpacedCorrectly.DiagnosticId);

        /// <inheritdoc/>
        public override ImmutableArray<string> GetFixableDiagnosticIds()
        {
            return FixableDiagnostics;
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
                if (!diagnostic.Id.Equals(SA1001CommasMustBeSpacedCorrectly.DiagnosticId))
                    continue;

                var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
                SyntaxToken token = root.FindToken(diagnostic.Location.SourceSpan.Start);
                if (!token.IsKind(SyntaxKind.CommaToken))
                    continue;

                Dictionary<SyntaxToken, SyntaxToken> replacements = new Dictionary<SyntaxToken, SyntaxToken>();

                // check for a following space
                bool missingFollowingSpace = true;
                if (token.HasTrailingTrivia)
                {
                    if (token.TrailingTrivia.First().IsKind(SyntaxKind.WhitespaceTrivia))
                        missingFollowingSpace = false;
                    else if (token.TrailingTrivia.First().IsKind(SyntaxKind.EndOfLineTrivia))
                        missingFollowingSpace = false;
                }
                else
                {
                    SyntaxToken nextToken = token.GetNextToken();
                    if (nextToken.IsKind(SyntaxKind.CommaToken) || nextToken.IsKind(SyntaxKind.GreaterThanToken))
                    {
                        // make an exception for things like typeof(Func<,>) and typeof(Func<,,>)
                        missingFollowingSpace = false;
                    }
                }

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

                if (missingFollowingSpace)
                {
                    SyntaxToken intermediate = token.WithoutTrailingWhitespace();
                    SyntaxToken corrected =
                        intermediate
                        .WithTrailingTrivia(intermediate.TrailingTrivia.Insert(0, SyntaxFactory.Whitespace(" ")))
                        .WithoutFormatting();
                    replacements[token] = corrected;
                }

                var transformed = root.ReplaceTokens(replacements.Keys, (original, maybeRewritten) => replacements[original]);
                Document updatedDocument = context.Document.WithSyntaxRoot(transformed);
                context.RegisterFix(CodeAction.Create("Fix spacing", updatedDocument), diagnostic);
            }
        }
    }
}
