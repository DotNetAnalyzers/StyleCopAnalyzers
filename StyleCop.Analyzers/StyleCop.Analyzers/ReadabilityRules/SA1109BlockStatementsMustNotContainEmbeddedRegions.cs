namespace StyleCop.Analyzers.ReadabilityRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// A C# statement contains a region tag between the declaration of the statement and the opening curly bracket of
    /// the statement.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when the code contains a region tag in between the declaration and the
    /// opening curly bracket. For example:</para>
    /// <code language="csharp">
    /// if (x != y)
    /// #region
    /// {
    /// }
    /// #endregion
    /// </code>
    /// <para>This will result in the body of the statement being hidden when the region is collapsed.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1109BlockStatementsMustNotContainEmbeddedRegions : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "SA1109";
        private const string Title = "Block statements must not contain embedded regions";
        private const string MessageFormat = "TODO: Message format";
        private const string Category = "StyleCop.CSharp.ReadabilityRules";
        private const string Description = "A C# statement contains a region tag between the declaration of the statement and the opening curly bracket of the statement.";
        private const string HelpLink = "http://www.stylecop.com/docs/SA1109.html";

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
