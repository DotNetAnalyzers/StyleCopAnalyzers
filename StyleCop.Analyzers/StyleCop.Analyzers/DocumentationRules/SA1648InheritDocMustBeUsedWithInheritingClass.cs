namespace StyleCop.Analyzers.DocumentationRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// <c>&lt;inheritdoc&gt;</c> has been used on an element that doesn't inherit from a base class or implement an
    /// interface.
    /// </summary>
    /// <remarks>
    /// <para>Verifies that an <c>inheritdoc</c> tag is not used when the class or interface does not inherit from a
    /// base class or interface.</para>
    ///
    /// <para>A violation of this rule occurs when the element having the <c>inheritdoc</c> tag doesn't inherit from a
    /// base case or implement an interface.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1648InheritDocMustBeUsedWithInheritingClass : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1648InheritDocMustBeUsedWithInheritingClass"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1648";
        private const string Title = "inheritdoc must be used with inheriting class";
        private const string MessageFormat = "TODO: Message format";
        private const string Category = "StyleCop.CSharp.DocumentationRules";
        private const string Description = "<inheritdoc> has been used on an element that doesn't inherit from a base class or implement an interface.";
        private const string HelpLink = "http://www.stylecop.com/docs/SA1648.html";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, AnalyzerConstants.DisabledNoTests, Description, HelpLink);

        private static readonly ImmutableArray<DiagnosticDescriptor> supportedDiagnostics =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return supportedDiagnostics;
            }
        }

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            // TODO: Implement analysis
        }
    }
}
