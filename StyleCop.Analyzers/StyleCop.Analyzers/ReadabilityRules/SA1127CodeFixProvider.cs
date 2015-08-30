namespace StyleCop.Analyzers.ReadabilityRules
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Composition;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Helpers;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using SpacingRules;

    /// <summary>
    /// Implements a code fix for <see cref="SA1127GenericTypeConstraintsMustBeOnOwnLine"/>.
    /// </summary>
    /// <remarks>
    /// <para>To fix a violation of this rule, ensure that each type constrait is placed
    /// on its own line.</para>
    /// </remarks>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1127CodeFixProvider))]
    [Shared]
    public class SA1127CodeFixProvider : CodeFixProvider
    {
        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds { get; } =
            ImmutableArray.Create(SA1127GenericTypeConstraintsMustBeOnOwnLine.DiagnosticId);

        /// <inheritdoc/>
        public override FixAllProvider GetFixAllProvider()
        {
            return CustomFixAllProviders.BatchFixer;
        }

        /// <inheritdoc/>
        public override Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            foreach (var diagnostic in context.Diagnostics.Where(d => this.FixableDiagnosticIds.Contains(d.Id)))
            {
                context.RegisterCodeFix(
                    CodeAction.Create(
                        ReadabilityResources.SA1127CodeFix,
                        cancellationToken => GetTransformedDocumentAsync(context.Document, diagnostic, cancellationToken),
                        equivalenceKey: nameof(SA1127CodeFixProvider)),
                    diagnostic);
            }

            return SpecializedTasks.CompletedTask;
        }

        private static async Task<Document> GetTransformedDocumentAsync(Document document, Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            var syntaxRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            var whereToken = syntaxRoot.FindToken(diagnostic.Location.SourceSpan.Start);
            var precedingToken = whereToken.GetPreviousToken();
            var endToken = syntaxRoot.FindToken(diagnostic.Location.SourceSpan.End);
            var afterEndToken = endToken.GetNextToken();

            var parentIndentation = GetParentIndentation(whereToken);
            var indentationOptions = IndentationOptions.FromDocument(document);
            var indentationTrivia = SyntaxFactory.Whitespace(parentIndentation + IndentationHelper.GenerateIndentationString(indentationOptions, 1));

            var replaceMap = new Dictionary<SyntaxToken, SyntaxToken>()
            {
                [precedingToken] = precedingToken.WithTrailingTrivia(SyntaxFactory.CarriageReturnLineFeed),
                [whereToken] = whereToken.WithLeadingTrivia(indentationTrivia),
                [endToken] = endToken.WithTrailingTrivia(RemoveUnnecessaryWhitespace(endToken).Add(SyntaxFactory.CarriageReturnLineFeed)),
            };

            if (afterEndToken.IsKind(SyntaxKind.EqualsGreaterThanToken))
            {
                replaceMap.Add(afterEndToken, afterEndToken.WithLeadingTrivia(indentationTrivia));
            }
            else if (afterEndToken.IsKind(SyntaxKind.OpenBraceToken))
            {
                replaceMap.Add(afterEndToken, afterEndToken.WithLeadingTrivia(SyntaxFactory.Whitespace(parentIndentation)));
            }
            else if (afterEndToken.IsKind(SyntaxKind.WhereKeyword))
            {
                replaceMap.Add(afterEndToken, afterEndToken.WithLeadingTrivia(indentationTrivia));
            }

            var newSyntaxRoot = syntaxRoot.ReplaceTokens(replaceMap.Keys, (t1, t2) => replaceMap[t1]).WithoutFormatting();
            return document.WithSyntaxRoot(newSyntaxRoot);
        }

        private static string GetParentIndentation(SyntaxToken token)
        {
            var parentLine = token.Parent.Parent;
            var parentIndentation = string.Empty;
            var parentTrivia = parentLine.GetLeadingTrivia();
            foreach (var trivia in parentTrivia)
            {
                if (trivia.IsKind(SyntaxKind.WhitespaceTrivia))
                {
                    parentIndentation += trivia.ToString();
                }
            }

            return parentIndentation;
        }

        // This function will remove any unnecessary whitespace or end-of-line trivia from a token.
        // If there is trailing comment trivia, it is preserved along with whitespace *before* it.
        private static SyntaxTriviaList RemoveUnnecessaryWhitespace(SyntaxToken token)
        {
            if (!token.HasTrailingTrivia)
            {
                return SyntaxFactory.TriviaList();
            }

            var triviaToKeep = new List<SyntaxTrivia>();
            var currentWhitespace = new List<SyntaxTrivia>();
            foreach (var trivia in token.TrailingTrivia)
            {
                if (trivia.IsKind(SyntaxKind.WhitespaceTrivia))
                {
                    currentWhitespace.Add(trivia);
                }
                else if (trivia.IsKind(SyntaxKind.SingleLineCommentTrivia))
                {
                    triviaToKeep.AddRange(currentWhitespace);
                    currentWhitespace.Clear();
                    triviaToKeep.Add(trivia);
                }
            }

            return SyntaxFactory.TriviaList(triviaToKeep);
        }
    }
}