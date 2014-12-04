namespace StyleCop.Analyzers.LayoutRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// The code file has blank lines at the start.
    /// </summary>
    /// <remarks>
    /// <para>To improve the layout of the code, StyleCop requires no blank lines at the start of files.</para>
    ///
    /// <para>A violation of this rule occurs when one or more blank lines are at the start of the file.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1517CodeMustNotContainBlankLinesAtStartOfFile : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "SA1517";
        internal const string Title = "Code must not contain blank lines at start of file";
        internal const string MessageFormat = "TODO: Message format";
        internal const string Category = "StyleCop.CSharp.LayoutRules";
        internal const string Description = "The code file has blank lines at the start.";
        internal const string HelpLink = "http://www.stylecop.com/docs/SA1517.html";

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
