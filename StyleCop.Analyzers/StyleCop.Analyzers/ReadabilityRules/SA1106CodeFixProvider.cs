﻿namespace StyleCop.Analyzers.ReadabilityRules
{
    using System.Collections.Generic;
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
    using Microsoft.CodeAnalysis.Text;
    using StyleCop.Analyzers.Helpers;
    using StyleCop.Analyzers.SpacingRules;

    /// <summary>
    /// This class provides a code fix for <see cref="SA1106CodeMustNotContainEmptyStatements"/>.
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1106CodeFixProvider))]
    [Shared]
    public class SA1106CodeFixProvider : CodeFixProvider
    {
        private static readonly ImmutableArray<string> FixableDiagnostics =
            ImmutableArray.Create(SA1106CodeMustNotContainEmptyStatements.DiagnosticId);

        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds => FixableDiagnostics;

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
                if (!diagnostic.Id.Equals(SA1106CodeMustNotContainEmptyStatements.DiagnosticId))
                {
                    continue;
                }

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
            if (token.IsMissingOrDefault())
            {
                return document;
            }

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
            SourceText sourceText = await document.GetTextAsync(cancellationToken).ConfigureAwait(false);
            TextLine line = sourceText.Lines.GetLineFromPosition(token.SpanStart);
            if (sourceText.ToString(line.Span).Trim() == token.Text)
            {
                // remove the line containing the semicolon token
                TextChange textChange = new TextChange(line.SpanIncludingLineBreak, string.Empty);
                return document.WithText(sourceText.WithChanges(textChange));
            }
            else
            {
                // remove just the semicolon
                TextChange textChange = new TextChange(token.Span, string.Empty);
                return document.WithText(sourceText.WithChanges(textChange));
            }
        }
    }
}