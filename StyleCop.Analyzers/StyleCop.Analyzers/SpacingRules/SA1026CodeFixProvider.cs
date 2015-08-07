namespace StyleCop.Analyzers.SpacingRules
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Composition;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using StyleCop.Analyzers.Helpers;

    /// <summary>
    /// Implements a code fix for <see cref="SA1026CodeMustNotContainSpaceAfterNewKeywordInImplicitlyTypedArrayAllocation"/>.
    /// </summary>
    /// <remarks>
    /// <para>To fix a violation of this rule, remove any whitespace between the new keyword and the opening array
    /// bracket.</para>
    /// </remarks>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1026CodeFixProvider))]
    [Shared]
    public class SA1026CodeFixProvider : CodeFixProvider
    {
        private static readonly ImmutableArray<string> FixableDiagnostics =
            ImmutableArray.Create(SA1026CodeMustNotContainSpaceAfterNewKeywordInImplicitlyTypedArrayAllocation.DiagnosticId);

        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds => FixableDiagnostics;

        /// <inheritdoc/>
        public override FixAllProvider GetFixAllProvider()
        {
            return CustomFixAllProviders.BatchFixer;
        }

        /// <inheritdoc/>
        public override Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            foreach (var diagnostic in context.Diagnostics)
            {
                if (!diagnostic.Id.Equals(SA1026CodeMustNotContainSpaceAfterNewKeywordInImplicitlyTypedArrayAllocation.DiagnosticId))
                {
                    continue;
                }

                context.RegisterCodeFix(
                    CodeAction.Create(
                        SpacingResources.SA1026CodeFix,
                        cancellationToken => GetTransformedDocumentAsync(context.Document, diagnostic, cancellationToken),
                        equivalenceKey: nameof(SA1026CodeFixProvider)),
                    diagnostic);
            }

            return SpecializedTasks.CompletedTask;
        }

        private static async Task<Document> GetTransformedDocumentAsync(Document document, Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            SyntaxToken newKeywordToken = root.FindToken(diagnostic.Location.SourceSpan.Start);
            SyntaxToken openBracketToken = newKeywordToken.GetNextToken();
            var replaceMap = new Dictionary<SyntaxToken, SyntaxToken>()
            {
                [newKeywordToken] = newKeywordToken.WithoutTrailingWhitespace(removeEndOfLineTrivia: true).WithoutFormatting(),
                [openBracketToken] = openBracketToken.WithoutLeadingWhitespace(removeEndOfLineTrivia: true).WithoutFormatting()
            };

            Document updatedDocument = document.WithSyntaxRoot(
                root.ReplaceTokens(replaceMap.Keys, (origin, maybeRewritten) => replaceMap[origin]));

            return updatedDocument;
        }
    }
}
