namespace StyleCop.Analyzers.Test.NamingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using StyleCop.Analyzers.NamingRules;
    using TestHelper;

    [TestClass]
    public class SA1309UnitTests : CodeFixVerifier
    {
        private const string DiagnosticId = SA1309FieldNamesMustNotBeginWithUnderscore.DiagnosticId;

        [TestMethod]
        public async Task TestEmptySource()
        {
            var testCode = string.Empty;
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestFieldStartingWithAnUnderscore()
        {
            var testCode = @"public class Foo
{
    public string _bar = ""baz"";
}";

            var expected = new[]
            {
                new DiagnosticResult
                {
                    Id = DiagnosticId,
                    Message = string.Format("Field '{0}' must not begin with an underscore", "_bar"),
                    Severity = DiagnosticSeverity.Warning,
                    Locations =
                        new[]
                        {
                            new DiagnosticResultLocation("Test0.cs", 3, 19)
                        }
                }
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);

            var fixedCode = @"public class Foo
{
    public string bar = ""baz"";
}";

            await this.VerifyCSharpFixAsync(testCode, fixedCode);
        }

        [TestMethod]
        public async Task TestFieldStartingWithLetter()
        {
            var testCode = @"public class Foo
{
    public string bar = ""baz"";
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestFieldStartingAnUnderscorePlacedInsideNativeMethodsClass()
        {
            var testCode = @"public class FooNativeMethods
{
    internal string _bar = ""baz"";
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestFieldStartingAnUnderscorePlacedInsideNativeMethodsClassWithIncorrectName()
        {
            var testCode = @"public class FooNativeMethodsClass
{
    internal string _bar = ""baz"";
}";

            var expected = new[]
            {
                new DiagnosticResult
                {
                    Id = DiagnosticId,
                    Message = string.Format("Field '{0}' must not begin with an underscore", "_bar"),
                    Severity = DiagnosticSeverity.Warning,
                    Locations =
                        new[]
                        {
                            new DiagnosticResultLocation("Test0.cs", 3, 21)
                        }
                }
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);

            var fixedCode = @"public class FooNativeMethodsClass
{
    internal string bar = ""baz"";
}";

            await this.VerifyCSharpFixAsync(testCode, fixedCode);
        }

        [TestMethod]
        public async Task TestFieldStartingAnUnderscorePlacedInsideOuterNativeMethodsClass()
        {
            var testCode = @"public class FooNativeMethods
{
    public class Foo
    {
        public string _bar = ""baz"";
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new SA1309FieldNamesMustNotBeginWithUnderscore();
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new SA1309CodeFixProvider();
        }
    }
}
