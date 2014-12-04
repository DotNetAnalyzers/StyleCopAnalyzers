namespace StyleCop.Analyzers.ReadabilityRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// A clause within a C# query expression begins on the same line as the previous clause, when the previous clause
    /// spans across multiple lines.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when a query clause spans across multiple lines, and the next clause
    /// begins on the same line as the end of the previous clause.</para>
    /// <code language="csharp">
    /// object x =
    ///     select a
    ///     in b.GetCustomers(
    ///         2, "x") from c;
    /// </code>
    /// <para>The query clause can correctly be written as:</para>
    /// <code language="csharp">
    /// object x =
    ///     select a
    ///     in b.GetCustomers(
    ///         2, "x")
    ///     from c;
    /// </code>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1104QueryClauseMustBeginOnNewLineWhenPreviousClauseSpansMultipleLines : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "SA1104";
        internal const string Title = "Query clause must begin on new line when previous clause spans multiple lines";
        internal const string MessageFormat = "TODO: Message format";
        internal const string Category = "StyleCop.CSharp.ReadabilityRules";
        internal const string Description = "A clause within a C# query expression begins on the same line as the previous clause, when the previous clause spans across multiple lines.";
        internal const string HelpLink = "http://www.stylecop.com/docs/SA1104.html";

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
