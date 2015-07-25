namespace StyleCop.Analyzers.ReadabilityRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// A C# statement contains a region tag between the declaration of the statement and the opening curly brace of
    /// the statement.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when the code contains a region tag in between the declaration and the
    /// opening curly brace. For example:</para>
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
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1109BlockStatementsMustNotContainEmbeddedRegions"/>
        /// analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1109";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(ReadabilityResources.SA1109Title), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(ReadabilityResources.SA1109MessageFormat), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(ReadabilityResources.SA1109Description), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly string HelpLink = "http://www.stylecop.com/docs/SA1109.html";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.ReadabilityRules, DiagnosticSeverity.Warning, AnalyzerConstants.DisabledNoTests, Description, HelpLink);

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
