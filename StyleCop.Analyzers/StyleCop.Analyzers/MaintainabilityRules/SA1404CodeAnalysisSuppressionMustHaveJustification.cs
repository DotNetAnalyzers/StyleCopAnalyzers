namespace StyleCop.Analyzers.MaintainabilityRules
{
    using System.Collections.Immutable;
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// A Code Analysis SuppressMessage attribute does not include a justification.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when the code contains a Code Analysis
    /// <see cref="SuppressMessageAttribute"/> attribute, but a justification for the suppression has not been provided
    /// within the attribute. Whenever a Code Analysis rule is suppressed, a justification should be provided. This can
    /// increase the long-term maintainability of the code.</para>
    ///
    /// <code language="csharp">
    /// [SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", Justification = "Used during unit testing")]
    /// public bool Enable()
    /// {
    /// }
    /// </code>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1404CodeAnalysisSuppressionMustHaveJustification : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "SA1404";
        internal const string Title = "Code analysis suppression must have justification";
        internal const string MessageFormat = "TODO: Message format";
        internal const string Category = "StyleCop.CSharp.MaintainabilityRules";
        internal const string Description = "A Code Analysis SuppressMessage attribute does not include a justification.";
        internal const string HelpLink = "http://www.stylecop.com/docs/SA1404.html";

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
