namespace StyleCop.Analyzers.Test.SpacingRules
{
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.SpacingRules;

    public class SA1022UnitTests : NumberSignSpacingTestBase
    {
        protected override string Sign
        {
            get
            {
                return "+";
            }
        }

        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1022PositiveSignsMustBeSpacedCorrectly();
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new SA1022CodeFixProvider();
        }
    }
}
