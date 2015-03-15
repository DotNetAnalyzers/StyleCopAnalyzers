namespace StyleCop.Analyzers.Test.MaintainabilityRules
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Xunit;
    using StyleCop.Analyzers.MaintainabilityRules;

    public class SA1406UnitTests : DebugMessagesUnitTestsBase
    {
        protected override string DiagnosticId
        {
            get
            {
                return SA1406DebugFailMustProvideMessageText.DiagnosticId;
            }
        }

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

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new SA1406DebugFailMustProvideMessageText();
        }
    }
}