// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.ReadabilityRules
{
    using System.Collections.Immutable;
    using System.Composition;
    using System.Threading;
    using System.Threading.Tasks;
    using Helpers;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Text;

    /// <summary>
    /// This class provides a code fix for <see cref="SA1106CodeMustNotContainEmptyStatements"/>.
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1106CodeFixProvider))]
    [Shared]
    internal class SA1106CodeFixProvider : CodeFixProvider
    {
        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds { get; }
            = ImmutableArray.Create(SA1106CodeMustNotContainEmptyStatements.DiagnosticId);

        /// <inheritdoc/>
        public override FixAllProvider GetFixAllProvider()
        {
            return CustomFixAllProviders.BatchFixer;
        }

        /// <inheritdoc/>
        public override Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            foreach (var diagnostic in context.Diagnostics)
            {
                context.RegisterCodeFix(
                    CodeAction.Create(
                        ReadabilityResources.SA1106CodeFix,
                        cancellationToken => GetTransformedDocumentAsync(context.Document, diagnostic, cancellationToken),
                        nameof(SA1106CodeFixProvider)),
                    diagnostic);
            }

            return SpecializedTasks.CompletedTask;
        }

        private static async Task<Document> GetTransformedDocumentAsync(Document document, Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            var token = root.FindToken(diagnostic.Location.SourceSpan.Start);

            if (!token.Parent.IsKind(SyntaxKind.EmptyStatement))
            {
                return await RemoveSemicolonTextAsync(document, token, cancellationToken).ConfigureAwait(false);
            }

            return await RemoveEmptyStatementAsync(document, root, (EmptyStatementSyntax)token.Parent, cancellationToken).ConfigureAwait(false);
        }

        private static async Task<Document> RemoveEmptyStatementAsync(Document document, SyntaxNode root, EmptyStatementSyntax node, CancellationToken cancellationToken)
        {
            SyntaxNode newRoot;

            switch (node.Parent.Kind())
            {
            case SyntaxKind.Block:
            case SyntaxKind.SwitchSection:
                // empty statements in a block or switch section can be removed
                return await RemoveSemicolonTextAsync(document, node.SemicolonToken, cancellationToken).ConfigureAwait(false);

            case SyntaxKind.IfStatement:
            case SyntaxKind.ElseClause:
            case SyntaxKind.ForStatement:
            case SyntaxKind.WhileStatement:
            case SyntaxKind.DoStatement:
                // these cases are always replaced with an empty block
                newRoot = root.ReplaceNode(node, SyntaxFactory.Block().WithTriviaFrom(node));
                return document.WithSyntaxRoot(newRoot);

            case SyntaxKind.LabeledStatement:
                // handle this case as a text manipulation for simplicity
                return await RemoveSemicolonTextAsync(document, node.SemicolonToken, cancellationToken).ConfigureAwait(false);

            default:
                return document;
            }
        }

        private static async Task<Document> RemoveSemicolonTextAsync(Document document, SyntaxToken token, CancellationToken cancellationToken)
        {
            TextChange textChange;

            SourceText sourceText = await document.GetTextAsync(cancellationToken).ConfigureAwait(false);
            TextLine line = sourceText.Lines.GetLineFromPosition(token.SpanStart);
            if (sourceText.ToString(line.Span).Trim() == token.Text)
            {
                // remove the line containing the semicolon token
                textChange = new TextChange(line.SpanIncludingLineBreak, string.Empty);
                return document.WithText(sourceText.WithChanges(textChange));
            }

            TextSpan spanToRemove;
            var whitespaceIndex = TriviaHelper.IndexOfTrailingWhitespace(token.LeadingTrivia);
            if (whitespaceIndex >= 0)
            {
                spanToRemove = TextSpan.FromBounds(token.LeadingTrivia[whitespaceIndex].Span.Start, token.Span.End);
            }
            else
            {
                var previousToken = token.GetPreviousToken();
                whitespaceIndex = TriviaHelper.IndexOfTrailingWhitespace(previousToken.TrailingTrivia);
                if (whitespaceIndex >= 0)
                {
                    spanToRemove = TextSpan.FromBounds(previousToken.TrailingTrivia[whitespaceIndex].Span.Start, token.Span.End);
                }
                else
                {
                    spanToRemove = token.Span;
                }
            }

            textChange = new TextChange(spanToRemove, string.Empty);
            return document.WithText(sourceText.WithChanges(textChange));
        }
    }
}
