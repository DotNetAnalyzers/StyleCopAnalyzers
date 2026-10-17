// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.DocumentationRules
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Composition;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Text;
    using StyleCop.Analyzers.Helpers;

    /// <summary>
    /// Implements a code fix for <see cref="SA1654ProperlyEncodeGenericBrackets"/>.
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1654CodeFixProvider))]
    [Shared]
    internal class SA1654CodeFixProvider : CodeFixProvider
    {
        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds { get; } =
            ImmutableArray.Create(SA1654ProperlyEncodeGenericBrackets.DiagnosticId);

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
                        DocumentationResources.SA1654CodeFix,
                        cancellationToken => GetTransformedDocumentAsync(context.Document, diagnostic, cancellationToken),
                        nameof(SA1654CodeFixProvider)),
                    diagnostic);
            }

            return SpecializedTasks.CompletedTask;
        }

        private static async Task<Document> GetTransformedDocumentAsync(Document document, Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            var text = await document.GetTextAsync(cancellationToken).ConfigureAwait(false);
            return document.WithText(ApplyTextChanges(text, GetTextChanges(root, diagnostic)));
        }

        private static List<TextChange> GetTextChanges(SyntaxNode root, Diagnostic diagnostic)
        {
            var changes = new List<TextChange>();
            foreach (var token in root.DescendantTokens(descendIntoTrivia: true))
            {
                if (diagnostic.Location.SourceSpan.Contains(token.Span)
                    && token.Parent is TypeArgumentListSyntax
                    && ((token.Text.Length > 0 && token.Text[0] == '&') || token.Text == ">")
                    && token.ValueText == "<")
                {
                    changes.Add(new TextChange(token.Span, "{"));
                }
                else if (diagnostic.Location.SourceSpan.Contains(token.Span)
                    && token.Parent is TypeArgumentListSyntax
                    && ((token.Text.Length > 0 && token.Text[0] == '&') || token.Text == ">")
                    && token.ValueText == ">")
                {
                    changes.Add(new TextChange(token.Span, "}"));
                }
            }

            return changes;
        }

        private static SourceText ApplyTextChanges(SourceText text, List<TextChange> changes)
        {
            changes.Sort((left, right) => right.Span.Start.CompareTo(left.Span.Start));
            foreach (var change in changes)
            {
                text = text.WithChanges(change);
            }

            return text;
        }

        private class FixAll : DocumentBasedFixAllProvider
        {
            public static FixAllProvider Instance { get; } = new FixAll();

            protected override string CodeActionTitle => DocumentationResources.SA1654CodeFix;

            protected override async Task<SyntaxNode?> FixAllInDocumentAsync(FixAllContext fixAllContext, Document document, ImmutableArray<Diagnostic> diagnostics)
            {
                if (diagnostics.IsEmpty)
                {
                    return null;
                }

                var root = await document.GetSyntaxRootAsync(fixAllContext.CancellationToken).ConfigureAwait(false);
                var text = await document.GetTextAsync(fixAllContext.CancellationToken).ConfigureAwait(false);
                var changes = new List<TextChange>();
                foreach (var diagnostic in diagnostics)
                {
                    changes.AddRange(GetTextChanges(root, diagnostic));
                }

                var tree = await document.GetSyntaxTreeAsync(fixAllContext.CancellationToken).ConfigureAwait(false);
                return await tree.WithChangedText(ApplyTextChanges(text, changes)).GetRootAsync(fixAllContext.CancellationToken).ConfigureAwait(false);
            }
        }
    }
}
