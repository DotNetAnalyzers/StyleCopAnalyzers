// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.SpacingRules
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Composition;
    using System.Threading;
    using System.Threading.Tasks;
    using Helpers;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Text;

    /// <summary>
    /// Implements a code fix for <see cref="SA1028CodeMustNotContainTrailingWhitespace"/>.
    /// </summary>
    /// <remarks>
    /// <para>To fix a violation of this rule, remove any whitespace at the end of a line of code.</para>
    /// </remarks>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1028CodeFixProvider))]
    [Shared]
    internal class SA1028CodeFixProvider : CodeFixProvider
    {
        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds { get; } =
            ImmutableArray.Create(SA1028CodeMustNotContainTrailingWhitespace.DiagnosticId);

        /// <inheritdoc/>
        public sealed override FixAllProvider GetFixAllProvider()
        {
            return FixAll.Instance;
        }

        /// <inheritdoc/>
        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            foreach (var diagnostic in context.Diagnostics)
            {
                context.RegisterCodeFix(
                    CodeAction.Create(
                        SpacingResources.SA1028CodeFix,
                        ct => RemoveWhitespaceAsync(context.Document, diagnostic, ct),
                        nameof(SA1028CodeFixProvider)),
                    diagnostic);
            }
        }

        /// <summary>
        /// Removes trailing whitespace.
        /// </summary>
        /// <param name="document">The document to be changed.</param>
        /// <param name="diagnostic">The diagnostic to fix.</param>
        /// <param name="cancellationToken">The cancellation token associated with the fix action.</param>
        /// <returns>The transformed document.</returns>
        private static async Task<Document> RemoveWhitespaceAsync(Document document, Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            var text = await document.GetTextAsync(cancellationToken).ConfigureAwait(false);
            return document.WithText(text.WithChanges(new TextChange(diagnostic.Location.SourceSpan, string.Empty)));
        }

        private class FixAll : DocumentBasedFixAllProvider
        {
            public static FixAllProvider Instance { get; } =
                new FixAll();

            protected override string CodeActionTitle =>
                SpacingResources.SA1028CodeFix;

            protected override async Task<SyntaxNode> FixAllInDocumentAsync(FixAllContext fixAllContext, Document document)
            {
                var diagnostics = await fixAllContext.GetDocumentDiagnosticsAsync(document).ConfigureAwait(false);
                if (diagnostics.IsEmpty)
                {
                    return null;
                }

                var text = await document.GetTextAsync().ConfigureAwait(false);

                List<TextChange> changes = new List<TextChange>();

                foreach (var diagnostic in diagnostics)
                {
                    changes.Add(new TextChange(diagnostic.Location.SourceSpan, string.Empty));
                }

                changes.Sort((left, right) => left.Span.Start.CompareTo(right.Span.Start));

                var tree = await document.GetSyntaxTreeAsync().ConfigureAwait(false);
                return await tree.WithChangedText(text.WithChanges(changes)).GetRootAsync().ConfigureAwait(false);
            }
        }
    }
}
