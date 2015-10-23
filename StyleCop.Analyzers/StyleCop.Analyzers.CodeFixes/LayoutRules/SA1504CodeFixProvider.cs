// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.LayoutRules
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
    using Microsoft.CodeAnalysis.CSharp.Syntax;

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
                    if (accessor.AttributeLists.Count > 0)
                    {
                        canOfferSingleLineFix = false;
                        break;
                    }

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
            var indentationOptions = IndentationOptions.FromDocument(document);

            var node = syntaxRoot.FindNode(diagnostic.Location.SourceSpan);
            var accessorList = GetAccessorList(node);
            var replacements = new Dictionary<SyntaxNode, SyntaxNode>();

            foreach (var accessor in accessorList.Accessors)
            {
                replacements[accessor] = ReformatAccessorAsSingleLine(indentationOptions, accessor);
            }

            var newSyntaxRoot = syntaxRoot.ReplaceNodes(replacements.Keys, (original, maybeRewritten) => replacements[original]);
            return document.WithSyntaxRoot(newSyntaxRoot.WithoutFormatting());
        }

        private static SyntaxNode ReformatAccessorAsSingleLine(IndentationOptions indentationOptions, AccessorDeclarationSyntax accessor)
        {
            var newAccessor = accessor
                .WithModifiers(ReformatModifiersAsSingleLine(accessor.Modifiers))
                .WithKeyword(ReformatKeywordAsSingleLine(accessor.Keyword))
                .WithBody(ReformatBodyAsSingleLine(accessor.Body));

            var accessorList = (AccessorListSyntax)accessor.Parent;

            var indentationSteps = IndentationHelper.GetIndentationSteps(indentationOptions, accessorList.OpenBraceToken);
            var indentation = IndentationHelper.GenerateWhitespaceTrivia(indentationOptions, indentationSteps + 1);

            newAccessor = newAccessor.WithLeadingTrivia(newAccessor.GetLeadingTrivia().Insert(0, indentation));
            return newAccessor;
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

        private static BlockSyntax ReformatBodyAsSingleLine(BlockSyntax body)
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
                .WithTrailingTrivia(ReformatTriviaListNoTrailingSpace(body.CloseBraceToken.TrailingTrivia).Add(SyntaxFactory.CarriageReturnLineFeed));

            return body.Update(newOpenBraceToken, newStatements, newCloseBraceToken);
        }

        private static async Task<Document> GetTransformedDocumentForMutipleLinesAsync(Document document, Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            var syntaxRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            var indentationOptions = IndentationOptions.FromDocument(document);

            var node = syntaxRoot.FindNode(diagnostic.Location.SourceSpan);
            var accessorList = GetAccessorList(node);
            var replacements = new Dictionary<SyntaxNode, SyntaxNode>();

            foreach (var accessor in accessorList.Accessors)
            {
                var reformattedAccessor = ReformatAccessorAsMultipleLines(indentationOptions, accessor);
                if (accessor != accessorList.Accessors.Last())
                {
                    // insert an empty line between accessors
                    reformattedAccessor = reformattedAccessor.WithTrailingTrivia(reformattedAccessor.GetTrailingTrivia().Add(SyntaxFactory.CarriageReturnLineFeed));
                }

                replacements[accessor] = reformattedAccessor;
            }

            var newSyntaxRoot = syntaxRoot.ReplaceNodes(replacements.Keys, (original, maybeRewritten) => replacements[original]);
            return document.WithSyntaxRoot(newSyntaxRoot.WithoutFormatting());
        }

        private static SyntaxNode ReformatAccessorAsMultipleLines(IndentationOptions indentationOptions, AccessorDeclarationSyntax accessor)
        {
            var accessorList = (AccessorListSyntax)accessor.Parent;
            var indentationSteps = IndentationHelper.GetIndentationSteps(indentationOptions, accessorList.OpenBraceToken) + 1;
            var indentation = IndentationHelper.GenerateWhitespaceTrivia(indentationOptions, indentationSteps);
            var indentationStatements = IndentationHelper.GenerateWhitespaceTrivia(indentationOptions, indentationSteps + 1);

            var newAccessor = accessor
                .WithModifiers(ReformatModifiersAsMultipleLines(accessor.Modifiers, indentation))
                .WithKeyword(ReformatKeywordAsMultipleLines(accessor.Keyword, indentation, accessor.Modifiers.Count == 0))
                .WithBody(ReformatBodyAsMultipleLines(accessor.Body, indentation, indentationStatements));

            return newAccessor;
        }

        private static SyntaxTokenList ReformatModifiersAsMultipleLines(SyntaxTokenList modifiers, SyntaxTrivia indentation)
        {
            if (modifiers.Count == 0)
            {
                return modifiers;
            }

            var reformattedModifiers = modifiers.Select(t => t.WithLeadingTrivia(ReformatTriviaListNoLeadingSpace(t.LeadingTrivia)).WithTrailingTrivia(ReformatTriviaList(t.TrailingTrivia))).ToArray();
            reformattedModifiers[0] = reformattedModifiers[0].WithLeadingTrivia(reformattedModifiers[0].LeadingTrivia.Insert(0, indentation));

            return SyntaxFactory.TokenList(reformattedModifiers);
        }

        private static SyntaxToken ReformatKeywordAsMultipleLines(SyntaxToken keyword, SyntaxTrivia indentation, bool insertIdentation)
        {
            var newLeadingTrivia = ReformatTriviaListNoLeadingSpace(keyword.LeadingTrivia);
            if (insertIdentation)
            {
                newLeadingTrivia = newLeadingTrivia.Insert(0, indentation);
            }

            var newTrailingTrivia = ReformatTriviaListNoTrailingSpace(keyword.TrailingTrivia).Add(SyntaxFactory.CarriageReturnLineFeed);

            return keyword
                .WithLeadingTrivia(newLeadingTrivia)
                .WithTrailingTrivia(newTrailingTrivia);
        }

        private static BlockSyntax ReformatBodyAsMultipleLines(BlockSyntax body, SyntaxTrivia indentation, SyntaxTrivia indentationStatements)
        {
            SyntaxTriviaList reformattedOpenBraceTrailingTrivia;
            SyntaxTriviaList reformattedCloseBraceLeadingTrivia;

            SyntaxList<StatementSyntax> newStatements;
            if (body.Statements.Count > 0)
            {
                var statements = new List<StatementSyntax>();

                foreach (var statement in body.Statements)
                {
                    var reformattedStatement = statement
                        .WithLeadingTrivia(ReformatTriviaListNoLeadingSpace(statement.GetLeadingTrivia()).Insert(0, indentationStatements))
                        .WithTrailingTrivia(ReformatTriviaListNoTrailingSpace(statement.GetTrailingTrivia()).Add(SyntaxFactory.CarriageReturnLineFeed));
                    statements.Add(reformattedStatement);
                }

                newStatements = SyntaxFactory.List<StatementSyntax>().AddRange(statements);

                reformattedOpenBraceTrailingTrivia = ReformatTriviaListNoTrailingSpace(body.OpenBraceToken.TrailingTrivia);
                reformattedCloseBraceLeadingTrivia = ReformatTriviaListNoLeadingSpace(body.CloseBraceToken.LeadingTrivia);
            }
            else
            {
                var triviaBetweenBraces = TriviaHelper.MergeTriviaLists(body.OpenBraceToken.TrailingTrivia, body.CloseBraceToken.LeadingTrivia);
                reformattedOpenBraceTrailingTrivia = ReformatTriviaListNoTrailingSpace(triviaBetweenBraces);
                newStatements = body.Statements;
                reformattedCloseBraceLeadingTrivia = SyntaxFactory.TriviaList();
            }

            var newOpenBraceToken = body.OpenBraceToken
                .WithLeadingTrivia(ReformatTriviaListNoLeadingSpace(body.OpenBraceToken.LeadingTrivia).Insert(0, indentation))
                .WithTrailingTrivia(reformattedOpenBraceTrailingTrivia.Add(SyntaxFactory.CarriageReturnLineFeed));

            var newCloseBraceToken = body.CloseBraceToken
                .WithLeadingTrivia(reformattedCloseBraceLeadingTrivia.Insert(0, indentation))
                .WithTrailingTrivia(ReformatTriviaListNoTrailingSpace(body.CloseBraceToken.TrailingTrivia).Add(SyntaxFactory.CarriageReturnLineFeed));

            return body.Update(newOpenBraceToken, newStatements, newCloseBraceToken);
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
