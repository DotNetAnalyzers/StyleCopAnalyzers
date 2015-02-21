namespace StyleCop.Analyzers.ReadabilityRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// The parameters to a C# method or indexer call or declaration span across multiple lines, but the first parameter
    /// does not start on the line after the opening bracket.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when the parameters to a method or indexer span across multiple lines, but
    /// the first parameter does not start on the line after the opening bracket. For example:</para>
    /// <code language="csharp">
    /// public string JoinName(string first, 
    ///     string last)
    /// {
    /// }
    /// </code>
    /// <para>The parameters must begin on the line after the declaration, whenever the parameter span across multiple
    /// lines:</para>
    /// <code language="csharp">
    /// public string JoinName(
    ///     string first,
    ///     string last)
    /// {
    /// }
    /// </code>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1116SplitParametersMustStartOnLineAfterDeclaration : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1116SplitParametersMustStartOnLineAfterDeclaration"/>
        /// analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1116";
        private const string Title = "Split parameters must start on line after declaration";
        private const string MessageFormat = "TODO: Message format";
        private const string Category = "StyleCop.CSharp.ReadabilityRules";
        private const string Description = "The parameters to a C# method or indexer call or declaration span across multiple lines, but the first parameter does not start on the line after the opening bracket.";
        private const string HelpLink = "http://www.stylecop.com/docs/SA1116.html";

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
