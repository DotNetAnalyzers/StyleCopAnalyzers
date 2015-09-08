namespace StyleCop.Analyzers.Test.SpacingRules
{
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.SpacingRules;

    public class SA1021UnitTests : NumberSignSpacingTestBase
    {
        protected override string Sign
        {
            get
            {
                return "-";
            }
        }

        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1021NegativeSignsMustBeSpacedCorrectly();
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new OpenCloseSpacingCodeFixProvider();
        }
    }
}
