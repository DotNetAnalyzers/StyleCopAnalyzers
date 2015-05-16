using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Diagnostics;
using StyleCop.Analyzers.MaintainabilityRules;
using TestHelper;
using Xunit;

namespace StyleCop.Analyzers.Test.MaintainabilityRules
{
    public class SA1401UnitTests : CodeFixVerifier
    {
        [Fact]
        public async Task TestEmptySourceAsync()
        {
            var testCode = string.Empty;
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestClassWithPublicFieldAsync()
        {
            var testCode = @"public class Foo
{
    public string bar;
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(3, 19);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestClassWithInternalFieldAsync()
        {
            var testCode = @"public class Foo
{
    internal string bar;
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(3, 21);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestClassWithFieldNoAccessModifierAsync()
        {
            var testCode = @"public class Foo
{
    string bar;
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestStructWithPublicFieldAsync()
        {
            var testCode = @"public struct Foo
{
    public string bar;
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestClassWithConstFieldAsync()
        {
            var testCode = @"public class Foo
{
    public const string bar = ""qwe"";
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new SA1401FieldsMustBePrivate();
        }
    }
}