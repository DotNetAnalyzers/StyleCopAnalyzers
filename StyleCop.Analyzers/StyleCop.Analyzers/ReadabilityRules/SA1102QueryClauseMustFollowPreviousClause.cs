namespace StyleCop.Analyzers.ReadabilityRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
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
        private const string Title = "Query clause must follow previous clause";
        private const string MessageFormat = "TODO: Message format";
        private const string Category = "StyleCop.CSharp.ReadabilityRules";
        private const string Description = "A C# query clause does not begin on the same line as the previous clause, or on the next line.";
        private const string HelpLink = "http://www.stylecop.com/docs/SA1102.html";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, AnalyzerConstants.DisabledNoTests, Description, HelpLink);

        private static readonly ImmutableArray<DiagnosticDescriptor> SupportedDiagnosticsValue =
            ImmutableArray.Create(Descriptor);

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
            // TODO: Implement analysis
        }
    }
}
