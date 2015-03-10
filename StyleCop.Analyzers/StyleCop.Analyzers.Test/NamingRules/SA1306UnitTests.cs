namespace StyleCop.Analyzers.Test.NamingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Xunit;
    using StyleCop.Analyzers.NamingRules;
    using TestHelper;

    public class SA1306UnitTests : CodeFixVerifier
    {
        private const string DiagnosticId = SA1306FieldNamesMustBeginWithLowerCaseLetter.DiagnosticId;

        [Fact]
        public async Task TestEmptySource()
        {
            var testCode = string.Empty;
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }


        private async Task TestThatDiagnosticIsNotReportedImpl(string modifiers)
        {
            var testCode = @"public class Foo
{{
{0}
string Bar, car, Dar;
}}";

            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, modifiers), EmptyDiagnosticResults, CancellationToken.None);
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

            var expected = new[]
            {
                new DiagnosticResult
                {
                    Id = DiagnosticId,
                    Message = string.Format("Field '{0}' must begin with lower-case letter", "Bar"),
                    Severity = DiagnosticSeverity.Warning,
                    Locations =
                        new[]
                        {
                            new DiagnosticResultLocation("Test0.cs", 4, 8)
                        }
                },
                new DiagnosticResult
                {
                    Id = DiagnosticId,
                    Message = string.Format("Field '{0}' must begin with lower-case letter", "Dar"),
                    Severity = DiagnosticSeverity.Warning,
                    Locations =
                        new[]
                        {
                            new DiagnosticResultLocation("Test0.cs", 8, 8)
                        }
                }
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

        private async Task TestThatDiagnosticIsReported_MultipleFieldsImpl(string modifiers)
        {
            var testCode = @"public class Foo
{{
{0}
string Bar, car, Dar;
}}";

            var expected = new[]
            {
                new DiagnosticResult
                {
                    Id = DiagnosticId,
                    Message = string.Format("Field '{0}' must begin with lower-case letter", "Bar"),
                    Severity = DiagnosticSeverity.Warning,
                    Locations =
                        new[]
                        {
                            new DiagnosticResultLocation("Test0.cs", 4, 8)
                        }
                },
                new DiagnosticResult
                {
                    Id = DiagnosticId,
                    Message = string.Format("Field '{0}' must begin with lower-case letter", "Dar"),
                    Severity = DiagnosticSeverity.Warning,
                    Locations =
                        new[]
                        {
                            new DiagnosticResultLocation("Test0.cs", 4, 18)
                        }
                }
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
        public async Task TestThatDiagnosticIsReported_SingleField()
        {
            await this.TestThatDiagnosticIsReported_SingleFieldImpl(string.Empty);
            await this.TestThatDiagnosticIsReported_SingleFieldImpl("readonly");
            await this.TestThatDiagnosticIsReported_SingleFieldImpl("private");
            await this.TestThatDiagnosticIsReported_SingleFieldImpl("private readonly");
        }

        [Fact(Skip = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/496")]
        public async Task TestThatDiagnosticIsReported_MultipleFields()
        {
            await this.TestThatDiagnosticIsReported_MultipleFieldsImpl(string.Empty);
            await this.TestThatDiagnosticIsReported_MultipleFieldsImpl("readonly");
            await this.TestThatDiagnosticIsReported_MultipleFieldsImpl("private");
            await this.TestThatDiagnosticIsReported_MultipleFieldsImpl("private readonly");
        }

        [Fact]
        public async Task TestThatDiagnosticIsNotReported()
        {
            await this.TestThatDiagnosticIsNotReportedImpl("const");
            await this.TestThatDiagnosticIsNotReportedImpl("private const");
            await this.TestThatDiagnosticIsNotReportedImpl("internal const");
            await this.TestThatDiagnosticIsNotReportedImpl("protected const");
            await this.TestThatDiagnosticIsNotReportedImpl("protected internal const");

            await this.TestThatDiagnosticIsNotReportedImpl("internal readonly");
            await this.TestThatDiagnosticIsNotReportedImpl("protected readonly");
            await this.TestThatDiagnosticIsNotReportedImpl("protected internal readonly");
            await this.TestThatDiagnosticIsNotReportedImpl("public");
            await this.TestThatDiagnosticIsNotReportedImpl("internal");
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
