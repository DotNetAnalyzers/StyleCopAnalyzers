namespace StyleCop.Analyzers.OrderingRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// An add accessor appears after a remove accessor within an event.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when an add accessor is placed after a remove accessor within an event. To
    /// comply with this rule, the add accessor should appear before the remove accessor.</para>
    ///
    /// <para>For example, the following code would raise an instance of this violation:</para>
    ///
    /// <code language="csharp">
    /// public event EventHandler NameChanged
    /// {
    ///     remove { this.nameChanged -= value; }
    ///     add { this.nameChanged += value; }
    /// }
    /// </code>
    ///
    /// <para>The code below would not raise this violation:</para>
    ///
    /// <code language="csharp">
    /// public event EventHandler NameChanged
    /// {
    ///     add { this.nameChanged += value; }
    ///     remove { this.nameChanged -= value; }
    /// }
    /// </code>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1213EventAccessorsMustFollowOrder : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "SA1213";
        internal const string Title = "Event accessors must follow order";
        internal const string MessageFormat = "TODO: Message format";
        internal const string Category = "StyleCop.CSharp.OrderingRules";
        internal const string Description = "An add accessor appears after a remove accessor within an event.";
        internal const string HelpLink = "http://www.stylecop.com/docs/SA1213.html";

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
