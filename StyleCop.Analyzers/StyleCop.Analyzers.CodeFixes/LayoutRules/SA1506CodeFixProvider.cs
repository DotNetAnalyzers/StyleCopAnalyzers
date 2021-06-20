// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.LayoutRules
{
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
    /// Implements a code fix for <see cref="SA1506ElementDocumentationHeadersMustNotBeFollowedByBlankLine"/>.
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1506CodeFixProvider))]
    [Shared]
    internal class SA1506CodeFixProvider : CodeFixProvider
    {
        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds { get; } =
            ImmutableArray.Create(SA1506ElementDocumentationHeadersMustNotBeFollowedByBlankLine.DiagnosticId);

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
                        LayoutResources.SA1506CodeFix,
                        cancellationToken => GetTransformedDocumentAsync(context.Document, diagnostic, cancellationToken),
                        nameof(SA1506CodeFixProvider)),
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
                    var triviaList = rewrittenToken.LeadingTrivia;

                    var index = triviaList.IndexOf(SyntaxKind.SingleLineDocumentationCommentTrivia);

                    int currentLineStart = index + 1;
                    bool onBlankLine = true;
                    for (int currentIndex = currentLineStart; currentIndex < triviaList.Count; currentIndex++)
                    {
                        switch (triviaList[currentIndex].Kind())
                        {
                        case SyntaxKind.EndOfLineTrivia:
                            if (onBlankLine)
                            {
                                triviaList = triviaList.RemoveRange(currentLineStart, currentIndex - currentLineStart + 1);
                                currentIndex = currentLineStart - 1;
                                continue;
                            }
                            else
                            {
                                currentLineStart = currentIndex + 1;
                                onBlankLine = true;
                                break;
                            }

                        case SyntaxKind.WhitespaceTrivia:
                            break;

                        default:
                            if (triviaList[currentIndex].HasBuiltinEndLine())
                            {
                                currentLineStart = currentIndex + 1;
                                onBlankLine = true;
                                break;
                            }
                            else
                            {
                                onBlankLine = false;
                                break;
                            }
                        }
                    }

                    return rewrittenToken.WithLeadingTrivia(triviaList);
                });
        }

        private class FixAll : DocumentBasedFixAllProvider
        {
            public static FixAllProvider Instance { get; } =
                new FixAll();

            protected override string CodeActionTitle => LayoutResources.SA1506CodeFix;

            protected override async Task<SyntaxNode> FixAllInDocumentAsync(FixAllContext fixAllContext, Document document, ImmutableArray<Diagnostic> diagnostics)
                => await GetTransformedDocumentAsync(document, diagnostics, fixAllContext.CancellationToken).ConfigureAwait(false);
        }
    }
}
