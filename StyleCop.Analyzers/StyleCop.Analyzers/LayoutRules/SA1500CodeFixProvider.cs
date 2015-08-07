namespace StyleCop.Analyzers.LayoutRules
{
    using System;
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
    using Microsoft.CodeAnalysis.Text;
    using StyleCop.Analyzers.Helpers;
    using StyleCop.Analyzers.SpacingRules;

    /// <summary>
    /// Implements a code fix for <see cref="SA1500CurlyBracketsForMultiLineStatementsMustNotShareLine"/>.
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1500CodeFixProvider))]
    [Shared]
    public class SA1500CodeFixProvider : CodeFixProvider
    {
        private static readonly ImmutableArray<string> FixableDiagnostics =
            ImmutableArray.Create(SA1500CurlyBracketsForMultiLineStatementsMustNotShareLine.DiagnosticId);

        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds
        {
            get
            {
                return FixableDiagnostics;
            }
        }

        /// <inheritdoc/>
        public override FixAllProvider GetFixAllProvider()
        {
            return CustomFixAllProviders.BatchFixer;
        }

        /// <inheritdoc/>
        public override Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            foreach (Diagnostic diagnostic in context.Diagnostics.Where(d => FixableDiagnostics.Contains(d.Id)))
            {
                context.RegisterCodeFix(CodeAction.Create(LayoutResources.SA1500CodeFix, token => GetTransformedDocumentAsync(context.Document, diagnostic, token), equivalenceKey: nameof(SA1500CodeFixProvider)), diagnostic);
            }

            return SpecializedTasks.CompletedTask;
        }

        private static async Task<Document> GetTransformedDocumentAsync(Document document, Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            var syntaxRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            var curlyBracketToken = syntaxRoot.FindToken(diagnostic.Location.SourceSpan.Start);
            var curlyBracketLine = curlyBracketToken.GetLineSpan().StartLinePosition.Line;
            var curlyBracketReplacementToken = curlyBracketToken;

            var indentationOptions = IndentationOptions.FromDocument(document);
            var indentationSteps = DetermineIndentationSteps(indentationOptions, curlyBracketToken);

            var previousToken = curlyBracketToken.GetPreviousToken();
            var nextToken = curlyBracketToken.GetNextToken();

            var rewriter = new Rewriter();

            if (IsAccessorWithSingleLineBlock(previousToken, curlyBracketToken))
            {
                var newTrailingTrivia = previousToken.TrailingTrivia
                    .WithoutTrailingWhitespace()
                    .Add(SyntaxFactory.Space);

                rewriter.AddReplacement(previousToken, previousToken.WithTrailingTrivia(newTrailingTrivia));

                curlyBracketReplacementToken = curlyBracketReplacementToken.WithLeadingTrivia(curlyBracketToken.LeadingTrivia.WithoutLeadingWhitespace());
            }
            else
            {
                // Check if we need to apply a fix before the curly bracket
                if (previousToken.GetLineSpan().StartLinePosition.Line == curlyBracketLine)
                {
                    var sharedTrivia = curlyBracketReplacementToken.LeadingTrivia.WithoutTrailingWhitespace();
                    var previousTokenNewTrailingTrivia = previousToken.TrailingTrivia
                        .WithoutTrailingWhitespace()
                        .AddRange(sharedTrivia)
                        .Add(SyntaxFactory.CarriageReturnLineFeed);

                    rewriter.AddReplacement(previousToken, previousToken.WithTrailingTrivia(previousTokenNewTrailingTrivia));

                    curlyBracketReplacementToken = curlyBracketReplacementToken.WithLeadingTrivia(IndentationHelper.GenerateWhitespaceTrivia(indentationOptions, indentationSteps));
                }

                // Check if we need to apply a fix after the curly bracket
                // if a closing curly bracket is followed by a semi-colon or closing paren, no fix is needed.
                if ((nextToken.GetLineSpan().StartLinePosition.Line == curlyBracketLine) &&
                    (!curlyBracketToken.IsKind(SyntaxKind.CloseBraceToken) || !IsValidFollowingToken(nextToken)))
                {
                    var sharedTrivia = nextToken.LeadingTrivia.WithoutTrailingWhitespace();
                    var newTrailingTrivia = curlyBracketReplacementToken.TrailingTrivia
                        .WithoutTrailingWhitespace()
                        .AddRange(sharedTrivia)
                        .Add(SyntaxFactory.CarriageReturnLineFeed);

                    int newIndentationSteps;
                    if (curlyBracketToken.IsKind(SyntaxKind.OpenBraceToken))
                    {
                        newIndentationSteps = indentationSteps + 1;
                    }
                    else if (nextToken.IsKind(SyntaxKind.CloseBraceToken))
                    {
                        newIndentationSteps = Math.Max(0, indentationSteps - 1);
                    }
                    else
                    {
                        newIndentationSteps = indentationSteps;
                    }

                    rewriter.AddReplacement(nextToken, nextToken.WithLeadingTrivia(IndentationHelper.GenerateWhitespaceTrivia(indentationOptions, newIndentationSteps)));

                    curlyBracketReplacementToken = curlyBracketReplacementToken.WithTrailingTrivia(newTrailingTrivia);
                }
            }

            rewriter.AddReplacement(curlyBracketToken, curlyBracketReplacementToken);

            var newSyntaxRoot = rewriter.Visit(syntaxRoot).WithoutFormatting();
            return document.WithSyntaxRoot(newSyntaxRoot);
        }

        private static bool IsAccessorWithSingleLineBlock(SyntaxToken previousToken, SyntaxToken curlyBracketToken)
        {
            if (!curlyBracketToken.IsKind(SyntaxKind.OpenBraceToken))
            {
                return false;
            }

            switch (previousToken.Kind())
            {
            case SyntaxKind.GetKeyword:
            case SyntaxKind.SetKeyword:
            case SyntaxKind.AddKeyword:
            case SyntaxKind.RemoveKeyword:
                break;

            default:
                return false;
            }

            var token = curlyBracketToken;
            var depth = 1;

            while (depth > 0)
            {
                token = token.GetNextToken();
                switch (token.Kind())
                {
                case SyntaxKind.CloseBraceToken:
                    depth--;
                    break;

                case SyntaxKind.OpenBraceToken:
                    depth++;
                    break;
                }
            }

            return curlyBracketToken.GetLineSpan().StartLinePosition.Line == token.GetLineSpan().StartLinePosition.Line;
        }

        private static bool IsValidFollowingToken(SyntaxToken nextToken)
        {
            switch (nextToken.Kind())
            {
            case SyntaxKind.SemicolonToken:
            case SyntaxKind.CloseParenToken:
            case SyntaxKind.CommaToken:
                return true;

            default:
                return false;
            }
        }

        private static int DetermineIndentationSteps(IndentationOptions indentationOptions, SyntaxToken token)
        {
            // For a closing curly bracket use the indentation of the corresponding opening curly bracket
            if (token.IsKind(SyntaxKind.CloseBraceToken))
            {
                var depth = 1;

                while (depth > 0)
                {
                    token = token.GetPreviousToken();
                    switch (token.Kind())
                    {
                    case SyntaxKind.CloseBraceToken:
                        depth++;
                        break;

                    case SyntaxKind.OpenBraceToken:
                        depth--;
                        break;
                    }
                }
            }

            var startLine = GetTokenStartLinePosition(token).Line;

            while (!ContainsStartOfLine(token, startLine))
            {
                token = token.GetPreviousToken();
            }

            return IndentationHelper.GetIndentationSteps(indentationOptions, token);
        }

        private static bool ContainsStartOfLine(SyntaxToken token, int startLine)
        {
            var startLinePosition = GetTokenStartLinePosition(token);

            return (startLinePosition.Line < startLine) || (startLinePosition.Character == 0);
        }

        private static LinePosition GetTokenStartLinePosition(SyntaxToken token)
        {
            return Location.Create(token.SyntaxTree, token.FullSpan).GetLineSpan().StartLinePosition;
        }

        private class Rewriter : CSharpSyntaxRewriter
        {
            private Dictionary<SyntaxToken, SyntaxToken> tokenReplacements = new Dictionary<SyntaxToken, SyntaxToken>();

            public void AddReplacement(SyntaxToken originalToken, SyntaxToken replacementToken)
            {
                this.tokenReplacements.Add(originalToken, replacementToken);
            }

            public override SyntaxToken VisitToken(SyntaxToken token)
            {
                SyntaxToken replacementToken;

                if (this.tokenReplacements.TryGetValue(token, out replacementToken))
                {
                    return replacementToken;
                }

                return base.VisitToken(token);
            }
        }
    }
}
