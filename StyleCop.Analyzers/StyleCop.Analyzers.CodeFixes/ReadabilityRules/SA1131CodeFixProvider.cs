// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.ReadabilityRules
{
    using System.Collections.Immutable;
    using System.Composition;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Helpers;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// Implements a code fix for <see cref="SA1131UseReadableConditions"/>.
    /// </summary>
    /// <remarks>
    /// <para>To fix a violation of this rule, switch the arguments of the comparison.</para>
    /// </remarks>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1131CodeFixProvider))]
    [Shared]
    internal class SA1131CodeFixProvider : CodeFixProvider
    {
        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds { get; } =
            ImmutableArray.Create(SA1131UseReadableConditions.DiagnosticId);

        /// <inheritdoc/>
        public override FixAllProvider GetFixAllProvider()
        {
            return FixAll.Instance;
        }

        /// <inheritdoc/>
        public override Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            foreach (var diagnostic in context.Diagnostics)
            {
                context.RegisterCodeFix(
                    CodeAction.Create(
                        ReadabilityResources.SA1131CodeFix,
                        cancellationToken => GetTransformedDocumentAsync(context.Document, diagnostic, cancellationToken),
                        nameof(SA1131CodeFixProvider)),
                    diagnostic);
            }

            return SpecializedTasks.CompletedTask;
        }

        private static async Task<Document> GetTransformedDocumentAsync(Document document, Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            var syntaxRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            var binaryExpression = (BinaryExpressionSyntax)syntaxRoot.FindNode(diagnostic.Location.SourceSpan, getInnermostNodeForTie: true);

            var newBinaryExpression = TransformExpression(binaryExpression);

            return document.WithSyntaxRoot(syntaxRoot.ReplaceNode(binaryExpression, newBinaryExpression));
        }

        private static BinaryExpressionSyntax TransformExpression(BinaryExpressionSyntax binaryExpression)
        {
            var newLeft = binaryExpression.Right.WithTriviaFrom(binaryExpression.Left);
            var newRight = binaryExpression.Left.WithTriviaFrom(binaryExpression.Right);
            return binaryExpression.WithLeft(newLeft).WithRight(newRight).WithOperatorToken(GetCorrectOperatorToken(binaryExpression.OperatorToken));
        }

        private static SyntaxToken GetCorrectOperatorToken(SyntaxToken operatorToken)
        {
            switch (operatorToken.Kind())
            {
            case SyntaxKind.EqualsEqualsToken:
            case SyntaxKind.ExclamationEqualsToken:
                return operatorToken;

            case SyntaxKind.GreaterThanToken:
                return SyntaxFactory.Token(operatorToken.LeadingTrivia, SyntaxKind.LessThanToken, operatorToken.TrailingTrivia);

            case SyntaxKind.GreaterThanEqualsToken:
                return SyntaxFactory.Token(operatorToken.LeadingTrivia, SyntaxKind.LessThanEqualsToken, operatorToken.TrailingTrivia);

            case SyntaxKind.LessThanToken:
                return SyntaxFactory.Token(operatorToken.LeadingTrivia, SyntaxKind.GreaterThanToken, operatorToken.TrailingTrivia);

            case SyntaxKind.LessThanEqualsToken:
                return SyntaxFactory.Token(operatorToken.LeadingTrivia, SyntaxKind.GreaterThanEqualsToken, operatorToken.TrailingTrivia);

            default:
                return SyntaxFactory.Token(SyntaxKind.None);
            }
        }

        private class FixAll : DocumentBasedFixAllProvider
        {
            public static FixAllProvider Instance { get; } =
                new FixAll();

            protected override string CodeActionTitle =>
                ReadabilityResources.SA1131CodeFix;

            protected override async Task<SyntaxNode> FixAllInDocumentAsync(FixAllContext fixAllContext, Document document)
            {
                var diagnostics = await fixAllContext.GetDocumentDiagnosticsAsync(document).ConfigureAwait(false);
                if (diagnostics.IsEmpty)
                {
                    return null;
                }

                var syntaxRoot = await document.GetSyntaxRootAsync(fixAllContext.CancellationToken).ConfigureAwait(false);

                var nodes = diagnostics.Select(diagnostic => (BinaryExpressionSyntax)syntaxRoot.FindNode(diagnostic.Location.SourceSpan, getInnermostNodeForTie: true));

                return syntaxRoot.ReplaceNodes(nodes, (originalNode, rewrittenNode) => TransformExpression(rewrittenNode));
            }
        }
    }
}
