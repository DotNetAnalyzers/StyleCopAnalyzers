namespace StyleCop.Analyzers.Test.NamingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Analyzers.NamingRules;
    using Helpers;
    using TestHelper;

    [TestClass]
    public class SA1307UnitTests : CodeFixVerifier
    {
        private const string DiagnosticId = SA1307AccessibleFieldsMustBeginWithUpperCaseLetter.DiagnosticId;

        [TestMethod]
        public async Task TestEmptySource()
        {
            var testCode = string.Empty;
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }


        private async Task TestThatDiagnosticIsNotReported(string modifiers)
        {
            var testCode = @"public class Foo
{{
{0}
string Bar, car, Dar;
}}";

            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, modifiers), EmptyDiagnosticResults, CancellationToken.None);
        }

        private async Task TestThatDiagnosticIsReported_SingleField(string modifiers)
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

            var expected = new[]
            {
                new DiagnosticResult
                {
                    Id = DiagnosticId,
                    Message = string.Format("Field '{0}' must begin with upper-case letter", "bar"),
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
                    Message = string.Format("Field '{0}' must begin with upper-case letter", "dar"),
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
string Bar;
{0}
string Car;
{0}
string Dar;
}}";

            await this.VerifyCSharpFixAsync(string.Format(testCode, modifiers), string.Format(fixedCode, modifiers));
        }

        private async Task TestThatDiagnosticIsReported_MultipleFields(string modifiers)
        {
            var testCode = @"public class Foo
{{
{0}
string bar, Car, dar;
}}";

            var expected = new[]
            {
                new DiagnosticResult
                {
                    Id = DiagnosticId,
                    Message = string.Format("Field '{0}' must begin with upper-case letter", "bar"),
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
                    Message = string.Format("Field '{0}' must begin with upper-case letter", "dar"),
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
string Bar, Car, Dar;
}}";

            await this.VerifyCSharpFixAsync(string.Format(testCode, modifiers), string.Format(fixedCode, modifiers));
        }

        [TestMethod]
        public async Task TestThatDiagnosticIsReported_SingleField()
        {
            await this.TestThatDiagnosticIsReported_SingleField("public");
            await this.TestThatDiagnosticIsReported_SingleField("internal");
            await this.TestThatDiagnosticIsReported_SingleField("protected internal");
            await this.TestThatDiagnosticIsReported_SingleField("public readonly");
            await this.TestThatDiagnosticIsReported_SingleField("internal readonly");
        }

        [TestMethod]
        [Ignore]
        [OpenIssue("https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/496")]
        public async Task TestThatDiagnosticIsReported_MultipleFields()
        {
            await this.TestThatDiagnosticIsReported_MultipleFields("public");
            await this.TestThatDiagnosticIsReported_MultipleFields("internal");
            await this.TestThatDiagnosticIsReported_MultipleFields("protected internal");
            await this.TestThatDiagnosticIsReported_MultipleFields("public readonly");
            await this.TestThatDiagnosticIsReported_MultipleFields("internal readonly");
        }

        [TestMethod]
        public async Task TestThatDiagnosticIsNotReported()
        {
            await this.TestThatDiagnosticIsNotReported(string.Empty);
            await this.TestThatDiagnosticIsNotReported("readonly");
            await this.TestThatDiagnosticIsNotReported("private");

            await this.TestThatDiagnosticIsNotReported("const");
            await this.TestThatDiagnosticIsNotReported("private const");
            await this.TestThatDiagnosticIsNotReported("protected const");
            await this.TestThatDiagnosticIsNotReported("protected const");
            
            await this.TestThatDiagnosticIsNotReported("private readonly");
            await this.TestThatDiagnosticIsNotReported("protected readonly");
            await this.TestThatDiagnosticIsNotReported("protected internal readonly");
        }

        [TestMethod]
        public async Task TestFieldStartingWithAnUnderscore()
        {
            // Makes sure SA1307 is not reported for fields starting with an underscore
            var testCode = @"public class Foo
{
    public string _bar = ""baz"";
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
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
