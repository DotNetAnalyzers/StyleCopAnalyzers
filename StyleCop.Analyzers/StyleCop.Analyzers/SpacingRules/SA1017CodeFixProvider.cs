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
    /// Implements a code fix for <see cref="SA1017ClosingAttributeBracketsMustBeSpacedCorrectly"/>.
    /// </summary>
    /// <remarks>
    /// <para>To fix a violation of this rule, ensure that there is no whitespace before the closing attribute
    /// bracket.</para>
    /// </remarks>
    [ExportCodeFixProvider(nameof(SA1017CodeFixProvider), LanguageNames.CSharp)]
    [Shared]
    public class SA1017CodeFixProvider : CodeFixProvider
    {
        private static readonly ImmutableArray<string> _fixableDiagnostics =
            ImmutableArray.Create(SA1017ClosingAttributeBracketsMustBeSpacedCorrectly.DiagnosticId);

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
                if (!diagnostic.Id.Equals(SA1017ClosingAttributeBracketsMustBeSpacedCorrectly.DiagnosticId))
                    continue;

                var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
                SyntaxToken token = root.FindToken(diagnostic.Location.SourceSpan.Start);
                if (!token.IsKind(SyntaxKind.CloseBracketToken))
                    continue;

                bool firstInLine = token.HasLeadingTrivia || token.GetLocation()?.GetMappedLineSpan().StartLinePosition.Character == 0;
                if (firstInLine)
                    continue;

                SyntaxToken precedingToken = token.GetPreviousToken();
                if (!precedingToken.TrailingTrivia.Any(SyntaxKind.WhitespaceTrivia))
                    continue;

                SyntaxToken corrected = precedingToken.WithoutTrailingWhitespace().WithoutElasticTrivia();
                SyntaxNode transformed = root.ReplaceToken(precedingToken, corrected);
                Document updatedDocument = context.Document.WithSyntaxRoot(transformed);
                context.RegisterFix(CodeAction.Create("Fix spacing", updatedDocument), diagnostic);
            }
        }
    }
}
