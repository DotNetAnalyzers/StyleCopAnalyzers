namespace StyleCop.Analyzers.SpacingRules
{
    using System;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Formatting;
    using Microsoft.CodeAnalysis.Text;

    internal static class SpacingExtensions
    {
        public static bool IsMissingOrDefault(this SyntaxToken token)
        {
            return token.IsKind(SyntaxKind.None)
                || token.IsMissing;
        }

        public static string GetTextBetweenTokens(this SourceText text, SyntaxToken token1, SyntaxToken token2)
        {
            return (token1.RawKind == 0) ? text.ToString(TextSpan.FromBounds(0, token2.SpanStart)) : text.ToString(TextSpan.FromBounds(token1.Span.End, token2.SpanStart));
        }

        public static SyntaxToken WithoutLeadingWhitespace(this SyntaxToken token, bool removeEndOfLineTrivia = false)
        {
            if (!token.HasLeadingTrivia)
            {
                return token;
            }

            return token.WithLeadingTrivia(token.LeadingTrivia.WithoutWhitespace(removeEndOfLineTrivia));
        }

        public static SyntaxToken WithoutTrailingWhitespace(this SyntaxToken token, bool removeEndOfLineTrivia = false)
        {
            if (!token.HasTrailingTrivia)
            {
                return token;
            }

            return token.WithTrailingTrivia(token.TrailingTrivia.WithoutWhitespace(removeEndOfLineTrivia));
        }

        public static SyntaxToken WithoutWhitespace(this SyntaxToken token, bool removeEndOfLineTrivia = false)
        {
            return token.WithoutLeadingWhitespace(removeEndOfLineTrivia).WithoutTrailingWhitespace(removeEndOfLineTrivia);
        }

        public static SyntaxTriviaList WithoutWhitespace(this SyntaxTriviaList syntaxTriviaList, bool removeEndOfLineTrivia = false)
        {
            if (syntaxTriviaList.Count == 0)
            {
                return syntaxTriviaList;
            }

            var trivia = syntaxTriviaList.Where(i => !i.IsKind(SyntaxKind.WhitespaceTrivia));
            if (removeEndOfLineTrivia)
            {
                trivia = trivia.Where(i => !i.IsKind(SyntaxKind.EndOfLineTrivia));
            }

            return SyntaxFactory.TriviaList(trivia);
        }

        /// <summary>
        /// Removes the leading and trailing trivia associated with a syntax token.
        /// </summary>
        /// <param name="token">The syntax token to remove trivia from.</param>
        /// <returns>A copy of the input syntax token with leading and trailing trivia removed.</returns>
        public static SyntaxToken WithoutTrivia(this SyntaxToken token)
        {
            return token.WithLeadingTrivia(default(SyntaxTriviaList)).WithTrailingTrivia(default(SyntaxTriviaList));
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
                StructuredTriviaSyntax structure = trivia.GetStructure() as StructuredTriviaSyntax;
                if (structure != null)
                {
                    result = SyntaxFactory.Trivia(structure.WithoutFormatting());
                }
            }

            return WithoutFormattingImpl(trivia);
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
    }
}
