// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.LayoutRules
{
    using System.Collections.Immutable;
    using System.Composition;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using StyleCop.Analyzers.Helpers;

    /// <summary>
    /// Implements a code fix for <see cref="SA1517CodeMustNotContainBlankLinesAtStartOfFile"/>.
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1517CodeFixProvider))]
    [Shared]
    internal class SA1517CodeFixProvider : CodeFixProvider
    {
        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds { get; } =
            ImmutableArray.Create(SA1517CodeMustNotContainBlankLinesAtStartOfFile.DiagnosticId);

        /// <inheritdoc/>
        public override FixAllProvider GetFixAllProvider()
        {
            return FixAll.Instance;
        }

        /// <inheritdoc/>
        public override Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            foreach (Diagnostic diagnostic in context.Diagnostics)
            {
                context.RegisterCodeFix(
                    CodeAction.Create(
                        LayoutResources.SA1517CodeFix,
                        cancellationToken => GetTransformedDocumentAsync(context.Document, cancellationToken),
                        nameof(SA1517CodeFixProvider)),
                    diagnostic);
            }

            return SpecializedTasks.CompletedTask;
        }

        private static async Task<Document> GetTransformedDocumentAsync(Document document, CancellationToken token)
        {
            var newSyntaxRoot = await GetTransformedSyntaxRootAsync(document, token).ConfigureAwait(false);

            return document.WithSyntaxRoot(newSyntaxRoot);
        }

        private static async Task<SyntaxNode> GetTransformedSyntaxRootAsync(Document document, CancellationToken token)
        {
            var syntaxRoot = await document.GetSyntaxRootAsync(token).ConfigureAwait(false);

            var firstToken = syntaxRoot.GetFirstToken(includeZeroWidth: true);
            var leadingTrivia = firstToken.LeadingTrivia;
            var newTriviaList = SyntaxFactory.TriviaList();

            var firstNonBlankLineTriviaIndex = TriviaHelper.IndexOfFirstNonBlankLineTrivia(leadingTrivia);

            if (firstNonBlankLineTriviaIndex != -1)
            {
                for (var index = firstNonBlankLineTriviaIndex; index < leadingTrivia.Count; index++)
                {
                    newTriviaList = newTriviaList.Add(leadingTrivia[index]);
                }
            }

            var newFirstToken = firstToken.WithLeadingTrivia(newTriviaList);
            var newSyntaxRoot = syntaxRoot.ReplaceToken(firstToken, newFirstToken);
            return newSyntaxRoot;
        }

        private class FixAll : DocumentBasedFixAllProvider
        {
            public static FixAllProvider Instance { get; } =
                new FixAll();

            protected override string CodeActionTitle =>
                LayoutResources.SA1517CodeFix;

            protected override Task<SyntaxNode> FixAllInDocumentAsync(FixAllContext fixAllContext, Document document, ImmutableArray<Diagnostic> diagnostics)
            {
                if (diagnostics.IsEmpty)
                {
                    return null;
                }

                return GetTransformedSyntaxRootAsync(document, fixAllContext.CancellationToken);
            }
        }
    }
}
