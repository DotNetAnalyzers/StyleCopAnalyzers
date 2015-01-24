namespace StyleCop.Analyzers.OrderingRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// The keywords within the declaration of an element do not follow a standard ordering scheme.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when the keywords within an element’s declaration do not follow a standard
    /// ordering scheme.</para>
    ///
    /// <para>Within an element declaration, keywords must appear in the following order:</para>
    ///
    /// <list type="bullet">
    /// <item>Access modifiers</item>
    /// <item><see langword="static"/></item>
    /// <item>All other keywords</item>
    /// </list>
    ///
    /// <para>Using a standard ordering scheme for element declaration keywords can make the code more readable by
    /// highlighting the access level of each element. This can help prevent elements from being given a higher access
    /// level than needed.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1206DeclarationKeywordsMustFollowOrder : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "SA1206";
        private const string Title = "Declaration keywords must follow order";
        private const string MessageFormat = "TODO: Message format";
        private const string Category = "StyleCop.CSharp.OrderingRules";
        private const string Description = "The keywords within the declaration of an element do not follow a standard ordering scheme.";
        private const string HelpLink = "http://www.stylecop.com/docs/SA1206.html";

        private static readonly DiagnosticDescriptor Descriptor =
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
