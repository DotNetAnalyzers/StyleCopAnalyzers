namespace StyleCop.Analyzers.MaintainabilityRules
{
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using SpacingRules;
    using StyleCop.Analyzers.Helpers;

    internal sealed class SA1407SA1408FixAllProvider : DocumentBasedFixAllProvider
    {
        private static readonly SyntaxAnnotation NeedsParenthesisAnnotation = new SyntaxAnnotation("StyleCop.NeedsParenthesis");

        protected override string CodeActionTitle => MaintainabilityResources.SA1407SA1408CodeFix;

        protected override async Task<SyntaxNode> FixAllInDocumentAsync(FixAllContext fixAllContext, Document document)
        {
            var diagnostics = await fixAllContext.GetDocumentDiagnosticsAsync(document).ConfigureAwait(false);

            var newDocument = document;

            var root = await newDocument.GetSyntaxRootAsync(fixAllContext.CancellationToken).ConfigureAwait(false);

            // First annotate all expressions that need parenthesis with a temporary annotation.
            // With this annotation we can find the nodes that need parenthesis even if
            // the source span changes.
            foreach (var diagnostic in diagnostics)
            {
                SyntaxNode node = root.FindNode(diagnostic.Location.SourceSpan);
                if (node.IsMissing)
                {
                    continue;
                }

                root = root.ReplaceNode(node, node.WithAdditionalAnnotations(NeedsParenthesisAnnotation));
            }

            return root.ReplaceNodes(root.GetAnnotatedNodes(NeedsParenthesisAnnotation), this.AddParentheses);
        }

        private SyntaxNode AddParentheses(SyntaxNode originalNode, SyntaxNode rewrittenNode)
        {
            BinaryExpressionSyntax syntax = rewrittenNode as BinaryExpressionSyntax;
            if (syntax == null)
            {
                return rewrittenNode;
            }

            BinaryExpressionSyntax trimmedSyntax = syntax
                .WithoutTrivia()
                .WithoutAnnotations(NeedsParenthesisAnnotation.Kind);

            return SyntaxFactory.ParenthesizedExpression(trimmedSyntax)
                .WithTriviaFrom(syntax)
                .WithoutFormatting();
        }
    }
}
