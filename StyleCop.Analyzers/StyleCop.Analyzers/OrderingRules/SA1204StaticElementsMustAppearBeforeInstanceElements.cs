namespace StyleCop.Analyzers.OrderingRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// A static element is positioned beneath an instance element of the same type.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when a static element is positioned beneath an instance element of the
    /// same type. All static elements must be placed above all instance elements of the same type to make it easier to
    /// see the interface exposed from the instance and static version of the class.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1204StaticElementsMustAppearBeforeInstanceElements : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1204StaticElementsMustAppearBeforeInstanceElements"/>
        /// analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1204";
        private const string Title = "Static elements must appear before instance elements";
        private const string MessageFormat = "TODO: Message format";
        private const string Category = "StyleCop.CSharp.OrderingRules";
        private const string Description = "A static element is positioned beneath an instance element of the same type.";
        private const string HelpLink = "http://www.stylecop.com/docs/SA1204.html";

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
