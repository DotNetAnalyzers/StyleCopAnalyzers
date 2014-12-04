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
        public const string DiagnosticId = "SA1102";
        internal const string Title = "Query clause must follow previous clause";
        internal const string MessageFormat = "TODO: Message format";
        internal const string Category = "StyleCop.CSharp.ReadabilityRules";
        internal const string Description = "A C# query clause does not begin on the same line as the previous clause, or on the next line.";
        internal const string HelpLink = "http://www.stylecop.com/docs/SA1102.html";

        public static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, AnalyzerConstants.DisabledNoTests, Description, HelpLink);

        private static readonly ImmutableArray<DiagnosticDescriptor> _supportedDiagnostics =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return _supportedDiagnostics;
            }
        }

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            // TODO: Implement analysis
        }
    }
}
