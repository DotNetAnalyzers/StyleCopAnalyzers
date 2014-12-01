namespace StyleCop.Analyzers.MaintainabilityRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// A call to a C# anonymous method does not contain any method parameters, yet the statement still includes
    /// parenthesis.
    /// </summary>
    /// <remarks>
    /// <para>When an anonymous method does not contain any method parameters, the parenthesis around the parameters are
    /// optional.</para>
    ///
    /// <para>A violation of this rule occurs when the parenthesis are present on an anonymous method call which takes
    /// no method parameters. For example:</para>
    ///
    /// <code language="csharp">
    /// this.Method(delegate() { return 2; });
    /// </code>
    ///
    /// <para>The parenthesis are unnecessary and should be removed:</para>
    ///
    /// <code language="csharp">
    /// this.Method(delegate { return 2; });
    /// </code>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1410RemoveDelegateParenthesisWhenPossible : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "SA1410";
        internal const string Title = "A call to a C# anonymous method does not contain any method parameters, yet the statement still includes parenthesis.";
        internal const string MessageFormat = "TODO: Message format";
        internal const string Category = "StyleCop.CSharp.MaintainabilityRules";
        internal const string Description = "A call to a C# anonymous method does not contain any method parameters, yet the statement still includes parenthesis.";
        internal const string HelpLink = "http://www.stylecop.com/docs/SA1410.html";

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
