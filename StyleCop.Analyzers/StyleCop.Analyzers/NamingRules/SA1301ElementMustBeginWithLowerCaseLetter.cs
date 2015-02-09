namespace StyleCop.Analyzers.NamingRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// There are currently no situations in which this rule will fire.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1301ElementMustBeginWithLowerCaseLetter : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "SA1301";
        private const string Title = "Element must begin with lower-case letter";
        private const string MessageFormat = "TODO: Message format";
        private const string Category = "StyleCop.CSharp.NamingRules";
        private const string Description = "There are currently no situations in which this rule will fire.";
        private const string HelpLink = "http://www.stylecop.com/docs/SA1301.html";

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
