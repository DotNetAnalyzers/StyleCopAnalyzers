// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.LayoutRules
{
    using System.Collections.Generic;
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
    /// Implements a code fix for <see cref="SA1508ClosingCurlyBracketsMustNotBePrecededByBlankLine"/>.
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1508CodeFixProvider))]
    [Shared]
    internal class SA1508CodeFixProvider : CodeFixProvider
    {
        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds { get; } =
            ImmutableArray.Create(SA1508ClosingCurlyBracketsMustNotBePrecededByBlankLine.DiagnosticId);

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
                        LayoutResources.SA1508CodeFix,
                        cancellationToken => GetTransformedDocumentAsync(context.Document, diagnostic, cancellationToken),
                        nameof(SA1508CodeFixProvider)),
                    diagnostic);
            }

            return SpecializedTasks.CompletedTask;
        }

        private static async Task<Document> GetTransformedDocumentAsync(Document document, Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            var syntaxRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            var closeBraceToken = syntaxRoot.FindToken(diagnostic.Location.SourceSpan.Start);
            var previousToken = closeBraceToken.GetPreviousToken();

            var triviaList = previousToken.TrailingTrivia.AddRange(closeBraceToken.LeadingTrivia);

            // skip all leading whitespace for the close brace
            var index = triviaList.Count - 1;
            while (triviaList[index].IsKind(SyntaxKind.WhitespaceTrivia))
            {
                index--;
            }

            var firstLeadingWhitespace = index + 1;

            var done = false;
            var lastEndOfLineIndex = -1;
            while (!done && index >= 0)
            {
                switch (triviaList[index].Kind())
                {
                case SyntaxKind.WhitespaceTrivia:
                    break;
                case SyntaxKind.EndOfLineTrivia:
                    lastEndOfLineIndex = index;
                    break;
                default:
                    done = true;
                    break;
                }

                index--;
            }

            var replaceMap = new Dictionary<SyntaxToken, SyntaxToken>()
            {
                [previousToken] = previousToken.WithTrailingTrivia(triviaList.Take(lastEndOfLineIndex + 1)),
                [closeBraceToken] = closeBraceToken.WithLeadingTrivia(triviaList.Skip(firstLeadingWhitespace))
            };

            var newSyntaxRoot = syntaxRoot.ReplaceTokens(replaceMap.Keys, (t1, t2) => replaceMap[t1]);
            return document.WithSyntaxRoot(newSyntaxRoot);
        }
    }
}
