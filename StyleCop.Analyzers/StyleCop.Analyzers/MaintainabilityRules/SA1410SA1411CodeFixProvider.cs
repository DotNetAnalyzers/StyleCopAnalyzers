namespace StyleCop.Analyzers.MaintainabilityRules
{
    using System;
    using System.Collections.Immutable;
    using System.Composition;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;


    /// <summary>
    /// Implements a code fix for <see cref="SA1410RemoveDelegateParenthesisWhenPossible"/> and <see cref="SA1411AttributeConstructorMustNotUseUnnecessaryParenthesis"/>.
    /// </summary>
    /// <remarks>
    /// <para>To fix a violation of this rule, insert parenthesis within the arithmetic expression to declare the precedence of the operations.</para>
    /// </remarks>
    [ExportCodeFixProvider(nameof(SA1410SA1411CodeFixProvider), LanguageNames.CSharp)]
    [Shared]
    public class SA1410SA1411CodeFixProvider : CodeFixProvider
    {
        private static readonly ImmutableArray<string> FixableDiagnostics =
            ImmutableArray.Create(SA1410RemoveDelegateParenthesisWhenPossible.DiagnosticId,
                SA1411AttributeConstructorMustNotUseUnnecessaryParenthesis.DiagnosticId);

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
            foreach (var diagnostic in context.Diagnostics)
            {
                if (!this.FixableDiagnosticIds.Contains(diagnostic.Id))
                    continue;

                var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
                SyntaxNode node = root.FindNode(diagnostic.Location.SourceSpan, getInnermostNodeForTie: true);
                if (node.IsMissing)
                    continue;

                // Check if we are interested in this node
                node = (SyntaxNode)(node as ParameterListSyntax) ?? node as AttributeArgumentListSyntax;

                if (node != null)
                {
                    var syntaxRoot = await context.Document.GetSyntaxRootAsync(context.CancellationToken);

                    var newSyntaxRoot = syntaxRoot.RemoveNode(node, SyntaxRemoveOptions.KeepExteriorTrivia);

                    var changedDocument = context.Document.WithSyntaxRoot(newSyntaxRoot);

                    context.RegisterCodeFix(CodeAction.Create("Remove parenthesis", token => Task.FromResult(changedDocument)), diagnostic);
                }
            }
        }
    }
}
