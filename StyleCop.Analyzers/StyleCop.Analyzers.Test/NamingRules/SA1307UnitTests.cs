namespace StyleCop.Analyzers.Test.NamingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Analyzers.NamingRules;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using TestHelper;
    using Xunit;

    public class SA1307UnitTests : CodeFixVerifier
    {
        private const string DiagnosticId = SA1307AccessibleFieldsMustBeginWithUpperCaseLetter.DiagnosticId;

        [Fact]
        public async Task TestEmptySource()
        {
            var testCode = string.Empty;
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Theory]
        [InlineData("")]
        [InlineData("readonly")]
        [InlineData("private")]

        [InlineData("const")]
        [InlineData("private const")]
        [InlineData("protected const")]
        [InlineData("protected internal const")]

        [InlineData("private readonly")]
        [InlineData("protected readonly")]
        [InlineData("protected internal readonly")]
        public async Task TestThatDiagnosticIsNotReported(string modifiers)
        {
            var testCode = @"public class Foo
{{
{0}
string Bar, car, Dar;
}}";

            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, modifiers), EmptyDiagnosticResults, CancellationToken.None);
        }

        [Theory]
        [InlineData("public")]
        [InlineData("internal")]
        [InlineData("protected internal")]
        [InlineData("public readonly")]
        [InlineData("internal readonly")]
        public async Task TestThatDiagnosticIsReported_SingleField(string modifiers)
        {
            var testCode = @"public class Foo
{{
{0}
string bar;
{0}
string Car;
{0}
string dar;
}}";

            DiagnosticResult[] expected =
                {
                    this.CSharpDiagnostic().WithArguments("bar").WithLocation(4, 8),
                    this.CSharpDiagnostic().WithArguments("dar").WithLocation(8, 8),
                };

            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, modifiers), expected, CancellationToken.None);

            var fixedCode = @"public class Foo
{{
{0}
string Bar;
{0}
string Car;
{0}
string Dar;
}}";

            await this.VerifyCSharpFixAsync(string.Format(testCode, modifiers), string.Format(fixedCode, modifiers));
        }

        [Theory(Skip = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/496")]
        [InlineData("public")]
        [InlineData("internal")]
        [InlineData("protected internal")]
        [InlineData("public readonly")]
        [InlineData("internal readonly")]
        public async Task TestThatDiagnosticIsReported_MultipleFields(string modifiers)
        {
            var testCode = @"public class Foo
{{
{0}
string bar, Car, dar;
}}";

            DiagnosticResult[] expected =
                {
                    this.CSharpDiagnostic().WithArguments("bar").WithLocation(4, 8),
                    this.CSharpDiagnostic().WithArguments("dar").WithLocation(4, 18),
                };

            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, modifiers), expected, CancellationToken.None);

            var fixedCode = @"public class Foo
{{
{0}
string Bar, Car, Dar;
}}";

            await this.VerifyCSharpFixAsync(string.Format(testCode, modifiers), string.Format(fixedCode, modifiers));
        }

        [Fact]
        public async Task TestFieldStartingWithAnUnderscore()
        {
            // Makes sure SA1307 is not reported for fields starting with an underscore
            var testCode = @"public class Foo
{
    public string _bar = ""baz"";
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestFieldPlacedInsideNativeMethodsClass()
        {
            var testCode = @"public class FooNativeMethods
{
    public string bar = ""baz"";
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new SA1307AccessibleFieldsMustBeginWithUpperCaseLetter();
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new SA1307CodeFixProvider();
        }
    }
}
