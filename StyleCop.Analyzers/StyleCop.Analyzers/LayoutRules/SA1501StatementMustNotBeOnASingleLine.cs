// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.LayoutRules
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.Helpers;

    /// <summary>
    /// A C# statement containing opening and closing braces is written completely on a single line.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when a statement that is wrapped in opening and closing braces is written
    /// on a single line. For example:</para>
    ///
    /// <code language="csharp">
    /// public object Method()
    /// {
    ///     lock (this) { return this.value; }
    /// }
    /// </code>
    ///
    /// <para>When StyleCop checks this code, a violation of this rule will occur because the entire lock statement is
    /// written on one line. The statement should be written across multiple lines, with the opening and closing braces
    /// each on their own line, as follows:</para>
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

        internal const string SuppressCodeFixKey = "SuppressCodeFix";
        internal const string SuppressCodeFixValue = "true";

        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1501.md";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(LayoutResources.SA1501Title), LayoutResources.ResourceManager, typeof(LayoutResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(LayoutResources.SA1501MessageFormat), LayoutResources.ResourceManager, typeof(LayoutResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(LayoutResources.SA1501Description), LayoutResources.ResourceManager, typeof(LayoutResources));

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.LayoutRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly ImmutableDictionary<string, string> SuppressCodeFixProperties =
            ImmutableDictionary<string, string>.Empty.Add(SuppressCodeFixKey, SuppressCodeFixValue);

        private static readonly Action<CompilationStartAnalysisContext> CompilationStartAction = HandleCompilationStart;

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(HandleBlock, SyntaxKind.Block);
            context.RegisterCompilationStartAction(CompilationStartAction);
        }

        private static void HandleCompilationStart(CompilationStartAnalysisContext context)
        {
            // If SA1503 is suppressed, we need to handle compound blocks as well.
            if (context.IsAnalyzerSuppressed(SA1503BracesMustNotBeOmitted.Descriptor))
            {
                context.RegisterSyntaxNodeAction(HandleIfStatement, SyntaxKind.IfStatement);
                context.RegisterSyntaxNodeAction(ctx => CheckChildStatement(ctx, ((DoStatementSyntax)ctx.Node).Statement), SyntaxKind.DoStatement);
                context.RegisterSyntaxNodeAction(ctx => CheckChildStatement(ctx, ((WhileStatementSyntax)ctx.Node).Statement), SyntaxKind.WhileStatement);
                context.RegisterSyntaxNodeAction(ctx => CheckChildStatement(ctx, ((ForStatementSyntax)ctx.Node).Statement), SyntaxKind.ForStatement);
                context.RegisterSyntaxNodeAction(ctx => CheckChildStatement(ctx, ((ForEachStatementSyntax)ctx.Node).Statement), SyntaxKind.ForEachStatement);
                context.RegisterSyntaxNodeAction(ctx => CheckChildStatement(ctx, ((LockStatementSyntax)ctx.Node).Statement), SyntaxKind.LockStatement);
                context.RegisterSyntaxNodeAction(ctx => CheckChildStatement(ctx, ((UsingStatementSyntax)ctx.Node).Statement), SyntaxKind.UsingStatement);
                context.RegisterSyntaxNodeAction(ctx => CheckChildStatement(ctx, ((FixedStatementSyntax)ctx.Node).Statement), SyntaxKind.FixedStatement);
            }
        }

        private static void HandleBlock(SyntaxNodeAnalysisContext context)
        {
            var block = (BlockSyntax)context.Node;
            if (!block.OpenBraceToken.IsMissing &&
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

                    ReportDiagnostic(context, block.OpenBraceToken.GetLocation());
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

            if (!context.IsAnalyzerSuppressed(SA1520UseBracesConsistently.Descriptor))
            {
                // inconsistencies will be reported as SA1520, as long as it's not suppressed
                if (clauses.OfType<BlockSyntax>().Any())
                {
                    return;
                }
            }

            foreach (StatementSyntax clause in clauses)
            {
                CheckChildStatement(context, clause);
            }
        }

        private static void CheckChildStatement(SyntaxNodeAnalysisContext context, StatementSyntax childStatement)
        {
            bool reportAsHidden = false;

            if (childStatement == null || childStatement.IsMissing)
            {
                return;
            }

            if (childStatement is BlockSyntax)
            {
                // BlockSyntax child statements are handled by HandleBlock
                return;
            }

            // We are only interested in the case where statement and childStatement start on the same line. Use
            // IsFirstInLine to detect this condition easily.
            SyntaxToken firstChildToken = childStatement.GetFirstToken();
            if (firstChildToken.IsMissingOrDefault() || firstChildToken.IsFirstInLine())
            {
                return;
            }

            if (!context.IsAnalyzerSuppressed(SA1519BracesMustNotBeOmittedFromMultiLineChildStatement.Descriptor))
            {
                // diagnostics for multi-line statements is handled by SA1519, as long as it's not suppressed
                FileLinePositionSpan lineSpan = childStatement.GetLineSpan();
                if (lineSpan.StartLinePosition.Line != lineSpan.EndLinePosition.Line)
                {
                    reportAsHidden = true;
                }
            }

            ReportDiagnostic(context, childStatement.GetLocation(), reportAsHidden);
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

        private static void ReportDiagnostic(SyntaxNodeAnalysisContext context, Location location, bool reportAsHidden = false)
        {
            Diagnostic diagnostic;

            if (reportAsHidden)
            {
                diagnostic = Diagnostic.Create(
                    Descriptor.Id,
                    Descriptor.Category,
                    Descriptor.MessageFormat,
                    DiagnosticSeverity.Hidden,
                    Descriptor.DefaultSeverity,
                    Descriptor.IsEnabledByDefault,
                    1,
                    Descriptor.Title,
                    Descriptor.Description,
                    Descriptor.HelpLinkUri,
                    location,
                    properties: SuppressCodeFixProperties);
            }
            else
            {
                diagnostic = Diagnostic.Create(Descriptor, location);
            }

            context.ReportDiagnostic(diagnostic);
        }
    }
}
