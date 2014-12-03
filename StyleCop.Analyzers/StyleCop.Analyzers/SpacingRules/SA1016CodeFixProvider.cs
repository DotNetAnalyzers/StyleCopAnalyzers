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
    /// Implements a code fix for <see cref="SA1016OpeningAttributeBracketsMustBeSpacedCorrectly"/>.
    /// </summary>
    /// <remarks>
    /// <para>To fix a violation of this rule, ensure that there is no whitespace after the opening attribute
    /// bracket.</para>
    /// </remarks>
    [ExportCodeFixProvider(nameof(SA1016CodeFixProvider), LanguageNames.CSharp)]
    [Shared]
    public class SA1016CodeFixProvider : CodeFixProvider
    {
        private static readonly ImmutableArray<string> _fixableDiagnostics =
            ImmutableArray.Create(SA1016OpeningAttributeBracketsMustBeSpacedCorrectly.DiagnosticId);

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
                if (!diagnostic.Id.Equals(SA1016OpeningAttributeBracketsMustBeSpacedCorrectly.DiagnosticId))
                    continue;

                var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
                SyntaxToken token = root.FindToken(diagnostic.Location.SourceSpan.Start);
                if (!token.IsKind(SyntaxKind.OpenBracketToken))
                    continue;

                if (token.TrailingTrivia.Any(SyntaxKind.EndOfLineTrivia))
                    continue;

                if (!token.TrailingTrivia.Any(SyntaxKind.WhitespaceTrivia))
                    continue;

                SyntaxToken corrected = token.WithoutTrailingWhitespace();
                SyntaxNode transformed = root.ReplaceToken(token, corrected);
                Document updatedDocument = context.Document.WithSyntaxRoot(transformed);
                context.RegisterFix(CodeAction.Create("Fix spacing", updatedDocument), diagnostic);
            }
        }
    }
}
