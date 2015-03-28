
namespace StyleCop.Analyzers.Test.NamingRules
{
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.NamingRules;
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using TestHelper;
    using Xunit;

    public class SA1308UnitTests : DiagnosticVerifier
    {
        private const string DiagnosticId = SA1308VariableNamesMustNotBePrefixed.DiagnosticId;

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
                await this.TestFieldSpecifyingModifierAndPrefix(modifier, "m\\u005F", "m_");
                await this.TestFieldSpecifyingModifierAndPrefix(modifier, "s\\u005F", "s_");
            }
        }

        private async Task TestFieldSpecifyingModifierAndPrefix(string modifier, string codePrefix, string diagnosticPrefix)
        {
            var testCode = @"public class Foo
{{
    {0} 
string {1}bar = ""baz"";
}}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments(string.Format("{0}bar", diagnosticPrefix),
                                                string.Format("{0}", diagnosticPrefix)).WithLocation(4, 8);

            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, modifier, codePrefix), expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestFieldStartingWithNonTriggeringPrefix()
        {
            var testCode = @"public class Foo
{{
    public 
string x_bar = ""baz"";
}}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestFieldInsideNativeMethodsClass()
        {
            var testCode = @"public class TestNativeMethods
{{
    public 
string m_bar = ""baz"";
}}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new SA1308VariableNamesMustNotBePrefixed();
        }
    }
}