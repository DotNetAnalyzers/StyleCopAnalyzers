namespace StyleCop.Analyzers.MaintainabilityRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// A call to <see cref="O:System.Diagnostics.Debug.Assert"/> in C# code does not include a descriptive message.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when the code contains a call to
    /// <see cref="O:System.Diagnostics.Debug.Assert"/> which does not provide a description for the end-user. For
    /// example, the following assert includes a description message:</para>
    ///
    /// <code language="csharp">
    /// Debug.Assert(value != true, "The value must always be true.");
    /// </code>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1405DebugAssertMustProvideMessageText : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "SA1405";
        internal const string Title = "Debug.Assert must provide message text";
        internal const string MessageFormat = "TODO: Message format";
        internal const string Category = "StyleCop.CSharp.MaintainabilityRules";
        internal const string Description = "A call to Debug.Assert in C# code does not include a descriptive message.";
        internal const string HelpLink = "http://www.stylecop.com/docs/SA1405.html";

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
