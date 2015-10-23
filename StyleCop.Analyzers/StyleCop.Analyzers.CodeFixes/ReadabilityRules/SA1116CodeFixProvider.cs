// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.ReadabilityRules
{
    using System.Collections.Immutable;
    using System.Composition;
    using System.Threading;
    using System.Threading.Tasks;
    using Helpers;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Text;

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

            IndentationOptions indentationOptions = IndentationOptions.FromDocument(document);
            SyntaxTriviaList newTrivia =
                SyntaxFactory.TriviaList(
                    SyntaxFactory.CarriageReturnLineFeed,
                    SyntaxFactory.Whitespace(lineText.Substring(0, indentLength) + IndentationHelper.GenerateIndentationString(indentationOptions, 1)));

            SyntaxToken updatedToken = originalToken.WithLeadingTrivia(originalToken.LeadingTrivia.AddRange(newTrivia));
            SyntaxNode updatedRoot = root.ReplaceToken(originalToken, updatedToken);
            return document.WithSyntaxRoot(updatedRoot);
        }
    }
}
