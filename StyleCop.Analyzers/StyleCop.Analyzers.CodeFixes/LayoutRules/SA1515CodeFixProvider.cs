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
    /// Implements a code fix for <see cref="SA1515SingleLineCommentMustBePrecededByBlankLine"/>.
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1515CodeFixProvider))]
    [Shared]
    internal class SA1515CodeFixProvider : CodeFixProvider
    {
        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds { get; } =
            ImmutableArray.Create(SA1515SingleLineCommentMustBePrecededByBlankLine.DiagnosticId);

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
                        LayoutResources.SA1515CodeFix,
                        cancellationToken => GetTransformedDocumentAsync(context.Document, diagnostic, cancellationToken),
                        nameof(SA1515CodeFixProvider)),
                    diagnostic);
            }

            return SpecializedTasks.CompletedTask;
        }

        private static async Task<Document> GetTransformedDocumentAsync(Document document, Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            var syntaxRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            var singleLineComment = syntaxRoot.FindTrivia(diagnostic.Location.SourceSpan.Start);
            var commentArray = new[] { singleLineComment };

            var leadingTrivia = FixTriviaList(singleLineComment.Token.LeadingTrivia, commentArray);
            var trailingTrivia = FixTriviaList(singleLineComment.Token.TrailingTrivia, commentArray);

            var newToken = singleLineComment.Token
                .WithLeadingTrivia(leadingTrivia)
                .WithTrailingTrivia(trailingTrivia);

            var newSyntaxRoot = syntaxRoot.ReplaceToken(singleLineComment.Token, newToken);

            return document.WithSyntaxRoot(newSyntaxRoot);
        }

        private static SyntaxTriviaList FixTriviaList(SyntaxTriviaList triviaList, IEnumerable<SyntaxTrivia> commentTrivias)
        {
            foreach (var singleLineComment in commentTrivias)
            {
                var index = triviaList.IndexOf(singleLineComment);
                if (index == -1)
                {
                    continue;
                }

                index--;
                while (index >= 0)
                {
                    switch (triviaList[index].Kind())
                    {
                    case SyntaxKind.WhitespaceTrivia:
                        index--;
                        break;

                    default:
                        triviaList = triviaList.ReplaceRange(triviaList[index], new[] { triviaList[index], SyntaxFactory.CarriageReturnLineFeed });

                        // We found the trivia so we don't have to loop any longer
                        index = -2;
                        break;
                    }
                }

                if (index == -1)
                {
                    triviaList = triviaList.ReplaceRange(triviaList[0], new[] { SyntaxFactory.CarriageReturnLineFeed, triviaList[0] });
                }
            }

            return triviaList;
        }

        private class FixAll : DocumentBasedFixAllProvider
        {
            public static FixAllProvider Instance { get; } =
                new FixAll();

            protected override string CodeActionTitle =>
                LayoutResources.SA1515CodeFix;

            protected override async Task<SyntaxNode> FixAllInDocumentAsync(FixAllContext fixAllContext, Document document)
            {
                var diagnostics = await fixAllContext.GetDocumentDiagnosticsAsync(document).ConfigureAwait(false);
                if (diagnostics.IsEmpty)
                {
                    return null;
                }

                var syntaxRoot = await document.GetSyntaxRootAsync().ConfigureAwait(false);

                List<SyntaxTrivia> trivias = new List<SyntaxTrivia>();

                foreach (var diagnostic in diagnostics)
                {
                    trivias.Add(syntaxRoot.FindTrivia(diagnostic.Location.SourceSpan.Start));
                }

                var tokensWithTrivia = trivias.GroupBy(x => x.Token);

                Dictionary<SyntaxToken, SyntaxToken> replacements = new Dictionary<SyntaxToken, SyntaxToken>();

                foreach (var tokenWithTrivia in tokensWithTrivia)
                {
                    var token = tokenWithTrivia.Key;
                    var newLeadingTrivia = FixTriviaList(token.LeadingTrivia, tokenWithTrivia);
                    var newTrailingTrivia = FixTriviaList(token.TrailingTrivia, tokenWithTrivia);

                    replacements.Add(token, token.WithLeadingTrivia(newLeadingTrivia).WithTrailingTrivia(newTrailingTrivia));
                }

                return syntaxRoot.ReplaceTokens(replacements.Keys, (oldToken, newToken) => replacements[oldToken]);
            }
        }
    }
}
