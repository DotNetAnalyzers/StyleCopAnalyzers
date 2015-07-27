namespace StyleCop.Analyzers.ReadabilityRules
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
    using StyleCop.Analyzers.Helpers;
    using StyleCop.Analyzers.SpacingRules;

    /// <summary>
    /// This class provides a code fix for the SA1103 diagnostic.
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1103CodeFixProvider))]
    [Shared]
    public class SA1103CodeFixProvider : CodeFixProvider
    {
        private static readonly ImmutableArray<string> FixableDiagnostics =
            ImmutableArray.Create(SA110xQueryClauses.SA1103Descriptor.Id);

        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds => FixableDiagnostics;

        /// <inheritdoc/>
        public override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        /// <inheritdoc/>
        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var syntaxRoot = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            foreach (var diagnostic in context.Diagnostics.Where(d => FixableDiagnostics.Contains(d.Id)))
            {
                var queryExpression = (QueryExpressionSyntax)syntaxRoot.FindNode(diagnostic.Location.SourceSpan).Parent;

                if (queryExpression.DescendantTrivia().All(AcceptableSingleLineTrivia))
                {
                    context.RegisterCodeFix(CodeAction.Create(ReadabilityResources.SA1103CodeFixSingleLine, token => GetTransformedDocumentFromSingleLineAsync(context.Document, diagnostic, token), equivalenceKey: nameof(SA1103CodeFixProvider)), diagnostic);
                }

                context.RegisterCodeFix(CodeAction.Create(ReadabilityResources.SA1103CodeFixMultipleLines, token => GetTransformedDocumentForMultipleLinesAsync(context.Document, diagnostic, token), equivalenceKey: nameof(SA1103CodeFixProvider)), diagnostic);
            }
        }

        private static bool AcceptableSingleLineTrivia(SyntaxTrivia trivia)
        {
            switch (trivia.Kind())
            {
            case SyntaxKind.WhitespaceTrivia:
            case SyntaxKind.EndOfLineTrivia:
                return true;

            case SyntaxKind.MultiLineCommentTrivia:
                var lineSpan = trivia.GetLineSpan();
                return lineSpan.StartLinePosition.Line == lineSpan.EndLinePosition.Line;

            default:
                return false;
            }
        }

        private static async Task<Document> GetTransformedDocumentFromSingleLineAsync(Document document, Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            var syntaxRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            var queryExpression = (QueryExpressionSyntax)syntaxRoot.FindNode(diagnostic.Location.SourceSpan).Parent;
            var replaceMap = new Dictionary<SyntaxToken, SyntaxToken>();

            var nodeList = CreateQueryNodeList(queryExpression);

            for (var i = 0; i < nodeList.Length; i++)
            {
                var token = nodeList[i].GetFirstToken();
                var precedingToken = token.GetPreviousToken();

                var triviaList = precedingToken.TrailingTrivia.AddRange(token.LeadingTrivia);
                var processedTriviaList = triviaList
                    .Where(t => t.IsKind(SyntaxKind.MultiLineCommentTrivia))
                    .ToSyntaxTriviaList()
                    .Add(SyntaxFactory.Space);

                if (processedTriviaList.Count > 1)
                {
                    processedTriviaList = processedTriviaList.Insert(0, SyntaxFactory.Space);
                }

                replaceMap.Add(precedingToken, precedingToken.WithTrailingTrivia(processedTriviaList));
                replaceMap.Add(token, token.WithLeadingTrivia());
            }

            var newSyntaxRoot = syntaxRoot.ReplaceTokens(replaceMap.Keys, (t1, t2) => replaceMap[t1]).WithoutFormatting();
            return document.WithSyntaxRoot(newSyntaxRoot);
        }

        private static async Task<Document> GetTransformedDocumentForMultipleLinesAsync(Document document, Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            var syntaxRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            var queryExpression = (QueryExpressionSyntax)syntaxRoot.FindNode(diagnostic.Location.SourceSpan).Parent;
            var replaceMap = new Dictionary<SyntaxToken, SyntaxToken>();

            var nodeList = CreateQueryNodeList(queryExpression);

            var indentationOptions = IndentationOptions.FromDocument(document);
            var indentationTrivia = QueryIndentationHelpers.GetQueryIndentationTrivia(indentationOptions, queryExpression);

            for (var i = 1; i < nodeList.Length; i++)
            {
                var token = nodeList[i].GetFirstToken();
                var precedingToken = token.GetPreviousToken();

                if (precedingToken.GetLine() == token.GetLine())
                {
                    var triviaList = precedingToken.TrailingTrivia.AddRange(token.LeadingTrivia);
                    var processedTriviaList = triviaList.WithoutTrailingWhitespace().Add(SyntaxFactory.CarriageReturnLineFeed);

                    replaceMap.Add(precedingToken, precedingToken.WithTrailingTrivia(processedTriviaList));
                    replaceMap.Add(token, token.WithLeadingTrivia(indentationTrivia));
                }
            }

            var newSyntaxRoot = syntaxRoot.ReplaceTokens(replaceMap.Keys, (t1, t2) => replaceMap[t1]).WithoutFormatting();
            return document.WithSyntaxRoot(newSyntaxRoot);
        }

        private static ImmutableArray<SyntaxNode> CreateQueryNodeList(QueryExpressionSyntax queryExpression)
        {
            var queryNodes = new List<SyntaxNode>();

            queryNodes.Add(queryExpression.FromClause);
            ProcessQueryBody(queryExpression.Body, queryNodes);

            return queryNodes.ToImmutableArray();
        }

        private static void ProcessQueryBody(QueryBodySyntax body, List<SyntaxNode> queryNodes)
        {
            queryNodes.AddRange(body.Clauses);
            queryNodes.Add(body.SelectOrGroup);

            if (body.Continuation != null)
            {
                ProcessQueryBody(body.Continuation.Body, queryNodes);
            }
        }
    }
}
