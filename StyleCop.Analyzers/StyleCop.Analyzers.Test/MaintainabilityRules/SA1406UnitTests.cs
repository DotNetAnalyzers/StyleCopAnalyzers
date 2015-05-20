namespace StyleCop.Analyzers.Test.MaintainabilityRules
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.MaintainabilityRules;

    public class SA1406UnitTests : DebugMessagesUnitTestsBase
    {
        protected override string MethodName
        {
            get
            {
                return nameof(Debug.Fail);
            }
        }

        protected override IEnumerable<string> InitialArguments
        {
            get
            {
                return Enumerable.Empty<string>();
            }
        }

        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1406DebugFailMustProvideMessageText();
        }
    }
}