namespace StyleCop.Analyzers.ReadabilityRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// The C# code contains a region.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs whenever a region is placed anywhere within the code. In many editors,
    /// including Visual Studio, the region will appear collapsed by default, hiding the code within the region. It is
    /// generally a bad practice to hide code, as this can lead to bad decisions as the code is maintained over
    /// time.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1124DoNotUseRegions : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "SA1124";
        internal const string Title = "Do not use regions";
        internal const string MessageFormat = "TODO: Message format";
        internal const string Category = "StyleCop.CSharp.ReadabilityRules";
        internal const string Description = "The C# code contains a region.";
        internal const string HelpLink = "http://www.stylecop.com/docs/SA1124.html";

        private static readonly DiagnosticDescriptor Descriptor =
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
