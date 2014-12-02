namespace StyleCop.Analyzers.OrderingRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// The using directives within a C# code file are not sorted alphabetically by namespace.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when the using directives are not sorted alphabetically by namespace.
    /// Sorting the using directives alphabetically makes the code cleaner and easier to read, and can help make it
    /// easier to identify the namespaces that are being used by the code. The <see cref="System"/> namespaces are an
    /// exception to this rule and will always precede all other namespaces. See
    /// <see cref="SA1208SystemUsingDirectivesMustBePlacedBeforeOtherUsingDirectives"/> for more details.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1210UsingDirectivesMustBeOrderedAlphabeticallyByNamespace : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "SA1210";
        internal const string Title = "The using directives within a C# code file are not sorted alphabetically by namespace.";
        internal const string MessageFormat = "TODO: Message format";
        internal const string Category = "StyleCop.CSharp.OrderingRules";
        internal const string Description = "The using directives within a C# code file are not sorted alphabetically by namespace.";
        internal const string HelpLink = "http://www.stylecop.com/docs/SA1210.html";

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
