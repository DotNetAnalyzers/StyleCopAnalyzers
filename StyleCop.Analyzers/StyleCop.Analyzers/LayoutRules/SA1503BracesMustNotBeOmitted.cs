// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

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
    using StyleCop.Analyzers.Settings.ObjectModel;

    /// <summary>
    /// The opening and closing braces for a C# statement have been omitted.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when the opening and closing braces for a statement have been omitted. In
    /// C#, some types of statements may optionally include braces. Examples include <c>if</c>, <c>while</c>, and
    /// <c>for</c> statements. For example, an if-statement may be written without braces:</para>
    ///
    /// <code language="csharp">
    /// if (true)
    ///     return this.value;
    /// </code>
    ///
    /// <para>Although this is legal in C#, StyleCop always requires the braces to be present, to increase the
    /// readability and maintainability of the code.</para>
    ///
    /// <para>When the braces are omitted, it is possible to introduce an error in the code by inserting an additional
    /// statement beneath the if-statement. For example:</para>
    ///
    /// <code language="csharp">
    /// if (true)
    ///     this.value = 2;
    ///     return this.value;
    /// </code>
    ///
    /// <para>Glancing at this code, it appears as if both the assignment statement and the return statement are
    /// children of the if-statement. In fact, this is not true. Only the assignment statement is a child of the
    /// if-statement, and the return statement will always execute regardless of the outcome of the if-statement.</para>
    ///
    /// <para>StyleCop always requires the opening and closing braces to be present, to prevent these kinds of
    /// errors:</para>
    ///
    /// <code language="csharp">
    /// if (true)
    /// {
    ///     this.value = 2;
    ///     return this.value;
    /// }
    /// </code>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1503BracesMustNotBeOmitted : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1503BracesMustNotBeOmitted"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1503";

        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1503.md";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(LayoutResources.SA1503Title), LayoutResources.ResourceManager, typeof(LayoutResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(LayoutResources.SA1503MessageFormat), LayoutResources.ResourceManager, typeof(LayoutResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(LayoutResources.SA1503Description), LayoutResources.ResourceManager, typeof(LayoutResources));

#pragma warning disable SA1202 // Elements should be ordered by access
        internal static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.LayoutRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);
#pragma warning restore SA1202 // Elements should be ordered by access

        private static readonly Action<SyntaxNodeAnalysisContext> IfStatementAction = HandleIfStatement;
        private static readonly Action<SyntaxNodeAnalysisContext, StyleCopSettings> UsingStatementAction = HandleUsingStatement;

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(IfStatementAction, SyntaxKind.IfStatement);
            context.RegisterSyntaxNodeAction(ctx => CheckChildStatement(ctx, ((DoStatementSyntax)ctx.Node).Statement), SyntaxKind.DoStatement);
            context.RegisterSyntaxNodeAction(ctx => CheckChildStatement(ctx, ((WhileStatementSyntax)ctx.Node).Statement), SyntaxKind.WhileStatement);
            context.RegisterSyntaxNodeAction(ctx => CheckChildStatement(ctx, ((ForStatementSyntax)ctx.Node).Statement), SyntaxKind.ForStatement);
            context.RegisterSyntaxNodeAction(ctx => CheckChildStatement(ctx, ((ForEachStatementSyntax)ctx.Node).Statement), SyntaxKind.ForEachStatement);
            context.RegisterSyntaxNodeAction(ctx => CheckChildStatement(ctx, ((FixedStatementSyntax)ctx.Node).Statement), SyntaxKind.FixedStatement);
            context.RegisterSyntaxNodeAction(UsingStatementAction, SyntaxKind.UsingStatement);
            context.RegisterSyntaxNodeAction(ctx => CheckChildStatement(ctx, ((LockStatementSyntax)ctx.Node).Statement), SyntaxKind.LockStatement);
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

        private static void HandleUsingStatement(SyntaxNodeAnalysisContext context, StyleCopSettings settings)
        {
            var usingStatement = (UsingStatementSyntax)context.Node;
            if (settings.LayoutRules.AllowConsecutiveUsings && usingStatement.Statement.IsKind(SyntaxKind.UsingStatement))
            {
                return;
            }

            CheckChildStatement(context, usingStatement.Statement);
        }

        private static void CheckChildStatement(SyntaxNodeAnalysisContext context, StatementSyntax childStatement)
        {
            if (childStatement is BlockSyntax)
            {
                return;
            }

            if (!context.IsAnalyzerSuppressed(SA1519BracesMustNotBeOmittedFromMultiLineChildStatement.Descriptor))
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
    }
}
