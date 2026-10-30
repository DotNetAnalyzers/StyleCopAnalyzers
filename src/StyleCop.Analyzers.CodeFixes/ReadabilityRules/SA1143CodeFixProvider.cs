// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.ReadabilityRules;

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
using Microsoft.CodeAnalysis.Text;
using StyleCop.Analyzers.Helpers;
using StyleCop.Analyzers.Lightup;
using StyleCop.Analyzers.Settings.ObjectModel;

/// <summary>
/// Formats conditional expressions without changing their tokens.
/// </summary>
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1143CodeFixProvider))]
[Shared]
internal class SA1143CodeFixProvider : CodeFixProvider
{
    private enum Layout
    {
        SingleLine,
        Expanded,
        Chain,
    }

    /// <inheritdoc/>
    public override ImmutableArray<string> FixableDiagnosticIds { get; } = ImmutableArray.Create(SA1143ConditionalExpressionsMustBeFormattedConsistently.DiagnosticId);

    /// <inheritdoc/>
    public override FixAllProvider GetFixAllProvider() => CustomFixAllProviders.BatchFixer;

    /// <inheritdoc/>
    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
        var text = await context.Document.GetTextAsync(context.CancellationToken).ConfigureAwait(false);
        foreach (var diagnostic in context.Diagnostics)
        {
            // Nested diagnostics must produce the same edit so Fix All cannot re-expand an inner branch.
            var expression = root.FindToken(diagnostic.Location.SourceSpan.Start).Parent?.AncestorsAndSelf().OfType<ConditionalExpressionSyntax>().LastOrDefault();
            if (expression == null)
            {
                continue;
            }

            if (CanFormat(expression, text))
            {
                context.RegisterCodeFix(
                    CodeAction.Create(
                        ReadabilityResources.SA1143CodeFixSingleLine,
                        cancellationToken => FormatAsync(context.Document, expression, Layout.SingleLine, cancellationToken),
                        nameof(Layout.SingleLine)),
                    diagnostic);
                context.RegisterCodeFix(
                    CodeAction.Create(
                        ReadabilityResources.SA1143CodeFixMultiLine,
                        cancellationToken => FormatAsync(context.Document, expression, Layout.Expanded, cancellationToken),
                        nameof(Layout.Expanded)),
                    diagnostic);
                if (expression.WhenFalse is ConditionalExpressionSyntax)
                {
                    context.RegisterCodeFix(
                        CodeAction.Create(
                            ReadabilityResources.SA1143CodeFixChain,
                            cancellationToken => FormatAsync(context.Document, expression, Layout.Chain, cancellationToken),
                            nameof(Layout.Chain)),
                        diagnostic);
                }
            }

            var settings = SettingsHelper.GetStyleCopSettingsInCodeFix(context.Document.Project.AnalyzerOptions, expression.SyntaxTree, context.CancellationToken);
            var alignedText = GetAlignedBranchText(expression, text, settings.Indentation, context.CancellationToken);
            if (alignedText != null)
            {
                context.RegisterCodeFix(
                    CodeAction.Create(
                        ReadabilityResources.SA1143CodeFixAlignBranchDelimiters,
                        cancellationToken => Task.FromResult(context.Document.WithText(alignedText)),
                        "AlignBranchDelimiters"),
                    diagnostic);
            }
        }
    }

    private static SourceText? GetAlignedBranchText(ConditionalExpressionSyntax expression, SourceText text, IndentationSettings settings, CancellationToken cancellationToken)
    {
        if (expression.ContainsDiagnostics || expression.ContainsDirectives)
        {
            return null;
        }

        var changes = new List<TextChange>();
        foreach (var conditional in expression.DescendantNodesAndSelf(descendIntoChildren: node => !(node is InterpolatedStringExpressionSyntax)).OfType<ConditionalExpressionSyntax>())
        {
            cancellationToken.ThrowIfCancellationRequested();
            AddDelimiterAlignmentChanges(conditional.WhenTrue, conditional.QuestionToken, text, settings, changes);
            AddDelimiterAlignmentChanges(conditional.WhenFalse, conditional.ColonToken, text, settings, changes);
        }

        if (changes.Count == 0)
        {
            return null;
        }

        var alignedText = text.WithChanges(changes.OrderBy(change => change.Span.Start));
        var alignedRoot = expression.SyntaxTree.WithChangedText(alignedText).GetRoot(cancellationToken);
        var alignedSpan = new TextSpan(expression.SpanStart, expression.Span.Length + alignedText.Length - text.Length);
        var alignedExpression = (ConditionalExpressionSyntax)alignedRoot.FindNode(alignedSpan, getInnermostNodeForTie: true);

        // Nested diagnostics share this action, so the complete replacement must satisfy the rule.
        foreach (var conditional in alignedExpression.DescendantNodesAndSelf().OfType<ConditionalExpressionSyntax>())
        {
            if (conditional.Parent is ConditionalExpressionSyntax parent && parent.WhenFalse == conditional)
            {
                continue;
            }

            var span = conditional.GetLineSpan();
            if (span.StartLinePosition.Line != span.EndLinePosition.Line
                && !SA1143ConditionalExpressionsMustBeFormattedConsistently.IsMultilineLayoutValid(conditional, alignedText, settings.TabSize))
            {
                return null;
            }
        }

        return alignedText;
    }

    private static void AddDelimiterAlignmentChanges(ExpressionSyntax branch, SyntaxToken operatorToken, SourceText text, IndentationSettings settings, List<TextChange> changes)
    {
        var operatorColumn = SA1143ConditionalExpressionsMustBeFormattedConsistently.GetColumn(text, operatorToken.SpanStart, settings.TabSize);
        foreach (var node in branch.DescendantNodesAndSelf(descendIntoChildren: node => !(node is InterpolatedStringExpressionSyntax) && !(node is ConditionalExpressionSyntax)))
        {
            SyntaxToken open;
            SyntaxToken close;
            switch (node)
            {
            case InitializerExpressionSyntax initializer:
                open = initializer.OpenBraceToken;
                close = initializer.CloseBraceToken;
                break;

            case ParenthesizedExpressionSyntax parenthesized:
                open = parenthesized.OpenParenToken;
                close = parenthesized.CloseParenToken;
                break;

            default:
                if (CollectionExpressionSyntaxWrapper.IsInstance(node))
                {
                    var collection = (CollectionExpressionSyntaxWrapper)node;
                    open = collection.OpenBracketToken;
                    close = collection.CloseBracketToken;
                }
                else if (TupleExpressionSyntaxWrapper.IsInstance(node))
                {
                    var tuple = (TupleExpressionSyntaxWrapper)node;
                    open = tuple.OpenParenToken;
                    close = tuple.CloseParenToken;
                }
                else
                {
                    continue;
                }

                break;
            }

            var line = text.Lines.GetLineFromPosition(close.SpanStart);
            if (line.LineNumber == text.Lines.GetLineFromPosition(open.SpanStart).LineNumber
                || !close.IsFirstInLine(allowNonWhitespaceTrivia: false)
                || close.GetPreviousToken().Span.End > line.Start)
            {
                continue;
            }

            var indentationSpan = TextSpan.FromBounds(line.Start, close.SpanStart);
            var indentation = text.ToString(indentationSpan);
            var column = SA1143ConditionalExpressionsMustBeFormattedConsistently.GetColumn(text, close.SpanStart, settings.TabSize);
            if (column > operatorColumn || (column == operatorColumn && close.IsKind(SyntaxKind.CloseBraceToken)))
            {
                continue;
            }

            var alignment = GenerateAlignment(SA1143ConditionalExpressionsMustBeFormattedConsistently.GetColumn(text, open.SpanStart, settings.TabSize), settings);
            if (alignment != indentation)
            {
                changes.Add(new TextChange(indentationSpan, alignment));
            }
        }
    }

    private static bool CanFormat(ConditionalExpressionSyntax expression, SourceText text)
    {
        foreach (var token in expression.DescendantTokens())
        {
            var span = token.GetLineSpan();
            if (span.StartLinePosition.Line != span.EndLinePosition.Line)
            {
                return false;
            }
        }

        foreach (var trivia in expression.DescendantTrivia())
        {
            if (!expression.Span.Contains(trivia.Span))
            {
                continue;
            }

            if (trivia.IsKind(SyntaxKind.WhitespaceTrivia) || trivia.IsKind(SyntaxKind.EndOfLineTrivia))
            {
                continue;
            }

            var span = text.Lines.GetLinePositionSpan(trivia.Span);
            if (!trivia.IsKind(SyntaxKind.MultiLineCommentTrivia) || span.Start.Line != span.End.Line)
            {
                return false;
            }
        }

        return true;
    }

    private static async Task<Document> FormatAsync(Document document, ConditionalExpressionSyntax expression, Layout layout, CancellationToken cancellationToken)
    {
        var text = await document.GetTextAsync(cancellationToken).ConfigureAwait(false);
        var settings = SettingsHelper.GetStyleCopSettingsInCodeFix(document.Project.AnalyzerOptions, expression.SyntaxTree, cancellationToken);
        var newLine = FormattingHelper.GetEndOfLineForCodeFix(expression.GetFirstToken(), text, document.Project.Solution.Workspace.Options).ToFullString();
        var changes = new Dictionary<TextSpan, TextChange>();
        var previous = expression.GetFirstToken();
        foreach (var token in expression.DescendantTokens().Skip(1))
        {
            var gap = TextSpan.FromBounds(previous.Span.End, token.SpanStart);
            if (text.Lines.GetLineFromPosition(previous.Span.End).LineNumber != text.Lines.GetLineFromPosition(token.SpanStart).LineNumber)
            {
                SetGap(changes, text, gap, " ");
            }

            previous = token;
        }

        if (layout != Layout.SingleLine)
        {
            var line = text.Lines.GetLineFromPosition(expression.SpanStart);
            var indentationEnd = line.Start;
            while (indentationEnd < expression.SpanStart && (text[indentationEnd] == ' ' || text[indentationEnd] == '\t'))
            {
                indentationEnd++;
            }

            var indentation = text.ToString(TextSpan.FromBounds(line.Start, indentationEnd));
            var column = SA1143ConditionalExpressionsMustBeFormattedConsistently.GetColumn(text, expression.SpanStart, settings.Indentation.TabSize);
            FormatConditional(expression, layout, indentation, column, settings.Indentation, newLine, text, changes);
        }

        return document.WithText(text.WithChanges(changes.Values.OrderBy(change => change.Span.Start)));
    }

    private static void FormatConditional(
        ConditionalExpressionSyntax expression,
        Layout layout,
        string indentation,
        int column,
        IndentationSettings settings,
        string newLine,
        SourceText text,
        Dictionary<TextSpan, TextChange> changes)
    {
        var continuation = layout == Layout.Chain
            ? GenerateAlignment(column, settings)
            : indentation + IndentationHelper.GenerateIndentationString(settings, 1);
        while (true)
        {
            SetGap(changes, text, TextSpan.FromBounds(expression.Condition.Span.End, expression.QuestionToken.SpanStart), layout == Layout.Chain ? " " : newLine + continuation);
            SetGap(changes, text, TextSpan.FromBounds(expression.QuestionToken.Span.End, expression.WhenTrue.SpanStart), " ");
            SetGap(changes, text, TextSpan.FromBounds(expression.WhenTrue.Span.End, expression.ColonToken.SpanStart), layout == Layout.Chain ? " " : newLine + continuation);
            SetGap(changes, text, TextSpan.FromBounds(expression.ColonToken.Span.End, expression.WhenFalse.SpanStart), layout == Layout.Chain ? newLine + continuation : " ");
            if (!(expression.WhenFalse is ConditionalExpressionSyntax next))
            {
                break;
            }

            expression = next;
            if (layout == Layout.Expanded)
            {
                continuation += IndentationHelper.GenerateIndentationString(settings, 1);
            }
        }
    }

    private static void SetGap(Dictionary<TextSpan, TextChange> changes, SourceText text, TextSpan gap, string separator)
    {
        // Keep inline block comments on the preceding line when moving a boundary.
        var comments = SyntaxFactory.ParseLeadingTrivia(text.ToString(gap))
            .Where(trivia => trivia.IsKind(SyntaxKind.MultiLineCommentTrivia))
            .Select(trivia => trivia.ToFullString())
            .ToArray();
        var replacement = comments.Length == 0 ? separator : " " + string.Join(" ", comments) + separator;
        changes[gap] = new TextChange(gap, replacement);
    }

    private static string GenerateAlignment(int column, IndentationSettings settings)
    {
        return settings.UseTabs && settings.TabSize > 0
            ? new string('\t', column / settings.TabSize) + new string(' ', column % settings.TabSize)
            : new string(' ', column);
    }
}
