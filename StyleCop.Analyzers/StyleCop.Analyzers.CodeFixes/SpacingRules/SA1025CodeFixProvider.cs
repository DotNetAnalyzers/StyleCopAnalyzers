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
    using Microsoft.CodeAnalysis.CSharp;

    /// <summary>
    /// Implements a code fix for <see cref="SA1025CodeMustNotContainMultipleWhitespaceInARow"/>.
    /// </summary>
    /// <remarks>
    /// <para>To fix a violation of this rule, remove the extra whitespace characters and leave only a single space.</para>
    /// </remarks>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1025CodeFixProvider))]
    [Shared]
    internal class SA1025CodeFixProvider : CodeFixProvider
    {
        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds { get; } =
            ImmutableArray.Create(SA1025CodeMustNotContainMultipleWhitespaceInARow.DiagnosticId);

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
                        SpacingResources.SA1025CodeFix,
                        cancellationToken => GetTransformedDocumentAsync(context.Document, diagnostic, cancellationToken),
                        nameof(SA1025CodeFixProvider)),
                    diagnostic);
            }

            return SpecializedTasks.CompletedTask;
        }

        private static async Task<Document> GetTransformedDocumentAsync(Document document, Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            var whitespaceTrivia = root.FindTrivia(diagnostic.Location.SourceSpan.Start, true);
            if (whitespaceTrivia.Span.Length > 1)
            {
                return document.WithSyntaxRoot(root.ReplaceTrivia(whitespaceTrivia, SyntaxFactory.Space));
            }

            return document;
        }

        private class FixAll : DocumentBasedFixAllProvider
        {
            public static FixAllProvider Instance { get; }
                = new FixAll();

            protected override string CodeActionTitle
                => SpacingResources.SA1025CodeFix;

            protected override async Task<SyntaxNode> FixAllInDocumentAsync(FixAllContext fixAllContext, Document document)
            {
                var diagnostics = await fixAllContext.GetDocumentDiagnosticsAsync(document).ConfigureAwait(false);
                if (diagnostics.IsEmpty)
                {
                    return null;
                }

                var syntaxRoot = await document.GetSyntaxRootAsync().ConfigureAwait(false);

                List<SyntaxTrivia> tokensToFix = new List<SyntaxTrivia>();
                foreach (var diagnostic in diagnostics)
                {
                    SyntaxTrivia whitespace = syntaxRoot.FindTrivia(diagnostic.Location.SourceSpan.Start, findInsideTrivia: true);
                    if (whitespace.Span.Length > 1)
                    {
                        tokensToFix.Add(whitespace);
                    }
                }

                return syntaxRoot.ReplaceTrivia(tokensToFix, (originalTrivia, rewrittenTrivia) => SyntaxFactory.Space);
            }
        }
    }
}
