namespace StyleCop.Analyzers.OrderingRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// The keyword <c>protected</c> is positioned after the keyword <c>internal</c> within the declaration of a
    /// protected internal C# element.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when a protected internal element's access modifiers are written as
    /// <c>internal protected</c>. In reality, an element with the keywords <c>protected internal</c> will have the same
    /// access level as an element with the keywords <c>internal protected</c>. To make the code easier to read and more
    /// consistent, StyleCop standardizes the ordering of these keywords, so that a protected internal element will
    /// always be described as such, and never as internal protected. This can help to reduce confusion about whether
    /// these access levels are indeed the same.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1207ProtectedMustComeBeforeInternal : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "SA1207";
        internal const string Title = "Protected must come before internal";
        internal const string MessageFormat = "TODO: Message format";
        internal const string Category = "StyleCop.CSharp.OrderingRules";
        internal const string Description = "The keyword 'protected' is positioned after the keyword 'internal' within the declaration of a protected internal C# element.";
        internal const string HelpLink = "http://www.stylecop.com/docs/SA1207.html";

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
