namespace StyleCop.Analyzers.OrderingRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// A using directive which declares a member of the <see cref="System"/> namespace appears after a using directive
    /// which declares a member of a different namespace, within a C# code file.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when a using directive for the <see cref="System"/> namespace is placed
    /// after a non-<see cref="System"/> using directive. Placing all <see cref="System"/> using directives at the top
    /// of the using directives can make the code cleaner and easier to read, and can help make it easier to identify
    /// the namespaces that are being used by the code.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1208SystemUsingDirectivesMustBePlacedBeforeOtherUsingDirectives : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "SA1208";
        internal const string Title = "A using directive which declares a member of the 'System' namespace appears after a using directive which declares a member of a different namespace, within a C# code file.";
        internal const string MessageFormat = "TODO: Message format";
        internal const string Category = "StyleCop.CSharp.OrderingRules";
        internal const string Description = "A using directive which declares a member of the 'System' namespace appears after a using directive which declares a member of a different namespace, within a C# code file.";
        internal const string HelpLink = "http://www.stylecop.com/docs/SA1208.html";

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
