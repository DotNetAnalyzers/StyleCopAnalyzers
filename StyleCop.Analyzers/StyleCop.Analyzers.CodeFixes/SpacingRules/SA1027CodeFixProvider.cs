// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.SpacingRules
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Composition;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using StyleCop.Analyzers.Helpers;
    using StyleCop.Analyzers.Helpers.ObjectPools;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Text;
    using StyleCop.Analyzers.Settings.ObjectModel;

    /// <summary>
    /// Implements a code fix for <see cref="SA1027UseTabsCorrectly"/>.
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1027CodeFixProvider))]
    [Shared]
    internal class SA1027CodeFixProvider : CodeFixProvider
    {
        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds { get; } =
            ImmutableArray.Create(SA1027UseTabsCorrectly.DiagnosticId);

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
                        SpacingResources.SA1027CodeFix,
                        cancellationToken => GetTransformedDocumentAsync(context.Document, diagnostic, cancellationToken),
                        nameof(SA1027CodeFixProvider)),
                    diagnostic);
            }

            return SpecializedTasks.CompletedTask;
        }

        private static async Task<Document> GetTransformedDocumentAsync(Document document, Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            var settings = SettingsHelper.GetStyleCopSettings(document.Project.AnalyzerOptions, cancellationToken);
            SourceText sourceText = await document.GetTextAsync(cancellationToken).ConfigureAwait(false);
            return document.WithText(sourceText.WithChanges(FixDiagnostic(settings.Indentation, sourceText, diagnostic)));
        }

        private static TextChange FixDiagnostic(IndentationSettings indentationSettings, SourceText sourceText, Diagnostic diagnostic)
        {
            TextSpan span = diagnostic.Location.SourceSpan;

            TextLine startLine = sourceText.Lines.GetLineFromPosition(span.Start);

            bool useTabs = false;
            string behavior;
            if (diagnostic.Properties.TryGetValue(SA1027UseTabsCorrectly.BehaviorKey, out behavior))
            {
                useTabs = behavior == SA1027UseTabsCorrectly.ConvertToTabsBehavior;
            }

            string text = sourceText.ToString(TextSpan.FromBounds(startLine.Start, span.End));
            StringBuilder replacement = StringBuilderPool.Allocate();
            int spaceCount = 0;
            int column = 0;
            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                if (c == '\t')
                {
                    var offsetWithinTabColumn = column % indentationSettings.TabSize;
                    var tabWidth = indentationSettings.TabSize - offsetWithinTabColumn;

                    if (i >= span.Start - startLine.Start)
                    {
                        if (useTabs)
                        {
                            replacement.Length = replacement.Length - spaceCount;
                            replacement.Append('\t');
                            spaceCount = 0;
                        }
                        else
                        {
                            replacement.Append(' ', tabWidth);
                        }
                    }

                    column += tabWidth;
                }
                else
                {
                    if (i >= span.Start - startLine.Start)
                    {
                        replacement.Append(c);
                        if (c == ' ')
                        {
                            spaceCount++;
                            if (useTabs)
                            {
                                // Note that we account for column not yet being incremented
                                var offsetWithinTabColumn = (column + 1) % indentationSettings.TabSize;
                                if (offsetWithinTabColumn == 0)
                                {
                                    // We reached a tab stop.
                                    replacement.Length = replacement.Length - spaceCount;
                                    replacement.Append('\t');
                                    spaceCount = 0;
                                }
                            }
                        }
                        else
                        {
                            spaceCount = 0;
                        }
                    }

                    if (c == '\r' || c == '\n')
                    {
                        // Handle newlines. We can ignore CR/LF/CRLF issues because we are only tracking column position
                        // in a line, and not the line numbers themselves.
                        column = 0;
                        spaceCount = 0;
                    }
                    else
                    {
                        column++;
                    }
                }
            }

            return new TextChange(span, StringBuilderPool.ReturnAndFree(replacement));
        }

        private class FixAll : DocumentBasedFixAllProvider
        {
            public static FixAllProvider Instance { get; }
                = new FixAll();

            protected override string CodeActionTitle
                => SpacingResources.SA1027CodeFix;

            protected override async Task<SyntaxNode> FixAllInDocumentAsync(FixAllContext fixAllContext, Document document, ImmutableArray<Diagnostic> diagnostics)
            {
                if (diagnostics.IsEmpty)
                {
                    return null;
                }

                var settings = SettingsHelper.GetStyleCopSettings(document.Project.AnalyzerOptions, fixAllContext.CancellationToken);
                SourceText sourceText = await document.GetTextAsync(fixAllContext.CancellationToken).ConfigureAwait(false);

                List<TextChange> changes = new List<TextChange>();
                foreach (var diagnostic in diagnostics)
                {
                    changes.Add(FixDiagnostic(settings.Indentation, sourceText, diagnostic));
                }

                changes.Sort((left, right) => left.Span.Start.CompareTo(right.Span.Start));

                SyntaxTree tree = await document.GetSyntaxTreeAsync(fixAllContext.CancellationToken).ConfigureAwait(false);
                return await tree.WithChangedText(sourceText.WithChanges(changes)).GetRootAsync(fixAllContext.CancellationToken).ConfigureAwait(false);
            }
        }
    }
}
