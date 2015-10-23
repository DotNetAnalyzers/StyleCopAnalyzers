// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Helpers
{
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Formatting;

    internal static class FormattingHelper
    {
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
