// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.LayoutRules
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using Helpers;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using SpacingRules;

    /// <summary>
    /// A C# statement containing opening and closing curly brackets is written completely on a single line.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when a statement that is wrapped in opening and closing curly brackets is
    /// written on a single line. For example:</para>
    ///
    /// <code language="csharp">
    /// public object Method()
    /// {
    ///     lock (this) { return this.value; }
    /// }
    /// </code>
    ///
    /// <para>When StyleCop checks this code, a violation of this rule will occur because the entire lock statement is
    /// written on one line. The statement should be written across multiple lines, with the opening and closing curly
    /// brackets each on their own line, as follows:</para>
    ///
    /// <code language="csharp">
    /// public object Method()
    /// {
    ///     lock (this)
    ///     {
    ///         return this.value;
    ///     }
    /// }
    /// </code>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1501StatementMustNotBeOnASingleLine : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1501StatementMustNotBeOnASingleLine"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1501";
        private const string Title = "Statement must not be on a single line";
        private const string MessageFormat = "Statement must not be on a single line";
        private const string Description = "A C# statement containing opening and closing curly brackets is written completely on a single line.";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1501.md";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.LayoutRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly Action<CompilationStartAnalysisContext> CompilationStartAction = HandleCompilationStart;

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.RegisterCompilationStartAction(CompilationStartAction);
        }

        private static void HandleCompilationStart(CompilationStartAnalysisContext context)
        {
            context.RegisterSyntaxNodeActionHonorExclusions(HandleBlock, SyntaxKind.Block);

            // If SA1503 is suppressed, we need to handle compound blocks as well.
            if (context.IsAnalyzerSuppressed(SA1503CurlyBracketsMustNotBeOmitted.DiagnosticId))
            {
                context.RegisterSyntaxNodeActionHonorExclusions(HandleIfStatement, SyntaxKind.IfStatement);
                context.RegisterSyntaxNodeActionHonorExclusions(ctx => CheckChildStatement(ctx, ctx.Node, ((DoStatementSyntax)ctx.Node).Statement), SyntaxKind.DoStatement);
                context.RegisterSyntaxNodeActionHonorExclusions(ctx => CheckChildStatement(ctx, ctx.Node, ((WhileStatementSyntax)ctx.Node).Statement), SyntaxKind.WhileStatement);
                context.RegisterSyntaxNodeActionHonorExclusions(ctx => CheckChildStatement(ctx, ctx.Node, ((ForStatementSyntax)ctx.Node).Statement), SyntaxKind.ForStatement);
                context.RegisterSyntaxNodeActionHonorExclusions(ctx => CheckChildStatement(ctx, ctx.Node, ((ForEachStatementSyntax)ctx.Node).Statement), SyntaxKind.ForEachStatement);
                context.RegisterSyntaxNodeActionHonorExclusions(ctx => CheckChildStatement(ctx, ctx.Node, ((LockStatementSyntax)ctx.Node).Statement), SyntaxKind.LockStatement);
                context.RegisterSyntaxNodeActionHonorExclusions(ctx => CheckChildStatement(ctx, ctx.Node, ((UsingStatementSyntax)ctx.Node).Statement), SyntaxKind.UsingStatement);
                context.RegisterSyntaxNodeActionHonorExclusions(ctx => CheckChildStatement(ctx, ctx.Node, ((FixedStatementSyntax)ctx.Node).Statement), SyntaxKind.FixedStatement);
            }
        }

        private static void HandleBlock(SyntaxNodeAnalysisContext context)
        {
            var block = context.Node as BlockSyntax;
            if ((block != null) &&
                !block.OpenBraceToken.IsMissing &&
                !block.CloseBraceToken.IsMissing &&
                IsPartOfStatement(block))
            {
                var openBraceLineNumber = block.SyntaxTree.GetLineSpan(block.OpenBraceToken.Span).StartLinePosition.Line;
                var closeBraceLineNumber = block.SyntaxTree.GetLineSpan(block.CloseBraceToken.Span).StartLinePosition.Line;

                if (openBraceLineNumber == closeBraceLineNumber)
                {
                    switch (block.Parent.Kind())
                    {
                    case SyntaxKind.AnonymousMethodExpression:
                    case SyntaxKind.SimpleLambdaExpression:
                    case SyntaxKind.ParenthesizedLambdaExpression:
                        var containingExpression = GetContainingExpression(block.Parent);
                        if (IsSingleLineExpression(containingExpression))
                        {
                            // Single line lambda expressions and anonymous method declarations are allowed for single line expressions.
                            return;
                        }

                        break;
                    }

                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, block.OpenBraceToken.GetLocation()));
                }
            }
        }

        private static void HandleIfStatement(SyntaxNodeAnalysisContext context)
        {
            var ifStatement = (IfStatementSyntax)context.Node;
            if (ifStatement.Parent.IsKind(SyntaxKind.ElseClause))
            {
                // this will be analyzed as a clause of the outer if statement
                return;
            }

            List<StatementSyntax> clauses = new List<StatementSyntax>();
            for (IfStatementSyntax current = ifStatement; current != null; current = current.Else?.Statement as IfStatementSyntax)
            {
                clauses.Add(current.Statement);
                if (current.Else != null && !(current.Else.Statement is IfStatementSyntax))
                {
                    clauses.Add(current.Else.Statement);
                }
            }

            if (!context.IsAnalyzerSuppressed(SA1520UseCurlyBracketsConsistently.DiagnosticId))
            {
                // inconsistencies will be reported as SA1520, as long as it's not suppressed
                if (clauses.OfType<BlockSyntax>().Any())
                {
                    return;
                }
            }

            foreach (StatementSyntax clause in clauses)
            {
                SyntaxNode node = clause.Parent;
                if (node.IsKind(SyntaxKind.IfStatement) && node.Parent.IsKind(SyntaxKind.ElseClause))
                {
                    node = node.Parent;
                }

                CheckChildStatement(context, node, clause);
            }
        }

        private static void CheckChildStatement(SyntaxNodeAnalysisContext context, SyntaxNode node, StatementSyntax childStatement)
        {
            if (childStatement == null || childStatement.IsMissing)
            {
                return;
            }

            if (childStatement is BlockSyntax)
            {
                // BlockSyntax child statements are handled by HandleBlock
                return;
            }

            // We are only interested in the first instance of this violation on a line.
            if (!node.GetFirstToken().IsFirstInLine())
            {
                return;
            }

            // We are only interested in the case where statement and childStatement start on the same line. Use
            // IsFirstInLine to detect this condition easily.
            SyntaxToken firstChildToken = childStatement.GetFirstToken();
            if (firstChildToken.IsMissingOrDefault() || firstChildToken.IsFirstInLine())
            {
                return;
            }

            if (!context.IsAnalyzerSuppressed(SA1519CurlyBracketsMustNotBeOmittedFromMultiLineChildStatement.DiagnosticId))
            {
                // diagnostics for multi-line statements is handled by SA1519, as long as it's not suppressed
                FileLinePositionSpan lineSpan = childStatement.GetLineSpan();
                if (lineSpan.StartLinePosition.Line != lineSpan.EndLinePosition.Line)
                {
                    return;
                }
            }

            context.ReportDiagnostic(Diagnostic.Create(Descriptor, childStatement.GetLocation()));
        }

        private static bool IsSingleLineExpression(ExpressionSyntax containingExpression)
        {
            if (containingExpression == null)
            {
                return false;
            }

            var lineSpan = containingExpression.SyntaxTree.GetLineSpan(containingExpression.Span);
            return lineSpan.StartLinePosition.Line == lineSpan.EndLinePosition.Line;
        }

        private static bool IsPartOfStatement(BlockSyntax block)
        {
            return block.Parent.FirstAncestorOrSelf<StatementSyntax>() != null;
        }

        private static ExpressionSyntax GetContainingExpression(SyntaxNode node)
        {
            return node.FirstAncestorOrSelf<ExpressionSyntax>();
        }
    }
}
