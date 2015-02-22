namespace StyleCop.Analyzers.MaintainabilityRules
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
    using Microsoft.CodeAnalysis.Formatting;
    using StyleCop.Analyzers.SpacingRules;

    /// <summary>
    /// Implements a code fix for <see cref="SA1119StatementMustNotUseUnnecessaryParenthesis"/>.
    /// </summary>
    /// <remarks>
    /// <para>To fix a violation of this rule, insert parenthesis within the arithmetic expression to declare the precedence of the operations.</para>
    /// </remarks>
    [ExportCodeFixProvider(nameof(SA1119CodeFixProvider), LanguageNames.CSharp)]
    [Shared]
    public class SA1119CodeFixProvider : CodeFixProvider
    {
        private static readonly ImmutableArray<string> FixableDiagnostics =
            ImmutableArray.Create(SA1119StatementMustNotUseUnnecessaryParenthesis.DiagnosticId);

        /// <inheritdoc/>
        public override ImmutableArray<string> GetFixableDiagnosticIds()
        {
            return FixableDiagnostics;
        }

        /// <inheritdoc/>
        public override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        /// <inheritdoc/>
        public override Task ComputeFixesAsync(CodeFixContext context)
        {
            foreach (var diagnostic in context.Diagnostics)
            {
                if (!diagnostic.Id.Equals(SA1119StatementMustNotUseUnnecessaryParenthesis.DiagnosticId))
                    continue;

                context.RegisterFix(CodeAction.Create("Remove parenthesis", cancellationToken => ComputeChangedDocumentAsync(context, diagnostic, cancellationToken)), diagnostic);
            }

            return Task.FromResult(default(object));
        }

        private async Task<Document> ComputeChangedDocumentAsync(CodeFixContext context, Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            var root = await context.Document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            SyntaxNode node = root.FindNode(diagnostic.Location.SourceSpan, getInnermostNodeForTie: true, findInsideTrivia: true);
            if (node.IsMissing)
                return context.Document;

            ParenthesizedExpressionSyntax syntax = node as ParenthesizedExpressionSyntax;
            if (syntax != null)
            {
                var syntaxRoot = await context.Document.GetSyntaxRootAsync(cancellationToken);
                var leadingTrivia = syntax.OpenParenToken.GetAllTrivia().Concat(syntax.Expression.GetLeadingTrivia());
                var trailingTrivia = syntax.Expression.GetTrailingTrivia().Concat(syntax.CloseParenToken.GetAllTrivia());

                var newNode = syntax.Expression
                    .WithLeadingTrivia(leadingTrivia)
                    .WithTrailingTrivia(trailingTrivia)
                    .WithoutFormatting();

                var newSyntaxRoot = syntaxRoot.ReplaceNode(syntax, newNode);
                if (!newSyntaxRoot.SyntaxTree.IsEquivalentTo(SyntaxFactory.ParseSyntaxTree(newSyntaxRoot.ToFullString())))
                {
                    newNode = newNode.WithLeadingTrivia(newNode.GetLeadingTrivia().Add(SyntaxFactory.Whitespace(" ", false)));
                    newSyntaxRoot = syntaxRoot.ReplaceNode(syntax, newNode);
                }

                var changedDocument = context.Document.WithSyntaxRoot(newSyntaxRoot);

                return changedDocument;
            }

            // no changes were made
            return context.Document;
        }
    }
}
