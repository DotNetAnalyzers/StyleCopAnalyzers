// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.MaintainabilityRules
{
    using System.Collections.Immutable;
    using System.Composition;
    using System.Linq;
    using System.Threading.Tasks;
    using Helpers;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Formatting;

    /// <summary>
    /// Implements a code fix for <see cref="SA1119StatementMustNotUseUnnecessaryParenthesis"/>.
    /// </summary>
    /// <remarks>
    /// <para>To fix a violation of this rule, insert parenthesis within the arithmetic expression to declare the precedence of the operations.</para>
    /// </remarks>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1119CodeFixProvider))]
    [Shared]
    public class SA1119CodeFixProvider : CodeFixProvider
    {
        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds { get; } =
            ImmutableArray.Create(SA1119StatementMustNotUseUnnecessaryParenthesis.DiagnosticId);

        /// <inheritdoc/>
        public override FixAllProvider GetFixAllProvider()
        {
            return CustomFixAllProviders.BatchFixer;
        }

        /// <inheritdoc/>
        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            foreach (var diagnostic in context.Diagnostics)
            {
                SyntaxNode node = root.FindNode(diagnostic.Location.SourceSpan, getInnermostNodeForTie: true, findInsideTrivia: true);
                if (node.IsMissing)
                {
                    continue;
                }

                ParenthesizedExpressionSyntax syntax = node as ParenthesizedExpressionSyntax;
                if (syntax != null)
                {
                    context.RegisterCodeFix(
                        CodeAction.Create(
                            MaintainabilityResources.SA1119CodeFix,
                            cancellationToken => GetTransformedDocumentAsync(context.Document, root, syntax),
                            equivalenceKey: nameof(SA1119CodeFixProvider)),
                        diagnostic);
                }
            }
        }

        private static Task<Document> GetTransformedDocumentAsync(Document document, SyntaxNode root, ParenthesizedExpressionSyntax syntax)
        {
            var leadingTrivia = SyntaxFactory.TriviaList(syntax.OpenParenToken.GetAllTrivia().Concat(syntax.Expression.GetLeadingTrivia()));
            var trailingTrivia = syntax.Expression.GetTrailingTrivia().AddRange(syntax.CloseParenToken.GetAllTrivia());

            var newNode = syntax.Expression
                .WithLeadingTrivia(leadingTrivia.Any() ? leadingTrivia : SyntaxFactory.TriviaList(SyntaxFactory.ElasticMarker))
                .WithTrailingTrivia(trailingTrivia.Any() ? trailingTrivia : SyntaxFactory.TriviaList(SyntaxFactory.ElasticMarker));

            var newSyntaxRoot = root.ReplaceNode(syntax, newNode);

            var changedDocument = document.WithSyntaxRoot(newSyntaxRoot);

            return Task.FromResult(changedDocument);
        }
    }
}
