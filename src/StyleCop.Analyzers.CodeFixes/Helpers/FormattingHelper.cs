// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Helpers
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Formatting;
    using Microsoft.CodeAnalysis.Options;
    using Microsoft.CodeAnalysis.Text;

    internal static class FormattingHelper
    {
        /// <summary>
        /// Retrieves the appropriate end-of-line trivia for use in code fixes at the specified token location.
        /// </summary>
        /// <remarks><para>This method attempts to preserve the existing line ending style near the specified
        /// token. If no suitable end-of-line trivia is found, a default carriage return and line feed (CRLF) is
        /// returned.</para></remarks>
        /// <param name="token">The syntax token for which to determine the end-of-line trivia.</param>
        /// <param name="options">The options to use for formatting. This value is only used if the document does not already contain line endings.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the asynchronous operation.</param>
        /// <returns>A <see cref="SyntaxTrivia"/> representing the end-of-line trivia suitable for code fixes at the given token
        /// position.</returns>
        public static async Task<SyntaxTrivia> GetEndOfLineForCodeFixAsync(SyntaxToken token, OptionSet options, CancellationToken cancellationToken)
        {
            if (TryGetPrecedingEndOfLineTrivia(token, out var precedingEndOfLine))
            {
                return precedingEndOfLine;
            }

            var text = await token.SyntaxTree.GetTextAsync(cancellationToken).ConfigureAwait(false);
            return GetEndOfLineForCodeFix(token, text, options);
        }

        /// <summary>
        /// Retrieves the appropriate end-of-line trivia for use in code fixes, based on the position of the specified
        /// syntax token and the provided source text.
        /// </summary>
        /// <remarks><para>This method examines the token's position and surrounding lines to select the most
        /// contextually appropriate end-of-line trivia, ensuring consistency with the existing file
        /// formatting.</para></remarks>
        /// <param name="token">The syntax token for which to determine the end-of-line trivia.</param>
        /// <param name="text">The source text containing the token. Used to identify line boundaries and end-of-line characters.</param>
        /// <param name="options">The options to use for formatting. This value is only used if the document does not already contain line endings.</param>
        /// <returns>A <see cref="SyntaxTrivia"/> representing the end-of-line trivia suitable for code fixes. If no specific
        /// trivia is found, returns a default carriage return and line feed trivia.</returns>
        public static SyntaxTrivia GetEndOfLineForCodeFix(SyntaxToken token, SourceText text, OptionSet options)
        {
            if (TryGetPrecedingEndOfLineTrivia(token, out var precedingEndOfLine))
            {
                return precedingEndOfLine;
            }

            var lineNumber = token.GetLine();
            if (lineNumber >= 0 && lineNumber < text.Lines.Count && GetEndOfLineTriviaForLine(text.Lines[lineNumber]) is { } followingTrivia)
            {
                return followingTrivia;
            }

            if (lineNumber > 0 && GetEndOfLineTriviaForLine(text.Lines[lineNumber - 1]) is { } precedingTrivia)
            {
                return precedingTrivia;
            }

            return SyntaxFactory.SyntaxTrivia(SyntaxKind.EndOfLineTrivia, options.GetOption(FormattingOptions.NewLine, LanguageNames.CSharp));

            static SyntaxTrivia? GetEndOfLineTriviaForLine(TextLine textLine)
            {
                return (textLine.EndIncludingLineBreak - textLine.End) switch
                {
                    2 => SyntaxFactory.CarriageReturnLineFeed,
                    1 => textLine.Text[textLine.End] switch
                    {
                        '\n' => SyntaxFactory.LineFeed,
                        char c => SyntaxFactory.EndOfLine(c.ToString()),
                    },
                    _ => null,
                };
            }
        }

        public static SyntaxTrivia GetNewLineTrivia(Document document)
        {
            return SyntaxFactory.SyntaxTrivia(SyntaxKind.EndOfLineTrivia, document.Project.Solution.Workspace.Options.GetOption(FormattingOptions.NewLine, LanguageNames.CSharp));
        }

        /// <summary>
        /// Transforms a <see cref="SyntaxNode"/> to ensure no formatting operations will be applied to the node or any
        /// of its descendants when a <see cref="CodeAction"/> is applied.
        /// </summary>
        /// <typeparam name="TNode">The specific type of syntax node.</typeparam>
        /// <param name="node">The syntax node.</param>
        /// <returns>
        /// A syntax node which is equivalent to the input <paramref name="node"/>, but which will not be subject to
        /// automatic code formatting operations when applied as part of a <see cref="CodeAction"/>.
        /// </returns>
        public static TNode WithoutFormatting<TNode>(this TNode node)
            where TNode : SyntaxNode
        {
            /* Strategy:
             *  1. Transform all descendants of the node (nodes, tokens, and trivia), but not the node itself
             *  2. Transform the resulting node itself
             */
            TNode result = node.ReplaceSyntax(
                node.DescendantNodes(descendIntoTrivia: true),
                (originalNode, rewrittenNode) => WithoutFormattingImpl(rewrittenNode),
                node.DescendantTokens(descendIntoTrivia: true),
                (originalToken, rewrittenToken) => WithoutFormattingImpl(rewrittenToken),
                node.DescendantTrivia(descendIntoTrivia: true),
                (originalTrivia, rewrittenTrivia) => WithoutFormattingImpl(rewrittenTrivia));

            return WithoutFormattingImpl(result);
        }

        /// <summary>
        /// Transforms a <see cref="SyntaxToken"/> to ensure no formatting operations will be applied to the token or
        /// any of its descendants when a <see cref="CodeAction"/> is applied.
        /// </summary>
        /// <param name="token">The syntax token.</param>
        /// <returns>
        /// A syntax token which is equivalent to the input <paramref name="token"/>, but which will not be subject to
        /// automatic code formatting operations when applied as part of a <see cref="CodeAction"/>.
        /// </returns>
        public static SyntaxToken WithoutFormatting(this SyntaxToken token)
        {
            /* Strategy:
             *  1. Replace the leading and trailing trivia with copies that will not be reformatted
             *  2. Remove formatting from the resulting token
             */
            SyntaxTriviaList newLeadingTrivia = token.LeadingTrivia.Select(WithoutFormatting).ToSyntaxTriviaList();
            SyntaxTriviaList newTrailingTrivia = token.TrailingTrivia.Select(WithoutFormatting).ToSyntaxTriviaList();
            return WithoutFormattingImpl(token.WithLeadingTrivia(newLeadingTrivia).WithTrailingTrivia(newTrailingTrivia));
        }

        /// <summary>
        /// Transforms a <see cref="SyntaxTrivia"/> to ensure no formatting operations will be applied to the trivia or
        /// any of its descendants when a <see cref="CodeAction"/> is applied.
        /// </summary>
        /// <param name="trivia">The syntax trivia.</param>
        /// <returns>
        /// A syntax trivia which is equivalent to the input <paramref name="trivia"/>, but which will not be subject to
        /// automatic code formatting operations when applied as part of a <see cref="CodeAction"/>.
        /// </returns>
        public static SyntaxTrivia WithoutFormatting(this SyntaxTrivia trivia)
        {
            /* Strategy
             *  1. Replace the structure, if any, with a structure that will not be reformatted
             *  2. Remove formatting from the resulting trivia
             */
            SyntaxTrivia result = trivia;
            if (trivia.HasStructure)
            {
                // GetStructure() returns SyntaxNode instead of StructuredTriviaSyntax. For C# code, this should always
                // be an actual instance of StructuredTriviaSyntax, but we handle the case where it is not by leaving
                // the structure node unaltered rather than throwing some sort of exception.
                if (trivia.GetStructure() is StructuredTriviaSyntax structure)
                {
                    result = SyntaxFactory.Trivia(structure.WithoutFormatting());
                }
            }

            return WithoutFormattingImpl(result);
        }

        /// <summary>
        /// Remove formatting from a single <see cref="SyntaxNode"/>. The descendants of the node, including its leading
        /// and trailing trivia, are not altered by this method.
        /// </summary>
        /// <remarks>
        /// <para>This method is responsible for the single-node transformation as part of
        /// <see cref="O:StyleCop.Analyzers.SpacingRules.SpacingExtensions.WithoutFormatting"/>.</para>
        /// </remarks>
        /// <typeparam name="TNode">The specific type of syntax node.</typeparam>
        /// <param name="node">The syntax node.</param>
        /// <returns>
        /// A syntax node which is equivalent to the input <paramref name="node"/>, but which will not be subject to
        /// automatic code formatting operations when applied as part of a <see cref="CodeAction"/>.
        /// </returns>
        private static TNode WithoutFormattingImpl<TNode>(TNode node)
            where TNode : SyntaxNode
        {
            return node.WithoutAnnotations(Formatter.Annotation, SyntaxAnnotation.ElasticAnnotation);
        }

        /// <summary>
        /// Remove formatting from a single <see cref="SyntaxToken"/>. The descendants of the token, including its
        /// leading and trailing trivia, are not altered by this method.
        /// </summary>
        /// <remarks>
        /// <para>This method is responsible for the single-token transformation as part of
        /// <see cref="O:StyleCop.Analyzers.SpacingRules.SpacingExtensions.WithoutFormatting"/>.</para>
        /// </remarks>
        /// <param name="token">The syntax token.</param>
        /// <returns>
        /// A syntax token which is equivalent to the input <paramref name="token"/>, but which will not be subject to
        /// automatic code formatting operations when applied as part of a <see cref="CodeAction"/>.
        /// </returns>
        private static SyntaxToken WithoutFormattingImpl(SyntaxToken token)
        {
            return token.WithoutAnnotations(Formatter.Annotation, SyntaxAnnotation.ElasticAnnotation);
        }

        /// <summary>
        /// Remove formatting from a single <see cref="SyntaxTrivia"/>. The descendants of the trivia, including any
        /// structure it contains, are not altered by this method.
        /// </summary>
        /// <remarks>
        /// <para>This method is responsible for the single-trivia transformation as part of
        /// <see cref="O:StyleCop.Analyzers.SpacingRules.SpacingExtensions.WithoutFormatting"/>.</para>
        /// </remarks>
        /// <param name="trivia">The syntax trivia.</param>
        /// <returns>
        /// A syntax trivia which is equivalent to the input <paramref name="trivia"/>, but which will not be subject to
        /// automatic code formatting operations when applied as part of a <see cref="CodeAction"/>.
        /// </returns>
        private static SyntaxTrivia WithoutFormattingImpl(SyntaxTrivia trivia)
        {
            return trivia.WithoutAnnotations(Formatter.Annotation, SyntaxAnnotation.ElasticAnnotation);
        }

        /// <summary>
        /// Returns the closest end of line trivia preceding the <paramref name="token"/>.
        /// This currently only looks immediately before the specified token.
        /// </summary>
        /// <param name="token">The token to process.</param>
        /// <param name="trivia">When this method returns, contains the closest preceding end of line trivia, if found; otherwise, the default value.</param>
        /// <returns><see langword="true"/> if an end of line trivia was found; otherwise, <see langword="false"/>.</returns>
        private static bool TryGetPrecedingEndOfLineTrivia(this SyntaxToken token, out SyntaxTrivia trivia)
        {
            var leadingTrivia = token.LeadingTrivia;
            for (var i = leadingTrivia.Count - 1; i >= 0; i--)
            {
                if (leadingTrivia[i].IsKind(SyntaxKind.EndOfLineTrivia))
                {
                    trivia = leadingTrivia[i];
                    return true;
                }
            }

            var prevToken = token.GetPreviousToken();
            var prevTrailingTrivia = prevToken.TrailingTrivia;
            for (var i = prevTrailingTrivia.Count - 1; i >= 0; i--)
            {
                if (prevTrailingTrivia[i].IsKind(SyntaxKind.EndOfLineTrivia))
                {
                    trivia = prevTrailingTrivia[i];
                    return true;
                }
            }

            trivia = default;
            return false;
        }
    }
}
