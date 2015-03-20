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

            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, modifiers), expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = @"public class Foo
{{
{0}
string Bar;
{0}
string Car;
{0}
string Dar;
}}";

            await this.VerifyCSharpFixAsync(string.Format(testCode, modifiers), string.Format(fixedCode, modifiers)).ConfigureAwait(false);
        }

        private async Task TestThatDiagnosticIsReported_MultipleFieldsImpl(string modifiers)
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

            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, modifiers), expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = @"public class Foo
{{
{0}
string Bar, Car, Dar;
}}";

            await this.VerifyCSharpFixAsync(string.Format(testCode, modifiers), string.Format(fixedCode, modifiers)).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestThatDiagnosticIsReported_SingleField()
        {
            await this.TestThatDiagnosticIsReported_SingleFieldImpl("public").ConfigureAwait(false);
            await this.TestThatDiagnosticIsReported_SingleFieldImpl("internal").ConfigureAwait(false);
            await this.TestThatDiagnosticIsReported_SingleFieldImpl("protected internal").ConfigureAwait(false);
            await this.TestThatDiagnosticIsReported_SingleFieldImpl("public readonly").ConfigureAwait(false);
            await this.TestThatDiagnosticIsReported_SingleFieldImpl("internal readonly").ConfigureAwait(false);
        }

        [Fact(Skip = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/496")]
        public async Task TestThatDiagnosticIsReported_MultipleFields()
        {
            await this.TestThatDiagnosticIsReported_MultipleFieldsImpl("public").ConfigureAwait(false);
            await this.TestThatDiagnosticIsReported_MultipleFieldsImpl("internal").ConfigureAwait(false);
            await this.TestThatDiagnosticIsReported_MultipleFieldsImpl("protected internal").ConfigureAwait(false);
            await this.TestThatDiagnosticIsReported_MultipleFieldsImpl("public readonly").ConfigureAwait(false);
            await this.TestThatDiagnosticIsReported_MultipleFieldsImpl("internal readonly").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestThatDiagnosticIsNotReported()
        {
            await this.TestThatDiagnosticIsNotReportedImpl(string.Empty).ConfigureAwait(false);
            await this.TestThatDiagnosticIsNotReportedImpl("readonly").ConfigureAwait(false);
            await this.TestThatDiagnosticIsNotReportedImpl("private").ConfigureAwait(false);

            await this.TestThatDiagnosticIsNotReportedImpl("const").ConfigureAwait(false);
            await this.TestThatDiagnosticIsNotReportedImpl("private const").ConfigureAwait(false);
            await this.TestThatDiagnosticIsNotReportedImpl("protected const").ConfigureAwait(false);
            await this.TestThatDiagnosticIsNotReportedImpl("protected const").ConfigureAwait(false);
            
            await this.TestThatDiagnosticIsNotReportedImpl("private readonly").ConfigureAwait(false);
            await this.TestThatDiagnosticIsNotReportedImpl("protected readonly").ConfigureAwait(false);
            await this.TestThatDiagnosticIsNotReportedImpl("protected internal readonly").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestFieldStartingWithAnUnderscore()
        {
            // Makes sure SA1307 is not reported for fields starting with an underscore
            var testCode = @"public class Foo
{
    public string _bar = ""baz"";
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestFieldPlacedInsideNativeMethodsClass()
        {
            var testCode = @"public class FooNativeMethods
{
    public string bar = ""baz"";
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
