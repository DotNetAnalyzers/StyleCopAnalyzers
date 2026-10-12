// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.DocumentationRules
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
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Text;
    using StyleCop.Analyzers.Helpers;

    /// <summary>
    /// Implements a code fix for <see cref="SA1626CodeFixProvider"/>.
    /// </summary>
    /// <remarks>
    /// <para>To fix a violation of this rule, remove a slash from the beginning of the comment so that it begins with
    /// only two slashes. When a single-line comment sits between the lines of a documentation comment, the code fix
    /// instead turns that comment into documentation lines.</para>
    /// </remarks>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1626CodeFixProvider))]
    [Shared]
    internal class SA1626CodeFixProvider : CodeFixProvider
    {
        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds { get; }
            = ImmutableArray.Create(SA1626SingleLineCommentsMustNotUseDocumentationStyleSlashes.DiagnosticId);

        /// <inheritdoc/>
        public override FixAllProvider GetFixAllProvider()
        {
            return FixAll.Instance;
        }

        /// <inheritdoc/>
        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            foreach (Diagnostic diagnostic in context.Diagnostics)
            {
                var strayComments = GetStrayComments(root, diagnostic);
                var title = strayComments.IsEmpty ? DocumentationResources.SA1626CodeFix : DocumentationResources.SA1626CodeFixStrayComment;
                context.RegisterCodeFix(
                    CodeAction.Create(
                        title,
                        cancellationToken => GetTransformedDocumentAsync(context.Document, diagnostic, strayComments, cancellationToken),
                        nameof(SA1626CodeFixProvider)),
                    diagnostic);
            }
        }

        /// <summary>
        /// Gets the single-line comments that sit between the documentation comment lines of one declaration and
        /// directly precede the documentation lines reported by <paramref name="diagnostic"/>. Such a comment is a
        /// documentation line that lost a slash.
        /// </summary>
        /// <returns>The stray comments, or an empty array if the diagnostic has a different layout.</returns>
        private static ImmutableArray<SyntaxTrivia> GetStrayComments(SyntaxNode root, Diagnostic diagnostic)
        {
            if (root == null)
            {
                return ImmutableArray<SyntaxTrivia>.Empty;
            }

            var reported = root.FindTrivia(diagnostic.Location.SourceSpan.Start);
            if (!reported.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia))
            {
                return ImmutableArray<SyntaxTrivia>.Empty;
            }

            var leadingTrivia = reported.Token.LeadingTrivia;
            var index = leadingTrivia.IndexOf(reported);
            if (index < 0)
            {
                return ImmutableArray<SyntaxTrivia>.Empty;
            }

            var comments = ImmutableArray.CreateBuilder<SyntaxTrivia>();
            var position = SkipWhitespaceBackwards(leadingTrivia, index - 1);

            // The reported documentation starts on the line after the last stray comment.
            if (position < 0 || !leadingTrivia[position].IsKind(SyntaxKind.EndOfLineTrivia))
            {
                return ImmutableArray<SyntaxTrivia>.Empty;
            }

            position--;
            while (true)
            {
                position = SkipWhitespaceBackwards(leadingTrivia, position);
                if (position < 0 || !IsPlainSingleLineComment(leadingTrivia[position]))
                {
                    return ImmutableArray<SyntaxTrivia>.Empty;
                }

                comments.Add(leadingTrivia[position]);
                position = SkipWhitespaceBackwards(leadingTrivia, position - 1);
                if (position < 0)
                {
                    return ImmutableArray<SyntaxTrivia>.Empty;
                }

                var previous = leadingTrivia[position];
                if (previous.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia))
                {
                    // Documentation that is not itself reported is a real documentation comment.
                    var structure = (DocumentationCommentTriviaSyntax)previous.GetStructure();
                    if (structure.Content.All(x => x.IsKind(SyntaxKind.XmlText)))
                    {
                        return ImmutableArray<SyntaxTrivia>.Empty;
                    }

                    // The comments were collected bottom-up; the text changes must be in source order.
                    comments.Reverse();
                    return comments.ToImmutable();
                }

                // Only an end-of-line separates this comment from the one above; a second one would be a blank line.
                if (!previous.IsKind(SyntaxKind.EndOfLineTrivia))
                {
                    return ImmutableArray<SyntaxTrivia>.Empty;
                }

                position--;
            }
        }

        private static int SkipWhitespaceBackwards(SyntaxTriviaList trivia, int position)
        {
            while (position >= 0 && trivia[position].IsKind(SyntaxKind.WhitespaceTrivia))
            {
                position--;
            }

            return position;
        }

        private static bool IsPlainSingleLineComment(SyntaxTrivia trivia)
        {
            return trivia.IsKind(SyntaxKind.SingleLineCommentTrivia)
                && !trivia.ToString().StartsWith("///", System.StringComparison.Ordinal);
        }

        private static TextChange GetDiagnosticChange(Diagnostic diagnostic)
        {
            return new TextChange(new TextSpan(diagnostic.Location.SourceSpan.Start, 1), string.Empty);
        }

        private static TextChange GetStrayCommentChange(SyntaxTrivia comment)
        {
            return new TextChange(new TextSpan(comment.SpanStart + 2, 0), "/");
        }

        private static async Task<Document> GetTransformedDocumentAsync(Document document, Diagnostic diagnostic, ImmutableArray<SyntaxTrivia> strayComments, CancellationToken cancellationToken)
        {
            var text = await document.GetTextAsync(cancellationToken).ConfigureAwait(false);

            if (strayComments.IsEmpty)
            {
                return document.WithText(text.WithChanges(GetDiagnosticChange(diagnostic)));
            }

            var changes = new List<TextChange>();
            foreach (var comment in strayComments)
            {
                changes.Add(GetStrayCommentChange(comment));
            }

            return document.WithText(text.WithChanges(changes));
        }

        private class FixAll : DocumentBasedFixAllProvider
        {
            public static FixAllProvider Instance { get; } =
                new FixAll();

            protected override string CodeActionTitle =>
                DocumentationResources.SA1626CodeFix;

            protected override async Task<SyntaxNode> FixAllInDocumentAsync(FixAllContext fixAllContext, Document document, ImmutableArray<Diagnostic> diagnostics)
            {
                if (diagnostics.IsEmpty)
                {
                    return null;
                }

                var text = await document.GetTextAsync().ConfigureAwait(false);
                var root = await document.GetSyntaxRootAsync().ConfigureAwait(false);

                List<TextChange> changes = new List<TextChange>();
                HashSet<int> strayCommentPositions = new HashSet<int>();
                foreach (var diagnostic in diagnostics)
                {
                    var strayComments = GetStrayComments(root, diagnostic);
                    if (strayComments.IsEmpty)
                    {
                        changes.Add(GetDiagnosticChange(diagnostic));
                        continue;
                    }

                    // Several diagnostics of one documentation comment share the same stray comments.
                    foreach (var comment in strayComments)
                    {
                        if (strayCommentPositions.Add(comment.SpanStart))
                        {
                            changes.Add(GetStrayCommentChange(comment));
                        }
                    }
                }

                changes.Sort((left, right) => left.Span.Start.CompareTo(right.Span.Start));

                var tree = await document.GetSyntaxTreeAsync().ConfigureAwait(false);
                return await tree.WithChangedText(text.WithChanges(changes)).GetRootAsync().ConfigureAwait(false);
            }
        }
    }
}
