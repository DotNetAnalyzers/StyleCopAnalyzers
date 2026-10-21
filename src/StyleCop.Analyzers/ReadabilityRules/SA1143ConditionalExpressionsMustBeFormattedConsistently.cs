// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.ReadabilityRules;

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using StyleCop.Analyzers.Helpers;
using StyleCop.Analyzers.Settings.ObjectModel;

/// <summary>
/// Conditional expressions must use a consistent single-line or indented multiline layout.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
internal class SA1143ConditionalExpressionsMustBeFormattedConsistently : DiagnosticAnalyzer
{
    /// <summary>
    /// The diagnostic identifier.
    /// </summary>
    public const string DiagnosticId = "SA1143";

    private static readonly LocalizableString Title = new LocalizableResourceString(nameof(ReadabilityResources.SA1143Title), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
    private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(ReadabilityResources.SA1143MessageFormat), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
    private static readonly LocalizableString Description = new LocalizableResourceString(nameof(ReadabilityResources.SA1143Description), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
    private static readonly DiagnosticDescriptor Descriptor = new DiagnosticDescriptor(
        DiagnosticId,
        Title,
        MessageFormat,
        AnalyzerCategory.ReadabilityRules,
        DiagnosticSeverity.Warning,
        AnalyzerConstants.EnabledByDefault,
        Description,
        HelpLinkHelper.GetHelpLink(DiagnosticId));

    private static readonly Action<SyntaxNodeAnalysisContext, StyleCopSettings> HandleConditionalAction = HandleConditional;

    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(Descriptor);

    /// <inheritdoc/>
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterCompilationStartAction(context =>
            context.RegisterSyntaxNodeAction(HandleConditionalAction, SyntaxKind.ConditionalExpression));
    }

    internal static int GetColumn(SourceText text, int position, int tabSize)
    {
        var column = 0;
        for (var i = text.Lines.GetLineFromPosition(position).Start; i < position; i++)
        {
            column += text[i] == '\t' && tabSize > 0 ? tabSize - (column % tabSize) : 1;
        }

        return column;
    }

    private static void HandleConditional(SyntaxNodeAnalysisContext context, StyleCopSettings settings)
    {
        var expression = (ConditionalExpressionSyntax)context.Node;
        if (expression.Parent is ConditionalExpressionSyntax parent && parent.WhenFalse == expression)
        {
            // The outermost expression checks the complete false-branch chain once.
            return;
        }

        if (expression.ContainsDiagnostics || IsSingleLine(expression))
        {
            return;
        }

        var text = context.Node.SyntaxTree.GetText(context.CancellationToken);
        if (!IsExpanded(expression, text, settings.Indentation.TabSize)
            && !IsChain(expression, text, settings.Indentation.TabSize))
        {
            context.ReportDiagnostic(Diagnostic.Create(Descriptor, expression.QuestionToken.GetLocation()));
        }
    }

    private static bool IsExpanded(ConditionalExpressionSyntax expression, SourceText text, int tabSize)
    {
        while (true)
        {
            var question = expression.QuestionToken.GetLineSpan().StartLinePosition;
            var colon = expression.ColonToken.GetLineSpan().StartLinePosition;
            if (question.Line <= expression.Condition.GetLineSpan().EndLinePosition.Line
                || colon.Line <= expression.WhenTrue.GetLineSpan().EndLinePosition.Line
                || expression.WhenTrue.GetLineSpan().StartLinePosition.Line != question.Line
                || expression.WhenFalse.GetLineSpan().StartLinePosition.Line != colon.Line
                || !StartsLine(text, expression.QuestionToken.SpanStart)
                || !StartsLine(text, expression.ColonToken.SpanStart)
                || GetColumn(text, expression.QuestionToken.SpanStart, tabSize) != GetColumn(text, expression.ColonToken.SpanStart, tabSize)
                || GetColumn(text, expression.QuestionToken.SpanStart, tabSize) <= GetIndentation(text, expression.Condition.Span.End - 1, tabSize))
            {
                return false;
            }

            var operatorColumn = GetColumn(text, expression.QuestionToken.SpanStart, tabSize);
            if (!IsContinuationIndented(expression.WhenTrue, text, operatorColumn, tabSize))
            {
                return false;
            }

            if (IsSingleLine(expression.WhenFalse))
            {
                return true;
            }

            if (!(expression.WhenFalse is ConditionalExpressionSyntax next))
            {
                return IsContinuationIndented(expression.WhenFalse, text, operatorColumn, tabSize);
            }

            expression = next;
        }
    }

    private static bool IsContinuationIndented(ExpressionSyntax branch, SourceText text, int operatorColumn, int tabSize)
    {
        var span = branch.GetLineSpan();
        for (var lineNumber = span.StartLinePosition.Line + 1; lineNumber <= span.EndLinePosition.Line; lineNumber++)
        {
            var line = text.Lines[lineNumber];
            var position = line.Start;
            while (position < line.End && (text[position] == ' ' || text[position] == '\t'))
            {
                position++;
            }

            // Blank lines and lines inside multi-line literals have no meaningful indentation.
            if (position == line.End || IsInsideLiteralContent(branch, position))
            {
                continue;
            }

            var column = GetColumn(text, position, tabSize);
            var isBrace = text[position] == '{' || text[position] == '}';
            if (column < operatorColumn || (column == operatorColumn && !isBrace))
            {
                return false;
            }
        }

        return true;
    }

    private static bool IsInsideLiteralContent(ExpressionSyntax branch, int position)
    {
        var token = branch.FindToken(position);
        if (token.SpanStart < position)
        {
            return true;
        }

        // An interpolation hole or closing quote starting a line follows string text that includes its indentation.
        var previous = token.GetPreviousToken();
        return previous.IsKind(SyntaxKind.InterpolatedStringTextToken) && previous.Span.End == position;
    }

    private static bool IsChain(ConditionalExpressionSyntax expression, SourceText text, int tabSize)
    {
        if (!(expression.WhenFalse is ConditionalExpressionSyntax))
        {
            return false;
        }

        var column = GetColumn(text, expression.SpanStart, tabSize);
        while (true)
        {
            var line = expression.Condition.GetLineSpan().StartLinePosition.Line;
            if (!IsSingleLine(expression.Condition)
                || !IsSingleLine(expression.WhenTrue)
                || expression.QuestionToken.GetLineSpan().StartLinePosition.Line != line
                || expression.WhenTrue.GetLineSpan().StartLinePosition.Line != line
                || expression.ColonToken.GetLineSpan().StartLinePosition.Line != line
                || expression.WhenFalse.GetLineSpan().StartLinePosition.Line <= line
                || !StartsLine(text, expression.WhenFalse.SpanStart)
                || GetColumn(text, expression.WhenFalse.SpanStart, tabSize) != column)
            {
                return false;
            }

            if (!(expression.WhenFalse is ConditionalExpressionSyntax next))
            {
                return IsSingleLine(expression.WhenFalse);
            }

            expression = next;
        }
    }

    private static bool IsSingleLine(SyntaxNode node)
    {
        var span = node.GetLineSpan();
        return span.StartLinePosition.Line == span.EndLinePosition.Line;
    }

    private static bool StartsLine(SourceText text, int position)
    {
        for (var i = text.Lines.GetLineFromPosition(position).Start; i < position; i++)
        {
            if (text[i] != ' ' && text[i] != '\t')
            {
                return false;
            }
        }

        return true;
    }

    private static int GetIndentation(SourceText text, int position, int tabSize)
    {
        var line = text.Lines.GetLineFromPosition(position);
        var end = line.Start;
        while (end < position && (text[end] == ' ' || text[end] == '\t'))
        {
            end++;
        }

        return GetColumn(text, end, tabSize);
    }
}
