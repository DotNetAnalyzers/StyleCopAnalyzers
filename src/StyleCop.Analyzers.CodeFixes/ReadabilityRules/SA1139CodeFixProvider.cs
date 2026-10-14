// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.ReadabilityRules
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
    /// Implements a code fix for <see cref="SA1139UseLiteralSuffixNotationInsteadOfCasting"/>.
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1139CodeFixProvider))]
    [Shared]
    internal class SA1139CodeFixProvider : CodeFixProvider
    {
        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds { get; } =
            ImmutableArray.Create(SA1139UseLiteralSuffixNotationInsteadOfCasting.DiagnosticId);

        /// <inheritdoc/>
        public override Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            foreach (var diagnostic in context.Diagnostics)
            {
                context.RegisterCodeFix(
                    CodeAction.Create(
                        ReadabilityResources.SA1139CodeFix,
                        cancellationToken => GetTransformedDocumentAsync(context.Document, diagnostic, cancellationToken),
                        nameof(SA1139CodeFixProvider)),
                    diagnostic);
            }

            return SpecializedTasks.CompletedTask;
        }

        /// <inheritdoc/>
        public override FixAllProvider GetFixAllProvider()
            => CustomFixAllProviders.BatchFixer;

        private static async Task<Document> GetTransformedDocumentAsync(Document document, Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            var syntaxRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            if (!(syntaxRoot.FindNode(diagnostic.Location.SourceSpan, getInnermostNodeForTie: true) is CastExpressionSyntax node))
            {
                return document;
            }

            var replacementNode = GenerateReplacementNode(node);
            var newSyntaxRoot = syntaxRoot.ReplaceNode(node, replacementNode);
            var newDocument = document.WithSyntaxRoot(newSyntaxRoot);
            return newDocument;
        }

        private static SyntaxNode GenerateReplacementNode(CastExpressionSyntax node)
        {
            var literalExpressionSyntax =
                !(node.Expression.WalkDownParentheses() is PrefixUnaryExpressionSyntax plusMinusSyntax) ?
                (LiteralExpressionSyntax)node.Expression.WalkDownParentheses() :
                (LiteralExpressionSyntax)plusMinusSyntax.Operand.WalkDownParentheses();
            var typeToken = node.Type.GetFirstToken();
            var replacementLiteral = literalExpressionSyntax.WithLiteralSuffix(typeToken.Kind());

            var newLeadingTrivia = SyntaxFactory.TriviaList(node.GetLeadingTrivia().Concat(node.CloseParenToken.TrailingTrivia).Concat(node.Expression.GetLeadingTrivia()))
                .WithoutLeadingWhitespace()
                .WithoutTrailingWhitespace();

            if (newLeadingTrivia.Count != 0)
            {
                newLeadingTrivia = newLeadingTrivia.Add(SyntaxFactory.Space);
            }

            var replacementNode = node.Expression.ReplaceNode(literalExpressionSyntax, replacementLiteral)
                .WithLeadingTrivia(newLeadingTrivia);

            return replacementNode;
        }
    }
}
