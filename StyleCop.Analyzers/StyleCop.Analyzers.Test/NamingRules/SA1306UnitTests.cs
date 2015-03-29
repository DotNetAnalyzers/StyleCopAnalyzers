namespace StyleCop.Analyzers.Test.NamingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.NamingRules;
    using TestHelper;
    using Xunit;

    public class SA1306UnitTests : CodeFixVerifier
    {
        private const string DiagnosticId = SA1306FieldNamesMustBeginWithLowerCaseLetter.DiagnosticId;

        [Fact]
        public async Task TestEmptySource()
        {
            var testCode = string.Empty;
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Theory]
        [InlineData("const")]
        [InlineData("private const")]
        [InlineData("internal const")]
        [InlineData("protected const")]
        [InlineData("protected internal const")]
        [InlineData("internal readonly")]
        [InlineData("protected readonly")]
        [InlineData("protected internal readonly")]
        [InlineData("public")]
        [InlineData("internal")]
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
        [InlineData("")]
        [InlineData("readonly")]
        [InlineData("private")]
        [InlineData("private readonly")]
        public async Task TestThatDiagnosticIsReported_SingleField(string modifiers)
        {
            var testCode = @"public class Foo
{{
{0}
string Bar;
{0}
string car;
{0}
string Dar;
}}";

            DiagnosticResult[] expected =
                {
                    this.CSharpDiagnostic().WithArguments("Bar").WithLocation(4, 8),
                    this.CSharpDiagnostic().WithArguments("Dar").WithLocation(8, 8)
                };

            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, modifiers), expected, CancellationToken.None);

            var fixedCode = @"public class Foo
{{
{0}
string bar;
{0}
string car;
{0}
string dar;
}}";

            await this.VerifyCSharpFixAsync(string.Format(testCode, modifiers), string.Format(fixedCode, modifiers));
        }

        [Theory(Skip = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/496")]
        [InlineData("")]
        [InlineData("readonly")]
        [InlineData("private")]
        [InlineData("private readonly")]
        public async Task TestThatDiagnosticIsReported_MultipleFields(string modifiers)
        {
            var testCode = @"public class Foo
{{
{0}
string Bar, car, Dar;
}}";

            DiagnosticResult[] expected =
                {
                    this.CSharpDiagnostic().WithArguments("Bar").WithLocation(4, 8),
                    this.CSharpDiagnostic().WithArguments("Dar").WithLocation(4, 18)
                };

            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, modifiers), expected, CancellationToken.None);

            var fixedCode = @"public class Foo
{{
{0}
string bar, car, dar;
}}";

            await this.VerifyCSharpFixAsync(string.Format(testCode, modifiers), string.Format(fixedCode, modifiers));
        }

        [Fact]
        public async Task TestFieldStartingWithAnUnderscore()
        {
            // Makes sure SA1306 is not reported for fields starting with an underscore
            var testCode = @"public class Foo
{
    public string _bar = ""baz"";
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestFieldStartingWithLetter()
        {
            var testCode = @"public class Foo
{
    public string bar = ""baz"";
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestFieldPlacedInsideNativeMethodsClass()
        {
            var testCode = @"public class FooNativeMethods
{
    string Bar = ""baz"";
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new SA1306FieldNamesMustBeginWithLowerCaseLetter();
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new SA1306CodeFixProvider();
        }
    }
}
