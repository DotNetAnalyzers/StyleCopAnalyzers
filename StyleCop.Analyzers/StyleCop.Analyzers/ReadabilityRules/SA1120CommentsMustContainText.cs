namespace StyleCop.Analyzers.ReadabilityRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// The C# comment does not contain any comment text.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs whenever the code contains a C# comment which does not contain any
    /// text.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1120CommentsMustContainText : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1120CommentsMustContainText"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1120";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(ReadabilityResources.SA1120Title), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(ReadabilityResources.SA1120MessageFormat), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly string Category = "StyleCop.CSharp.ReadabilityRules";
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(ReadabilityResources.SA1120Description), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly string HelpLink = "http://www.stylecop.com/docs/SA1120.html";

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
