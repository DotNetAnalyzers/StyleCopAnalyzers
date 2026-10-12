// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

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

    /// <summary>
    /// This class provides a code fix for the SA1102 diagnostic.
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1102CodeFixProvider))]
    [Shared]
    internal class SA1102CodeFixProvider : CodeFixProvider
    {
        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds { get; } =
            ImmutableArray.Create(SA110xQueryClauses.SA1102Descriptor.Id);

        /// <inheritdoc/>
        public override FixAllProvider GetFixAllProvider()
        {
            return CustomFixAllProviders.BatchFixer;
        }

        /// <inheritdoc/>
        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var syntaxRoot = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            foreach (var diagnostic in context.Diagnostics)
            {
                var token = syntaxRoot.FindToken(diagnostic.Location.SourceSpan.Start);
                if (HasNonWhitespaceTriviaBefore(token))
                {
                    // The code fix removes the lines between the clauses, so it is not offered when they contain
                    // comments or other non-whitespace trivia that would be lost.
                    continue;
                }

                context.RegisterCodeFix(
                    CodeAction.Create(
                        ReadabilityResources.SA1102CodeFix,
                        cancellationToken => GetTransformedDocumentAsync(context.Document, diagnostic, cancellationToken),
                        nameof(SA1102CodeFixProvider)),
                    diagnostic);
            }
        }

        private static async Task<Document> GetTransformedDocumentAsync(Document document, Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            var syntaxRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            var text = await document.GetTextAsync(cancellationToken).ConfigureAwait(false);
            var token = syntaxRoot.FindToken(diagnostic.Location.SourceSpan.Start);

            var settings = SettingsHelper.GetStyleCopSettingsInCodeFix(document.Project.AnalyzerOptions, syntaxRoot.SyntaxTree, cancellationToken);
            var indentationTrivia = QueryIndentationHelpers.GetQueryIndentationTrivia(settings.Indentation, token);

            var precedingToken = token.GetPreviousToken();
            var options = document.Project.Solution.Workspace.Options;
            var endOfLineTrivia = FormattingHelper.GetEndOfLineForCodeFix(token, text, options);

            var replaceMap = new Dictionary<SyntaxToken, SyntaxToken>()
            {
                [precedingToken] = precedingToken.WithTrailingTrivia(endOfLineTrivia),
                [token] = token.WithLeadingTrivia(indentationTrivia),
            };

            var newSyntaxRoot = syntaxRoot.ReplaceTokens(replaceMap.Keys, (t1, t2) => replaceMap[t1]).WithoutFormatting();
            return document.WithSyntaxRoot(newSyntaxRoot);
        }

        private static bool HasNonWhitespaceTriviaBefore(SyntaxToken token)
        {
            var precedingToken = token.GetPreviousToken();
            foreach (var trivia in precedingToken.TrailingTrivia.Concat(token.LeadingTrivia))
            {
                if (!trivia.IsKind(SyntaxKind.WhitespaceTrivia) && !trivia.IsKind(SyntaxKind.EndOfLineTrivia))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
