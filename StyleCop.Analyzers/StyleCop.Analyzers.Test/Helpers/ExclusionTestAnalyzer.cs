namespace TestHelper
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.Text;
    using StyleCop.Analyzers;

    /// <summary>
    /// A analyzer that will report a diagnostic at the start of the code file if the
    /// file is not excluded from code analysis.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class ExclusionTestAnalyzer : DiagnosticAnalyzer
    {
        internal const string DiagnosticId = "SA9999";
        private const string Title = "Exclusion test";
        private const string MessageFormat = "Exclusion test";
        private const string Description = "Exclusion test";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, "TestRules", DiagnosticSeverity.Warning, true, Description);

        private static readonly ImmutableArray<DiagnosticDescriptor> SupportedDiagnosticsValue =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return SupportedDiagnosticsValue;
            }
        }

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxTreeActionHonorExclusions(this.AnalyzeTree);
        }

        private void AnalyzeTree(SyntaxTreeAnalysisContext context)
        {
            // Report a diagnostic if we got called
            context.ReportDiagnostic(Diagnostic.Create(Descriptor, context.Tree.GetLocation(TextSpan.FromBounds(0, 0))));
        }
    }
}
