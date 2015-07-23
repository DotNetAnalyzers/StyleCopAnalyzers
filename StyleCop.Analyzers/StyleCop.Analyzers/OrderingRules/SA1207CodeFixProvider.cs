namespace StyleCop.Analyzers.OrderingRules
{
    using System.Collections.Immutable;
    using System.Composition;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using StyleCop.Analyzers.Helpers;

    /// <summary>
    /// Implements code fixes for <see cref="SA1207ProtectedMustComeBeforeInternal"/>.
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1207CodeFixProvider))]
    [Shared]
    public class SA1207CodeFixProvider : CodeFixProvider
    {
        private static readonly ImmutableArray<string> FixableDiagnostics =
            ImmutableArray.Create(SA1207ProtectedMustComeBeforeInternal.DiagnosticId);

        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds => FixableDiagnostics;

        /// <inheritdoc/>
        public override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        /// <inheritdoc/>
        public override Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            foreach (Diagnostic diagnostic in context.Diagnostics.Where(d => FixableDiagnostics.Contains(d.Id)))
            {
                context.RegisterCodeFix(CodeAction.Create(OrderingResources.SA1207CodeFix, token => GetTransformedDocumentAsync(context.Document, diagnostic, token)), diagnostic);
            }

            return SpecializedTasks.CompletedTask;
        }

        private static async Task<Document> GetTransformedDocumentAsync(Document document, Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            var syntaxRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            var originalDeclarationNode = syntaxRoot.FindNode(diagnostic.Location.SourceSpan) as MemberDeclarationSyntax;

            var childTokens = originalDeclarationNode?.ChildTokens();
            if (childTokens == null)
            {
                return document;
            }

            var newDeclarationNode = originalDeclarationNode.ReplaceTokens(childTokens, ComputeReplacementToken);

            var newSyntaxRoot = syntaxRoot.ReplaceNode(originalDeclarationNode, newDeclarationNode);
            return document.WithSyntaxRoot(newSyntaxRoot);
        }

        private static SyntaxToken ComputeReplacementToken(SyntaxToken originalToken, SyntaxToken rewrittenToken)
        {
            if (originalToken.IsKind(SyntaxKind.InternalKeyword))
            {
                return SyntaxFactory.Token(SyntaxKind.ProtectedKeyword).WithTriviaFrom(rewrittenToken);
            }
            else if (originalToken.IsKind(SyntaxKind.ProtectedKeyword))
            {
                return SyntaxFactory.Token(SyntaxKind.InternalKeyword).WithTriviaFrom(rewrittenToken);
            }
            else
            {
                return rewrittenToken;
            }
        }
    }
}
