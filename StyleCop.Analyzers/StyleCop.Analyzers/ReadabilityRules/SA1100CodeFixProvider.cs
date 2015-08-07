namespace StyleCop.Analyzers.ReadabilityRules
{
    using System.Collections.Immutable;
    using System.Composition;
    using System.Threading.Tasks;
    using Helpers;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using SpacingRules;

    /// <summary>
    /// This class provides a code fix for <see cref="SA1100DoNotPrefixCallsWithBaseUnlessLocalImplementationExists"/>.
    /// </summary>
    /// <remarks>
    /// <para>To fix a violation of this rule, change the <c>base.</c> prefix to <c>this.</c>.</para>
    /// </remarks>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1100CodeFixProvider))]
    [Shared]
    public class SA1100CodeFixProvider : CodeFixProvider
    {
        private static readonly ImmutableArray<string> FixableDiagnostics =
            ImmutableArray.Create(SA1100DoNotPrefixCallsWithBaseUnlessLocalImplementationExists.DiagnosticId);

        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds => FixableDiagnostics;

        /// <inheritdoc/>
        public override FixAllProvider GetFixAllProvider()
        {
            return CustomFixAllProviders.BatchFixer;
        }

        /// <inheritdoc/>
        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            foreach (var diagnostic in context.Diagnostics)
            {
                if (!diagnostic.Id.Equals(SA1100DoNotPrefixCallsWithBaseUnlessLocalImplementationExists.DiagnosticId))
                {
                    continue;
                }

                var node = root.FindNode(diagnostic.Location.SourceSpan) as BaseExpressionSyntax;
                if (node == null)
                {
                    return;
                }

                context.RegisterCodeFix(CodeAction.Create(ReadabilityResources.SA1100CodeFix, token => GetTransformedDocumentAsync(context.Document, root, node), equivalenceKey: nameof(SA1100CodeFixProvider)), diagnostic);
            }
        }

        private static Task<Document> GetTransformedDocumentAsync(Document document, SyntaxNode root, SyntaxNode node)
        {
            var thisExpressionSyntax = SyntaxFactory.ThisExpression()
                .WithTriviaFrom(node)
                .WithoutFormatting();

            SyntaxNode newSyntaxRoot = root.ReplaceNode(node, thisExpressionSyntax);
            return Task.FromResult(document.WithSyntaxRoot(newSyntaxRoot));
        }
    }
}