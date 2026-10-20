// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.LayoutRules
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
    using StyleCop.Analyzers.Lightup;
    using StyleCop.Analyzers.Settings.ObjectModel;

    /// <summary>
    /// Implements a code fix for <see cref="SA1504AllAccessorsMustBeSingleLineOrMultiLine"/>.
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1504CodeFixProvider))]
    [Shared]
    internal class SA1504CodeFixProvider : CodeFixProvider
    {
        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds { get; } =
            ImmutableArray.Create(SA1504AllAccessorsMustBeSingleLineOrMultiLine.DiagnosticId);

        /// <inheritdoc/>
        public override FixAllProvider GetFixAllProvider()
        {
            return CustomFixAllProviders.BatchFixer;
        }

        /// <inheritdoc/>
        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var syntaxRoot = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            foreach (var diagnostic in context.Diagnostics)
            {
                var node = syntaxRoot.FindNode(diagnostic.Location.SourceSpan);
                var accessorList = GetAccessorList(node);

                bool canOfferSingleLineFix = true;
                foreach (var accessor in accessorList.Accessors)
                {
                    // only offer single line fix for accessors without attributes
                    if (accessor.AttributeLists.Count > 0
                        || accessor.Keyword.TrailingTrivia.Any(t => t.IsKind(SyntaxKind.SingleLineCommentTrivia)))
                    {
                        canOfferSingleLineFix = false;
                        break;
                    }

                    if (accessor.Body == null)
                    {
                        var expressionBody = accessor.ExpressionBody();
                        if (expressionBody == null)
                        {
                            canOfferSingleLineFix = false;
                            break;
                        }

                        var lineSpan = expressionBody.Expression.GetLocation().GetLineSpan();
                        if (lineSpan.StartLinePosition.Line != lineSpan.EndLinePosition.Line)
                        {
                            canOfferSingleLineFix = false;
                            break;
                        }

                        if (expressionBody.DescendantTrivia().Any(t => !IsAllowedTrivia(t)))
                        {
                            canOfferSingleLineFix = false;
                            break;
                        }
                    }
                    else
                    {
                        // only offer single line fix for accessors with at most a single statement
                        if (accessor.Body.Statements.Count > 1)
                        {
                            canOfferSingleLineFix = false;
                            break;
                        }

                        if (accessor.Body.Statements.Count == 1)
                        {
                            // only offer single line fix when the statement is on a single line
                            var lineSpan = accessor.Body.Statements[0].GetLineSpan();
                            if (lineSpan.StartLinePosition.Line != lineSpan.EndLinePosition.Line)
                            {
                                canOfferSingleLineFix = false;
                                break;
                            }
                        }

                        // only offer single line fix if there are no interfering trivia
                        if (accessor.Body.DescendantTrivia().Any(t => !IsAllowedTrivia(t)))
                        {
                            canOfferSingleLineFix = false;
                            break;
                        }
                    }
                }

                if (canOfferSingleLineFix)
                {
                    context.RegisterCodeFix(
                        CodeAction.Create(
                            LayoutResources.SA1504CodeFixSingleLine,
                            cancellationToken => GetTransformedDocumentForSingleLineAsync(context.Document, diagnostic, cancellationToken),
                            nameof(SA1504CodeFixProvider) + "SingleLine"),
                        diagnostic);
                }

                context.RegisterCodeFix(
                    CodeAction.Create(
                        LayoutResources.SA1504CodeFixMultipleLines,
                        cancellationToken => GetTransformedDocumentForMutipleLinesAsync(context.Document, diagnostic, cancellationToken),
                        nameof(SA1504CodeFixProvider) + "MultipleLines"),
                    diagnostic);
            }
        }

        private static bool IsAllowedTrivia(SyntaxTrivia trivia)
        {
            return trivia.IsKind(SyntaxKind.WhitespaceTrivia)
                || trivia.IsKind(SyntaxKind.EndOfLineTrivia)
                || (trivia.IsKind(SyntaxKind.MultiLineCommentTrivia) && !trivia.SpansMultipleLines());
        }

        private static async Task<Document> GetTransformedDocumentForSingleLineAsync(Document document, Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            var syntaxRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            var sourceText = await document.GetTextAsync(cancellationToken).ConfigureAwait(false);
            var settings = SettingsHelper.GetStyleCopSettingsInCodeFix(document.Project.AnalyzerOptions, syntaxRoot.SyntaxTree, cancellationToken);

            var node = syntaxRoot.FindNode(diagnostic.Location.SourceSpan);
            var accessorList = GetAccessorList(node);
            var endOfLineTrivia = FormattingHelper.GetEndOfLineForCodeFix(accessorList.OpenBraceToken, sourceText, document.Project.Solution.Workspace.Options);
            var replacements = new Dictionary<SyntaxNode, SyntaxNode>();

            foreach (var accessor in accessorList.Accessors)
            {
                replacements[accessor] = ReformatAccessorAsSingleLine(
                    settings.Indentation,
                    accessor,
                    CanConvertBlockBodyToExpressionBody(accessorList, accessor),
                    endOfLineTrivia);
            }

            var newSyntaxRoot = syntaxRoot.ReplaceNodes(replacements.Keys, (original, maybeRewritten) => replacements[original]);
            return document.WithSyntaxRoot(newSyntaxRoot.WithoutFormatting());
        }

        private static bool CanConvertBlockBodyToExpressionBody(AccessorListSyntax accessorList, AccessorDeclarationSyntax accessor)
        {
            if (accessor.Body == null
                || accessor.Body.Statements.Count != 1
                || accessor.Body.DescendantTrivia().Any(t => t.IsKind(SyntaxKind.SingleLineCommentTrivia) || t.IsKind(SyntaxKind.MultiLineCommentTrivia)))
            {
                return false;
            }

            bool hasOtherAccessor = false;
            foreach (var otherAccessor in accessorList.Accessors)
            {
                if (otherAccessor == accessor)
                {
                    continue;
                }

                hasOtherAccessor = true;
                if (otherAccessor.ExpressionBody() == null)
                {
                    return false;
                }
            }

            if (!hasOtherAccessor)
            {
                return false;
            }

            var statement = accessor.Body.Statements[0];
            return (accessor.IsKind(SyntaxKind.GetAccessorDeclaration)
                    && statement is ReturnStatementSyntax returnStatement
                    && returnStatement.Expression != null)
                || (!accessor.IsKind(SyntaxKind.GetAccessorDeclaration)
                    && statement is ExpressionStatementSyntax);
        }

        private static SyntaxNode ReformatAccessorAsSingleLine(IndentationSettings indentationSettings, AccessorDeclarationSyntax accessor, bool convertBlockBodyToExpressionBody, SyntaxTrivia endOfLineTrivia)
        {
            var newAccessor = accessor
                .WithModifiers(ReformatModifiersAsSingleLine(accessor.Modifiers))
                .WithKeyword(ReformatKeywordAsSingleLine(accessor.Keyword));

            if (convertBlockBodyToExpressionBody)
            {
                var statement = accessor.Body.Statements[0];
                ExpressionSyntax expression = statement is ReturnStatementSyntax returnStatement
                    ? returnStatement.Expression
                    : ((ExpressionStatementSyntax)statement).Expression;
                expression = expression
                    .WithLeadingTrivia(ReformatTriviaListNoLeadingSpace(expression.GetLeadingTrivia()))
                    .WithTrailingTrivia(ReformatTriviaListNoTrailingSpace(expression.GetTrailingTrivia()));
                var arrowToken = SyntaxFactory.Token(SyntaxKind.EqualsGreaterThanToken).WithTrailingTrivia(SyntaxFactory.Space);
                var keyword = newAccessor.Keyword.WithTrailingTrivia(SyntaxFactory.Space);

                newAccessor = newAccessor
                    .WithKeyword(keyword)
                    .WithBody(null)
                    .WithExpressionBody(SyntaxFactory.ArrowExpressionClause(arrowToken, expression))
                    .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken).WithTrailingTrivia(endOfLineTrivia));
            }
            else if (accessor.Body != null)
            {
                newAccessor = newAccessor.WithBody(ReformatBodyAsSingleLine(accessor.Body, endOfLineTrivia));
            }
            else
            {
                var keyword = newAccessor.Keyword.WithTrailingTrivia(ReformatTriviaList(accessor.Keyword.TrailingTrivia));
                newAccessor = newAccessor
                    .WithKeyword(keyword)
                    .WithExpressionBody(ReformatExpressionBodyAsSingleLine(accessor.ExpressionBody()!))
                    .WithSemicolonToken(ReformatSemicolonAsSingleLine(accessor.SemicolonToken));
            }

            var accessorList = (AccessorListSyntax)accessor.Parent;

            var indentationSteps = IndentationHelper.GetIndentationSteps(indentationSettings, accessorList.OpenBraceToken);
            var indentation = IndentationHelper.GenerateWhitespaceTrivia(indentationSettings, indentationSteps + 1);

            newAccessor = newAccessor.WithLeadingTrivia(newAccessor.GetLeadingTrivia().Insert(0, indentation));
            return newAccessor;
        }

        private static ArrowExpressionClauseSyntax ReformatExpressionBodyAsSingleLine(ArrowExpressionClauseSyntax expressionBody)
        {
            var arrowToken = ReformatKeywordAsSingleLine(expressionBody.ArrowToken)
                .WithTrailingTrivia(ReformatTriviaList(expressionBody.ArrowToken.TrailingTrivia));
            var expression = expressionBody.Expression
                .WithLeadingTrivia(ReformatTriviaListNoLeadingSpace(expressionBody.Expression.GetLeadingTrivia()))
                .WithTrailingTrivia(ReformatTriviaListNoTrailingSpace(expressionBody.Expression.GetTrailingTrivia()));

            return expressionBody
                .WithArrowToken(arrowToken)
                .WithExpression(expression);
        }

        private static SyntaxToken ReformatSemicolonAsSingleLine(SyntaxToken semicolon)
        {
            return semicolon.WithLeadingTrivia(ReformatTriviaListNoLeadingSpace(semicolon.LeadingTrivia));
        }

        private static SyntaxTokenList ReformatModifiersAsSingleLine(SyntaxTokenList modifiers)
        {
            if (modifiers.Count == 0)
            {
                return modifiers;
            }

            var reformattedModifiers = modifiers.Select(t => t.WithLeadingTrivia(ReformatTriviaListNoLeadingSpace(t.LeadingTrivia)).WithTrailingTrivia(ReformatTriviaList(t.TrailingTrivia)));
            return SyntaxFactory.TokenList(reformattedModifiers);
        }

        private static SyntaxToken ReformatKeywordAsSingleLine(SyntaxToken keyword)
        {
            return keyword
                .WithLeadingTrivia(ReformatTriviaListNoLeadingSpace(keyword.LeadingTrivia))
                .WithTrailingTrivia(ReformatTriviaListNoTrailingSpace(keyword.TrailingTrivia));
        }

        private static BlockSyntax ReformatBodyAsSingleLine(BlockSyntax body, SyntaxTrivia endOfLineTrivia)
        {
            var newOpenBraceToken = body.OpenBraceToken
                .WithLeadingTrivia(ReformatTriviaList(body.OpenBraceToken.LeadingTrivia))
                .WithTrailingTrivia(ReformatTriviaListNoTrailingSpace(body.OpenBraceToken.TrailingTrivia));

            SyntaxList<StatementSyntax> newStatements;
            if (body.Statements.Count > 0)
            {
                var reformattedStatement = body.Statements[0]
                    .WithLeadingTrivia(ReformatTriviaList(body.Statements[0].GetLeadingTrivia()))
                    .WithTrailingTrivia(ReformatTriviaListNoTrailingSpace(body.Statements[0].GetTrailingTrivia()));

                newStatements = SyntaxFactory.List<StatementSyntax>().Add(reformattedStatement);
            }
            else
            {
                newStatements = body.Statements;
            }

            var newCloseBraceToken = body.CloseBraceToken
                .WithLeadingTrivia(ReformatTriviaList(body.CloseBraceToken.LeadingTrivia))
                .WithTrailingTrivia(ReformatTriviaListNoTrailingSpace(body.CloseBraceToken.TrailingTrivia).Add(endOfLineTrivia));

            return body.Update(newOpenBraceToken, newStatements, newCloseBraceToken);
        }

        private static async Task<Document> GetTransformedDocumentForMutipleLinesAsync(Document document, Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            var syntaxRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            var sourceText = await document.GetTextAsync(cancellationToken).ConfigureAwait(false);
            var settings = SettingsHelper.GetStyleCopSettingsInCodeFix(document.Project.AnalyzerOptions, syntaxRoot.SyntaxTree, cancellationToken);

            var node = syntaxRoot.FindNode(diagnostic.Location.SourceSpan);
            var accessorList = GetAccessorList(node);
            var endOfLineTrivia = FormattingHelper.GetEndOfLineForCodeFix(accessorList.OpenBraceToken, sourceText, document.Project.Solution.Workspace.Options);
            var replacements = new Dictionary<SyntaxNode, SyntaxNode>();

            for (var index = 0; index < accessorList.Accessors.Count; index++)
            {
                var accessor = accessorList.Accessors[index];
                var reformattedAccessor = ReformatAccessorAsMultipleLines(settings.Indentation, accessor, endOfLineTrivia);
                if (index < accessorList.Accessors.Count - 1
                    && !TriviaHelper.ContainsBlankLines(reformattedAccessor.GetTrailingTrivia(), startsOnNewLine: false)
                    && !TriviaHelper.ContainsBlankLines(accessorList.Accessors[index + 1].GetLeadingTrivia()))
                {
                    // insert an empty line between accessors
                    reformattedAccessor = reformattedAccessor.WithTrailingTrivia(reformattedAccessor.GetTrailingTrivia().Add(endOfLineTrivia));
                }

                replacements[accessor] = reformattedAccessor;
            }

            var newSyntaxRoot = syntaxRoot.ReplaceNodes(replacements.Keys, (original, maybeRewritten) => replacements[original]);
            return document.WithSyntaxRoot(newSyntaxRoot.WithoutFormatting());
        }

        private static SyntaxNode ReformatAccessorAsMultipleLines(IndentationSettings indentationSettings, AccessorDeclarationSyntax accessor, SyntaxTrivia endOfLineTrivia)
        {
            var accessorList = (AccessorListSyntax)accessor.Parent;
            var indentationSteps = IndentationHelper.GetIndentationSteps(indentationSettings, accessorList.OpenBraceToken) + 1;
            var indentation = IndentationHelper.GenerateWhitespaceTrivia(indentationSettings, indentationSteps);
            var indentationStatements = IndentationHelper.GenerateWhitespaceTrivia(indentationSettings, indentationSteps + 1);

            var newAccessor = accessor.WithModifiers(ReformatModifiersAsMultipleLines(accessor.Modifiers, indentation));
            if (accessor.Body != null)
            {
                newAccessor = newAccessor
                    .WithKeyword(ReformatKeywordAsMultipleLines(accessor.Keyword, indentation, accessor.Modifiers.Count == 0, endOfLineTrivia))
                    .WithBody(ReformatBodyAsMultipleLines(accessor.Body, indentation, indentationStatements, endOfLineTrivia));
            }
            else
            {
                var expressionBody = accessor.ExpressionBody()!;
                var keyword = accessor.Keyword.WithTrailingTrivia(
                    accessor.Keyword.TrailingTrivia.AddRange(expressionBody.ArrowToken.LeadingTrivia));
                keyword = ReformatKeywordAsMultipleLines(keyword, indentation, accessor.Modifiers.Count == 0, endOfLineTrivia);
                var body = SyntaxFactory.Block(CreateStatementFromExpressionBody(accessor, expressionBody));

                newAccessor = newAccessor
                    .WithKeyword(keyword)
                    .WithExpressionBody(null)
                    .WithSemicolonToken(default(SyntaxToken))
                    .WithBody(ReformatBodyAsMultipleLines(body, indentation, indentationStatements, endOfLineTrivia));
            }

            return newAccessor;
        }

        private static StatementSyntax CreateStatementFromExpressionBody(AccessorDeclarationSyntax accessor, ArrowExpressionClauseSyntax expressionBody)
        {
            var leadingTrivia = expressionBody.ArrowToken.TrailingTrivia.AddRange(expressionBody.Expression.GetLeadingTrivia());
            leadingTrivia = leadingTrivia.WithoutLeadingWhitespace(endOfLineIsWhitespace: false);
            if (leadingTrivia.Count > 0 && leadingTrivia[0].IsKind(SyntaxKind.EndOfLineTrivia))
            {
                // The new opening brace supplies the line break formerly following the arrow.
                leadingTrivia = leadingTrivia.RemoveAt(0);
            }

            var expression = expressionBody.Expression.WithoutLeadingTrivia();

            StatementSyntax statement;
            if (ThrowExpressionSyntaxWrapper.IsInstance(expression))
            {
                var throwExpression = (ThrowExpressionSyntaxWrapper)expression;
                statement = SyntaxFactory.ThrowStatement(throwExpression.Expression)
                    .WithThrowKeyword(throwExpression.ThrowKeyword)
                    .WithSemicolonToken(accessor.SemicolonToken);
            }
            else if (accessor.IsKind(SyntaxKind.GetAccessorDeclaration))
            {
                statement = SyntaxFactory.ReturnStatement(expression)
                    .WithReturnKeyword(SyntaxFactory.Token(SyntaxKind.ReturnKeyword).WithTrailingTrivia(SyntaxFactory.Space))
                    .WithSemicolonToken(accessor.SemicolonToken);
            }
            else
            {
                statement = SyntaxFactory.ExpressionStatement(expression).WithSemicolonToken(accessor.SemicolonToken);
            }

            return statement.WithLeadingTrivia(leadingTrivia);
        }

        private static SyntaxTokenList ReformatModifiersAsMultipleLines(SyntaxTokenList modifiers, SyntaxTrivia indentation)
        {
            if (modifiers.Count == 0)
            {
                return modifiers;
            }

            var reformattedModifiers = modifiers.Select(token => token
                .WithLeadingTrivia(ReformatAccessorHeaderTrivia(token.LeadingTrivia))
                .WithTrailingTrivia(token.TrailingTrivia.All(IsAllowedTrivia) ? ReformatTriviaList(token.TrailingTrivia) : token.TrailingTrivia)).ToArray();
            reformattedModifiers[0] = reformattedModifiers[0].WithLeadingTrivia(
                ReformatLeadingTriviaAsMultipleLines(reformattedModifiers[0].LeadingTrivia, indentation));
            return SyntaxFactory.TokenList(reformattedModifiers);
        }

        private static SyntaxToken ReformatKeywordAsMultipleLines(SyntaxToken keyword, SyntaxTrivia indentation, bool insertIdentation, SyntaxTrivia endOfLineTrivia)
        {
            return keyword
                .WithLeadingTrivia(insertIdentation
                    ? ReformatLeadingTriviaAsMultipleLines(ReformatAccessorHeaderTrivia(keyword.LeadingTrivia), indentation)
                    : ReformatAccessorHeaderTrivia(keyword.LeadingTrivia))
                .WithTrailingTrivia(ReformatTrailingTriviaAsMultipleLines(keyword.TrailingTrivia, endOfLineTrivia));
        }

        private static BlockSyntax ReformatBodyAsMultipleLines(BlockSyntax body, SyntaxTrivia indentation, SyntaxTrivia indentationStatements, SyntaxTrivia endOfLineTrivia)
        {
            var interiorTrivia = body.OpenBraceToken.TrailingTrivia.AddRange(body.CloseBraceToken.LeadingTrivia);
            if (body.Statements.Count == 0
                && interiorTrivia.Any(t => t.IsKind(SyntaxKind.MultiLineCommentTrivia))
                && body.OpenBraceToken.TrailingTrivia.All(IsAllowedTrivia)
                && body.CloseBraceToken.LeadingTrivia.All(IsAllowedTrivia)
                && !interiorTrivia.ContainsBlankLines(startsOnNewLine: false))
            {
                body = body
                    .WithOpenBraceToken(body.OpenBraceToken.WithTrailingTrivia(ReformatTriviaListNoTrailingSpace(interiorTrivia)))
                    .WithCloseBraceToken(body.CloseBraceToken.WithLeadingTrivia(default(SyntaxTriviaList)));
            }

            var newStatements = SyntaxFactory.List(body.Statements.Select(statement => statement
                .WithLeadingTrivia(ReformatLeadingTriviaAsMultipleLines(statement.GetLeadingTrivia(), indentationStatements))
                .WithTrailingTrivia(ReformatTrailingTriviaAsMultipleLines(statement.GetTrailingTrivia(), endOfLineTrivia))));

            var newOpenBraceToken = body.OpenBraceToken
                .WithLeadingTrivia(ReformatLeadingTriviaAsMultipleLines(body.OpenBraceToken.LeadingTrivia, indentation))
                .WithTrailingTrivia(ReformatTrailingTriviaAsMultipleLines(body.OpenBraceToken.TrailingTrivia, endOfLineTrivia));

            var newCloseBraceToken = body.CloseBraceToken
                .WithLeadingTrivia(ReformatLeadingTriviaAsMultipleLines(body.CloseBraceToken.LeadingTrivia, indentation))
                .WithTrailingTrivia(ReformatTrailingTriviaAsMultipleLines(body.CloseBraceToken.TrailingTrivia, endOfLineTrivia));

            return body.Update(newOpenBraceToken, newStatements, newCloseBraceToken);
        }

        private static SyntaxTriviaList ReformatAccessorHeaderTrivia(SyntaxTriviaList trivia)
        {
            return trivia.All(t => t.IsKind(SyntaxKind.WhitespaceTrivia)
                    || t.IsKind(SyntaxKind.EndOfLineTrivia)
                    || t.IsKind(SyntaxKind.MultiLineCommentTrivia))
                && !trivia.ContainsBlankLines(startsOnNewLine: false)
                ? ReformatTriviaListNoLeadingSpace(trivia)
                : trivia;
        }

        private static SyntaxTriviaList ReformatLeadingTriviaAsMultipleLines(SyntaxTriviaList trivia, SyntaxTrivia indentation)
        {
            if (trivia.Any(t => t.IsKind(SyntaxKind.EndOfLineTrivia) || t.IsDirective))
            {
                trivia = trivia.WithoutTrailingWhitespace(endOfLineIsWhitespace: false).Add(indentation);
                return trivia[0].IsKind(SyntaxKind.WhitespaceTrivia) || trivia[0].HasBuiltinEndLine()
                    ? trivia
                    : trivia.Insert(0, indentation);
            }

            return trivia.WithoutLeadingWhitespace().Insert(0, indentation);
        }

        private static SyntaxTriviaList ReformatTrailingTriviaAsMultipleLines(SyntaxTriviaList trivia, SyntaxTrivia endOfLineTrivia)
        {
            trivia = trivia.WithoutTrailingWhitespace(endOfLineIsWhitespace: false);
            return trivia.Count > 0 && trivia[trivia.Count - 1].HasBuiltinEndLine()
                ? trivia
                : trivia.Add(endOfLineTrivia);
        }

        private static AccessorListSyntax GetAccessorList(SyntaxNode node)
        {
            while (!node.IsKind(SyntaxKind.AccessorList))
            {
                node = node.Parent;
            }

            return (AccessorListSyntax)node;
        }

        private static SyntaxTriviaList ReformatTriviaList(IReadOnlyList<SyntaxTrivia> triviaList)
        {
            return ReformatTriviaListNoTrailingSpace(triviaList).Add(SyntaxFactory.Space);
        }

        private static SyntaxTriviaList ReformatTriviaListNoTrailingSpace(IReadOnlyList<SyntaxTrivia> triviaList)
        {
            return SyntaxFactory.TriviaList(triviaList.Where(t => t.IsKind(SyntaxKind.MultiLineCommentTrivia)).SelectMany(ExpandComment));
        }

        private static SyntaxTriviaList ReformatTriviaListNoLeadingSpace(IReadOnlyList<SyntaxTrivia> triviaList)
        {
            return ReformatTriviaList(triviaList).RemoveAt(0);
        }

        private static IEnumerable<SyntaxTrivia> ExpandComment(SyntaxTrivia comment)
        {
            yield return SyntaxFactory.Space;
            yield return comment;
        }
    }
}
