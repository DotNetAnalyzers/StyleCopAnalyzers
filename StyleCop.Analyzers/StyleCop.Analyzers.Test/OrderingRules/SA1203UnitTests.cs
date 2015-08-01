namespace StyleCop.Analyzers.Test.OrderingRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.OrderingRules;
    using TestHelper;
    using Xunit;

    public class SA1203UnitTests : DiagnosticVerifier
    {
        [Fact]
        public async Task TestNoDiagnosticAsync()
        {
            var testCode = @"
public class Foo
{
    private const int Bar = 2;
    private int Baz = 1;
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestClassViolationAsync()
        {
            var testCode = @"
public class Foo
{
    private int Baz = 1;
    private const int Bar = 2;
}";
            var firstDiagnostic = this.CSharpDiagnostic().WithLocation(5, 23);
            await this.VerifyCSharpDiagnosticAsync(testCode, firstDiagnostic, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestStructViolationAsync()
        {
            var testCode = @"
public struct Foo
{
    private int baz;
    private const int Bar = 2;
}";
            var firstDiagnostic = this.CSharpDiagnostic().WithLocation(5, 23);
            await this.VerifyCSharpDiagnosticAsync(testCode, firstDiagnostic, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestSecondConstAfterNonConstAsync()
        {
            var testCode = @"
public class Foo
{
    private const int Bar = 2;
    private int Baz = 1;
    private const int FooBar = 2;
}";
            var firstDiagnostic = this.CSharpDiagnostic().WithLocation(6, 23);
            await this.VerifyCSharpDiagnosticAsync(testCode, firstDiagnostic, CancellationToken.None).ConfigureAwait(false);
        }

        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1203ConstantsMustAppearBeforeFields();
        }
    }
}
