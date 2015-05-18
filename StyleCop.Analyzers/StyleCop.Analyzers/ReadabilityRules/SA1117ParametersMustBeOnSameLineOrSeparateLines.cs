namespace StyleCop.Analyzers.ReadabilityRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// The parameters to a C# method or indexer call or declaration are not all on the same line or each on a separate
    /// line.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when the parameters to a method or indexer are not all on the same line or
    /// each on its own line. For example:</para>
    /// <code language="csharp">
    /// public string JoinName(string first, string middle,
    ///     string last)
    /// {
    /// }
    /// </code>
    /// <para>The parameters can all be placed on the same line:</para>
    /// <code language="csharp">
    /// public string JoinName(string first, string middle, string last)
    /// {
    /// }
    ///
    /// public string JoinName(
    ///     string first, string middle, string last)
    /// {
    /// }
    /// </code>
    /// <para>Alternatively, each parameter can be placed on its own line:</para>
    /// <code language="csharp">
    /// public string JoinName(
    ///     string first,
    ///     string middle,
    ///     string last)
    /// {
    /// }
    /// </code>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1117ParametersMustBeOnSameLineOrSeparateLines : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1117ParametersMustBeOnSameLineOrSeparateLines"/>
        /// analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1117";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(ReadabilityResources.SA1117Title), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(ReadabilityResources.SA1117MessageFormat), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly string Category = "StyleCop.CSharp.ReadabilityRules";
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(ReadabilityResources.SA1117Description), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly string HelpLink = "http://www.stylecop.com/docs/SA1117.html";

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
