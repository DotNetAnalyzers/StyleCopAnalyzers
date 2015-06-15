namespace StyleCop.Analyzers.Helpers
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// Provides helper methods to work with diagnostics options.
    /// </summary>
    internal static class DiagnosticOptionsHelper
    {
        internal static bool IsAnalyzerEnabled(this SyntaxNodeAnalysisContext context, string analyzerId)
        {
            return context.SemanticModel.Compilation.Options.SpecificDiagnosticOptions.GetValueOrDefault(analyzerId, ReportDiagnostic.Default) != ReportDiagnostic.Suppress;
        }
    }
}
