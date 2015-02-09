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
    /// Implements a code fix for <see cref="SA1006PreprocessorKeywordsMustNotBePrecededBySpace"/>.
    /// </summary>
    /// <remarks>
    /// <para>To fix a violation of this rule, ensure that there is no whitespace between the opening hash mark and the
    /// preprocessor-type keyword.</para>
    /// </remarks>
    [ExportCodeFixProvider(nameof(SA1006CodeFixProvider), LanguageNames.CSharp)]
    [Shared]
    public class SA1006CodeFixProvider : CodeFixProvider
    {
        private static readonly ImmutableArray<string> fixableDiagnostics =
            ImmutableArray.Create(SA1006PreprocessorKeywordsMustNotBePrecededBySpace.DiagnosticId);

        /// <inheritdoc/>
        public override ImmutableArray<string> GetFixableDiagnosticIds()
        {
            return fixableDiagnostics;
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
                if (!diagnostic.Id.Equals(SA1006PreprocessorKeywordsMustNotBePrecededBySpace.DiagnosticId))
                    continue;

                var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
                SyntaxToken keywordToken = root.FindToken(diagnostic.Location.SourceSpan.Start, findInsideTrivia: true);
                if (keywordToken.IsMissing)
                    continue;

                SyntaxToken hashToken = keywordToken.GetPreviousToken(includeDirectives: true);
                if (!hashToken.IsKind(SyntaxKind.HashToken))
                    continue;

                SyntaxToken corrected = hashToken.WithoutTrailingWhitespace().WithoutFormatting();
                Document updatedDocument = context.Document.WithSyntaxRoot(root.ReplaceToken(hashToken, corrected));
                context.RegisterFix(CodeAction.Create("Remove space", updatedDocument), diagnostic);
            }
        }
    }
}
