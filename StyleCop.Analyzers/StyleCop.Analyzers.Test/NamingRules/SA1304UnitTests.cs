using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using StyleCop.Analyzers.NamingRules;
using TestHelper;
using Xunit;

namespace StyleCop.Analyzers.Test.NamingRules
{
    public class SA1304UnitTests : CodeFixVerifier
    {
        [Fact]
        public async Task TestEmptySourceAsync()
        {
            var testCode = string.Empty;
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestPublicReadonlyFieldStartingWithLowerCaseAsync()
        {
            var testCode = @"public class Foo
{
    public readonly string bar = ""baz"";
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(3, 28);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = @"public class Foo
{
    public readonly string Bar = ""baz"";
}";

            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestPublicReadonlyFieldStartingWithUpperCaseAsync()
        {
            var testCode = @"public class Foo
{
    public readonly string Bar = ""baz"";
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestProtectedReadonlyFieldStartingWithLowerCaseAsync()
        {
            var testCode = @"public class Foo
{
    protected readonly string bar = ""baz"";
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(3, 31);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = @"public class Foo
{
    protected readonly string Bar = ""baz"";
}";
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestProtectedReadonlyFieldStartingWithUpperCaseAsync()
        {
            var testCode = @"public class Foo
{
    protected readonly string Bar = ""baz"";
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestInternalReadonlyFieldStartingWithLowerCaseAsync()
        {
            var testCode = @"public class Foo
{
    internal readonly string bar = ""baz"";
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(3, 30);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = @"public class Foo
{
    internal readonly string Bar = ""baz"";
}";
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestInternalReadonlyFieldStartingWithUpperCaseAsync()
        {
            var testCode = @"public class Foo
{
    internal readonly string Bar = ""baz"";
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestlWithNoAccessibilityKeywordReadonlyFieldStartingWithLowerCaseAsync()
        {
            var testCode = @"public class Foo
{
    readonly string bar = ""baz"";
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestlPublicFieldStartingWithLowerCaseAsync()
        {
            var testCode = @"public class Foo
{
    public string bar = ""baz"";
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestPrivateReadonlyFieldStartingWithLowerCaseAsync()
        {
            var testCode = @"public class Foo
{
    private readonly string bar = ""baz"";
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new SA1304NonPrivateReadonlyFieldsMustBeginWithUpperCaseLetter();
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new SA1304SA1311CodeFixProvider();
        }
    }
}