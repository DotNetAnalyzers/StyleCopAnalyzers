// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.ReadabilityRules;

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using StyleCop.Analyzers.Lightup;

/// <summary>
/// An assignment expression is used as a condition, or as an operand of a logical operator.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
internal class SA1145DoNotUseAssignmentAsCondition : DiagnosticAnalyzer
{
    /// <summary>
    /// The ID for diagnostics produced by the <see cref="SA1145DoNotUseAssignmentAsCondition"/> analyzer.
    /// </summary>
    public const string DiagnosticId = "SA1145";

    private static readonly LocalizableString Title = new LocalizableResourceString(nameof(ReadabilityResources.SA1145Title), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
    private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(ReadabilityResources.SA1145MessageFormat), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
    private static readonly LocalizableString Description = new LocalizableResourceString(nameof(ReadabilityResources.SA1145Description), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
    private static readonly DiagnosticDescriptor Descriptor = new DiagnosticDescriptor(
        DiagnosticId,
        Title,
        MessageFormat,
        AnalyzerCategory.ReadabilityRules,
        DiagnosticSeverity.Warning,
        AnalyzerConstants.EnabledByDefault,
        Description,
        HelpLinkHelper.GetHelpLink(DiagnosticId));

    private static readonly Action<SyntaxNodeAnalysisContext> HandleAssignmentAction = HandleAssignment;

    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(Descriptor);

    /// <inheritdoc/>
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterCompilationStartAction(context =>
        {
            context.RegisterSyntaxNodeAction(
                HandleAssignmentAction,
                SyntaxKind.SimpleAssignmentExpression,
                SyntaxKind.AddAssignmentExpression,
                SyntaxKind.SubtractAssignmentExpression,
                SyntaxKind.MultiplyAssignmentExpression,
                SyntaxKind.DivideAssignmentExpression,
                SyntaxKind.ModuloAssignmentExpression,
                SyntaxKind.AndAssignmentExpression,
                SyntaxKind.ExclusiveOrAssignmentExpression,
                SyntaxKind.OrAssignmentExpression,
                SyntaxKind.LeftShiftAssignmentExpression,
                SyntaxKind.RightShiftAssignmentExpression,
                SyntaxKindEx.CoalesceAssignmentExpression,
                SyntaxKindEx.UnsignedRightShiftAssignmentExpression);
        });
    }

    private static void HandleAssignment(SyntaxNodeAnalysisContext context)
    {
        SyntaxNode child = context.Node;
        SyntaxNode parent = child.Parent;
        while (parent.IsKind(SyntaxKind.ParenthesizedExpression))
        {
            child = parent;
            parent = parent.Parent;
        }

        if (!IsConditionOrBooleanOperand(parent, child))
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(Descriptor, context.Node.GetLocation()));
    }

    private static bool IsConditionOrBooleanOperand(SyntaxNode parent, SyntaxNode child)
    {
        switch (parent.Kind())
        {
        case SyntaxKind.LogicalAndExpression:
        case SyntaxKind.LogicalOrExpression:
        case SyntaxKind.LogicalNotExpression:
            return true;

        case SyntaxKind.ConditionalExpression:
            return ((ConditionalExpressionSyntax)parent).Condition == child;

        case SyntaxKind.IfStatement:
            return ((IfStatementSyntax)parent).Condition == child;

        case SyntaxKind.WhileStatement:
            return ((WhileStatementSyntax)parent).Condition == child;

        case SyntaxKind.DoStatement:
            return ((DoStatementSyntax)parent).Condition == child;

        case SyntaxKind.ForStatement:
            return ((ForStatementSyntax)parent).Condition == child;

        case SyntaxKind.CatchFilterClause:
            return ((CatchFilterClauseSyntax)parent).FilterExpression == child;

        default:
            return false;
        }
    }
}
