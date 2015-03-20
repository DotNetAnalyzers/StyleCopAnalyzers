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
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }


        private async Task TestThatDiagnosticIsNotReportedImpl(string modifiers)
        {
            var testCode = @"public class Foo
{{
{0}
string Bar, car, Dar;
}}";

            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, modifiers), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        private async Task TestThatDiagnosticIsReported_SingleFieldImpl(string modifiers)
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

            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, modifiers), expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = @"public class Foo
{{
{0}
string bar;
{0}
string car;
{0}
string dar;
}}";

            await this.VerifyCSharpFixAsync(string.Format(testCode, modifiers), string.Format(fixedCode, modifiers)).ConfigureAwait(false);
        }

        private async Task TestThatDiagnosticIsReported_MultipleFieldsImpl(string modifiers)
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

            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, modifiers), expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = @"public class Foo
{{
{0}
string bar, car, dar;
}}";

            await this.VerifyCSharpFixAsync(string.Format(testCode, modifiers), string.Format(fixedCode, modifiers)).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestThatDiagnosticIsReported_SingleField()
        {
            await this.TestThatDiagnosticIsReported_SingleFieldImpl(string.Empty).ConfigureAwait(false);
            await this.TestThatDiagnosticIsReported_SingleFieldImpl("readonly").ConfigureAwait(false);
            await this.TestThatDiagnosticIsReported_SingleFieldImpl("private").ConfigureAwait(false);
            await this.TestThatDiagnosticIsReported_SingleFieldImpl("private readonly").ConfigureAwait(false);
        }

        [Fact(Skip = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/496")]
        public async Task TestThatDiagnosticIsReported_MultipleFields()
        {
            await this.TestThatDiagnosticIsReported_MultipleFieldsImpl(string.Empty).ConfigureAwait(false);
            await this.TestThatDiagnosticIsReported_MultipleFieldsImpl("readonly").ConfigureAwait(false);
            await this.TestThatDiagnosticIsReported_MultipleFieldsImpl("private").ConfigureAwait(false);
            await this.TestThatDiagnosticIsReported_MultipleFieldsImpl("private readonly").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestThatDiagnosticIsNotReported()
        {
            await this.TestThatDiagnosticIsNotReportedImpl("const").ConfigureAwait(false);
            await this.TestThatDiagnosticIsNotReportedImpl("private const").ConfigureAwait(false);
            await this.TestThatDiagnosticIsNotReportedImpl("internal const").ConfigureAwait(false);
            await this.TestThatDiagnosticIsNotReportedImpl("protected const").ConfigureAwait(false);
            await this.TestThatDiagnosticIsNotReportedImpl("protected internal const").ConfigureAwait(false);

            await this.TestThatDiagnosticIsNotReportedImpl("internal readonly").ConfigureAwait(false);
            await this.TestThatDiagnosticIsNotReportedImpl("protected readonly").ConfigureAwait(false);
            await this.TestThatDiagnosticIsNotReportedImpl("protected internal readonly").ConfigureAwait(false);
            await this.TestThatDiagnosticIsNotReportedImpl("public").ConfigureAwait(false);
            await this.TestThatDiagnosticIsNotReportedImpl("internal").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestFieldStartingWithAnUnderscore()
        {
            // Makes sure SA1306 is not reported for fields starting with an underscore
            var testCode = @"public class Foo
{
    public string _bar = ""baz"";
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestFieldStartingWithLetter()
        {
            var testCode = @"public class Foo
{
    public string bar = ""baz"";
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestFieldPlacedInsideNativeMethodsClass()
        {
            var testCode = @"public class FooNativeMethods
{
    string Bar = ""baz"";
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
