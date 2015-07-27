namespace StyleCop.Analyzers.ReadabilityRules
{
    using System;
    using System.Collections.Immutable;
    using System.Composition;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using StyleCop.Analyzers.SpacingRules;

    /// <summary>
    /// Implements a code fix for <see cref="SA1122UseStringEmptyForEmptyStrings"/>.
    /// </summary>
    /// <remarks>
    /// <para>To fix a violation of this rule, add or remove a space after the keyword, according to the description
    /// above.</para>
    /// </remarks>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1122CodeFixProvider))]
    [Shared]
    public class SA1122CodeFixProvider : CodeFixProvider
    {
        private static readonly SyntaxNode StringEmptyExpression;

        private static readonly ImmutableArray<string> FixableDiagnostics =
            ImmutableArray.Create(SA1122UseStringEmptyForEmptyStrings.DiagnosticId);

        static SA1122CodeFixProvider()
        {
            var identifierNameSyntax = SyntaxFactory.IdentifierName(nameof(string.Empty));
            var stringKeyword = SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.StringKeyword));
            StringEmptyExpression = SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, stringKeyword, identifierNameSyntax)
                .WithoutFormatting();
        }

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
                if (!diagnostic.Id.Equals(SA1122UseStringEmptyForEmptyStrings.DiagnosticId))
                {
                    continue;
                }

                var node = root?.FindNode(diagnostic.Location.SourceSpan, findInsideTrivia: true, getInnermostNodeForTie: true);
                if (node != null && node.IsKind(SyntaxKind.StringLiteralExpression))
                {
                    context.RegisterCodeFix(CodeAction.Create(ReadabilityResources.SA1122CodeFix, token => GetTransformedDocumentAsync(context.Document, root, node), equivalenceKey: nameof(SA1122CodeFixProvider)), diagnostic);
                }
            }
        }

        private static Task<Document> GetTransformedDocumentAsync(Document document, SyntaxNode root, SyntaxNode node)
        {
            var newSyntaxRoot = root.ReplaceNode(node, StringEmptyExpression.WithTriviaFrom(node));
            return Task.FromResult(document.WithSyntaxRoot(newSyntaxRoot));
        }
    }
}
