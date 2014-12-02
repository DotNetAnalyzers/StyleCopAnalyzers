namespace StyleCop.Analyzers.OrderingRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// An instance readonly element is positioned beneath an instance non-readonly element of the same type.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when an instance readonly element is positioned beneath an instance
    /// non-readonly element of the same type.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1215InstanceReadonlyElementsMustAppearBeforeInstanceNonReadonlyElements : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "SA1215";
        internal const string Title = "An instance readonly element is positioned beneath an instance non-readonly element of the same type.";
        internal const string MessageFormat = "TODO: Message format";
        internal const string Category = "StyleCop.CSharp.OrderingRules";
        internal const string Description = "An instance readonly element is positioned beneath an instance non-readonly element of the same type.";
        internal const string HelpLink = "http://www.stylecop.com/docs/SA1215.html";

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
