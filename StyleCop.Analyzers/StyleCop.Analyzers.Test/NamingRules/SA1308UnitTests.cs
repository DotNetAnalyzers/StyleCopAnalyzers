// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.NamingRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.NamingRules;
    using TestHelper;
    using Xunit;

    public class SA1308UnitTests : CodeFixVerifier
    {
        private readonly string[] modifiers = new[] { "public", "private", "protected", "public readonly", "internal readonly", "public static", "private static" };

        public static IEnumerable<object[]> PrefixesData()
        {
            yield return new object[] { "m_" };
            yield return new object[] { "s_" };
            yield return new object[] { "t_" };
            yield return new object[] { "m\\u005F" };
            yield return new object[] { "s\\u005F" };
            yield return new object[] { "t\\u005F" };
        }

        public static IEnumerable<object[]> MultipleDistinctPrefixesData()
        {
            yield return new object[] { "m_t_s_", "m_" };
            yield return new object[] { "s\\u005Fm\\u005Ft\\u005F", "s_" };
        }

        [Fact]
        public async Task TestFieldStartingWithPrefixesToTriggerDiagnosticAsync()
        {
            foreach (var modifier in this.modifiers)
            {
                await this.TestFieldSpecifyingModifierAndPrefixAsync(modifier, "m_", "m_").ConfigureAwait(false);
                await this.TestFieldSpecifyingModifierAndPrefixAsync(modifier, "s_", "s_").ConfigureAwait(false);
                await this.TestFieldSpecifyingModifierAndPrefixAsync(modifier, "t_", "t_").ConfigureAwait(false);
                await this.TestFieldSpecifyingModifierAndPrefixAsync(modifier, "m\\u005F", "m_").ConfigureAwait(false);
                await this.TestFieldSpecifyingModifierAndPrefixAsync(modifier, "s\\u005F", "s_").ConfigureAwait(false);
                await this.TestFieldSpecifyingModifierAndPrefixAsync(modifier, "t\\u005F", "t_").ConfigureAwait(false);
            }
        }

        [Fact]
        public async Task TestMUnderscoreOnlyAsync()
        {
            var originalCode = @"public class Foo
{
private string m_ = ""baz"";
}";
            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments("m_", "m_").WithLocation(3, 16);
            await this.VerifyCSharpDiagnosticAsync(originalCode, expected, CancellationToken.None).ConfigureAwait(false);

            // When the variable name is simply the disallowed prefix, we will not offer a code fix, as we cannot infer the correct variable name.
            await this.VerifyCSharpFixAsync(originalCode, originalCode).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestFieldStartingWithNonTriggeringPrefixAsync()
        {
            var testCode = @"public class Foo
{
    public
string x_bar = ""baz"";
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestFieldInsideNativeMethodsClassAsync()
        {
            var testCode = @"public class TestNativeMethods
{
    public
string m_bar = ""baz"";
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// This is a regression test for DotNetAnalyzers/StyleCopAnalyzers#627.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        /// <seealso href="https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/627">#627: Code Fixes For Naming
        /// Rules SA1308 and SA1309 Do Not Always Fix The Name Entirely</seealso>
        [Theory]
        [MemberData(nameof(PrefixesData))]
        public async Task TestFixingMultipleIdenticalPrefixesAsync(string prefix)
        {
            var testCode = $@"public class Foo
{{
    private string {prefix}{prefix}bar = ""baz"";
}}";

            DiagnosticResult expected =
                this.CSharpDiagnostic()
                .WithArguments($"{prefix}{prefix}bar", prefix)
                .WithLocation(3, 20);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = testCode.Replace(prefix, string.Empty);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(PrefixesData))]
        public async Task TestMultipleIdenticalPrefixesOnlyAsync(string prefix)
        {
            var testCode = $@"public class Foo
{{
    private string {prefix}{prefix} = ""baz"";
}}";

            DiagnosticResult expected =
                this.CSharpDiagnostic()
                .WithArguments($"{prefix}{prefix}", prefix)
                .WithLocation(3, 20);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            // A code fix is not offered as removing the prefixes would create an empty identifier.
            await this.VerifyCSharpFixAsync(testCode, testCode).ConfigureAwait(false);
        }

        /// <summary>
        /// This is a regression test for DotNetAnalyzers/StyleCopAnalyzers#627.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        /// <seealso href="https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/627">#627: Code Fixes For Naming
        /// Rules SA1308 and SA1309 Do Not Always Fix The Name Entirely</seealso>
        [Theory]
        [MemberData(nameof(MultipleDistinctPrefixesData))]
        public async Task TestFixingMultipleDistinctPrefixesAsync(string prefixes, string diagnosticPrefix)
        {
            var testCode = $@"public class Foo
{{
    private string {prefixes}bar = ""baz"";
}}";

            DiagnosticResult expected =
                this.CSharpDiagnostic()
                .WithArguments($"{prefixes}bar", firstPrefix)
                .WithLocation(3, 20);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = testCode.Replace(prefixes, string.Empty);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(MultipleDistinctPrefixesData))]
        public async Task TestMultipleDistinctPrefixesOnlyAsync(string prefixes, string diagnosticPrefix)
        {
            var testCode = $@"public class Foo
{{
    private string {prefixes} = ""baz"";
}}";

            DiagnosticResult expected =
                this.CSharpDiagnostic()
                .WithArguments(prefixes, firstPrefix)
                .WithLocation(3, 20);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            // A code fix is not offered as removing the prefixes would create an empty identifier.
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1308VariableNamesMustNotBePrefixed();
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new SA1308CodeFixProvider();
        }

        private async Task TestFieldSpecifyingModifierAndPrefixAsync(string modifier, string codePrefix, string diagnosticPrefix)
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
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = string.Format(originalCode, modifier, string.Empty);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }
    }
}
