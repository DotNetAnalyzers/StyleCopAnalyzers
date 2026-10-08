// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.ReadabilityRules
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Composition;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Text;
    using StyleCop.Analyzers.Helpers;

    /// <summary>
    /// Implements a code fix for <see cref="SA1116SplitParametersMustStartOnLineAfterDeclaration"/>.
    /// </summary>
    /// <remarks>
    /// <para>To fix a violation of this rule, ensure that the first parameter starts on the line after the opening
    /// bracket, or place all parameters on the same line if the parameters are not too long.</para>
    /// </remarks>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1116CodeFixProvider))]
    [Shared]
    internal class SA1116CodeFixProvider : CodeFixProvider
    {
        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds { get; } =
            ImmutableArray.Create(SA1116SplitParametersMustStartOnLineAfterDeclaration.DiagnosticId);

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
                        ReadabilityResources.SA1116CodeFix,
                        cancellationToken => GetTransformedDocumentAsync(context.Document, diagnostic, cancellationToken),
                        nameof(SA1116CodeFixProvider)),
                    diagnostic);
            }

            return SpecializedTasks.CompletedTask;
        }

        private static async Task<Document> GetTransformedDocumentAsync(Document document, Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            SyntaxToken originalToken = root.FindToken(diagnostic.Location.SourceSpan.Start);

            SyntaxTree tree = root.SyntaxTree;
            SourceText sourceText = await tree.GetTextAsync(cancellationToken).ConfigureAwait(false);
            TextLine sourceLine = sourceText.Lines.GetLineFromPosition(originalToken.SpanStart);

            string lineText = sourceText.ToString(sourceLine.Span);
            int indentLength;
            for (indentLength = 0; indentLength < lineText.Length; indentLength++)
            {
                if (!char.IsWhiteSpace(lineText[indentLength]))
                {
                    break;
                }
            }

            var options = document.Project.Solution.Workspace.Options;
            var endOfLineTrivia = FormattingHelper.GetEndOfLineForCodeFix(originalToken, sourceText, options);
            var settings = SettingsHelper.GetStyleCopSettingsInCodeFix(document.Project.AnalyzerOptions, tree, cancellationToken);
            string indentationStep = IndentationHelper.GenerateIndentationString(settings.Indentation, 1);
            SyntaxTriviaList newTrivia =
                SyntaxFactory.TriviaList(
                    endOfLineTrivia,
                    SyntaxFactory.Whitespace(lineText.Substring(0, indentLength) + indentationStep));

            // The first element moves one indentation step to the right, so every other line of it moves as well.
            SyntaxNode element = GetListElement(originalToken);
            IEnumerable<SyntaxToken> tokensToUpdate = element != null ? element.DescendantTokens() : new[] { originalToken };
            SyntaxNode updatedRoot = root.ReplaceTokens(
                tokensToUpdate,
                (original, rewritten) => original == originalToken
                    ? rewritten.WithLeadingTrivia(IndentationHelper.IndentLeadingTrivia(original, rewritten.LeadingTrivia, indentationStep, newTrivia))
                    : rewritten.WithLeadingTrivia(IndentationHelper.IndentLeadingTrivia(original, rewritten.LeadingTrivia, indentationStep, default(SyntaxTriviaList))));
            return document.WithSyntaxRoot(updatedRoot);
        }

        private static SyntaxNode GetListElement(SyntaxToken token)
        {
            SyntaxNode current = token.Parent;
            while (current != null)
            {
                SyntaxNode parent = current.Parent;
                if (parent is BaseArgumentListSyntax
                    || parent is BaseParameterListSyntax
                    || parent is AttributeArgumentListSyntax
                    || parent is ArrayRankSpecifierSyntax)
                {
                    return current.SpanStart == token.SpanStart ? current : null;
                }

                current = parent;
            }

            return null;
        }
    }
}
