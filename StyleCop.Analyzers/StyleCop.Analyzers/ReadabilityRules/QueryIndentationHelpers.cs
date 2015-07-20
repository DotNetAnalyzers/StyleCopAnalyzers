namespace StyleCop.Analyzers.ReadabilityRules
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using StyleCop.Analyzers.Helpers;

    /// <summary>
    /// Helper class containing methods for determining query indentation.
    /// </summary>
    internal static class QueryIndentationHelpers
    {
        /// <summary>
        /// Gets a whitespace trivia containing the proper amount of indentation for new lines in the given query.
        /// </summary>
        /// <param name="indentationOptions">The indentation options to use.</param>
        /// <param name="queryExpression">The query expression to determine indentation from.</param>
        /// <returns>A whitespace trivia containing the proper amount of indentation.</returns>
        internal static SyntaxTrivia GetQueryIndentationTrivia(IndentationOptions indentationOptions, QueryExpressionSyntax queryExpression)
        {
            var firstTokenOnTextLine = IndentationHelper.GetFirstTokenOnTextLine(queryExpression.FromClause.FromKeyword);
            var indentationSteps = IndentationHelper.GetIndentationSteps(indentationOptions, firstTokenOnTextLine);

            // add an extra indentation step when the first from clause is not properly indented yet
            if (!firstTokenOnTextLine.IsKind(SyntaxKind.OpenParenToken) && (firstTokenOnTextLine != queryExpression.FromClause.FromKeyword))
            {
                indentationSteps++;
            }

            return IndentationHelper.GenerateWhitespaceTrivia(indentationOptions, indentationSteps);
        }

        /// <summary>
        /// Gets a whitespace trivia containing the proper amount of indentation for new lines in the query of which the given token is a part.
        /// </summary>
        /// <param name="indentationOptions">The indentation options to use.</param>
        /// <param name="token">A token within a query expression.</param>
        /// <returns>A whitespace trivia containing the proper amount of indentation.</returns>
        internal static SyntaxTrivia GetQueryIndentationTrivia(IndentationOptions indentationOptions, SyntaxToken token)
        {
            var currentNode = token.Parent;
            while (!currentNode.IsKind(SyntaxKind.QueryExpression))
            {
                currentNode = currentNode.Parent;
            }

            return GetQueryIndentationTrivia(indentationOptions, (QueryExpressionSyntax)currentNode);
        }
    }
}
