// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.ReadabilityRules
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
    using Microsoft.CodeAnalysis.Text;
    using Settings.ObjectModel;

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(IndentationCodeFixProvider))]
    [Shared]
    internal class IndentationCodeFixProvider : CodeFixProvider
    {
        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds { get; } =
            ImmutableArray.Create(
                SA1137ElementsShouldHaveTheSameIndentation.SA1137DiagnosticId,
                SA1137ElementsShouldHaveTheSameIndentation.SA1138DiagnosticId);

        /// <inheritdoc/>
        public sealed override FixAllProvider GetFixAllProvider() =>
            null;

        /// <inheritdoc/>
        public override Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            foreach (var diagnostic in context.Diagnostics)
            {
                context.RegisterCodeFix(
                    CodeAction.Create(
                        ReadabilityResources.IndentationCodeFix,
                        cancellationToken => GetTransformedDocumentAsync(context.Document, diagnostic, cancellationToken),
                        nameof(IndentationCodeFixProvider)),
                    diagnostic);
            }

            return SpecializedTasks.CompletedTask;
        }

        private static async Task<Document> GetTransformedDocumentAsync(Document document, Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            var syntaxRoot = await document.GetSyntaxRootAsync().ConfigureAwait(false);

            StyleCopSettings settings = SettingsHelper.GetStyleCopSettings(document.Project.AnalyzerOptions, cancellationToken);
            ImmutableArray<TextChange> textChanges = await GetTextChangesAsync(diagnostic, syntaxRoot, settings.Indentation, cancellationToken).ConfigureAwait(false);
            if (textChanges.IsEmpty)
            {
                return document;
            }

            var text = await document.GetTextAsync(cancellationToken).ConfigureAwait(false);
            return document.WithText(text.WithChanges(textChanges));
        }

        private static async Task<ImmutableArray<TextChange>> GetTextChangesAsync(Diagnostic diagnostic, SyntaxNode syntaxRoot, IndentationSettings indentationSettings, CancellationToken cancellationToken)
        {
            string replacement;
            if (!diagnostic.Properties.TryGetValue(SA1137ElementsShouldHaveTheSameIndentation.ExpectedIndentationKey, out replacement))
            {
                return ImmutableArray<TextChange>.Empty;
            }

            SyntaxTrivia trivia = syntaxRoot.FindTrivia(diagnostic.Location.SourceSpan.Start);
            SyntaxToken token = trivia != default(SyntaxTrivia) ? trivia.Token : syntaxRoot.FindToken(diagnostic.Location.SourceSpan.Start, findInsideTrivia: true);
            SyntaxNode node = GetNodeForAdjustment(token);

            TextSpan originalSpan;
            if (trivia == default(SyntaxTrivia))
            {
                // The warning was reported on a token because the line is not indented
                originalSpan = new TextSpan(diagnostic.Location.SourceSpan.Start, 0);
            }
            else
            {
                originalSpan = trivia.Span;
            }

            FileLinePositionSpan fullSpan = syntaxRoot.SyntaxTree.GetLineSpan(node.FullSpan, cancellationToken);
            if (fullSpan.StartLinePosition.Line == fullSpan.EndLinePosition.Line)
            {
                return ImmutableArray.Create(new TextChange(originalSpan, replacement));
            }

            SyntaxTree tree = node.SyntaxTree;
            SourceText sourceText = await tree.GetTextAsync(cancellationToken).ConfigureAwait(false);

            int originalIndentation = GetIndentationWidth(indentationSettings, sourceText.ToString(originalSpan));
            int newIndentation = GetIndentationWidth(indentationSettings, replacement);

            ImmutableArray<TextSpan> excludedSpans = SyntaxTreeHelpers.GetExcludedSpans(node);
            TextLineCollection lines = sourceText.Lines;

            // For each line in the full span of the syntax node:
            // 1. If the line is indented less than originalIndentation, ignore the line
            // 2. If the indentation characters are not located within the full span, ignore the line
            // 2. If the indentation characters of the line overlap with an excluded span, ignore the line
            // 3. Replace the first original.Length characters on the line with replacement
            ImmutableArray<TextChange>.Builder builder = ImmutableArray.CreateBuilder<TextChange>();
            for (int i = fullSpan.StartLinePosition.Line; i <= fullSpan.EndLinePosition.Line; i++)
            {
                TextLine line = lines[i];
                string lineText = sourceText.ToString(line.Span);

                int indentationCount;
                int indentationWidth = GetIndentationWidth(indentationSettings, lineText, out indentationCount);
                if (indentationWidth < originalIndentation)
                {
                    continue;
                }

                if (indentationCount == line.Span.Length)
                {
                    // The line is just whitespace
                    continue;
                }

                TextSpan indentationSpan = new TextSpan(line.Start, indentationCount);
                if (indentationSpan.Start >= node.FullSpan.End)
                {
                    // The line does not contain any non-whitespace content which is part of the full span of the node
                    continue;
                }

                if (!node.FullSpan.Contains(indentationSpan))
                {
                    // The indentation of the line is not part of the full span of the node
                    continue;
                }

                if (IsExcluded(excludedSpans, indentationSpan))
                {
                    // The line indentation is partially- or fully-excluded from adjustments
                    continue;
                }

                if (originalIndentation == indentationWidth)
                {
                    builder.Add(new TextChange(indentationSpan, replacement));
                }
                else if (newIndentation > originalIndentation)
                {
                    // TODO: This needs to handle UseTabs setting
                    builder.Add(new TextChange(new TextSpan(indentationSpan.End, 0), new string(' ', newIndentation - originalIndentation)));
                }
                else if (newIndentation < originalIndentation)
                {
                    builder.Add(new TextChange(indentationSpan, IndentationHelper.GenerateIndentationString(indentationSettings, indentationWidth + (newIndentation - originalIndentation))));
                }
            }

            return builder.ToImmutable();
        }

        private static SyntaxNode GetNodeForAdjustment(SyntaxToken token)
        {
            return token.Parent;
        }

        private static int GetIndentationWidth(IndentationSettings indentationSettings, string text)
        {
            int ignored;
            return GetIndentationWidth(indentationSettings, text, out ignored);
        }

        private static int GetIndentationWidth(IndentationSettings indentationSettings, string text, out int count)
        {
            int tabSize = indentationSettings.TabSize;
            int indentationWidth = 0;
            for (int i = 0; i < text.Length; i++)
            {
                switch (text[i])
                {
                case ' ':
                    indentationWidth++;
                    break;

                case '\t':
                    indentationWidth = tabSize * ((indentationWidth / tabSize) + 1);
                    break;

                default:
                    count = i;
                    return indentationWidth;
                }
            }

            count = text.Length;
            return indentationWidth;
        }

        private static bool IsExcluded(ImmutableArray<TextSpan> excludedSpans, TextSpan textSpan)
        {
            int index = excludedSpans.BinarySearch(textSpan);
            if (index > 0)
            {
                return true;
            }

            int nextLarger = ~index;
            if (nextLarger > 0 && excludedSpans[nextLarger - 1].OverlapsWith(textSpan))
            {
                return true;
            }

            if (nextLarger < excludedSpans.Length - 1 && excludedSpans[nextLarger].OverlapsWith(textSpan))
            {
                return true;
            }

            return false;
        }

        private class FixAll : DocumentBasedFixAllProvider
        {
            public static FixAllProvider Instance { get; } =
                new FixAll();

            protected override string CodeActionTitle =>
                ReadabilityResources.IndentationCodeFix;

            protected override async Task<SyntaxNode> FixAllInDocumentAsync(FixAllContext fixAllContext, Document document, ImmutableArray<Diagnostic> diagnostics)
            {
                if (diagnostics.IsEmpty)
                {
                    return null;
                }

                var syntaxRoot = await document.GetSyntaxRootAsync().ConfigureAwait(false);
                StyleCopSettings settings = SettingsHelper.GetStyleCopSettings(document.Project.AnalyzerOptions, fixAllContext.CancellationToken);

                List<TextChange> changes = new List<TextChange>();

                foreach (var diagnostic in diagnostics)
                {
                    changes.AddRange(await GetTextChangesAsync(diagnostic, syntaxRoot, settings.Indentation, fixAllContext.CancellationToken).ConfigureAwait(false));
                }

                changes.Sort((left, right) => left.Span.Start.CompareTo(right.Span.Start));

                var text = await document.GetTextAsync().ConfigureAwait(false);
                return await document.WithText(text.WithChanges(changes)).GetSyntaxRootAsync(fixAllContext.CancellationToken).ConfigureAwait(false);
            }
        }
    }
}
