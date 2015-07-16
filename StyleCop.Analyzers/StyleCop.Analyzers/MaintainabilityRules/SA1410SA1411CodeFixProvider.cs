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
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1410SA1411CodeFixProvider))]
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
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            foreach (var diagnostic in context.Diagnostics)
            {
                if (!this.FixableDiagnosticIds.Contains(diagnostic.Id))
                {
                    continue;
                }

                SyntaxNode node = root.FindNode(diagnostic.Location.SourceSpan, getInnermostNodeForTie: true);
                if (node.IsMissing)
                {
                    continue;
                }

                // Check if we are interested in this node
                node = (SyntaxNode)(node as ParameterListSyntax) ?? node as AttributeArgumentListSyntax;

                if (node != null)
                {
                    context.RegisterCodeFix(CodeAction.Create(MaintainabilityResources.SA1410SA1411CodeFix, token => GetTransformedDocumentAsync(context.Document, root, node)), diagnostic);
                }
            }
        }

        private static Task<Document> GetTransformedDocumentAsync(Document document, SyntaxNode root, SyntaxNode node)
        {
            // The first token is the open parenthesis token. This token has all the inner trivia
            var firstToken = node.GetFirstToken();
            var lastToken = node.GetLastToken();

            var previousToken = firstToken.GetPreviousToken();

            // We want to keep all trivia. The easiest way to do that is by doing it manually
            var newSyntaxRoot = root.RemoveNode(node, SyntaxRemoveOptions.KeepNoTrivia);

            // The removing operation has not changed the location of the previous token
            var newPreviousToken = newSyntaxRoot.FindToken(previousToken.Span.Start);

            var newTrailingTrivia = newPreviousToken.TrailingTrivia.AddRange(firstToken.GetAllTrivia()).AddRange(lastToken.GetAllTrivia());

            newSyntaxRoot = newSyntaxRoot.ReplaceToken(newPreviousToken, newPreviousToken.WithTrailingTrivia(newTrailingTrivia));

            return Task.FromResult(document.WithSyntaxRoot(newSyntaxRoot));
        }
    }
}
