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
    using Microsoft.CodeAnalysis.Options;
    using Microsoft.CodeAnalysis.Text;
    using StyleCop.Analyzers.Helpers;

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(IndentationCodeFixProvider))]
    [Shared]
    internal class IndentationCodeFixProvider : CodeFixProvider
    {
        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds { get; } =
            ImmutableArray.Create(SA1137ElementsShouldHaveTheSameIndentation.DiagnosticId);

        /// <inheritdoc/>
        public sealed override FixAllProvider GetFixAllProvider() =>
            FixAll.Instance;

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
            var text = await document.GetTextAsync(cancellationToken).ConfigureAwait(false);

            TextChange textChange;
            if (!TryGetTextChange(diagnostic, syntaxRoot, text, document.Project.Solution.Workspace.Options, out textChange))
            {
                return document;
            }

            return document.WithText(text.WithChanges(textChange));
        }

        private static bool TryGetTextChange(Diagnostic diagnostic, SyntaxNode syntaxRoot, SourceText text, OptionSet options, out TextChange textChange)
        {
            string replacement;
            if (!diagnostic.Properties.TryGetValue(SA1137ElementsShouldHaveTheSameIndentation.ExpectedIndentationKey, out replacement))
            {
                textChange = default;
                return false;
            }

            var trivia = syntaxRoot.FindTrivia(diagnostic.Location.SourceSpan.Start);

            TextSpan originalSpan;
            if (trivia == default)
            {
                // The warning was reported on a token because the line is not indented
                originalSpan = new TextSpan(diagnostic.Location.SourceSpan.Start, 0);
            }
            else
            {
                originalSpan = trivia.Span;
            }

            var token = syntaxRoot.FindToken(diagnostic.Location.SourceSpan.Start);
            if ((token.IsKind(SyntaxKind.CloseBraceToken) || token.IsKind(SyntaxKind.CloseBracketToken))
                && !token.IsFirstInLine())
            {
                // Only replace whitespace immediately before the token, preserving any preceding comments.
                var start = token.SpanStart;
                var lineStart = text.Lines.GetLineFromPosition(start).Start;
                while (start > lineStart && char.IsWhiteSpace(text[start - 1]))
                {
                    start--;
                }

                originalSpan = TextSpan.FromBounds(start, token.SpanStart);
                replacement = FormattingHelper.GetEndOfLineForCodeFix(token, text, options).ToFullString() + replacement;
            }

            textChange = new TextChange(originalSpan, replacement);
            return true;
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
                var text = await document.GetTextAsync().ConfigureAwait(false);

                List<TextChange> changes = new List<TextChange>();

                foreach (var diagnostic in diagnostics)
                {
                    // Some compilers report the same diagnostic twice for positional records and in files with
                    // top-level statements (https://github.com/dotnet/roslyn/issues/53136 and
                    // https://github.com/dotnet/roslyn/issues/58561), and overlapping text changes are not allowed.
                    if (TryGetTextChange(diagnostic, syntaxRoot, text, document.Project.Solution.Workspace.Options, out var textChange) && !changes.Contains(textChange))
                    {
                        changes.Add(textChange);
                    }
                }

                changes.Sort((left, right) => left.Span.Start.CompareTo(right.Span.Start));

                return await document.WithText(text.WithChanges(changes)).GetSyntaxRootAsync(fixAllContext.CancellationToken).ConfigureAwait(false);
            }
        }
    }
}
