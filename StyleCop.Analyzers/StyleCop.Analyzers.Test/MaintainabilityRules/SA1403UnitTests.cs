using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Diagnostics;
using StyleCop.Analyzers.MaintainabilityRules;
using TestHelper;
using Xunit;

namespace StyleCop.Analyzers.Test.MaintainabilityRules
{
    public class SA1403UnitTests : FileMayOnlyContainTestBase
    {
        public override string Keyword
        {
            get
            {
                return "namespace";
            }
        }

        public override string DiagnosticId
        {
            get
            {
                return SA1403FileMayOnlyContainASingleNamespace.DiagnosticId;
            }
        }

        [Fact]
        public async Task TestNestedNamespaces()
        {
            var testCode = @"namespace Foo
{
    namespace Bar
    {

    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(3, 15);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new SA1403FileMayOnlyContainASingleNamespace();
        }
    }
}