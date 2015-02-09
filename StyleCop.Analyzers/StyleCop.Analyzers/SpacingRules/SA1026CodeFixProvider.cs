namespace StyleCop.Analyzers.SpacingRules
{
    using System.Collections.Immutable;
    using System.Composition;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;

    /// <summary>
    /// Implements a code fix for <see cref="SA1026CodeMustNotContainSpaceAfterNewKeywordInImplicitlyTypedArrayAllocation"/>.
    /// </summary>
    /// <remarks>
    /// <para>To fix a violation of this rule, remove any whitespace between the new keyword and the opening array
    /// bracket.</para>
    /// </remarks>
    [ExportCodeFixProvider(nameof(SA1026CodeFixProvider), LanguageNames.CSharp)]
    [Shared]
    public class SA1026CodeFixProvider : CodeFixProvider
    {
        private static readonly ImmutableArray<string> fixableDiagnostics =
            ImmutableArray.Create(SA1026CodeMustNotContainSpaceAfterNewKeywordInImplicitlyTypedArrayAllocation.DiagnosticId);

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
                if (!diagnostic.Id.Equals(SA1026CodeMustNotContainSpaceAfterNewKeywordInImplicitlyTypedArrayAllocation.DiagnosticId))
                    continue;

                var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
                SyntaxToken token = root.FindToken(diagnostic.Location.SourceSpan.Start);
                if (token.IsMissing)
                    continue;

                SyntaxToken corrected = token.WithoutTrailingWhitespace().WithoutFormatting();
                Document updatedDocument = context.Document.WithSyntaxRoot(root.ReplaceToken(token, corrected));
                context.RegisterFix(CodeAction.Create("Remove space", updatedDocument), diagnostic);
            }
        }
    }
}
