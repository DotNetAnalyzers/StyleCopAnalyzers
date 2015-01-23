namespace StyleCop.Analyzers.MaintainabilityRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// The access modifier for a C# element has not been explicitly defined.
    /// </summary>
    /// <remarks>
    /// <para>C# allows elements to be defined without an access modifier. Depending upon the type of element, C# will
    /// automatically assign an access level to the element in this case.</para>
    ///
    /// <para>This rule requires an access modifier to be explicitly defined for every element. This removes the need
    /// for the reader to make assumptions about the code, improving the readability of the code.</para>
    /// </remarks>
    public class SA1400AccessModifierMustBeDeclared : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "SA1400";
        internal const string Title = "Access modifier must be declared";
        internal const string MessageFormat = "TODO: Message format";
        internal const string Category = "StyleCop.CSharp.Maintainability";
        internal const string Description = "The access modifier for a C# element has not been explicitly defined.";
        internal const string HelpLink = "http://www.stylecop.com/docs/SA1400.html";

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
