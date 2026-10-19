// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.ReadabilityRules;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using StyleCop.Analyzers.Helpers;

/// <summary>
/// Implements a code fix for <see cref="SA1117ParametersMustBeOnSameLineOrSeparateLines"/>.
/// </summary>
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1117CodeFixProvider))]
[Shared]
internal class SA1117CodeFixProvider : CodeFixProvider
{
    /// <inheritdoc/>
    public override ImmutableArray<string> FixableDiagnosticIds { get; } =
        ImmutableArray.Create(SA1117ParametersMustBeOnSameLineOrSeparateLines.DiagnosticId);

    /// <inheritdoc/>
    public override FixAllProvider GetFixAllProvider()
    {
        return CustomFixAllProviders.BatchFixer;
    }

    /// <inheritdoc/>
    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
        foreach (var diagnostic in context.Diagnostics)
        {
            SyntaxToken diagnosticToken = root.FindToken(diagnostic.Location.SourceSpan.Start);
            SyntaxNode? list = GetContainingList(diagnosticToken);
            if (list == null || HasZeroWidthElement(list))
            {
                continue;
            }

            context.RegisterCodeFix(
                CodeAction.Create(
                    ReadabilityResources.SA1117CodeFix,
                    cancellationToken => GetTransformedDocumentAsync(context.Document, diagnostic, cancellationToken),
                    nameof(SA1117CodeFixProvider)),
                diagnostic);
        }
    }

    private static async Task<Document> GetTransformedDocumentAsync(Document document, Diagnostic diagnostic, CancellationToken cancellationToken)
    {
        SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        SyntaxToken diagnosticToken = root.FindToken(diagnostic.Location.SourceSpan.Start);
        SyntaxNode? list = GetContainingList(diagnosticToken);
        if (list == null)
        {
            throw new InvalidOperationException("The diagnostic is not contained in a supported parameter or argument list.");
        }

        var elements = new List<SyntaxNode>(list.ChildNodes());
        if (elements.Count < 2)
        {
            throw new InvalidOperationException("The diagnostic list does not contain at least two elements.");
        }

        SyntaxTree tree = root.SyntaxTree;
        SourceText sourceText = await tree.GetTextAsync(cancellationToken).ConfigureAwait(false);
        SyntaxToken openDelimiter = list.GetFirstToken();
        TextLine sourceLine = sourceText.Lines.GetLineFromPosition(openDelimiter.SpanStart);
        string lineText = sourceText.ToString(sourceLine.Span);
        int indentationLength;
        for (indentationLength = 0; indentationLength < lineText.Length; indentationLength++)
        {
            if (!char.IsWhiteSpace(lineText[indentationLength]))
            {
                break;
            }
        }

        var options = document.Project.Solution.Workspace.Options;
        SyntaxTrivia endOfLineTrivia = FormattingHelper.GetEndOfLineForCodeFix(openDelimiter, sourceText, options);
        var settings = SettingsHelper.GetStyleCopSettingsInCodeFix(document.Project.AnalyzerOptions, tree, cancellationToken);
        string indentationStep = IndentationHelper.GenerateIndentationString(settings.Indentation, 1);
        SyntaxTriviaList newLeadingTrivia = SyntaxFactory.TriviaList(
            endOfLineTrivia,
            SyntaxFactory.Whitespace(lineText.Substring(0, indentationLength) + indentationStep));

        var separators = new List<SyntaxToken>();
        foreach (SyntaxToken childToken in list.ChildTokens())
        {
            if (childToken.IsKind(SyntaxKind.CommaToken))
            {
                separators.Add(childToken);
            }
        }

        if (separators.Count != elements.Count - 1)
        {
            throw new InvalidOperationException("The list separator count does not match its element count.");
        }

        var updatedLeadingTrivia = new Dictionary<SyntaxToken, SyntaxTriviaList>();
        var tokensToIndent = new HashSet<SyntaxToken>();
        var tokensToUpdate = new List<SyntaxToken> { openDelimiter };
        for (int i = 0; i < elements.Count; i++)
        {
            SyntaxNode element = elements[i];
            SyntaxToken firstToken = element.GetFirstToken();
            SyntaxToken precedingToken = i == 0 ? openDelimiter : separators[i - 1];
            SyntaxTriviaList originalLeadingTrivia = precedingToken.TrailingTrivia.AddRange(firstToken.LeadingTrivia);
            updatedLeadingTrivia.Add(firstToken, newLeadingTrivia.AddRange(RemoveInitialWhitespace(originalLeadingTrivia)));
            bool startsOnSameLine = !ContainsEndOfLine(originalLeadingTrivia);
            foreach (SyntaxToken elementToken in element.DescendantTokens())
            {
                tokensToUpdate.Add(elementToken);
                if (startsOnSameLine && elementToken != firstToken)
                {
                    tokensToIndent.Add(elementToken);
                }
            }

            if (i < separators.Count)
            {
                tokensToUpdate.Add(separators[i]);
            }
        }

        var separatorsToClear = new HashSet<SyntaxToken>(separators) { openDelimiter };
        SyntaxNode updatedRoot = root.ReplaceTokens(
            tokensToUpdate,
            (original, rewritten) => updatedLeadingTrivia.TryGetValue(original, out SyntaxTriviaList leadingTrivia)
                ? rewritten.WithLeadingTrivia(leadingTrivia)
                : tokensToIndent.Contains(original)
                    ? rewritten.WithLeadingTrivia(IndentationHelper.IndentLeadingTrivia(original, rewritten.LeadingTrivia, indentationStep, default(SyntaxTriviaList)))
                    : separatorsToClear.Contains(original)
                        ? rewritten.WithTrailingTrivia(default(SyntaxTriviaList))
                        : rewritten);
        return document.WithSyntaxRoot(updatedRoot);
    }

    private static SyntaxNode? GetContainingList(SyntaxToken token)
    {
        SyntaxNode? current = token.Parent;
        while (current?.Parent != null)
        {
            SyntaxNode parent = current.Parent;
            if ((parent is BaseArgumentListSyntax
                    || parent is BaseParameterListSyntax
                    || parent is AttributeArgumentListSyntax
                    || parent is ArrayRankSpecifierSyntax)
                && IsListElement(parent, current))
            {
                return parent;
            }

            current = parent;
        }

        return null;
    }

    private static bool HasZeroWidthElement(SyntaxNode list)
    {
        foreach (SyntaxNode element in list.ChildNodes())
        {
            if (element.Span.IsEmpty)
            {
                return true;
            }
        }

        return false;
    }

    private static bool ContainsEndOfLine(SyntaxTriviaList trivia)
    {
        foreach (SyntaxTrivia item in trivia)
        {
            if (item.IsKind(SyntaxKind.EndOfLineTrivia))
            {
                return true;
            }
        }

        return false;
    }

    private static bool IsListElement(SyntaxNode list, SyntaxNode node)
    {
        foreach (SyntaxNode child in list.ChildNodes())
        {
            if (child == node)
            {
                return true;
            }
        }

        return false;
    }

    private static SyntaxTriviaList RemoveInitialWhitespace(SyntaxTriviaList trivia)
    {
        int firstNonWhitespace = 0;
        while (firstNonWhitespace < trivia.Count
            && (trivia[firstNonWhitespace].IsKind(SyntaxKind.WhitespaceTrivia)
                || trivia[firstNonWhitespace].IsKind(SyntaxKind.EndOfLineTrivia)))
        {
            firstNonWhitespace++;
        }

        return trivia.RemoveRange(0, firstNonWhitespace);
    }
}
