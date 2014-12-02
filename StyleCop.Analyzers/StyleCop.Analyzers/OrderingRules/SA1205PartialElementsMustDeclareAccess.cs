namespace StyleCop.Analyzers.OrderingRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// The partial element does not have an access modifier defined. StyleCop may not be able to determine the correct
    /// placement of the elements in the file. Please declare an access modifier for all partial elements.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when the partial elements does not have an access modifier defined.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1205PartialElementsMustDeclareAccess : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "SA1205";
        internal const string Title = "The partial element does not have an access modifier defined. StyleCop may not be able to determine the correct placement of the elements in the file. Please declare an access modifier for all partial elements.";
        internal const string MessageFormat = "TODO: Message format";
        internal const string Category = "StyleCop.CSharp.OrderingRules";
        internal const string Description = "The partial element does not have an access modifier defined. StyleCop may not be able to determine the correct placement of the elements in the file. Please declare an access modifier for all partial elements.";
        internal const string HelpLink = "http://www.stylecop.com/docs/SA1205.html";

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
