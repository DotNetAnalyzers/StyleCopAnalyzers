namespace StyleCop.Analyzers
{
    using Microsoft.CodeAnalysis;

    internal static class AnalyzerConstants
    {
        static AnalyzerConstants()
        {
#if DEBUG
            // In DEBUG builds, the tests are enabled to simplify development and testing.
            DisabledNoTests = true;
#else
            DisabledNoTests = false;
#endif
        }

        /// <summary>
        /// Provides a reference value which can be passed to
        /// <see cref="DiagnosticDescriptor(string, string, string, string, DiagnosticSeverity, bool, string, string, string[])"/>
        /// to disable a diagnostic which is currently untested.
        /// </summary>
        internal static bool DisabledNoTests { get; }
    }
}
