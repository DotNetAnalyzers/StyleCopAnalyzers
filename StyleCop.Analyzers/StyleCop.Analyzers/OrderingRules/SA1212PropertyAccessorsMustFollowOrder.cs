namespace StyleCop.Analyzers.OrderingRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// A get accessor appears after a set accessor within a property or indexer.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when a get accessor is placed after a set accessor within a property or
    /// indexer. To comply with this rule, the get accessor should appear before the set accessor.</para>
    ///
    /// <para>For example, the following code would raise an instance of this violation:</para>
    ///
    /// <code language="csharp">
    /// public string Name
    /// {
    ///     set { this.name = value; }
    ///     get { return this.name; }
    /// }
    /// </code>
    ///
    /// <para>The code below would not raise this violation:</para>
    ///
    /// <code language="csharp">
    /// public string Name
    /// {
    ///     get { return this.name; }
    ///     set { this.name = value; }
    /// }
    /// </code>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1212PropertyAccessorsMustFollowOrder : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1212PropertyAccessorsMustFollowOrder"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1212";
        private const string Title = "Property accessors must follow order";
        private const string MessageFormat = "TODO: Message format";
        private const string Category = "StyleCop.CSharp.OrderingRules";
        private const string Description = "A get accessor appears after a set accessor within a property or indexer.";
        private const string HelpLink = "http://www.stylecop.com/docs/SA1212.html";

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
