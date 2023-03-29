// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.LayoutRules
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
    using StyleCop.Analyzers.Helpers;

    /// <summary>
    /// Implements a code fix for <see cref="SA1509OpeningBracesMustNotBePrecededByBlankLine"/>.
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1509CodeFixProvider))]
    [Shared]
    internal class SA1509CodeFixProvider : CodeFixProvider
    {
        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds { get; } =
            ImmutableArray.Create(SA1509OpeningBracesMustNotBePrecededByBlankLine.DiagnosticId);

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
                        LayoutResources.SA1509CodeFix,
                        token => GetTransformedDocumentAsync(context.Document, diagnostic, token),
                        nameof(SA1509CodeFixProvider)),
                    diagnostic);
            }

            return SpecializedTasks.CompletedTask;
        }

        private static async Task<Document> GetTransformedDocumentAsync(Document document, Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            var newRoot = await GetTransformedDocumentAsync(document, ImmutableArray.Create(diagnostic), cancellationToken).ConfigureAwait(false);
            return document.WithSyntaxRoot(newRoot);
        }

        private static async Task<SyntaxNode> GetTransformedDocumentAsync(Document document, ImmutableArray<Diagnostic> diagnostics, CancellationToken cancellationToken)
        {
            var syntaxRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            return syntaxRoot.ReplaceTokens(
                diagnostics.Select(diagnostic => syntaxRoot.FindToken(diagnostic.Location.SourceSpan.Start)),
                (originalToken, rewrittenToken) =>
                {
                    var openBrace = rewrittenToken;
                    var leadingTrivia = openBrace.LeadingTrivia;

                    var newTriviaList = SyntaxFactory.TriviaList();

                    var previousEmptyLines = GetPreviousEmptyLines(openBrace).ToList();
                    newTriviaList = newTriviaList.AddRange(leadingTrivia.Except(previousEmptyLines));

                    var newOpenBrace = openBrace.WithLeadingTrivia(newTriviaList);
                    return newOpenBrace;
                });
        }

        private static IEnumerable<SyntaxTrivia> GetPreviousEmptyLines(SyntaxToken openBrace)
        {
            var result = new List<SyntaxTrivia>();

            var lineOfOpenBrace = openBrace.GetLineSpan().StartLinePosition.Line;
            var lineToCheck = lineOfOpenBrace - 1;

            while (lineToCheck > -1)
            {
                var trivias = openBrace.LeadingTrivia
                    .Where(t => t.GetLineSpan().StartLinePosition.Line == lineToCheck)
                    .ToList();
                var endOfLineTrivia = trivias.Where(t => t.IsKind(SyntaxKind.EndOfLineTrivia)).ToList();
                if (endOfLineTrivia.Any() && trivias.Except(endOfLineTrivia).All(t => t.IsKind(SyntaxKind.WhitespaceTrivia)))
                {
                    lineToCheck--;
                    result.AddRange(trivias);
                }
                else
                {
                    break;
                }
            }

            return result;
        }

        private class FixAll : DocumentBasedFixAllProvider
        {
            public static FixAllProvider Instance { get; } =
                new FixAll();

            protected override string CodeActionTitle => LayoutResources.SA1509CodeFix;

            protected override async Task<SyntaxNode> FixAllInDocumentAsync(FixAllContext fixAllContext, Document document, ImmutableArray<Diagnostic> diagnostics)
                => await GetTransformedDocumentAsync(document, diagnostics, fixAllContext.CancellationToken).ConfigureAwait(false);
        }
    }
}
