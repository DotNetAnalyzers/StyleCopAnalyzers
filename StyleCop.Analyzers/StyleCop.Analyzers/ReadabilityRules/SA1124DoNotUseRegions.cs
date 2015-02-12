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
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1124DoNotUseRegions"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1124";
        private const string Title = "Do not use regions";
        private const string MessageFormat = "TODO: Message format";
        private const string Category = "StyleCop.CSharp.ReadabilityRules";
        private const string Description = "The C# code contains a region.";
        private const string HelpLink = "http://www.stylecop.com/docs/SA1124.html";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, AnalyzerConstants.DisabledNoTests, Description, HelpLink);

        private static readonly ImmutableArray<DiagnosticDescriptor> supportedDiagnostics =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return supportedDiagnostics;
            }
        }

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            // TODO: Implement analysis
        }
    }
}
