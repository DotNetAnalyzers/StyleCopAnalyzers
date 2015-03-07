namespace StyleCop.Analyzers.Test.SpacingRules
{
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Xunit;
    using StyleCop.Analyzers.SpacingRules;

    public class SA1021UnitTests : NumberSignSpacingTestBase
    {
        protected override string DiagnosticId
        {
            get
            {
                return SA1021NegativeSignsMustBeSpacedCorrectly.DiagnosticId;
            }
        }

        protected override string Sign
        {
            get
            {
                return "-";
            }
        }

        protected override string SignName
        {
            get
            {
                return "Negative";
            }
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new SA1021CodeFixProvider();
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new SA1021NegativeSignsMustBeSpacedCorrectly();
        }
    }
}
