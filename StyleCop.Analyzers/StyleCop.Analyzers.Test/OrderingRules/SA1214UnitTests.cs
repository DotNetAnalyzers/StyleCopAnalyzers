namespace StyleCop.Analyzers.Test.OrderingRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.OrderingRules;
    using TestHelper;
    using Xunit;

    public class SA1214UnitTests : CodeFixVerifier
    {
        [Fact]
        public async Task TestEmptySourceAsync()
        {
            var testCode = string.Empty;
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestTwoFieldsInClassStaticReadonlyFieldPlacedAfterStaticNonReadonlyAsync()
        {
            var testCode = @"
public class Foo
{
    private static int i = 0;
    private static readonly int j = 0;
}";

            var expected = new[]
            {
                this.CSharpDiagnostic().WithLocation(5, 5)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestTwoFieldsAllStaticReadonlyAsync()
        {
            var testCode = @"
public class Foo
{
    private static readonly int i = 0;
    private static readonly int j = 0;
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestTwoFieldsInClassStaticReadonlyFieldPlacedAfterStaticNonReadonlyOneLineAsync()
        {
            var testCode = @"
public class Foo
{
    private static int i = 0;private static readonly int j = 0;
}";

            var expected = new[]
            {
                this.CSharpDiagnostic().WithLocation(4, 30)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestTwoFieldsInClassStaticReadonlyFieldPlacedBeforeStaticNonReadonlyAsync()
        {
            var testCode = @"
public class Foo
{
    private static readonly int i = 0;
    private static int j = 0;
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestTwoFieldsInClassNonStaticReadonlyFieldPlacedAfterNonStaticNonReadonlyAsync()
        {
            var testCode = @"
public class Foo
{
    private int i = 0;
    private readonly int j = 0;
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestTwoFieldsInStructStaticReadonlyFieldPlacedAfterStaticNonReadonlyAsync()
        {
            var testCode = @"
public struct Foo
{
    private static int i = 0;
    private static readonly int j = 0;
}";

            var expected = new[]
            {
                this.CSharpDiagnostic().WithLocation(5, 5)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task ComplexExampleAsync()
        {
            var testCode = @"
public class Foo
{
    public string s = ""qwe"";
    private static readonly int i = 0;

    public void Ff() {}

    public static string s2 = ""qwe"";

    public static readonly int  u = 5;

    public class FooInner 
    {
        private int aa = 0;
        public static readonly int t = 2;
        private static int z = 999;
        private static readonly int e = 1;
    }

    public static readonly int j = 0;
}";

            var expected = new[]
            {
                this.CSharpDiagnostic().WithLocation(11, 5),
                this.CSharpDiagnostic().WithLocation(18, 9),
                this.CSharpDiagnostic().WithLocation(21, 5),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1214StaticReadonlyElementsMustAppearBeforeStaticNonReadonlyElements();
        }
    }
}
