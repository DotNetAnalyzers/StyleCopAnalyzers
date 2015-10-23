// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.MaintainabilityRules
{
    using System.Collections.Immutable;
    using System.Composition;
    using System.Threading.Tasks;
    using Helpers;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// Implements a code fix for <see cref="SA1407ArithmeticExpressionsMustDeclarePrecedence"/> and  <see cref="SA1408ConditionalExpressionsMustDeclarePrecedence"/>.
    /// </summary>
    /// <remarks>
    /// <para>To fix a violation of this rule, insert parenthesis within the arithmetic expression to declare the precedence of the operations.</para>
    /// </remarks>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1407SA1408CodeFixProvider))]
    [Shared]
    internal class SA1407SA1408CodeFixProvider : CodeFixProvider
    {
        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds { get; } =
            ImmutableArray.Create(
                SA1407ArithmeticExpressionsMustDeclarePrecedence.DiagnosticId,
                SA1408ConditionalExpressionsMustDeclarePrecedence.DiagnosticId);

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
                SyntaxNode node = root.FindNode(diagnostic.Location.SourceSpan);
                if (node.IsMissing)
                {
                    continue;
                }

                BinaryExpressionSyntax syntax = node as BinaryExpressionSyntax;
                if (syntax != null)
                {
                    context.RegisterCodeFix(
                        CodeAction.Create(
                            MaintainabilityResources.SA1407SA1408CodeFix,
                            cancellationToken => GetTransformedDocumentAsync(context.Document, root, syntax),
                            nameof(SA1407SA1408CodeFixProvider)),
                        diagnostic);
                }
            }
        }

        private static Task<Document> GetTransformedDocumentAsync(Document document, SyntaxNode root, BinaryExpressionSyntax syntax)
        {
            var newNode = SyntaxFactory.ParenthesizedExpression(syntax.WithoutTrivia())
                .WithTriviaFrom(syntax)
                .WithoutFormatting();

            var newSyntaxRoot = root.ReplaceNode(syntax, newNode);

            return Task.FromResult(document.WithSyntaxRoot(newSyntaxRoot));
        }
    }
}
