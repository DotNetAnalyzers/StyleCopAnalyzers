namespace StyleCop.Analyzers.SpacingRules
{
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal static class SpacingExtensions
    {
        public static SyntaxToken WithoutLeadingWhitespace(this SyntaxToken token, bool removeEndOfLineTrivia = false)
        {
            if (!token.HasLeadingTrivia)
                return token;

            return token.WithLeadingTrivia(token.LeadingTrivia.WithoutWhitespace(removeEndOfLineTrivia));
        }

        public static SyntaxToken WithoutTrailingWhitespace(this SyntaxToken token, bool removeEndOfLineTrivia = false)
        {
            if (!token.HasTrailingTrivia)
                return token;

            return token.WithTrailingTrivia(token.TrailingTrivia.WithoutWhitespace(removeEndOfLineTrivia));
        }

        public static SyntaxToken WithoutWhitespace(this SyntaxToken token, bool removeEndOfLineTrivia = false)
        {
            return token.WithoutLeadingWhitespace(removeEndOfLineTrivia).WithoutTrailingWhitespace(removeEndOfLineTrivia);
        }

        public static SyntaxTriviaList WithoutWhitespace(this SyntaxTriviaList syntaxTriviaList, bool removeEndOfLineTrivia = false)
        {
            if (syntaxTriviaList.Count == 0)
                return syntaxTriviaList;

            var trivia = syntaxTriviaList.Where(i => !i.IsKind(SyntaxKind.WhitespaceTrivia));
            if (removeEndOfLineTrivia)
                trivia = trivia.Where(i => !i.IsKind(SyntaxKind.EndOfLineTrivia));

            return SyntaxFactory.TriviaList(trivia);
        }

        public static TNode WithoutElasticTrivia<TNode>(this TNode node)
            where TNode : SyntaxNode
        {
            return node.ReplaceTrivia(node.DescendantTrivia(), RemoveElasticAnnotation);
        }

        public static SyntaxToken WithoutElasticTrivia(this SyntaxToken token)
        {
            SyntaxTriviaList newLeadingTrivia = token.LeadingTrivia.Select(WithoutElasticTrivia).ToSyntaxTriviaList();
            SyntaxTriviaList newTrailingTrivia = token.TrailingTrivia.Select(WithoutElasticTrivia).ToSyntaxTriviaList();
            return token.WithLeadingTrivia(newLeadingTrivia).WithTrailingTrivia(newTrailingTrivia);
        }

        public static SyntaxTrivia WithoutElasticTrivia(this SyntaxTrivia trivia)
        {
            if (trivia.HasStructure)
            {
                return SyntaxFactory.Trivia(((StructuredTriviaSyntax)trivia.GetStructure()).WithoutElasticTrivia())
                    .WithoutAnnotations(SyntaxAnnotation.ElasticAnnotation);
            }
            else
            {
                return trivia.WithoutAnnotations(SyntaxAnnotation.ElasticAnnotation);
            }
        }

        private static SyntaxTrivia RemoveElasticAnnotation(SyntaxTrivia originalTrivia, SyntaxTrivia rewrittenTrivia)
        {
            return rewrittenTrivia.WithoutElasticTrivia();
        }
    }
}
