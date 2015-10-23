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
    using Microsoft.CodeAnalysis.Text;

    /// <summary>
    /// Implements a code fix for <see cref="SA1005SingleLineCommentsMustBeginWithSingleSpace"/>.
    /// </summary>
    /// <remarks>
    /// <para>To fix a violation of this rule, ensure that the comment begins with a single space. If the comment is
    /// being used to comment out a line of code, ensure that the comment begins with four forward slashes, in which
    /// case the leading space can be omitted.</para>
    /// </remarks>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1005CodeFixProvider))]
    [Shared]
    internal class SA1005CodeFixProvider : CodeFixProvider
    {
        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds { get; } =
            ImmutableArray.Create(SA1005SingleLineCommentsMustBeginWithSingleSpace.DiagnosticId);

        /// <inheritdoc/>
        public override FixAllProvider GetFixAllProvider()
        {
            return FixAll.Instance;
        }

        /// <inheritdoc/>
        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            foreach (var diagnostic in context.Diagnostics)
            {
                context.RegisterCodeFix(
                    CodeAction.Create(
                        SpacingResources.SA1005CodeFix,
                        cancellationToken => GetTransformedDocumentAsync(context.Document, diagnostic.Location, cancellationToken),
                        nameof(SA1005CodeFixProvider)),
                    diagnostic);
            }
        }

        private static async Task<Document> GetTransformedDocumentAsync(Document document, Location location, CancellationToken cancellationToken)
        {
            var text = await document.GetTextAsync(cancellationToken).ConfigureAwait(false);
            var sourceSpan = location.SourceSpan;

            return document.WithText(text.WithChanges(GetTextChange(text, sourceSpan)));
        }

        private static TextChange GetTextChange(SourceText text, TextSpan sourceSpan)
        {
            var subText = text.GetSubText(sourceSpan).ToString();

            int i = 2;
            for (; i < subText.Length; i++)
            {
                if (!char.IsWhiteSpace(subText[i]))
                {
                    break;
                }
            }

            return new TextChange(new TextSpan(sourceSpan.Start + 2, i - 2), " ");
        }

        private class FixAll : DocumentBasedFixAllProvider
        {
            public static FixAllProvider Instance { get; } =
                new FixAll();

            protected override string CodeActionTitle =>
                SpacingResources.SA1005CodeFix;

            protected override async Task<SyntaxNode> FixAllInDocumentAsync(FixAllContext fixAllContext, Document document)
            {
                var diagnostics = await fixAllContext.GetDocumentDiagnosticsAsync(document).ConfigureAwait(false);
                if (diagnostics.IsEmpty)
                {
                    return null;
                }

                var text = await document.GetTextAsync().ConfigureAwait(false);

                List<TextChange> changes = new List<TextChange>();

                foreach (var diagnostic in diagnostics)
                {
                    var sourceSpan = diagnostic.Location.SourceSpan;
                    changes.Add(GetTextChange(text, sourceSpan));
                }

                changes.Sort((left, right) => left.Span.Start.CompareTo(right.Span.Start));

                var tree = await document.GetSyntaxTreeAsync().ConfigureAwait(false);
                return await tree.WithChangedText(text.WithChanges(changes)).GetRootAsync().ConfigureAwait(false);
            }
        }
    }
}
