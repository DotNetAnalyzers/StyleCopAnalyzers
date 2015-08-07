namespace StyleCop.Analyzers.Test.OrderingRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.OrderingRules;
    using TestHelper;
    using Xunit;

    public class SA1203UnitTests : CodeFixVerifier
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

            var fixedTestCode = @"
public class Foo
{
    private const int Bar = 2;
    private int Baz = 1;
}";
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
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

            var fixedTestCode = @"
public struct Foo
{
    private const int Bar = 2;
    private int baz;
}";
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
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

            var fixedTestCode = @"
public class Foo
{
    private const int Bar = 2;
    private const int FooBar = 2;
    private int Baz = 1;
}";
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the code fix will move the non constant fields before the constant ones.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestCodeFixAsync()
        {
            var testCode = @"public class Foo
{
    private const string Before1 = ""test"";

    public const string Before2 = ""test"";

    private int field1;

    private const string After1 = ""test"";

    private int between;

    public const string After2 = ""test"";
}
";

            var fixedTestCode = @"public class Foo
{
    private const string Before1 = ""test"";

    public const string Before2 = ""test"";

    private const string After1 = ""test"";

    public const string After2 = ""test"";

    private int field1;

    private int between;
}
";

            var diagnosticResults = new[]
            {
                this.CSharpDiagnostic().WithLocation(9, 26),
                this.CSharpDiagnostic().WithLocation(13, 25)
            };
            await this.VerifyCSharpDiagnosticAsync(testCode, diagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new SA1203CodeFixProvider();
        }

        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1203ConstantsMustAppearBeforeFields();
        }
    }
}
