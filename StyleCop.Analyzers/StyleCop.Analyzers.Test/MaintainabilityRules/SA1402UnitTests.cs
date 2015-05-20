namespace StyleCop.Analyzers.Test.MaintainabilityRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.MaintainabilityRules;
    using TestHelper;
    using Xunit;

    public class SA1402UnitTests : FileMayOnlyContainTestBase
    {
        public override string Keyword
        {
            get
            {
                return "class";
            }
        }

        [Fact]
        public async Task TestPartialClassesAsync()
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
        public async Task TestDifferentPartialClassesAsync()
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
        public async Task TestNestedClassesAsync()
        {
            var testCode = @"public class Foo
{
    public class Bar
    {
    
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);

        }

        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1402FileMayOnlyContainASingleClass();
        }
    }
}