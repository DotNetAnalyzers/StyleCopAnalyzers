namespace StyleCop.Analyzers.LayoutRules
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

    /// <summary>
    /// Implements a code fix for <see cref="SA1503CurlyBracketsMustNotBeOmitted"/>.
    /// </summary>
    /// <remarks>
    /// <para>To fix a violation of this rule, the violating statement will be converted to a block statement.</para>
    /// </remarks>
    [ExportCodeFixProvider(nameof(SA1503CodeFixProvider), LanguageNames.CSharp)]
    [Shared]
    public class SA1503CodeFixProvider : CodeFixProvider
    {
        private static readonly ImmutableArray<string> FixableDiagnostics =
            ImmutableArray.Create(SA1503CurlyBracketsMustNotBeOmitted.DiagnosticId);

        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds
        {
            get
            {
                return FixableDiagnostics;
            }
        }

        /// <inheritdoc/>
        public override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        /// <inheritdoc/>
        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var syntaxRoot = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            foreach (Diagnostic diagnostic in context.Diagnostics.Where(d => FixableDiagnostics.Contains(d.Id)))
            {
                var node = syntaxRoot.FindNode(diagnostic.Location.SourceSpan, true, true) as StatementSyntax;
                if (node == null || node.IsMissing)
                {
                    continue;
                }

                // If the parent of the statement contains a conditional directive, stuff will be really hard to fix correctly, so don't offer a code fix.
                if (this.ContainsConditionalDirectiveTrivia(node.Parent))
                {
                    continue;
                }

                context.RegisterCodeFix(CodeAction.Create("Wrap with curly brackets", token => GetTransformedDocumentAsync(context.Document, syntaxRoot, node, token)), diagnostic);
            }
        }

        private static Task<Document> GetTransformedDocumentAsync(Document document, SyntaxNode root, StatementSyntax node, CancellationToken cancellationToken)
        {
            var newSyntaxRoot = root.ReplaceNode(node, SyntaxFactory.Block(node));
            return Task.FromResult(document.WithSyntaxRoot(newSyntaxRoot));
        }

        private bool ContainsConditionalDirectiveTrivia(SyntaxNode node)
        {
            for (var currentDirective = node.GetFirstDirective(); currentDirective != null && node.Contains(currentDirective); currentDirective = currentDirective.GetNextDirective())
            {
                switch (currentDirective.Kind())
                {
                case SyntaxKind.IfDirectiveTrivia:
                case SyntaxKind.ElseDirectiveTrivia:
                case SyntaxKind.ElifDirectiveTrivia:
                case SyntaxKind.EndIfDirectiveTrivia:
                    return true;
                }
            }

            return false;
        }
    }
}
