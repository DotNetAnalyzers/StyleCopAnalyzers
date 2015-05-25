using System.Threading;
using System.Threading.Tasks;
using TestHelper;
using Xunit;

namespace StyleCop.Analyzers.Test.MaintainabilityRules
{
    public abstract class FileMayOnlyContainTestBase : CodeFixVerifier
    {
        public abstract string Keyword { get; }

        [Fact]
        public async Task TestEmptySourceAsync()
        {
            var testCode = string.Empty;
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestOneElementAsync()
        {
            var testCode = @"%1 Foo
{
}";
            await this.VerifyCSharpDiagnosticAsync(testCode.Replace("%1", this.Keyword), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestTwoElementsAsync()
        {
            var testCode = @"%1 Foo
{
}
%1 Bar
{
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(4, this.Keyword.Length + 2);

            await this.VerifyCSharpDiagnosticAsync(testCode.Replace("%1", this.Keyword), expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestThreeElementsAsync()
        {
            var testCode = @"%1 Foo
{
}
%1 Bar
{
}
%1 FooBar
{
}";

            DiagnosticResult[] expected =
                {
                    this.CSharpDiagnostic().WithLocation(4, this.Keyword.Length + 2),
                    this.CSharpDiagnostic().WithLocation(7, this.Keyword.Length + 2)
                };

            await this.VerifyCSharpDiagnosticAsync(testCode.Replace("%1", this.Keyword), expected, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
