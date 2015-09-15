﻿// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

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
    public class SA1506CodeFixProvider : CodeFixProvider
    {
        private static readonly ImmutableArray<string> FixableDiagnostics =
            ImmutableArray.Create(SA1506ElementDocumentationHeadersMustNotBeFollowedByBlankLine.DiagnosticId);

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
            foreach (var diagnostic in context.Diagnostics.Where(d => FixableDiagnostics.Contains(d.Id)))
            {
                context.RegisterCodeFix(CodeAction.Create(LayoutResources.SA1506CodeFix, token => GetTransformedDocumentAsync(context.Document, diagnostic, token), equivalenceKey: nameof(SA1506CodeFixProvider)), diagnostic);
            }

            return SpecializedTasks.CompletedTask;
        }

        private static async Task<Document> GetTransformedDocumentAsync(Document document, Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            var syntaxRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            var token = syntaxRoot.FindToken(diagnostic.Location.SourceSpan.Start);
            var triviaList = token.LeadingTrivia;

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

            var newSyntaxRoot = syntaxRoot.ReplaceToken(token, token.WithLeadingTrivia(triviaList));
            return document.WithSyntaxRoot(newSyntaxRoot);
        }
    }
}
