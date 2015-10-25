// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.ReadabilityRules
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
    /// This analyzer will analyze several diagnostics related to query expressions.
    /// </summary>
    /// <seealso href="https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1102.md">SA1102 Query clause must follow previous clause</seealso>
    /// <seealso href="https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1103.md">SA1103 Query clauses must be on separate lines or all on one line</seealso>
    /// <seealso href="https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1104.md">SA1104 Query clause must begin on new line when previous clause spans multiple lines</seealso>
    /// <seealso href="https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1105.md">SA1105 Query clauses spanning multiple lines must begin on own line</seealso>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA110xQueryClauses : DiagnosticAnalyzer
    {
        private const string SA1102Identifier = "SA1102";
        private const string SA1103Identifier = "SA1103";
        private const string SA1104Identifier = "SA1104";
        private const string SA1105Identifier = "SA1105";

        private static readonly LocalizableString SA1102Title = new LocalizableResourceString(nameof(ReadabilityResources.SA1102Title), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly LocalizableString SA1102MessageFormat = new LocalizableResourceString(nameof(ReadabilityResources.SA1102MessageFormat), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly LocalizableString SA1102Description = new LocalizableResourceString(nameof(ReadabilityResources.SA1102Description), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly string SA1102HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1102.md";

        private static readonly LocalizableString SA1103Title = new LocalizableResourceString(nameof(ReadabilityResources.SA1103Title), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly LocalizableString SA1103MessageFormat = new LocalizableResourceString(nameof(ReadabilityResources.SA1103MessageFormat), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly LocalizableString SA1103Description = new LocalizableResourceString(nameof(ReadabilityResources.SA1103Description), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly string SA1103HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1103.md";

        private static readonly LocalizableString SA1104Title = new LocalizableResourceString(nameof(ReadabilityResources.SA1104Title), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly LocalizableString SA1104MessageFormat = new LocalizableResourceString(nameof(ReadabilityResources.SA1104MessageFormat), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly LocalizableString SA1104Description = new LocalizableResourceString(nameof(ReadabilityResources.SA1104Description), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly string SA1104HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1104.md";

        private static readonly LocalizableString SA1105Title = new LocalizableResourceString(nameof(ReadabilityResources.SA1105Title), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly LocalizableString SA1105MessageFormat = new LocalizableResourceString(nameof(ReadabilityResources.SA1105MessageFormat), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly LocalizableString SA1105Description = new LocalizableResourceString(nameof(ReadabilityResources.SA1105Description), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly string SA1105HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1105.md";

        private static readonly Action<CompilationStartAnalysisContext> CompilationStartAction = HandleCompilationStart;
        private static readonly Action<SyntaxNodeAnalysisContext> QueryExpressionAction = HandleQueryExpression;

        /// <summary>
        /// Gets the diagnostic descriptor for SA1102.
        /// </summary>
        /// <value>The <see cref="DiagnosticDescriptor"/> for SA1102.</value>
        public static DiagnosticDescriptor SA1102Descriptor { get; } =
            new DiagnosticDescriptor(SA1102Identifier, SA1102Title, SA1102MessageFormat, AnalyzerCategory.ReadabilityRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, SA1102Description, SA1102HelpLink);

        /// <summary>
        /// Gets the diagnostic descriptor for SA1103.
        /// </summary>
        /// <value>The <see cref="DiagnosticDescriptor"/> for SA1103.</value>
        public static DiagnosticDescriptor SA1103Descriptor { get; } =
            new DiagnosticDescriptor(SA1103Identifier, SA1103Title, SA1103MessageFormat, AnalyzerCategory.ReadabilityRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, SA1103Description, SA1103HelpLink);

        /// <summary>
        /// Gets the diagnostic descriptor for SA1104.
        /// </summary>
        /// <value>The <see cref="DiagnosticDescriptor"/> for SA1104.</value>
        public static DiagnosticDescriptor SA1104Descriptor { get; } =
            new DiagnosticDescriptor(SA1104Identifier, SA1104Title, SA1104MessageFormat, AnalyzerCategory.ReadabilityRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, SA1104Description, SA1104HelpLink);

        /// <summary>
        /// Gets the diagnostic descriptor for SA1105.
        /// </summary>
        /// <value>The <see cref="DiagnosticDescriptor"/> for SA1105.</value>
        public static DiagnosticDescriptor SA1105Descriptor { get; } =
            new DiagnosticDescriptor(SA1105Identifier, SA1105Title, SA1105MessageFormat, AnalyzerCategory.ReadabilityRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, SA1105Description, SA1105HelpLink);

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(
                SA1102Descriptor,
                SA1103Descriptor,
                SA1104Descriptor,
                SA1105Descriptor);

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.RegisterCompilationStartAction(CompilationStartAction);
        }

        private static void HandleCompilationStart(CompilationStartAnalysisContext context)
        {
            context.RegisterSyntaxNodeActionHonorExclusions(QueryExpressionAction, SyntaxKind.QueryExpression);
        }

        private static void HandleQueryExpression(SyntaxNodeAnalysisContext context)
        {
            var queryExpression = (QueryExpressionSyntax)context.Node;
            var tokensToCheck = new List<SyntaxToken>();

            HandleQueryClause(queryExpression.FromClause, tokensToCheck);
            HandleQueryBody(queryExpression.Body, tokensToCheck);

            bool isEnabledSA1102 = !context.IsAnalyzerSuppressed(SA1102Identifier);
            bool isEnabledSA1103 = !context.IsAnalyzerSuppressed(SA1103Identifier);
            bool isEnabledSA1104 = !context.IsAnalyzerSuppressed(SA1104Identifier);
            bool isEnabledSA1105 = !context.IsAnalyzerSuppressed(SA1105Identifier);

            bool allOnSameLine = true;
            bool allOnSeparateLine = true;
            bool suppressSA1103 = false;

            for (var i = 0; i < tokensToCheck.Count - 1; i++)
            {
                var token1 = tokensToCheck[i];
                var token2 = tokensToCheck[i + 1];
                var parent1 = token1.Parent;
                var parent2 = token2.Parent;

                if (parent2 is QueryContinuationSyntax)
                {
                    continue;
                }

                // The node before the query clause may encompass the entire query clause,
                // so we need to use the token within that node for the line determination.
                var location1 = !(parent1 is QueryContinuationSyntax) ? parent1.GetLineSpan() : token1.GetLineSpan();
                var location2 = parent2.GetLineSpan();

                if (((location2.StartLinePosition.Line - location1.EndLinePosition.Line) > 1)
                    && isEnabledSA1102
                    && !token2.LeadingTrivia.Any(trivia => trivia.IsDirective))
                {
                    context.ReportDiagnostic(Diagnostic.Create(SA1102Descriptor, token2.GetLocation()));
                }

                var onSameLine = location1.EndLinePosition.Line == location2.StartLinePosition.Line;

                if (onSameLine
                    && isEnabledSA1104
                    && !(parent1 is QueryContinuationSyntax)
                    && parent1.SpansMultipleLines())
                {
                    context.ReportDiagnostic(Diagnostic.Create(SA1104Descriptor, token2.GetLocation()));

                    // Make sure that SA1103 will not be reported, as there is a more specific diagnostic reported.
                    suppressSA1103 = true;
                }

                if (onSameLine
                    && isEnabledSA1105
                    && parent2.SpansMultipleLines())
                {
                    context.ReportDiagnostic(Diagnostic.Create(SA1105Descriptor, token2.GetLocation()));

                    // Make sure that SA1103 will not be reported, as there is a more specific diagnostic reported.
                    suppressSA1103 = true;
                }

                allOnSameLine = allOnSameLine && onSameLine;
                allOnSeparateLine = allOnSeparateLine && !onSameLine;
            }

            if (!allOnSameLine && !allOnSeparateLine && !suppressSA1103 && isEnabledSA1103)
            {
                context.ReportDiagnostic(Diagnostic.Create(SA1103Descriptor, queryExpression.FromClause.FromKeyword.GetLocation()));
            }
        }

        private static void HandleQueryBody(QueryBodySyntax body, List<SyntaxToken> tokensToCheck)
        {
            foreach (var clause in body.Clauses)
            {
                HandleQueryClause(clause, tokensToCheck);
            }

            HandleSelectOrGroup(body.SelectOrGroup, tokensToCheck);

            if (body.Continuation != null)
            {
                HandleContinuation(body.Continuation, tokensToCheck);
            }
        }

        private static void HandleQueryClause(QueryClauseSyntax queryClause, List<SyntaxToken> tokensToCheck)
        {
            switch (queryClause.Kind())
            {
            case SyntaxKind.FromClause:
                var fromClause = (FromClauseSyntax)queryClause;
                tokensToCheck.Add(fromClause.FromKeyword);
                break;
            case SyntaxKind.LetClause:
                var letClause = (LetClauseSyntax)queryClause;
                tokensToCheck.Add(letClause.LetKeyword);
                break;
            case SyntaxKind.WhereClause:
                var whereClause = (WhereClauseSyntax)queryClause;
                tokensToCheck.Add(whereClause.WhereKeyword);
                break;
            case SyntaxKind.JoinClause:
            case SyntaxKind.JoinIntoClause:
                var joinClause = (JoinClauseSyntax)queryClause;
                tokensToCheck.Add(joinClause.JoinKeyword);
                break;
            case SyntaxKind.OrderByClause:
                var orderByClause = (OrderByClauseSyntax)queryClause;
                tokensToCheck.Add(orderByClause.OrderByKeyword);
                break;
            }
        }

        private static void HandleSelectOrGroup(SelectOrGroupClauseSyntax selectOrGroup, List<SyntaxToken> tokensToCheck)
        {
            switch (selectOrGroup.Kind())
            {
            case SyntaxKind.SelectClause:
                var selectClause = (SelectClauseSyntax)selectOrGroup;
                tokensToCheck.Add(selectClause.SelectKeyword);
                break;
            case SyntaxKind.GroupClause:
                var groupClause = (GroupClauseSyntax)selectOrGroup;
                tokensToCheck.Add(groupClause.GroupKeyword);
                break;
            }
        }

        private static void HandleContinuation(QueryContinuationSyntax continuation, List<SyntaxToken> tokensToCheck)
        {
            tokensToCheck.Add(continuation.IntoKeyword);
            HandleQueryBody(continuation.Body, tokensToCheck);
        }
    }
}
