namespace StyleCop.Analyzers.Test.MaintainabilityRules
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Xunit;
    using StyleCop.Analyzers.MaintainabilityRules;
    using TestHelper;

    public class SA1405UnitTests : DebugMessagesUnitTestsBase
    {
        protected override string DiagnosticId
        {
            get
            {
                return SA1405DebugAssertMustProvideMessageText.DiagnosticId;
            }
        }

        protected override string MethodName
        {
            get
            {
                return nameof(Debug.Assert);
            }
        }

        protected override IEnumerable<string> InitialArguments
        {
            get
            {
                yield return "true";
            }
        }

        [Fact]
        public async Task TestWrongOverload()
        {
            var testCode = @"using System.Diagnostics;
public class Foo
{
    public void Bar()
    {
        Debug.Assert(true);
    }
}";

            var expected = new[]
            {
                new DiagnosticResult
                {
                    Id = this.DiagnosticId,
                    Message = string.Format("Debug.Assert must provide message text", this.MethodName),
                    Severity = DiagnosticSeverity.Warning,
                    Locations =
                        new[]
                        {
                            new DiagnosticResultLocation("Test0.cs", 6, 9)
                        }
                }
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new SA1405DebugAssertMustProvideMessageText();
        }
    }
}