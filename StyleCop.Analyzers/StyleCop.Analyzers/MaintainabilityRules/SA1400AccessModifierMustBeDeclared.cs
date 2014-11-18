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
        internal const string Title = "Access Modifier Must Be Declared";
        internal const string MessageFormat = "TODO: Message format";
        internal const string Category = "Maintainability";
        internal const string Description = "The access modifier for a C# element has not been explicitly defined.";

        public static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, AnalyzerConstants.DisabledNoTests, Description);

        private static readonly ImmutableArray<DiagnosticDescriptor> _supportedDiagnostics =
            ImmutableArray.Create(Descriptor);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return _supportedDiagnostics;
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            // TODO: Implement analysis
        }
    }
}
