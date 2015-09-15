﻿// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.ReadabilityRules
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
    using StyleCop.Analyzers.SpacingRules;

    /// <summary>
    /// This class provides a code fix for the SA1104 diagnostic.
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1104SA1105CodeFixProvider))]
    [Shared]
    public class SA1104SA1105CodeFixProvider : CodeFixProvider
    {
        private static readonly ImmutableArray<string> FixableDiagnostics =
            ImmutableArray.Create(SA110xQueryClauses.SA1104Descriptor.Id, SA110xQueryClauses.SA1105Descriptor.Id);

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
                context.RegisterCodeFix(CodeAction.Create(ReadabilityResources.SA1104SA1105CodeFix, token => GetTransformedDocumentAsync(context.Document, diagnostic, token), equivalenceKey: nameof(SA1104SA1105CodeFixProvider)), diagnostic);
            }

            return SpecializedTasks.CompletedTask;
        }

        private static async Task<Document> GetTransformedDocumentAsync(Document document, Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            var syntaxRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            var token = syntaxRoot.FindToken(diagnostic.Location.SourceSpan.Start);

            var indentationOptions = IndentationOptions.FromDocument(document);
            var indentationTrivia = QueryIndentationHelpers.GetQueryIndentationTrivia(indentationOptions, token);

            var precedingToken = token.GetPreviousToken();
            var triviaList = precedingToken.TrailingTrivia.AddRange(token.LeadingTrivia);
            var processedTriviaList = triviaList.WithoutTrailingWhitespace().Add(SyntaxFactory.CarriageReturnLineFeed);

            var replaceMap = new Dictionary<SyntaxToken, SyntaxToken>()
            {
                [precedingToken] = precedingToken.WithTrailingTrivia(processedTriviaList),
                [token] = token.WithLeadingTrivia(indentationTrivia)
            };

            var newSyntaxRoot = syntaxRoot.ReplaceTokens(replaceMap.Keys, (t1, t2) => replaceMap[t1]).WithoutFormatting();
            return document.WithSyntaxRoot(newSyntaxRoot);
        }
    }
}
