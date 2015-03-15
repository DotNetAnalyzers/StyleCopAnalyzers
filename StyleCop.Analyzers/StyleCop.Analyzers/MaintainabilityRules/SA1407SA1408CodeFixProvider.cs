namespace StyleCop.Analyzers.MaintainabilityRules
{
    using System.Collections.Immutable;
    using System.Composition;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using StyleCop.Analyzers.SpacingRules;

    /// <summary>
    /// Implements a code fix for <see cref="SA1407ArithmeticExpressionsMustDeclarePrecedence"/> and  <see cref="SA1408ConditionalExpressionsMustDeclarePrecedence"/>.
    /// </summary>
    /// <remarks>
    /// <para>To fix a violation of this rule, insert parenthesis within the arithmetic expression to declare the precedence of the operations.</para>
    /// </remarks>
    [ExportCodeFixProvider(nameof(SA1407SA1408CodeFixProvider), LanguageNames.CSharp)]
    [Shared]
    public class SA1407SA1408CodeFixProvider : CodeFixProvider
    {
        private static readonly ImmutableArray<string> FixableDiagnostics =
            ImmutableArray.Create(SA1407ArithmeticExpressionsMustDeclarePrecedence.DiagnosticId, SA1408ConditionalExpressionsMustDeclarePrecedence.DiagnosticId);

        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds => FixableDiagnostics;

        /// <inheritdoc/>
        public override FixAllProvider GetFixAllProvider()
        {
            return new SA1407SA1408FixAllProvider();
        }

        /// <inheritdoc/>
        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            foreach (var diagnostic in context.Diagnostics)
            {
                if (!this.FixableDiagnosticIds.Contains(diagnostic.Id))
                    continue;

                SyntaxNode node = root.FindNode(diagnostic.Location.SourceSpan);
                if (node.IsMissing)
                    continue;
                BinaryExpressionSyntax syntax = node as BinaryExpressionSyntax;
                if (syntax != null)
                {
                    context.RegisterCodeFix(CodeAction.Create("Add parenthesis", token => GetTransformedDocument(context, root, syntax)), diagnostic);
                }
            }
        }

        private static Task<Document> GetTransformedDocument(CodeFixContext context, SyntaxNode root, BinaryExpressionSyntax syntax)
        {
            var newNode = SyntaxFactory.ParenthesizedExpression(syntax.WithoutTrivia())
                .WithTriviaFrom(syntax)
                .WithoutFormatting();

            var newSyntaxRoot = root.ReplaceNode(syntax, newNode);

            return Task.FromResult(context.Document.WithSyntaxRoot(newSyntaxRoot));
        }
    }
}
