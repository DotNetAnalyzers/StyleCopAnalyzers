namespace StyleCop.Analyzers.MaintainabilityRules
{
    using System.Linq;
    using System.Collections.Immutable;
    using System.Composition;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Formatting;

    /// <summary>
    /// Implements a code fix for <see cref="SA1407ArithmeticExpressionsMustDeclarePrecedence"/>.
    /// </summary>
    /// <remarks>
    /// <para>To fix a violation of this rule, insert parenthesis within the arithmetic expression to declare the precedence of the operations.</para>
    /// </remarks>
    [ExportCodeFixProvider(nameof(SA1119CodeFixProvider), LanguageNames.CSharp)]
    [Shared]
    public class SA1119CodeFixProvider : CodeFixProvider
    {
        private static readonly ImmutableArray<string> _fixableDiagnostics =
            ImmutableArray.Create(SA1119StatementMustNotUseUnnecessaryParenthesis.DiagnosticId);

        /// <inheritdoc/>
        public override ImmutableArray<string> GetFixableDiagnosticIds()
        {
            return _fixableDiagnostics;
        }

        /// <inheritdoc/>
        public override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        /// <inheritdoc/>
        public override async Task ComputeFixesAsync(CodeFixContext context)
        {
            foreach (var diagnostic in context.Diagnostics)
            {
                if (!diagnostic.Id.Equals(SA1119StatementMustNotUseUnnecessaryParenthesis.DiagnosticId))
                    continue;

                var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
                SyntaxNode node = root.FindNode(diagnostic.Location.SourceSpan, getInnermostNodeForTie: true, findInsideTrivia: true);
                if (node.IsMissing)
                    continue;
                ParenthesizedExpressionSyntax syntax = node as ParenthesizedExpressionSyntax;
                if (syntax != null)
                {
                    var syntaxRoot = await context.Document.GetSyntaxRootAsync(context.CancellationToken);
                    var leadingTrivia = syntax.OpenParenToken.GetAllTrivia().Concat(syntax.Expression.GetLeadingTrivia());
                    var trailingTrivia = syntax.Expression.GetTrailingTrivia().Concat(syntax.CloseParenToken.GetAllTrivia());

                    var newNode = syntax.Expression
                        .WithLeadingTrivia(leadingTrivia)
                        .WithTrailingTrivia(trailingTrivia);

                    var newSyntaxRoot = syntaxRoot.ReplaceNode(syntax, newNode);

                    var changedDocument = context.Document.WithSyntaxRoot(newSyntaxRoot);

                    context.RegisterFix(CodeAction.Create("Remove parenthesis", changedDocument), diagnostic);
                }
            }
        }
    }
}
