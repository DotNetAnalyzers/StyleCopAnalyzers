namespace StyleCop.Analyzers.Test.SpacingRules
{
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

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new SA1022CodeFixProvider();
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new SA1022PositiveSignsMustBeSpacedCorrectly();
        }
    }
}
