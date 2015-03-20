using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Diagnostics;
using StyleCop.Analyzers.MaintainabilityRules;
using TestHelper;
using Xunit;

namespace StyleCop.Analyzers.Test.MaintainabilityRules
{
    public class SA1402UnitTests : FileMayOnlyContainTestBase
    {

        public override string Keyword
        {
            get
            {
                return "class";
            }
        }

        public override string DiagnosticId
        {
            get
            {
                return SA1402FileMayOnlyContainASingleClass.DiagnosticId;
            }
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new SA1402FileMayOnlyContainASingleClass();
        }

        [Fact]
        public async Task TestPartialClasses()
        {
            var testCode = @"public partial class Foo
{
}
public partial class Foo
{

}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);

        }

        [Fact]
        public async Task TestDifferentPartialClasses()
        {
            var testCode = @"public partial class Foo
{
}
public partial class Bar
{

}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(4, 22);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

        }

        [Fact]
        public async Task TestNestedClasses()
        {
            var testCode = @"public class Foo
{
    public class Bar
    {
    
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);

        }
    }
}