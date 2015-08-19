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
    /// This class provides a code fix for <see cref="SA1101PrefixLocalCallsWithThis"/>.
    /// </summary>
    /// <remarks>
    /// <para>To fix a violation of this rule, insert the <c>this.</c> prefix before the call to the class
    /// member.</para>
    /// </remarks>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1101CodeFixProvider))]
    [Shared]
    public class SA1101CodeFixProvider : CodeFixProvider
    {
        private static readonly ImmutableArray<string> FixableDiagnostics =
            ImmutableArray.Create(SA1101PrefixLocalCallsWithThis.DiagnosticId);

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
                if (!diagnostic.Id.Equals(SA1101PrefixLocalCallsWithThis.DiagnosticId))
                {
                    continue;
                }

                var node = root.FindNode(diagnostic.Location.SourceSpan, getInnermostNodeForTie: true) as SimpleNameSyntax;
                if (node == null)
                {
                    return;
                }

                context.RegisterCodeFix(CodeAction.Create(ReadabilityResources.SA1101CodeFix, token => GetTransformedDocumentAsync(context.Document, root, node), equivalenceKey: nameof(SA1101CodeFixProvider)), diagnostic);
            }
        }

        private static Task<Document> GetTransformedDocumentAsync(Document document, SyntaxNode root, SimpleNameSyntax node)
        {
            var qualifiedExpression =
                SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, SyntaxFactory.ThisExpression(), node.WithoutTrivia().WithoutFormatting())
                .WithTriviaFrom(node)
                .WithoutFormatting();

            var newSyntaxRoot = root.ReplaceNode(node, qualifiedExpression);

            return Task.FromResult(document.WithSyntaxRoot(newSyntaxRoot));
        }
    }
}
