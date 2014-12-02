namespace StyleCop.Analyzers.OrderingRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// A using-alias directive is positioned before a regular using directive.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when a using-alias directive is placed before a normal using directive.
    /// Using-alias directives have special behavior which can alter the meaning of the rest of the code within the file
    /// or namespace. Placing the using-alias directives together below all other using-directives can make the code
    /// cleaner and easier to read, and can help make it easier to identify the types used throughout the code.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1209UsingAliasDirectivesMustBePlacedAfterOtherUsingDirectives : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "SA1209";
        internal const string Title = "A using-alias directive is positioned before a regular using directive.";
        internal const string MessageFormat = "TODO: Message format";
        internal const string Category = "StyleCop.CSharp.OrderingRules";
        internal const string Description = "A using-alias directive is positioned before a regular using directive.";
        internal const string HelpLink = "http://www.stylecop.com/docs/SA1209.html";

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
