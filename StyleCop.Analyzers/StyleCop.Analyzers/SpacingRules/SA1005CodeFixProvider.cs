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
    /// Implements a code fix for <see cref="SA1005SingleLineCommentsMustBeginWithSingleSpace"/>.
    /// </summary>
    /// <remarks>
    /// <para>To fix a violation of this rule, ensure that the comment begins with a single space. If the comment is
    /// being used to comment out a line of code, ensure that the comment begins with four forward slashes, in which
    /// case the leading space can be omitted.</para>
    /// </remarks>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1005CodeFixProvider))]
    [Shared]
    public class SA1005CodeFixProvider : CodeFixProvider
    {
        private static readonly ImmutableArray<string> FixableDiagnostics =
            ImmutableArray.Create(SA1005SingleLineCommentsMustBeginWithSingleSpace.DiagnosticId);

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
                if (!diagnostic.Id.Equals(SA1005SingleLineCommentsMustBeginWithSingleSpace.DiagnosticId))
                {
                    continue;
                }

                SyntaxTrivia trivia = root.FindTrivia(diagnostic.Location.SourceSpan.Start, findInsideTrivia: true);
                if (!trivia.IsKind(SyntaxKind.SingleLineCommentTrivia))
                {
                    continue;
                }

                context.RegisterCodeFix(CodeAction.Create(SpacingResources.SA1005CodeFix, t => GetTransformedDocumentAsync(context.Document, root, trivia), equivalenceKey: nameof(SA1005CodeFixProvider)), diagnostic);
            }
        }

        private static Task<Document> GetTransformedDocumentAsync(Document document, SyntaxNode root, SyntaxTrivia trivia)
        {
            string text = trivia.ToFullString();
            if (!text.StartsWith("//"))
            {
                return Task.FromResult(document);
            }

            string correctedText = "// " + text.Substring(2).TrimStart(' ');
            SyntaxTrivia corrected = SyntaxFactory.Comment(correctedText).WithoutFormatting();
            Document updatedDocument = document.WithSyntaxRoot(root.ReplaceTrivia(trivia, corrected));

            return Task.FromResult(updatedDocument);
        }
    }
}
