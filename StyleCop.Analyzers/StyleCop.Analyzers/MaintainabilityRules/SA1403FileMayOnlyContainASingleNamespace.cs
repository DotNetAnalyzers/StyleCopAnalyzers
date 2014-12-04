namespace StyleCop.Analyzers.MaintainabilityRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// A C# code file contains more than one namespace.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when a C# file contains more than one namespace. To increase long-term
    /// maintainability of the code-base, each file should contain at most one namespace.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1403FileMayOnlyContainASingleNamespace : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "SA1403";
        internal const string Title = "File may only contain a single namespace";
        internal const string MessageFormat = "TODO: Message format";
        internal const string Category = "StyleCop.CSharp.MaintainabilityRules";
        internal const string Description = "A C# code file contains more than one namespace.";
        internal const string HelpLink = "http://www.stylecop.com/docs/SA1403.html";

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
