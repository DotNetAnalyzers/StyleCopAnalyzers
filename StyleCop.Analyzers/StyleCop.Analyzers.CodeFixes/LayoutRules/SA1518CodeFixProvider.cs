// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.LayoutRules
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

    /// <summary>
    /// Implements a code fix for <see cref="SA1518CodeMustNotContainBlankLinesAtEndOfFile"/>.
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1518CodeFixProvider))]
    [Shared]
    internal class SA1518CodeFixProvider : CodeFixProvider
    {
        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds { get; } =
            ImmutableArray.Create(SA1518CodeMustNotContainBlankLinesAtEndOfFile.DiagnosticId);

        /// <inheritdoc/>
        public override FixAllProvider GetFixAllProvider()
        {
            return CustomFixAllProviders.BatchFixer;
        }

        /// <inheritdoc/>
        public override Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            foreach (Diagnostic diagnostic in context.Diagnostics)
            {
                context.RegisterCodeFix(
                    CodeAction.Create(
                        LayoutResources.SA1518CodeFix,
                        cancellationToken => GetTransformedDocumentAsync(context.Document, cancellationToken),
                        nameof(SA1518CodeFixProvider)),
                    diagnostic);
            }

            return SpecializedTasks.CompletedTask;
        }

        private static async Task<Document> GetTransformedDocumentAsync(Document document, CancellationToken token)
        {
            var syntaxRoot = await document.GetSyntaxRootAsync(token).ConfigureAwait(false);

            var lastToken = syntaxRoot.GetLastToken(includeZeroWidth: true);

            var newLastToken = StripViolatingWhitespace(lastToken);
            var newSyntaxRoot = syntaxRoot.ReplaceToken(lastToken, newLastToken);
            var newDocument = document.WithSyntaxRoot(newSyntaxRoot);

            return newDocument;
        }

        private static SyntaxToken StripViolatingWhitespace(SyntaxToken token)
        {
            SyntaxToken result = token;

            var trailingWhitespaceIndex = TriviaHelper.IndexOfTrailingWhitespace(token.LeadingTrivia);
            if (trailingWhitespaceIndex != -1)
            {
                var newTriviaList = SyntaxFactory.TriviaList(token.LeadingTrivia.Take(trailingWhitespaceIndex));
                result = token.WithLeadingTrivia(newTriviaList);
            }

            return result;
        }
    }
}
