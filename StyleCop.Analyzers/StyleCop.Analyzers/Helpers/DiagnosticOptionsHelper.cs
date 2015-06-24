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
        /// <summary>
        /// Determines if the diagnostic identified by the given identifier is currently suppressed.
        /// </summary>
        /// <param name="context">The context that will be used to determine if the diagnostic is currenlty suppressed.</param>
        /// <param name="diagnosticId">The diagnostic identifier to check.</param>
        /// <returns>True if the diagnostic is currently suppressed.</returns>
        internal static bool IsAnalyzerSuppressed(this SyntaxNodeAnalysisContext context, string diagnosticId)
        {
            return context.SemanticModel.Compilation.Options.SpecificDiagnosticOptions.GetValueOrDefault(diagnosticId, ReportDiagnostic.Default) == ReportDiagnostic.Suppress;
        }
    }
}
