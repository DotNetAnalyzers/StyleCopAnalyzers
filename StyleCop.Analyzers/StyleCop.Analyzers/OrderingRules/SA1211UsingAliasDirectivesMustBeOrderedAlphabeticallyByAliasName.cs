namespace StyleCop.Analyzers.OrderingRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// The using-alias directives within a C# code file are not sorted alphabetically by alias name.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when the using-alias directives are not sorted alphabetically by alias
    /// name. Sorting the using-alias directives alphabetically can make the code cleaner and easier to read, and can
    /// help make it easier to identify the namespaces that are being used by the code.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1211UsingAliasDirectivesMustBeOrderedAlphabeticallyByAliasName : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "SA1211";
        internal const string Title = "The using-alias directives within a C# code file are not sorted alphabetically by alias name.";
        internal const string MessageFormat = "TODO: Message format";
        internal const string Category = "StyleCop.CSharp.OrderingRules";
        internal const string Description = "The using-alias directives within a C# code file are not sorted alphabetically by alias name.";
        internal const string HelpLink = "http://www.stylecop.com/docs/SA1211.html";

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
