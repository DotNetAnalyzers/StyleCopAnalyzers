namespace StyleCop.Analyzers.LayoutRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// Adjacent C# elements are not separated by a blank line.
    /// </summary>
    /// <remarks>
    /// <para>To improve the readability of the code, StyleCop requires blank lines in certain situations, and prohibits
    /// blank lines in other situations. This results in a consistent visual pattern across the code, which can improve
    /// recognition and readability of unfamiliar code.</para>
    ///
    /// <para>A violation of this rule occurs when two adjacent element are not separated by a blank line. For
    /// example:</para>
    ///
    /// <code language="csharp">
    /// public void Method1()
    /// {
    /// }
    /// public bool Property
    /// {
    ///     get { return true; }
    /// }
    /// </code>
    ///
    /// <para>In the example above, the method and property are not separated by a blank line, so a violation of this
    /// rule would occur.</para>
    ///
    /// <code language="csharp">
    /// public event EventHandler SomeEvent
    /// {
    ///     add
    ///     {
    ///         // add event subscriber here
    ///     }
    ///     remove
    ///     {
    ///         // remove event subscriber here
    ///     }
    /// }
    /// </code>
    ///
    /// <para>In the example above, the add and remove of the event need to be separated by a blank line because the add
    /// is multi-line.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1516ElementsMustBeSeparatedByBlankLine : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "SA1516";
        internal const string Title = "Elements must be separated by blank line";
        internal const string MessageFormat = "TODO: Message format";
        internal const string Category = "StyleCop.CSharp.LayoutRules";
        internal const string Description = "Adjacent C# elements are not separated by a blank line.";
        internal const string HelpLink = "http://www.stylecop.com/docs/SA1516.html";

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
