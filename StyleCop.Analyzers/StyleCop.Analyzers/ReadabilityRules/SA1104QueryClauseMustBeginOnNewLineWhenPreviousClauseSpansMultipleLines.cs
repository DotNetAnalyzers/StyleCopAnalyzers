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
        /// <summary>
        /// The ID for diagnostics produced by the
        /// <see cref="SA1104QueryClauseMustBeginOnNewLineWhenPreviousClauseSpansMultipleLines"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1104";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(ReadabilityResources.SA1104Title), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(ReadabilityResources.SA1104MessageFormat), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly string Category = "StyleCop.CSharp.ReadabilityRules";
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(ReadabilityResources.SA1104Description), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly string HelpLink = "http://www.stylecop.com/docs/SA1104.html";

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
