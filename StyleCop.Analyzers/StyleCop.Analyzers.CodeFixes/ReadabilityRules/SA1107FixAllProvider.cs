// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.ReadabilityRules
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeFixes;
    using StyleCop.Analyzers.Helpers;

    internal class SA1107FixAllProvider : DocumentBasedFixAllProvider
    {
        protected override string CodeActionTitle => ReadabilityResources.SA1107CodeFix;

        protected override async Task<SyntaxNode> FixAllInDocumentAsync(FixAllContext fixAllContext, Document document, ImmutableArray<Diagnostic> diagnostics)
        {
            if (diagnostics.IsEmpty)
            {
                return null;
            }

            var root = await document.GetSyntaxRootAsync(fixAllContext.CancellationToken).ConfigureAwait(false);
            var text = await document.GetTextAsync(fixAllContext.CancellationToken).ConfigureAwait(false);
            var options = document.Project.Solution.Workspace.Options;
            var settings = SettingsHelper.GetStyleCopSettingsInCodeFix(document.Project.AnalyzerOptions, root.SyntaxTree, fixAllContext.CancellationToken);

            ImmutableList<SyntaxNode> nodesToChange = ImmutableList.Create<SyntaxNode>();

            // Make sure all nodes we care about are tracked
            foreach (var diagnostic in diagnostics)
            {
                var location = diagnostic.Location;
                var syntaxNode = root.FindNode(location.SourceSpan);
                if (syntaxNode != null)
                {
                    nodesToChange = nodesToChange.Add(syntaxNode);
                }
            }

            var replacementTokens = new Dictionary<SyntaxToken, SyntaxToken>();
            foreach (var node in nodesToChange)
            {
                var firstToken = node.GetFirstToken();
                var endOfLine = FormattingHelper.GetEndOfLineForCodeFix(firstToken, text, options);

                var firstTokenOnLine = IndentationHelper.GetFirstTokenOnTextLine(firstToken);
                var previousToken = firstToken.GetPreviousToken(includeZeroWidth: true);
                var replacementPreviousToken = previousToken.WithTrailingTrivia(previousToken.TrailingTrivia.WithoutTrailingWhitespace().Add(endOfLine));
                var indentSteps = IndentationHelper.GetIndentationSteps(settings.Indentation, firstTokenOnLine);
                var indentTrivia = IndentationHelper.GenerateWhitespaceTrivia(settings.Indentation, indentSteps);

                replacementTokens.Add(previousToken, replacementPreviousToken);
                replacementTokens.Add(firstToken, firstToken.WithLeadingTrivia(indentTrivia));
            }

            return root.ReplaceTokens(
                replacementTokens.Keys,
                (originalToken, rewrittenToken) => replacementTokens[originalToken]);
        }
    }
}
