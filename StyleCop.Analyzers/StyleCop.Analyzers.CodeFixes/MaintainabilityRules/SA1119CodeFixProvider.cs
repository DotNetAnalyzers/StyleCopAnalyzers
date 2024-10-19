﻿// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.MaintainabilityRules
{
    using System.Collections.Immutable;
    using System.Composition;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Formatting;
    using StyleCop.Analyzers.Helpers;

    /// <summary>
    /// Implements a code fix for <see cref="SA1119StatementMustNotUseUnnecessaryParenthesis"/>.
    /// </summary>
    /// <remarks>
    /// <para>To fix a violation of this rule, insert parenthesis within the arithmetic expression to declare the precedence of the operations.</para>
    /// </remarks>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1119CodeFixProvider))]
    [Shared]
    internal class SA1119CodeFixProvider : CodeFixProvider
    {
        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds { get; } =
            ImmutableArray.Create(SA1119StatementMustNotUseUnnecessaryParenthesis.DiagnosticId, SA1119StatementMustNotUseUnnecessaryParenthesis.ParenthesesDiagnosticId);

        /// <inheritdoc/>
        public override FixAllProvider GetFixAllProvider()
        {
            return FixAll.Instance;
        }

        /// <inheritdoc/>
        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            foreach (var diagnostic in context.Diagnostics)
            {
                if (diagnostic.Id != SA1119StatementMustNotUseUnnecessaryParenthesis.DiagnosticId)
                {
                    continue;
                }

                SyntaxNode node = root.FindNode(diagnostic.Location.SourceSpan, getInnermostNodeForTie: true, findInsideTrivia: true);
                if (node.IsMissing)
                {
                    continue;
                }

                if (node is ParenthesizedExpressionSyntax syntax)
                {
                    context.RegisterCodeFix(
                        CodeAction.Create(
                            MaintainabilityResources.SA1119CodeFix,
                            cancellationToken => GetTransformedDocumentAsync(context.Document, root, syntax),
                            nameof(SA1119CodeFixProvider)),
                        diagnostic);
                }
            }
        }

        private static SyntaxNode GetReplacement(ParenthesizedExpressionSyntax oldNode)
        {
            var leadingTrivia = SyntaxFactory.TriviaList(oldNode.OpenParenToken.GetAllTrivia().Concat(oldNode.Expression.GetLeadingTrivia()));
            var trailingTrivia = oldNode.Expression.GetTrailingTrivia().AddRange(oldNode.CloseParenToken.GetAllTrivia());

            // Workaround for Roslyn not handling elastic markers for directive trivia correctly.
            if (!leadingTrivia.Any())
            {
                var previousToken = oldNode.OpenParenToken.GetPreviousToken();
                if (!(previousToken.IsKind(SyntaxKind.OpenParenToken) || previousToken.IsKind(SyntaxKind.CloseParenToken))
                    && (TriviaHelper.IndexOfTrailingWhitespace(previousToken.TrailingTrivia) == -1))
                {
                    leadingTrivia = SyntaxFactory.TriviaList(SyntaxFactory.Space);
                }
            }

            return oldNode.Expression
                .WithLeadingTrivia(leadingTrivia)
                .WithTrailingTrivia(trailingTrivia.Any() ? trailingTrivia : SyntaxFactory.TriviaList(SyntaxFactory.ElasticMarker));
        }

        private static Task<Document> GetTransformedDocumentAsync(Document document, SyntaxNode root, ParenthesizedExpressionSyntax syntax)
        {
            var newSyntaxRoot = root.ReplaceNode(syntax, GetReplacement(syntax));

            var changedDocument = document.WithSyntaxRoot(newSyntaxRoot);

            return Task.FromResult(changedDocument);
        }

        private class FixAll : DocumentBasedFixAllProvider
        {
            public static FixAllProvider Instance { get; }
                = new FixAll();

            protected override string CodeActionTitle
                => MaintainabilityResources.SA1119CodeFix;

            protected override async Task<SyntaxNode> FixAllInDocumentAsync(FixAllContext fixAllContext, Document document, ImmutableArray<Diagnostic> diagnostics)
            {
                if (diagnostics.IsEmpty)
                {
                    return null;
                }

                var syntaxRoot = await document.GetSyntaxRootAsync().ConfigureAwait(false);

                var nodes = diagnostics.Select(diagnostic => syntaxRoot.FindNode(diagnostic.Location.SourceSpan, getInnermostNodeForTie: true, findInsideTrivia: true));

                return syntaxRoot.ReplaceNodes(nodes, (originalNode, rewrittenNode) => GetReplacement((ParenthesizedExpressionSyntax)rewrittenNode));
            }
        }
    }
}
