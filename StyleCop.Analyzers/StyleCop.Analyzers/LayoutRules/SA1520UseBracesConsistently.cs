// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

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
    /// The opening and closing braces of a chained <c>if</c>/<c>else if</c>/<c>else</c> construct were included for
    /// some clauses, but omitted for others.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when the opening and closing braces for a chained statement have been
    /// included for some clauses but omitted for others. In C#, some types of statements may optionally include
    /// braces. For example, an <c>if</c>-statement may be written with inconsistent braces:</para>
    ///
    /// <code language="csharp">
    /// if (true)
    ///     return this.value;
    /// else
    /// {
    ///     return that.value.
    /// }
    /// </code>
    ///
    /// <para>Although this is legal in C#, StyleCop requires the braces to be present for all clauses of a chained
    /// <c>if</c>/<c>else if</c>/<c>else</c> construct when braces are included for any clause, to increase the
    /// readability and maintainability of the code.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1520UseBracesConsistently : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1520UseBracesConsistently"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1520";

        /// <summary>
        /// The diagnostic descriptor for the <see cref="SA1520UseBracesConsistently"/> analyzer.
        /// </summary>
        public static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.LayoutRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(LayoutResources.SA1520Title), LayoutResources.ResourceManager, typeof(LayoutResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(LayoutResources.SA1520MessageFormat), LayoutResources.ResourceManager, typeof(LayoutResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(LayoutResources.SA1520Description), LayoutResources.ResourceManager, typeof(LayoutResources));
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1520.md";

        private static readonly Action<SyntaxNodeAnalysisContext> IfStatementAction = HandleIfStatement;

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(IfStatementAction, SyntaxKind.IfStatement);
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

            if (clauses.All(i => i is BlockSyntax))
            {
                // consistent inclusion of braces
                return;
            }

            if (!clauses.OfType<BlockSyntax>().Any())
            {
                // consistent lack of braces
                return;
            }

            foreach (StatementSyntax clause in clauses)
            {
                CheckChildStatement(context, clause);
            }
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
