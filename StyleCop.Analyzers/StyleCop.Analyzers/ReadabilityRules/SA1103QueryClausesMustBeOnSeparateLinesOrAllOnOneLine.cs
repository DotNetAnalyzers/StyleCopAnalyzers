namespace StyleCop.Analyzers.ReadabilityRules
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.Helpers;

    /// <summary>
    /// The clauses within a C# query expression are not all placed on the same line, and each clause is not placed on
    /// its own line.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when the query clauses are not either placed all on the same line, or each
    /// on its own line. For example:</para>
    /// <code language="csharp">
    /// object x = from num in numbers
    ///     select num;
    /// </code>
    /// <para>The query clauses can correctly be written as:</para>
    /// <code language="csharp">
    /// object x = from num in numbers select num;
    /// </code>
    /// <para>or:</para>
    /// <code language="csharp">
    /// object x =
    ///     from num in numbers
    ///     select num;
    /// </code>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1103QueryClausesMustBeOnSeparateLinesOrAllOnOneLine : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1103QueryClausesMustBeOnSeparateLinesOrAllOnOneLine"/>
        /// analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1103";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(ReadabilityResources.SA1103Title), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(ReadabilityResources.SA1103MessageFormat), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly string Category = "StyleCop.CSharp.ReadabilityRules";
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(ReadabilityResources.SA1103Description), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly string HelpLink = "http://www.stylecop.com/docs/SA1103.html";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly ImmutableArray<DiagnosticDescriptor> SupportedDiagnosticsValue =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => SupportedDiagnosticsValue;

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeActionHonorExclusions(HandleQueryExpression, SyntaxKind.QueryExpression);
        }

        private static void HandleQueryExpression(SyntaxNodeAnalysisContext context)
        {
            var queryExpression = (QueryExpressionSyntax)context.Node;
            var tokensToCheck = new List<SyntaxToken>();

            tokensToCheck.Add(queryExpression.FromClause.FromKeyword.GetPreviousToken());
            HandleQueryClause(queryExpression.FromClause, tokensToCheck);
            HandleQueryBody(queryExpression.Body, tokensToCheck);

            bool allOnSameLine = true;
            bool allOnSeparateLine = true;
            for (var i = 0; (allOnSameLine || allOnSeparateLine) && (i < tokensToCheck.Count - 1); i++)
            {
                var tokensOnSameLine = tokensToCheck[i].GetLine() == tokensToCheck[i + 1].GetLine();

                allOnSameLine = allOnSameLine && tokensOnSameLine;
                allOnSeparateLine = allOnSeparateLine && !tokensOnSameLine;
            }

            if (!allOnSameLine && !allOnSeparateLine)
            {
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, queryExpression.FromClause.FromKeyword.GetLocation()));
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
            HandleQueryBody(continuation.Body, tokensToCheck);
        }
    }
}
