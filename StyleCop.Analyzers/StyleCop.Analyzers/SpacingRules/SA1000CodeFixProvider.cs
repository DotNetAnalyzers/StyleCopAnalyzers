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
    [ExportCodeFixProvider(nameof(SA1000CodeFixProvider), LanguageNames.CSharp)]
    [Shared]
    public class SA1000CodeFixProvider : CodeFixProvider
    {
        private static readonly ImmutableArray<string> _fixableDiagnostics =
            ImmutableArray.Create(SA1000KeywordsMustBeSpacedCorrectly.DiagnosticId);

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
                if (!diagnostic.Id.Equals(SA1000KeywordsMustBeSpacedCorrectly.DiagnosticId))
                    continue;

                var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
                SyntaxToken token = root.FindToken(diagnostic.Location.SourceSpan.Start);
                if (token.IsMissing)
                    continue;

                bool isAddingSpace = true;
                switch (token.CSharpKind())
                {
                case SyntaxKind.NewKeyword:
                    {
                        SyntaxToken nextToken = token.GetNextToken();
                        if (nextToken.IsKind(SyntaxKind.OpenBracketToken) || nextToken.IsKind(SyntaxKind.OpenParenToken))
                            isAddingSpace = false;
                    }

                    break;

                case SyntaxKind.ReturnKeyword:
                case SyntaxKind.ThrowKeyword:
                    {
                        SyntaxToken nextToken = token.GetNextToken();
                        if (nextToken.IsKind(SyntaxKind.SemicolonToken))
                            isAddingSpace = false;
                    }

                    break;

                case SyntaxKind.CheckedKeyword:
                case SyntaxKind.DefaultKeyword:
                case SyntaxKind.SizeOfKeyword:
                case SyntaxKind.TypeOfKeyword:
                case SyntaxKind.UncheckedKeyword:
                    isAddingSpace = false;
                    break;

                default:
                    break;
                }

                if (isAddingSpace)
                {
                    if (token.HasTrailingTrivia)
                        continue;

                    SyntaxTrivia whitespace = SyntaxFactory.Whitespace(" ").WithoutElasticTrivia();
                    SyntaxToken corrected = token.WithTrailingTrivia(token.TrailingTrivia.Insert(0, whitespace));
                    Document updatedDocument = context.Document.WithSyntaxRoot(root.ReplaceToken(token, corrected));
                    context.RegisterFix(CodeAction.Create("Fix spacing", updatedDocument), diagnostic);
                }
                else
                {
                    if (!token.HasTrailingTrivia)
                        continue;

                    SyntaxToken corrected = token.WithoutTrailingWhitespace().WithoutElasticTrivia();
                    Document updatedDocument = context.Document.WithSyntaxRoot(root.ReplaceToken(token, corrected));
                    context.RegisterFix(CodeAction.Create("Fix spacing", updatedDocument), diagnostic);
                }
            }
        }
    }
}
