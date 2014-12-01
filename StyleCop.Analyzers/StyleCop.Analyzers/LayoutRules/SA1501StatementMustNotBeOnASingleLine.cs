namespace StyleCop.Analyzers.LayoutRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// A C# statement containing opening and closing curly brackets is written completely on a single line.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when a statement that is wrapped in opening and closing curly brackets is
    /// written on a single line. For example:</para>
    ///
    /// <code language="csharp">
    /// public object Method()
    /// {
    ///     lock (this) { return this.value; }
    /// }
    /// </code>
    ///
    /// <para>When StyleCop checks this code, a violation of this rule will occur because the entire lock statement is
    /// written on one line. The statement should be written across multiple lines, with the opening and closing curly
    /// brackets each on their own line, as follows:</para>
    ///
    /// <code language="csharp">
    /// public object Method()
    /// {
    ///     lock (this) 
    ///     {
    ///         return this.value; 
    ///     }
    /// }
    /// </code>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1501StatementMustNotBeOnASingleLine : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "SA1501";
        internal const string Title = "A C# statement containing opening and closing curly brackets is written completely on a single line.";
        internal const string MessageFormat = "TODO: Message format";
        internal const string Category = "StyleCop.CSharp.LayoutRules";
        internal const string Description = "A C# statement containing opening and closing curly brackets is written completely on a single line.";
        internal const string HelpLink = "http://www.stylecop.com/docs/SA1501.html";

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
