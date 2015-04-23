namespace StyleCop.Analyzers.Test.NamingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.NamingRules;
    using TestHelper;
    using Xunit;

    public class SA1311UnitTests : CodeFixVerifier
    {
        [Fact]
        public async Task TestEmptySource()
        {
            var testCode = string.Empty;
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestStaticReadonlyFieldStartingWithLoweCase()
        {
            var testCode = @"public class Foo
{
    public static readonly string bar = ""baz"";
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(3, 35);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = @"public class Foo
{
    public static readonly string Bar = ""baz"";
}";

            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestStaticReadonlyFieldStartingWithLoweCaseFieldIsJustOneLetter()
        {
            var testCode = @"public class Foo
{
    internal static readonly string b = ""baz"";
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(3, 37);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = @"public class Foo
{
    internal static readonly string B = ""baz"";
}";

            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestStaticReadonlyFieldAssignmentInConstructor()
        {
            var testCode = @"public class Foo
{
    public static readonly string bar;

    static Foo()
    {
        bar = ""aa"";
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(3, 35);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = @"public class Foo
{
    public static readonly string Bar;

    static Foo()
    {
        Bar = ""aa"";
    }
}";

            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestStaticReadonlyFieldStartingWithUpperCase()
        {
            var testCode = @"public class Foo
{
    public static readonly string Bar = ""baz"";
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestReadonlyFieldStartingWithLoweCase()
        {
            var testCode = @"public class Foo
{
    public readonly string bar = ""baz"";
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestStaticFieldStartingWithLoweCase()
        {
            var testCode = @"public class Foo
{
    public static string bar = ""baz"";
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestClassNameConflict()
        {
            var testCode = @"public class Bar
{
    public static readonly string bar;

    static Bar()
    {
        bar = ""aa"";
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(3, 35);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = @"public class Bar
{
    public static readonly string BarValue;

    static Bar()
    {
        BarValue = ""aa"";
    }
}";

            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMemberNameConflict()
        {
            var testCode = @"public class Foo
{
    public static readonly string bar;

    static Foo()
    {
        bar = ""aa"";
    }

    public static readonly string Bar;
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(3, 35);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = @"public class Foo
{
    public static readonly string BarValue;

    static Foo()
    {
        BarValue = ""aa"";
    }

    public static readonly string Bar;
}";

            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new SA1311StaticReadonlyFieldsMustBeginWithUpperCaseLetter();
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new SA1304SA1311CodeFixProvider();
        }
    }
}
