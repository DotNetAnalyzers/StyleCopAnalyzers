namespace StyleCop.Analyzers.Test.NamingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.NamingRules;
    using TestHelper;
    using Xunit;

    public class SA1310UnitTests : CodeFixVerifier
    {
        [Fact]
        public async Task TestEmptySource()
        {
            var testCode = string.Empty;
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestFieldWithUnderscore()
        {
            var testCode = @"public class ClassName
{
    public string name_bar = ""baz"";
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments("name_bar").WithLocation(3, 19);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);

            var fixedCode = @"public class ClassName
{
    public string nameBar = ""baz"";
}";

            await this.VerifyCSharpFixAsync(testCode, fixedCode);
        }

        [Fact]
        public async Task TestFieldStartingWithoutUnderscore()
        {
            var testCode = @"public class ClassName
{
    public string nameBar = ""baz"";
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestFieldWithUnderscorePlacedInsideNativeMethodsClass()
        {
            var testCode = @"public class ClassNameNativeMethods
{
    internal string name_bar = ""baz"";
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestFieldWithUnderscorePlacedInsideNativeMethodsClassWithIncorrectName()
        {
            var testCode = @"public class ClassNameNativeMethodsClass
{
    internal string name_bar = ""baz"";
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments("name_bar").WithLocation(3, 21);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);

            var fixedCode = @"public class ClassNameNativeMethodsClass
{
    internal string nameBar = ""baz"";
}";

            await this.VerifyCSharpFixAsync(testCode, fixedCode);
        }

        [Fact]
        public async Task TestFieldWithUnderscorePlacedInsideOuterNativeMethodsClass()
        {
            var testCode = @"public class ClassNameNativeMethods
{
    public class Foo
    {
        public string name_bar = ""baz"";
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestUnderscoreOnlyVariableName()
        {
            var testCode = @"public class ClassNameNativeMethodsClass
{
    internal string _ = ""baz"";
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new SA1310FieldNamesMustNotContainUnderscore();
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new SA1310CodeFixProvider();
        }
    }
}
