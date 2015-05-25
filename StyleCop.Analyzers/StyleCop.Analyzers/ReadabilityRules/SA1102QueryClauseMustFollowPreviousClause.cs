namespace StyleCop.Analyzers.ReadabilityRules
{
    using System.Collections.Immutable;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// A C# query clause does not begin on the same line as the previous clause, or on the next line.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when a clause within a query expression does not begin on the same line as
    /// the previous clause, or on the line after the query clause. For example:</para>
    /// <code language="csharp">
    /// object x = select a in b
    ///     from c;
    /// </code>
    /// <para>The query clause can correctly be written as:</para>
    /// <code language="csharp">
    /// object x = select a in b from c;
    /// </code>
    /// <para>or:</para>
    /// <code language="csharp">
    /// object x =
    ///     select a
    ///     in b
    ///     from c;
    /// </code>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1102QueryClauseMustFollowPreviousClause : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1102QueryClauseMustFollowPreviousClause"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1102";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(ReadabilityResources.SA1102Title), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(ReadabilityResources.SA1102MessageFormat), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly string Category = "StyleCop.CSharp.ReadabilityRules";
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(ReadabilityResources.SA1102Description), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly string HelpLink = "http://www.stylecop.com/docs/SA1102.html";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly ImmutableArray<DiagnosticDescriptor> SupportedDiagnosticsValue =
            ImmutableArray.Create(Descriptor);

        private readonly SyntaxKind[] supportedKinds = new[]
        {
            SyntaxKind.WhereKeyword,
            SyntaxKind.GroupKeyword,
            SyntaxKind.LetKeyword,
            SyntaxKind.JoinKeyword,
            SyntaxKind.SelectKeyword,
            SyntaxKind.OrderByKeyword
        };

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return SupportedDiagnosticsValue;
            }
        }

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleQuery, SyntaxKind.QueryExpression);
        }

        private void HandleQuery(SyntaxNodeAnalysisContext context)
        {
            var query = (QueryExpressionSyntax)context.Node;

            var clauses = query.DescendantTokens()
                .Where(t => this.supportedKinds.Any(sk => t.IsKind(sk)))
                .Where(t => this.IsQueryParentForNode(t.Parent, query))
                .Where(t => !t.IsMissing)
                .ToList();

            var allElementsAreAtTheSameLine = this.CheckIfAllElementsAreAtTheSameLine(query);
            if (allElementsAreAtTheSameLine.HasValue && !allElementsAreAtTheSameLine.Value)
            {
                foreach (var clause in clauses)
                {
                    var isPreviousLineEmpty = this.IsPreviousLineEmpty(clause);
                    var isFirstToken = this.IsFirstToken(clause);
                    if ((isPreviousLineEmpty.HasValue && isPreviousLineEmpty.Value) ||
                        (isFirstToken.HasValue && !isFirstToken.Value))
                    {
                        context.ReportDiagnostic(Diagnostic.Create(Descriptor, clause.GetLocation()));
                    }
                }
            }
        }

        private bool IsQueryParentForNode(SyntaxNode syntaxNode, QueryExpressionSyntax queryExpressionSyntax)
        {
            if (syntaxNode == null)
            {
                return false;
            }

            if (syntaxNode.IsKind(queryExpressionSyntax.Kind()))
            {
                return syntaxNode == queryExpressionSyntax;
            }

            return this.IsQueryParentForNode(syntaxNode.Parent, queryExpressionSyntax);
        }

        private bool? IsFirstToken(SyntaxToken clause)
        {
            var fileLinePositionSpan = clause.GetLocation().GetLineSpan();
            if (!fileLinePositionSpan.IsValid)
            {
                return null;
            }

            var startLine = fileLinePositionSpan.StartLinePosition.Line;
            var previousToken = clause.GetPreviousToken();
            if (previousToken.IsMissing)
            {
                return true;
            }

            var endLineOfPreviousToken = previousToken.GetLocation().GetLineSpan().EndLinePosition.Line;
            return startLine > endLineOfPreviousToken;
        }

        private bool? IsPreviousLineEmpty(SyntaxToken clause)
        {
            var fileLinePositionSpan = clause.GetLocation().GetLineSpan();
            if (!fileLinePositionSpan.IsValid)
            {
                return null;
            }

            var startLine = fileLinePositionSpan.StartLinePosition.Line;
            var previousToken = clause.GetPreviousToken();
            if (previousToken.IsMissing)
            {
                return false;
            }

            var endLineOfPreviousToken = previousToken.GetLocation().GetLineSpan().EndLinePosition.Line;
            return (startLine - endLineOfPreviousToken) > 1;
        }

        private bool? CheckIfAllElementsAreAtTheSameLine(QueryExpressionSyntax query)
        {
            var fileLinePositionSpan = query.GetLocation().GetLineSpan();

            if (fileLinePositionSpan.IsValid)
            {
                return fileLinePositionSpan.StartLinePosition.Line == fileLinePositionSpan.EndLinePosition.Line;
            }

            return null;
        }
    }
}
