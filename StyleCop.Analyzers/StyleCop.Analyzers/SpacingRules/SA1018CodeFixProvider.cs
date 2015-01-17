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
    /// Implements a code fix for <see cref="SA1018NullableTypeSymbolsMustNotBePrecededBySpace"/>.
    /// </summary>
    /// <remarks>
    /// <para>To fix a violation of this rule, ensure that there is no whitespace before the nullable type
    /// symbol.</para>
    /// </remarks>
    [ExportCodeFixProvider(nameof(SA1018CodeFixProvider), LanguageNames.CSharp)]
    [Shared]
    public class SA1018CodeFixProvider : CodeFixProvider
    {
        private static readonly ImmutableArray<string> _fixableDiagnostics =
            ImmutableArray.Create(SA1018NullableTypeSymbolsMustNotBePrecededBySpace.DiagnosticId);

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
                if (!diagnostic.Id.Equals(SA1018NullableTypeSymbolsMustNotBePrecededBySpace.DiagnosticId))
                    continue;

                var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
                SyntaxToken token = root.FindToken(diagnostic.Location.SourceSpan.Start);
                if (!token.IsKind(SyntaxKind.QuestionToken))
                    continue;

                bool firstInLine = token.HasLeadingTrivia || token.GetLocation()?.GetMappedLineSpan().StartLinePosition.Character == 0;
                if (firstInLine)
                    continue;

                SyntaxToken previousToken = token.GetPreviousToken();
                if (!previousToken.HasTrailingTrivia)
                    continue;

                SyntaxToken corrected = previousToken.WithoutTrailingWhitespace().WithoutFormatting();
                Document updatedDocument = context.Document.WithSyntaxRoot(root.ReplaceToken(previousToken, corrected));
                context.RegisterFix(CodeAction.Create("Remove space", updatedDocument), diagnostic);
            }
        }
    }
}
