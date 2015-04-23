namespace StyleCop.Analyzers.Test.NamingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.NamingRules;
    using TestHelper;
    using Xunit;

    public class SA1308UnitTests : CodeFixVerifier
    {
        [Fact]
        public async Task TestEmptySource()
        {
            var testCode = string.Empty;
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        private readonly string[] modifiers = new[] { "public", "private", "protected", "public readonly", "internal readonly", "public static", "private static" };

        [Fact]
        public async Task TestFieldStartingWithPrefixesToTriggerDiagnostic()
        {
            foreach (var modifier in this.modifiers)
            {
                await this.TestFieldSpecifyingModifierAndPrefix(modifier, "m_", "m_");
                await this.TestFieldSpecifyingModifierAndPrefix(modifier, "s_", "s_");
                await this.TestFieldSpecifyingModifierAndPrefix(modifier, "t_", "t_");
                await this.TestFieldSpecifyingModifierAndPrefix(modifier, "m\\u005F", "m_");
                await this.TestFieldSpecifyingModifierAndPrefix(modifier, "s\\u005F", "s_");
                await this.TestFieldSpecifyingModifierAndPrefix(modifier, "t\\u005F", "t_");
            }
        }

        private async Task TestFieldSpecifyingModifierAndPrefix(string modifier, string codePrefix, string diagnosticPrefix)
        {
            var originalCode = @"public class Foo
{{
    {0}
string {1}bar = ""baz"";
}}";

            DiagnosticResult expected =
                this.CSharpDiagnostic()
                .WithArguments($"{diagnosticPrefix}bar", diagnosticPrefix)
                .WithLocation(4, 8);

            var testCode = string.Format(originalCode, modifier, codePrefix);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);

            var fixedCode = string.Format(originalCode, modifier, string.Empty);
            await this.VerifyCSharpFixAsync(testCode, fixedCode);
        }

        [Fact]
        public async Task TestMUnderscoreOnly()
        {
            var originalCode = @"public class Foo
{
private string m_ = ""baz"";
}";
            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments("m_", "m_").WithLocation(3, 16);
            await this.VerifyCSharpDiagnosticAsync(originalCode, expected, CancellationToken.None);

            // When the variable name is simply the disallowed prefix, we will not offer a code fix, as we cannot infer the correct variable name.
            await this.VerifyCSharpFixAsync(originalCode, originalCode);
        }

        [Fact]
        public async Task TestFieldStartingWithNonTriggeringPrefix()
        {
            var testCode = @"public class Foo
{
    public
string x_bar = ""baz"";
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestFieldInsideNativeMethodsClass()
        {
            var testCode = @"public class TestNativeMethods
{
    public
string m_bar = ""baz"";
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        /// <summary>
        /// This is a regression test for DotNetAnalyzers/StyleCopAnalyzers#627.
        /// </summary>
        /// <seealso href="https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/627">#627: Code Fixes For Naming
        /// Rules SA1308 and SA1309 Do Not Always Fix The Name Entirely</seealso>
        [Fact]
        public async Task TestFixingMultipleIdenticalPrefixes()
        {
            var testCode = @"public class Foo
{
    private string m_m_bar = ""baz"";
}";

            DiagnosticResult expected =
                this.CSharpDiagnostic()
                .WithArguments("m_m_bar", "m_")
                .WithLocation(3, 20);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);

            var fixedCode = testCode.Replace("m_", string.Empty);
            await this.VerifyCSharpFixAsync(testCode, fixedCode);
        }

        /// <summary>
        /// This is a regression test for DotNetAnalyzers/StyleCopAnalyzers#627.
        /// </summary>
        /// <seealso href="https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/627">#627: Code Fixes For Naming
        /// Rules SA1308 and SA1309 Do Not Always Fix The Name Entirely</seealso>
        [Fact]
        public async Task TestFixingMultipleIndependentPrefixes()
        {
            var testCode = @"public class Foo
{
    private string m_t_s_bar = ""baz"";
}";

            DiagnosticResult expected =
                this.CSharpDiagnostic()
                .WithArguments("m_t_s_bar", "m_")
                .WithLocation(3, 20);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);

            var fixedCode = testCode.Replace("m_", string.Empty);
            fixedCode = fixedCode.Replace("s_", string.Empty);
            fixedCode = fixedCode.Replace("t_", string.Empty);

            await this.VerifyCSharpFixAsync(testCode, fixedCode);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new SA1308VariableNamesMustNotBePrefixed();
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new SA1308CodeFixProvider();
        }
    }
}
