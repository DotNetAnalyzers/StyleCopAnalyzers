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
    /// Implements a code fix for <see cref="SA1003SymbolsMustBeSpacedCorrectly"/>.
    /// </summary>
    /// <remarks>
    /// <para>To fix a violation of this rule, ensure that the spacing around the symbol follows the rule described in
    /// <see cref="SA1003SymbolsMustBeSpacedCorrectly"/>.</para>
    /// </remarks>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1003CodeFixProvider))]
    [Shared]
    internal class SA1003CodeFixProvider : CodeFixProvider
    {
        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds { get; } =
            ImmutableArray.Create(SA1003SymbolsMustBeSpacedCorrectly.DiagnosticId);

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
                if (diagnostic.Properties.ContainsKey(SA1003SymbolsMustBeSpacedCorrectly.CodeFixAction))
                {
                    context.RegisterCodeFix(
                        CodeAction.Create(
                            SpacingResources.SA1003CodeFix,
                            cancellationToken => GetTransformedDocumentAsync(context.Document, diagnostic, cancellationToken),
                            nameof(SA1003CodeFixProvider)),
                        diagnostic);
                }
            }

            return SpecializedTasks.CompletedTask;
        }

        private static async Task<Document> GetTransformedDocumentAsync(Document document, Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            var syntaxRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            var replacements = new Dictionary<SyntaxToken, SyntaxToken>();

            var token = syntaxRoot.FindToken(diagnostic.Location.SourceSpan.Start);
            SyntaxToken followingToken;

            switch (diagnostic.Properties[SA1003SymbolsMustBeSpacedCorrectly.CodeFixAction])
            {
            case SA1003SymbolsMustBeSpacedCorrectly.InsertBeforeTag:
                replacements[token] = token.WithLeadingTrivia(token.LeadingTrivia.Add(SyntaxFactory.Space));
                break;
            case SA1003SymbolsMustBeSpacedCorrectly.InsertAfterTag:
                replacements[token] = token.WithTrailingTrivia(token.TrailingTrivia.Insert(0, SyntaxFactory.Space));
                break;
            case SA1003SymbolsMustBeSpacedCorrectly.RemoveBeforeTag:
                var precedingToken = token.GetPreviousToken();
                replacements[precedingToken] = precedingToken.WithTrailingTrivia(precedingToken.TrailingTrivia.WithoutTrailingWhitespace());
                replacements[token] = token.WithLeadingTrivia(token.LeadingTrivia.WithoutLeadingWhitespace());
                break;
            case SA1003SymbolsMustBeSpacedCorrectly.RemoveAfterTag:
                followingToken = token.GetNextToken();
                replacements[token] = token.WithTrailingTrivia(token.TrailingTrivia.WithoutLeadingWhitespace());
                replacements[followingToken] = followingToken.WithLeadingTrivia(followingToken.LeadingTrivia.WithoutLeadingWhitespace());
                break;
            case SA1003SymbolsMustBeSpacedCorrectly.RemoveEndOfLineTag:
                followingToken = token.GetNextToken();
                replacements[token] = token.WithTrailingTrivia(token.TrailingTrivia.WithoutTrailingWhitespace());
                replacements[followingToken] = followingToken.WithLeadingTrivia(followingToken.LeadingTrivia.WithoutLeadingWhitespace());
                break;
            case SA1003SymbolsMustBeSpacedCorrectly.RemoveEndOfLineWithTrailingSpaceTag:
                followingToken = token.GetNextToken();
                replacements[token] = token.WithTrailingTrivia(token.TrailingTrivia.WithoutTrailingWhitespace().Add(SyntaxFactory.Space));
                replacements[followingToken] = followingToken.WithLeadingTrivia(followingToken.LeadingTrivia.WithoutLeadingWhitespace());
                break;
            }

            var newSyntaxRoot = syntaxRoot.ReplaceTokens(replacements.Keys, (original, maybeRewritten) => replacements[original]);
            return document.WithSyntaxRoot(newSyntaxRoot);
        }
    }
}
